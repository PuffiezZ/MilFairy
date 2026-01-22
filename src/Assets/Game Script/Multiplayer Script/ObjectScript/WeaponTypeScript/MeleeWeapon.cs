using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponScript
{
    [Header("Melee Weapon Raycast")]
    [SerializeField] private Transform bladeBase;
    [SerializeField] private Transform bladeTip;
    [SerializeField] private float swordRadius = 0.3f;
    public bool EnableHitbox { get; private set; } = false;

    public Transform BladeBase { get { return bladeBase; } }
    public Transform BladeTip { get { return bladeTip; } }

    private List<IDamageable> damagedTargets = new List<IDamageable>();

    private void Update()
    {
        if (EnableHitbox == false)
            return;
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
                knockback.Knockback(knockbackDir.normalized, 5f);
            }
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

    public override void WeaponAnimationEventTrigger()
    {
            
    }
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
        if(EnableHitbox == false)
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
    }
}
