using NaughtyAttributes;
using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
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
    private bool isSheathing = false;
    private bool enableToSheath = false;
    private int currentIndexWeaponSlotNumber = 0;
    private int comboIndex = 0;

    private float currentComboTime = 0f;
    private float resetComboTime = 1.8f;
    private float currentResetComboTime = 0f;
    private float nTime;
    #region Combo Section
    [Foldout("Combo")]
    public ComboNode startingNode; 
    [Foldout("Combo")]
    private ComboNode currentComboNode;
    #endregion

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
            if (nTime >= currentComboNode.normalizedTimeToNext && !IsOnCooldown())
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
            currentIndexWeaponSlotNumber = 0;
            WeaponScript currentWeapon = equipment.CurrentWeaponOnHanded;

            if (currentWeapon == null) return;
            if (!currentWeapon.IsShethed)
            {
                OnStartSheath();
            }
        }

    }
    private IEnumerator DashTowardsTarget()
    {
        Vector3 startPos = transform.position + Vector3.up; // ยิงจากระดับอก
        Vector3 dashDirection = transform.forward;
        float finalDistance = dashDistance;

        // --- ส่วนการยิง Ray หลายเส้น ---
        int rayCount = 8; // จำนวนเส้นที่จะยิง
        float fanAngle = 50f; // องศาที่กระจายออก (ซ้าย 15 ขวา 15)
        RaycastHit closestHit;
        bool hasHit = false;
        float minDistance = snapDistance;

        for (int i = 0; i < rayCount; i++)
        {
            // คำนวณองศาของแต่ละเส้น
            float angle = (i - (rayCount - 1) / 2f) * (fanAngle / (rayCount - 1));
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;

            // วาด Ray ใน Console เพื่อ Debug (สีแดง)
            Debug.DrawRay(startPos, dir * snapDistance, Color.red, 1f);

            if (Physics.Raycast(startPos, dir, out RaycastHit hit, snapDistance, enemyLayer))
            {
                if (hit.distance < minDistance)
                {
                    minDistance = hit.distance;
                    closestHit = hit;
                    hasHit = true;

                    // หันหน้าหาตัวที่ใกล้ที่สุด
                    Vector3 dirToEnemy = (hit.collider.transform.position - transform.position);
                    dirToEnemy.y = 0;
                    transform.rotation = Quaternion.LookRotation(dirToEnemy);
                    dashDirection = transform.forward;
                }
            }
        }

        if (hasHit)
        {
            // คำนวณระยะหยุด: ระยะชนลบออก 0.7f (กันทะลุ)
            finalDistance = Mathf.Max(0, minDistance - 0.7f);
        }

        // --- ส่วนการเคลื่อนที่ ---
        Vector3 targetPos = transform.position + (dashDirection * finalDistance);

        // Check กำแพงกันวาร์ปทะลุฉาก
        if (Physics.Raycast(startPos, dashDirection, out RaycastHit wallHit, finalDistance, LayerMask.GetMask("Environment")))
        {
            targetPos = wallHit.point - (dashDirection * 0.5f);
        }

        float elapsed = 0;
        float duration = 0.1f;
        Vector3 initialPos = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(initialPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
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
        playerAnimation.SetAttackSpeed(currentComboNode.attackSpeedCombo);
        playerAnimation.PerformAttackAnimation(currentComboNode.AnimationOverrideCtrl);
        StartCoroutine(DashTowardsTarget());

        isAttacking = true;
        enableToSheath = false;

        currentResetComboTime = resetComboTime;
        currentComboTime = currentComboNode.delayToNextCombo;

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
