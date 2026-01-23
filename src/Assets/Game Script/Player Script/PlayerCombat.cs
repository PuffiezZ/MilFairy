using NaughtyAttributes;
using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem.XR;
public class PlayerCombat : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    [SerializeField] private float dashDistance = 2.0f; // ระยะพุ่งปกติ
    [SerializeField] private float snapDistance = 4.0f; // ระยะตรวจจับศัตรูเพื่อพุ่งใส่
    [SerializeField] private LayerMask enemyLayer;     // เลือก Layer ที่เป็นศัตรู

    [SerializeField] private PlayerEquipment equipment;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerLocomotion playerLocomotion;
    private List<ComboNode> comboNodes = new List<ComboNode>();
    private bool isAttacking = false;
    public bool isSheathing { get; set; }
    private bool enableToSheath = false;
    private int comboIndex = 0;

    private float currentComboTime = 0f;
    private float resetComboTime = 1.8f;
    private float currentResetComboTime = 0f;
    private float nTime;

    public int currentIndexWeaponSlotNumber { get; set; }
    #region Combo Section
    [Foldout("Combo")]
    public ComboNode startingNode; 
    [Foldout("Combo")]
    private ComboNode currentComboNode;

    private CharacterController controller;
    private Vector3 impact = Vector3.zero;
    private float verticalVelocity = 0f;
    #endregion
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!photonView.IsMine) return;
        }
        if (currentComboTime > 0)
        {
            currentComboTime -= Time.deltaTime;
        }
        if(currentResetComboTime > 0)
        {
            currentResetComboTime -= Time.deltaTime;
        }
        else
        {
            comboIndex = 0;
            currentResetComboTime = 0f;
        }

        nTime = playerAnimation.GetNormalizedTime("Attack");
        if (isAttacking == true)
        {
            // ปลดล็อคเมื่อเล่นจบจริง หรือ หา Tag ไม่เจอ (-1)
            if (nTime >= currentComboNode.NormalizedTimeToNext && !IsOnCooldown())
            {
                isAttacking = false;
            }
        }

        if (!isAttacking && (nTime >= 0.98f || nTime <= -1f))
        {
            enableToSheath = true;
        }
        else
        {
            // ถ้ากำลังโจมตีอยู่ ห้ามเก็บอาวุธเด็ดขาด
            enableToSheath = false;
        }

        if (isAttacking == false)
        {
            if (playerLocomotion.EnableToAttack && isSheathing == false)
            {
                OnInvokeAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt) && enableToSheath && !isSheathing)
        {
            WeaponScript currentWeapon = equipment.CurrentWeaponOnHanded;

            if (currentWeapon == null) return;
            if (!currentWeapon.IsShethed)
            {
                OnStartSheath();
            }
        }
        if (impact.magnitude > 0.2f)
        {
            // ระบบจะหยุดอัตโนมัติเมื่อชนกำแพง เพราะ CharacterController.Move มีระบบตรวจจับการชนในตัวอยู่แล้ว
            controller.Move(impact * Time.deltaTime);
        }
        // ค่อยๆ ลดแรงพุ่งลง (Friction)
        impact = Vector3.Lerp(impact, Vector3.zero, 10 * Time.deltaTime);
    }
    private IEnumerator DashTowardsTarget()
    {
        Vector3 startPos = transform.position + Vector3.up;

        // 1. ระบบ Snap (หาศัตรูเพื่อหันหน้า) - ใช้ Code เดิมของคุณ
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
                    // หันหน้าหาศัตรู
                    Vector3 dirToEnemy = (hit.collider.transform.position - transform.position);
                    dirToEnemy.y = 0;
                    transform.rotation = Quaternion.LookRotation(dirToEnemy);
                }
            }
        }

        // 2. สั่งพุ่งด้วยระบบ Impact (เหมือน Knockback)
        // ใช้ทิศทางที่ตัวละครหันอยู่ (transform.forward)
        impact = transform.forward * 10f;

        yield return null;
    }

    private bool IsOnCooldown()
    {
        return currentComboTime > 0;
    }
    #region Attack Functions
    public void OnInvokeAttack()
    {
        WeaponScript weapon = equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber];
        if (weapon == null)
        {
            Debug.Log($"Weapon is null or empty ||");
            return;
        }

        if (weapon.IsShethed)
        {
            OnStartDrawedWeapon(); // Fixed Bug
        }
        else
        {
            ExecuteAttack(); // Current Fix!
        }

    }
    void ExecuteAttack()
    {
        if(photonView.IsMine && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_ExecuteAttack), RpcTarget.All);
        }
        else if (!PhotonNetwork.InRoom)
        {
            LocalExcuteAttack();
        }
    }
    [PunRPC]
    private void RPC_ExecuteAttack()
    {
        LocalExcuteAttack();
    }
    private void LocalExcuteAttack()
    {
        if (comboIndex >= comboNodes.Count)
        {
            comboIndex = 0;
        }
        currentComboNode = comboNodes[comboIndex];

        if (currentComboNode == null) return;

        currentComboNode.PlayerCombat = this;
        playerAnimation.SetAttackSpeed(currentComboNode.AttackSpeedCombo);
        playerAnimation.PerformAttackAnimation(currentComboNode);
        StartCoroutine(DashTowardsTarget());

        isAttacking = true;
        enableToSheath = false;

        currentResetComboTime = resetComboTime;
        currentComboTime = currentComboNode.DelayToNextCombo;

        comboIndex++;
        Debug.Log("Perform Light Attack");
    }
    #endregion
    #region Draw Weapon Functions   
    public void OnInvokeDrawed()
    {
        playerAnimation.SetOnUsingWeaponAnimation(true);
        equipment.SetDrawedEquipmentPOS(currentIndexWeaponSlotNumber);
        comboNodes = equipment.CurrentWeaponOnHanded.WeaponData.ComboList;
        //Play Animation Sheathed
    }

    public void OnStartDrawedWeapon()
    {
        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            // บอกทุกคนให้เล่นแอนิเมชันชักดาบ
            photonView.RPC(nameof(RPC_StartDrawWeapon), RpcTarget.All);
        }
        else if (!PhotonNetwork.InRoom)
        {
            LocalStartDraw();
        }
    }
    [PunRPC]
    private void RPC_StartDrawWeapon()
    {
        LocalStartDraw();
    }

    private void LocalStartDraw()
    {
        playerAnimation.OnTriggerDrawOrSheathed(UtilityDev.DrawOrSheath.Draw);
        playerLocomotion.EnableToAttack = false;
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
        equipment.SetSheathEquipmentPOS(currentIndexWeaponSlotNumber);
        playerAnimation.SetOnUsingWeaponAnimation(false);
    }

    public void OnStartSheath()
    {
        if (PhotonNetwork.InRoom && photonView.IsMine)
        {
            // บอกทุกคนให้เล่นแอนิเมชันชักดาบ
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
        playerAnimation.OnTriggerDrawOrSheathed(UtilityDev.DrawOrSheath.Sheath);
    }

    public void WeaponIsSheathed()
    {
        WeaponScript weapon = equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber];
        weapon.IsShethed = true;
        isSheathing = false;
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
