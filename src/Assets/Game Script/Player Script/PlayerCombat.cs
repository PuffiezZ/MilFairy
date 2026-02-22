using NaughtyAttributes;
using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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

    [Header("Attack Settings")]
    [SerializeField] private float cooldownAttackTime = 1.5f;
    
    private float currentCooldownAttackTime = 0f;

    private ComboNode combo;
    private bool isAttacking = false;
    public bool isSheathing { get; set; }
    private bool enableToSheath = false;
    private int comboIndex = 0;

    private float currentComboTime = 0f;
    private float resetComboTime = 1.8f;
    private float currentResetComboTime = 0f;
    private float nTime;

    public int currentIndexWeaponSlotNumber { get; set; }
    public UnityAction OnAttackAction;


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
        AttackUpdateHandler();
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
    private void AttackUpdateHandler()
    {
        if (!PhotonNetwork.OfflineMode)
        {
            if (!photonView.IsMine) return; //ไม่ใช่ computer ตัวเอง ไม่ต้องทำอะไร
        }

        if(currentCooldownAttackTime > 0f)
        {
            currentCooldownAttackTime -= Time.deltaTime;

            if(currentCooldownAttackTime < 0.0f)
            {
                currentCooldownAttackTime = 0f;
                enableToSheath = true;
            }
        }

        if (currentCooldownAttackTime <= 0f)
        {
            if (playerLocomotion.SendAttackSignal && isSheathing == false)
            {
                OnInvokeAttack();
                currentCooldownAttackTime = cooldownAttackTime;
                enableToSheath = false;
            }
        }

    }
    private void HandleImpact()
    {
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
        impact = transform.forward * dashDistance;

        yield return null;
    }

    private bool IsOnCooldown()
    {
        return currentComboTime > 0;
    }
    #region Attack Functions
    public void OnInvokeAttack()
    {
        WeaponScript weapon = equipment.CurrentWeaponOnHanded;

        bool noCurrentWeaponOnHanded = weapon == null;
        ExecuteAttack(noCurrentWeaponOnHanded);
    }

    void ExecuteAttack(bool noWeaponOnHanded)
    {
        if(photonView.IsMine && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_ExecuteAttack), RpcTarget.All,noWeaponOnHanded);
        }
        else if (!PhotonNetwork.InRoom)
        {
            LocalExcuteAttack(noWeaponOnHanded);
        }
    }
    [PunRPC]
    private void RPC_ExecuteAttack(bool noWeaponOnHanded)
    {
        LocalExcuteAttack(noWeaponOnHanded);
    }
    private void LocalExcuteAttack(bool noWeaponOnHanded)
    {
        if (noWeaponOnHanded == true)
        {
            equipment.SetNewHandedWeapon();
        }

        ComboNode getCN = equipment.CurrentWeaponOnHanded.WeaponData.GetComboAnimation(false);
        playerAnimation.SetAttackSpeed(3f);
        playerAnimation.PerformAttackAnimation(getCN);
        StartCoroutine(DashTowardsTarget());

        isAttacking = true;
        enableToSheath = false;

        Debug.Log("Perform Light Attack");

    }
    #endregion
    #region Draw Weapon Functions   
    public void OnInvokeDrawed()
    {
        playerAnimation.SetOnUsingWeaponAnimation(true);
        equipment.SetWeaponDrawPosition(currentIndexWeaponSlotNumber);

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
        playerLocomotion.SendAttackSignal = false;
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
