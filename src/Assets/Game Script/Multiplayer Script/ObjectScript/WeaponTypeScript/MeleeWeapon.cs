using NaughtyAttributes;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UtilityDev;

public class MeleeWeapon : WeaponScript
{
    [BoxGroup("Capsule Colldier Hitbox")]
    [ShowIf(nameof(hitboxType), HitboxTriggerType.CapsuleCollider)]
    [SerializeField] private Transform bladeBase;
    [BoxGroup("Capsule Colldier Hitbox")]
    [ShowIf(nameof(hitboxType), HitboxTriggerType.CapsuleCollider)]
    [SerializeField] private Transform bladeTip;
    [BoxGroup("Capsule Colldier Hitbox")]
    [ShowIf(nameof(hitboxType), HitboxTriggerType.CapsuleCollider)]
    [SerializeField] private float swordRadius = 0.3f;

    [BoxGroup("Box Collider Hitbox")]
    [ShowIf(nameof(hitboxType), HitboxTriggerType.BoxCollider)]
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 1f, 1.5f); // ������ҧ�ҡ����Ф� (X, Y, Z forward)
    [BoxGroup("Box Collider Hitbox")]
    [ShowIf(nameof(hitboxType), HitboxTriggerType.BoxCollider)]
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f); // ��Ҵ�������ҧ/�٧/�֡ �ͧ���ͧ


    public bool EnableHitbox { get; private set; } = false;
    public Transform BladeBase { get { return bladeBase; } }
    public Transform BladeTip { get { return bladeTip; } }

    private List<IDamageable> damagedTargets = new List<IDamageable>();
    [SerializeField] private HitboxTriggerType hitboxType;
    private Action hitActionEventUpdate;
    private void OnEnable()
    {
        RegisterHitbox();
    }

    private void OnDisable()
    {
        hitActionEventUpdate -= CapsuleColliderHitboxTrigger;
        hitActionEventUpdate -= HitboxColliderTrigger;
    }

    private void Update()
    {
        if (EnableHitbox == false)
            return;

        // สำคัญ: ป้องกันไม่ให้เครื่องเพื่อนคำนวณ Hitbox ซ้ำซ้อน และป้องกัน Error จาก PlayerTransform เป็น null
        if (PhotonNetwork.InRoom && !photonView.IsMine)
            return;

        hitActionEventUpdate?.Invoke();
    }

    public void RegisterHitbox()
    {
        hitActionEventUpdate = null;
        switch (hitboxType)
        {
            case HitboxTriggerType.CapsuleCollider:
                hitActionEventUpdate += CapsuleColliderHitboxTrigger;
                break;
            case HitboxTriggerType.BoxCollider:
                hitActionEventUpdate += HitboxColliderTrigger;
                break;
        }
    }

    public override void WeaponTrigger()
    {
        EnableHitbox = !EnableHitbox;
        if (EnableHitbox == false)
        {
            damagedTargets.Clear();
        }
    }

    #region Hitbox Function
    private void CapsuleColliderHitboxTrigger()
    {
        Collider[] hitCollision = null;
        hitCollision = Physics.OverlapCapsule(bladeBase.position, bladeTip.position, swordRadius);

        foreach (Collider hit in hitCollision)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            IKnockback knockback = hit.GetComponent<IKnockback>();
            bool isNotPlayer = !hit.CompareTag("Player");
            bool isNotDamaged = !damagedTargets.Contains(damageable);
            bool isWallBlocking = IsWallBlocking(hit.transform.position);

            if (damageable != null && isNotPlayer && isNotDamaged && !isWallBlocking && damageable.EnableDamage)
            {
                float damageOut = WeaponData.damage;
                damagedTargets.Add(damageable);
                damageable.TakeDamage(WeaponData.damage,PlayerTransform.gameObject);

                Vector3 knockbackDir = hit.transform.position - PlayerTransform.position;

                // 2. ��Ѻ��� Y �� 0 ���������������Һ��ҹ�� (�ѹ�͹������ش�Թ������¿��Ẻ�š�)
                knockbackDir.y = 0;
                knockback.Knockback(knockbackDir.normalized, WeaponData.knockbackForce);
            }
        }

    }
    private void HitboxColliderTrigger()
    {
        // 1. �ӹǳ���˹觡�ҧ���ͧ��������ҹ˹�ҵ���Ф�����
        // �� transform.TransformPoint ���������˹觢�Ѻ�����ع�������Ф��ѵ��ѵ�
        Vector3 centerPosition = PlayerTransform.TransformPoint(boxOffset);

        // 2. �ԧ BoxCast
        RaycastHit[] hits = Physics.BoxCastAll(
            centerPosition,
            boxHalfExtents,
            PlayerTransform.forward,
            PlayerTransform.rotation,
            0.1f, // ���� cast ���� �����社�鹷�� � �ش���
            LayerMask.GetMask("Enemy","Damageable") // �й������� LayerMask ���� Performance AI 32 ���
        );

        foreach (RaycastHit hitInfo in hits)
        {
            Collider hit = hitInfo.collider;
            if (hit.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                if (!damagedTargets.Contains(damageable) && !hit.CompareTag("Player"))
                {
                    // ��Ǩ�ͺ��ᾧ��鹡�͹�Ӵ����
                    if (!IsWallBlocking(hit.transform.position))
                    {
                        damagedTargets.Add(damageable);
                        damageable.TakeDamage(WeaponData.damage, PlayerTransform.gameObject);
                        // �к� Knockback
                        if (hit.TryGetComponent<IKnockback>(out IKnockback knockback))
                        {
                            Vector3 knockbackDir = hit.transform.position - PlayerTransform.position;

                            // 2. ��Ѻ��� Y �� 0 ���������������Һ��ҹ�� (�ѹ�͹������ش�Թ������¿��Ẻ�š�)
                            knockbackDir.y = 0;
                            knockback.Knockback(knockbackDir.normalized, WeaponData.knockbackForce);
                        }   
                    }
                }
            }
        }
    }
    #endregion
    public bool IsWallBlocking(Vector3 targetPOS)
    {
        // 1. ��駤�Ҩش������� (�дѺ͡) ��Фӹǳ��ȷҧ
        Vector3 start = PlayerTransform.transform.position + Vector3.up;

        // ��Ѻ target ����٧�����ҡѹ������� Ray �ԧ��ҹ���
        Vector3 adjustedTarget = targetPOS + Vector3.up;
        Vector3 direction = adjustedTarget - start;
        float distance = direction.magnitude;

        // 2. ��駤�� LayerMask
        int wallMask = LayerMask.GetMask("Obstacle");

        // 3. �ԧ Raycast
        bool isHit = Physics.Raycast(start, direction.normalized, distance, wallMask);

        // --- ��ǹ�ͧ DEBUG ---
        // ��Ҫ���ᾧ����������ᴧ (Blocked) �����誹����������� (Clear)
        Color debugColor = isHit ? Color.red : Color.green;
        Debug.DrawRay(start, direction.normalized * distance, debugColor, 0.5f);

        if (isHit)
        {
            // ���º͡��Ҫ��ѵ�ت�������� Console ��������� Layer �١���
            if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, distance, wallMask))
            {
                Debug.Log($"<color=red>Blocked by:</color> {hit.collider.name} on Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            }
        }
        // ----------------------

        return isHit;
    }
    private void OnDrawGizmos()
    {
        switch (hitboxType)
        {
            case HitboxTriggerType.CapsuleCollider:
                if (EnableHitbox == false)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.green;

                }
                Gizmos.DrawWireSphere(bladeBase.position, swordRadius);
                Gizmos.DrawWireSphere(bladeTip.position, swordRadius);
                Gizmos.DrawLine(bladeBase.position + Vector3.up * swordRadius, bladeTip.position + Vector3.up * swordRadius);
                Gizmos.DrawLine(bladeBase.position - Vector3.up * swordRadius, bladeTip.position - Vector3.up * swordRadius);
                Gizmos.DrawLine(bladeBase.position + Vector3.right * swordRadius, bladeTip.position + Vector3.right * swordRadius);
                Gizmos.DrawLine(bladeBase.position - Vector3.right * swordRadius, bladeTip.position - Vector3.right * swordRadius);
                break;

            case HitboxTriggerType.BoxCollider:
                if ((PlayerTransform) == null) return;
                Gizmos.color = EnableHitbox ? Color.green : Color.red;

                // �ӹǳ Matrix ��� Gizmos �Ҵ������˹���������ع�ͧ����Ф�
                Vector3 gizmoCenter = PlayerTransform.TransformPoint(boxOffset);
                Matrix4x4 cubeMatrix = Matrix4x4.TRS(gizmoCenter, PlayerTransform.rotation, Vector3.one);
                Gizmos.matrix = cubeMatrix;

                // �Ҵ���ͧ (DrawWireCube �颹Ҵ��� �֧��ͧ�ٳ 2 �ҡ halfExtents)
                Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);
                break;
        }   
    }
}
