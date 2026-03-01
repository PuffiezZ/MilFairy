using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCombat : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    [SerializeField] private float dashDistance = 2.0f; // ���о�觻���
    [SerializeField] private float snapDistance = 4.0f; // ���е�Ǩ�Ѻ�ѵ�����;�����
    [SerializeField] private LayerMask enemyLayer;     // ���͡ Layer ������ѵ��

    [SerializeField] private PlayerEquipment equipment;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerLocomotion playerLocomotion;
    [SerializeField] private Player player;

    [Header("Attack Settings")]
    [SerializeField] private float cooldownAttackTime = 1.5f;
    private bool enableToSheath = false;
    private float currentCooldownAttackTime = 0f;

    public bool isSheathing { get; set; }
    public bool IsCharging { get; private set; }

    public int currentIndexWeaponSlotNumber { get; set; }
    public UnityAction OnAttackAction;

    private CharacterController controller;
    private Vector3 impact = Vector3.zero;
    private UtilityDev.WeaponType lastWeaponTypeBeforeSheath;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    private void Update()
    {
        AttackUpdateHandler(equipment.CurrentWeaponOnHanded);
        if (Input.GetKeyDown(KeyCode.LeftAlt) && enableToSheath && !isSheathing)
        {
            SheathUpdateHandler();
        }
        HandleImpact();
    }
    private void SheathUpdateHandler()
    {
        WeaponScript currentWeapon = equipment.CurrentWeaponOnHanded;

        if (currentWeapon == null) return;

        bool isNotUnarmed = currentWeapon.WeaponData.weaponType != UtilityDev.WeaponType.Unarmed;
        if (!currentWeapon.IsShethed && isNotUnarmed)
        {
            OnStartSheath();
        }
    }
    private void AttackUpdateHandler(WeaponScript currentWeapon)
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!photonView.IsMine) return; //����� computer ����ͧ ����ͧ������
        }
        
        if(currentWeapon == null) return;
        
        if(currentWeapon.WeaponData.weaponType != UtilityDev.WeaponType.SlingshotOrBow)
        {
            CooldownAttack();
            if (currentCooldownAttackTime <= 0f)
            { 
                if (playerLocomotion.SendMainActionSignal && isSheathing == false)
                {
                    if(Player.CheckboolActionLeftClick(OnInvokeAttack))
                    {
                        player.InvokeCallOnMainActionCalled();
                    }
                    currentCooldownAttackTime = cooldownAttackTime;
                    enableToSheath = false;
                }
            }
        }
       else if (currentWeapon.WeaponData.weaponType == UtilityDev.WeaponType.SlingshotOrBow)
        {
            ChargeableWeapon chargeableWeapon = equipment.CurrentWeaponOnHanded.GetComponent<ChargeableWeapon>();
            
            CooldownAttack();
            if (currentCooldownAttackTime <= 0f)
            {
                if (playerLocomotion.SendMainActionSignal && !chargeableWeapon.IsCharging)
                {
                    chargeableWeapon.StartCharging();
                    IsCharging = true;
                }
                else if (!playerLocomotion.SendMainActionSignal && chargeableWeapon.IsCharging)
                {
                    OnInvokeAttack();
                    currentCooldownAttackTime = cooldownAttackTime;
                    enableToSheath = false;
                    IsCharging = false;
                }
            }
            chargeableWeapon.UpdateCharge();
       }
    }
    private void CooldownAttack()
    {
        if(currentCooldownAttackTime > 0f)
        {
            currentCooldownAttackTime -= Time.deltaTime;
            if(currentCooldownAttackTime < 0.0f)
            {
                currentCooldownAttackTime = 0f;
                enableToSheath = true;
            }
        }
    }
    private void HandleImpact()
    {
        if (impact.magnitude > 0.2f)
        {
            // �к�����ش�ѵ��ѵ�����ͪ���ᾧ ���� CharacterController.Move ���к���Ǩ�Ѻ��ê�㹵����������
            controller.Move(impact * Time.deltaTime);
        }
        // ����� Ŵ�ç���ŧ (Friction)
        impact = Vector3.Lerp(impact, Vector3.zero, 10 * Time.deltaTime);
    }
    private IEnumerator DashTowardsTarget()
    {
        Vector3 startPos = transform.position + Vector3.up;

        // 1. �к� Snap (���ѵ�������ѹ˹��) - �� Code ����ͧ�س
        int rayCount = 8;
        float fanAngle = 50f;
        float minDistance = snapDistance;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (i - (rayCount - 1) / 2f) * (fanAngle / (rayCount - 1));
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(startPos, dir, out RaycastHit hit, snapDistance, enemyLayer))
            {
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    // �ѹ˹�����ѵ��
                    Vector3 dirToEnemy = (hit.collider.transform.position - transform.position);
                    dirToEnemy.y = 0;
                    transform.rotation = Quaternion.LookRotation(dirToEnemy);
                }
            }
        }

        // 2. ��觾�觴����к� Impact (����͹ Knockback)
        // ���ȷҧ������Ф��ѹ���� (transform.forward)
        impact = transform.forward * dashDistance;

        yield return null;
    }

    #region Attack Functions
    public void OnInvokeAttack()
    {
        WeaponScript weapon = equipment.CurrentWeaponOnHanded;
        float power = 0;

        // ถ้าเป็นอาวุธระยะไกล ให้ดึงค่าพลังจากการชาร์จออกมา
        if (weapon != null && weapon is RangeWeapon rangeWeapon)
        {
            power = rangeWeapon.GetChargeSystem().ReleaseCharge();
        }

        bool noCurrentWeaponOnHanded = weapon == null;
        ExecuteAttack(noCurrentWeaponOnHanded, power);
    }

    void ExecuteAttack(bool noWeaponOnHanded, float power)
    {
        if(photonView.IsMine && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_ExecuteAttack), RpcTarget.All, noWeaponOnHanded, power);
        }
        else if (!PhotonNetwork.InRoom)
        {
            LocalExcuteAttack(noWeaponOnHanded, power);
        }
    }
    [PunRPC]
    private void RPC_ExecuteAttack(bool noWeaponOnHanded, float power)
    {
        LocalExcuteAttack(noWeaponOnHanded, power);
    }
    private void LocalExcuteAttack(bool noWeaponOnHanded, float power)
    {
        if (noWeaponOnHanded == true)
        {
            equipment.SetNewHandedWeapon();
        }
        
        if(equipment.CurrentWeaponOnHanded.WeaponData.weaponType == UtilityDev.WeaponType.OneHandedMelee
        || equipment.CurrentWeaponOnHanded.WeaponData.weaponType == UtilityDev.WeaponType.Unarmed)
        {
            ComboNode getCN = equipment.CurrentWeaponOnHanded.WeaponData.GetComboAnimation(false);
            playerAnimation.SetAttackSpeed(3f);
            playerAnimation.PerformAttackAnimation(getCN);
            StartCoroutine(DashTowardsTarget());

            enableToSheath = false;

            Debug.Log("Perform Light Attack");
        }
        else if (equipment.CurrentWeaponOnHanded.WeaponData.weaponType == UtilityDev.WeaponType.SlingshotOrBow)
        {
            if (equipment.CurrentWeaponOnHanded is RangeWeapon rangeWeapon)
            {
                rangeWeapon.Fire(power);
            }
        }

    }
    #endregion
    #region Draw Weapon Functions   
    public void OnInvokeDrawed()
    {
        playerAnimation.SetOnUsingWeaponAnimation(true,equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber].WeaponData.weaponType);
        equipment.SetWeaponDrawPosition(currentIndexWeaponSlotNumber,equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber].WeaponData.weaponType);

    }

    public void OnStartDrawedWeapon(int weaponIndex)
    {
        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            // �͡�ء���������͹����ѹ�ѡ�Һ
            photonView.RPC(nameof(RPC_StartDrawWeapon), RpcTarget.All,weaponIndex);
        }
        else if (!PhotonNetwork.InRoom)
        {
            LocalStartDraw(weaponIndex);
        }
    }
    [PunRPC]
    private void RPC_StartDrawWeapon(int weaponIndex)
    {
        LocalStartDraw(weaponIndex);
    }

    private void LocalStartDraw(int weaponIndex)
    {
        playerAnimation.OnTriggerDrawOrSheathed(UtilityDev.DrawOrSheath.Draw,equipment.CurrentCarriedWeapons[weaponIndex].WeaponData.weaponType);
        playerLocomotion.SendMainActionSignal = false;
    }

    public void WeaponIsDrawed()
    {
        WeaponScript weapon = equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber];
        equipment.SetNewHandedWeapon(weapon);
        weapon.IsShethed = false;
    }
    #endregion
    #region Sheath Weapon Functions
    public void OnInvokeSheathed()
    {
        playerAnimation.SetOnUsingWeaponAnimation(false, lastWeaponTypeBeforeSheath);
    }

    public void OnStartSheath()
    {
        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            // �͡�ء���������͹����ѹ�ѡ�Һ
            photonView.RPC(nameof(RPC_StartSheathWeapon), RpcTarget.All);
        }
        else if (!PhotonNetwork.InRoom)
        {
            LocalStartSheath();
        }
    }
    [PunRPC]
    private void RPC_StartSheathWeapon()
    {
        LocalStartSheath();
    }
    private void LocalStartSheath()
    {
        isSheathing = true;
        lastWeaponTypeBeforeSheath = equipment.CurrentWeaponOnHanded.WeaponData.weaponType;
        playerAnimation.OnTriggerDrawOrSheathed(UtilityDev.DrawOrSheath.Sheath, lastWeaponTypeBeforeSheath);
    }

    public void WeaponIsSheathed()
    {
        WeaponScript weapon = equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber];
        weapon.IsShethed = true;
        isSheathing = false;
        
        int indexSlot = equipment.CurrentWeaponOnHanded.IndexSlotNumber;
        equipment.OnHandedCallShethedWeapon(indexSlot);
        equipment.SetNewHandedWeapon(); // hand reset
    }
    #endregion

    #region Hit Detection Functions

    public void TriggerHitboxFromWeaponAnimationEvent()
    {
        if(PhotonNetwork.InRoom && photonView.IsMine)
        {
            photonView.RPC(nameof(RPC_TriggerHitboxFromWeaponAnimationEvent), RpcTarget.All);
        }
        else if (!PhotonNetwork.InRoom)
        {
            LocalTriggerHitboxFromWeaponAnimationEvent();
        }
    }
    [PunRPC]
    private void RPC_TriggerHitboxFromWeaponAnimationEvent()
    {
        LocalTriggerHitboxFromWeaponAnimationEvent();
    }
    private void LocalTriggerHitboxFromWeaponAnimationEvent()
    {
        WeaponScript weapon = equipment.CurrentWeaponOnHanded;

        weapon.WeaponTrigger();
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        int rayCount = 8;
        float fanAngle = 50f;
        Vector3 startPos = transform.position + Vector3.up;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = (i - (rayCount - 1) / 2f) * (fanAngle / (rayCount - 1));
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Gizmos.DrawRay(startPos, dir * snapDistance);
        }
    }
}
