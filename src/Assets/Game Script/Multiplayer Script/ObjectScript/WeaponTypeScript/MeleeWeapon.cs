using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private Vector3 boxOffset = new Vector3(0, 1f, 1.5f); // ระยะห่างจากตัวละคร (X, Y, Z forward)
    [BoxGroup("Box Collider Hitbox")]
    [ShowIf(nameof(hitboxType), HitboxTriggerType.BoxCollider)]
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(1f, 1f, 1f); // ขนาดความกว้าง/สูง/ลึก ของกล่อง

    public bool EnableHitbox { get; private set; } = false;
    public Transform BladeBase { get { return bladeBase; } }
    public Transform BladeTip { get { return bladeTip; } }

    private List<IDamageable> damagedTargets = new List<IDamageable>();
    [SerializeField] private HitboxTriggerType hitboxType;
    private Action hitActionEventUpdate;
    private void OnEnable()
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
    private void OnDisable()
    {
        hitActionEventUpdate -= CapsuleColliderHitboxTrigger;
        hitActionEventUpdate -= HitboxColliderTrigger;
    }
    private void Update()
    {
        if (EnableHitbox == false)
            return;

        hitActionEventUpdate?.Invoke();
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

            if (damageable != null && isNotPlayer && isNotDamaged && !isWallBlocking)
            {
                float damageOut = WeaponData.damage;
                damagedTargets.Add(damageable);
                damageable.TakeDamage(WeaponData.damage);

                Vector3 knockbackDir = hit.transform.position - PlayerTransform.position;

                // 2. ปรับค่า Y เป็น 0 เพื่อให้กระเด็นในแนวราบเท่านั้น (กันมอนสเตอร์มุดดินหรือลอยฟ้าแบบแปลกๆ)
                knockbackDir.y = 0;
                knockback.Knockback(knockbackDir.normalized, WeaponData.knockbackForce);
            }
        }

    }
    private void HitboxColliderTrigger()
    {
        // 1. คำนวณตำแหน่งกลางกล่องให้อยู่ด้านหน้าตัวละครเสมอ
        // ใช้ transform.TransformPoint เพื่อให้ตำแหน่งขยับและหมุนตามตัวละครอัตโนมัติ
        Vector3 centerPosition = PlayerTransform.TransformPoint(boxOffset);

        // 2. ยิง BoxCast
        RaycastHit[] hits = Physics.BoxCastAll(
            centerPosition,
            boxHalfExtents,
            PlayerTransform.forward,
            PlayerTransform.rotation,
            0.1f, // ระยะ cast สั้นๆ เพื่อเช็คพื้นที่ ณ จุดนั้น
            LayerMask.GetMask("Enemy") // แนะนำให้ใส่ LayerMask เพื่อ Performance AI 32 ตัว
        );

        foreach (RaycastHit hitInfo in hits)
        {
            Collider hit = hitInfo.collider;
            if (hit.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                if (!damagedTargets.Contains(damageable) && !hit.CompareTag("Player"))
                {
                    // ตรวจสอบกำแพงกั้นก่อนทำดาเมจ
                    if (!IsWallBlocking(hit.transform.position))
                    {
                        // ระบบ Knockback
                        if (hit.TryGetComponent<IKnockback>(out IKnockback knockback))
                        {
                            Vector3 dir = (hit.transform.position - transform.position).normalized;
                            dir.y = 0;
                            knockback.Knockback(dir, WeaponData.knockbackForce);
                        }
                        damagedTargets.Add(damageable);
                        damageable.TakeDamage(WeaponData.damage);    
                    }
                }
            }
        }
    }
    #endregion
    public bool IsWallBlocking(Vector3 targetPOS)
    {
        // 1. ตั้งค่าจุดเริ่มต้น (ระดับอก) และคำนวณทิศทาง
        Vector3 start = PlayerTransform.transform.position + Vector3.up;

        // ปรับ target ให้สูงขึ้นเท่ากันเพื่อให้ Ray ยิงขนานพื้น
        Vector3 adjustedTarget = targetPOS + Vector3.up;
        Vector3 direction = adjustedTarget - start;
        float distance = direction.magnitude;

        // 2. ตั้งค่า LayerMask
        int wallMask = LayerMask.GetMask("Environment", "Obstacle");

        // 3. ยิง Raycast
        bool isHit = Physics.Raycast(start, direction.normalized, distance, wallMask);

        // --- ส่วนของ DEBUG ---
        // ถ้าชนกำแพงให้เส้นเป็นสีแดง (Blocked) ถ้าไม่ชนให้เป็นสีเขียว (Clear)
        Color debugColor = isHit ? Color.red : Color.green;
        Debug.DrawRay(start, direction.normalized * distance, debugColor, 0.5f);

        if (isHit)
        {
            // ช่วยบอกว่าชนวัตถุชื่ออะไรใน Console เพื่อเช็คว่า Layer ถูกไหม
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

                // คำนวณ Matrix ให้ Gizmos วาดตามตำแหน่งและมุมหมุนของตัวละคร
                Vector3 gizmoCenter = PlayerTransform.TransformPoint(boxOffset);
                Matrix4x4 cubeMatrix = Matrix4x4.TRS(gizmoCenter, PlayerTransform.rotation, Vector3.one);
                Gizmos.matrix = cubeMatrix;

                // วาดกล่อง (DrawWireCube ใช้ขนาดเต็ม จึงต้องคูณ 2 จาก halfExtents)
                Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2);
                break;
        }   
    }
}
