using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bacteria : MonsterBase
{
    private const float biteRadius = 0.5f;
    private const float biteDistance = 1.0f;
    private const float yOffset = 0.5f;

    public override void AttackHandle()
    {
        float dashForce = monsterData.GetStatValue("DashForce");
        float dashDuration = monsterData.GetStatValue("DashDuration");

        if (gameObject.activeSelf)
        {
            StartCoroutine(BitingAttack());
        }

    }
    private void OnDisable()
    {
        StopCoroutine(BitingAttack());
    }
    private IEnumerator BitingAttack()
    {
        float time = 0f;

        while(time < 1f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        // คำนวณจุดเริ่มต้น (Origin) โดยบวก Offset แกน Y
        Vector3 origin = transform.position + (Vector3.up * yOffset);
        // ทิศทางที่จะยิง Raycast ออกไป
        Vector3 direction = transform.forward;

        // ใช้ SphereCast เพื่อเช็คการปะทะตามทิศทางด้านหน้า
        RaycastHit[] hits = Physics.SphereCastAll(origin, biteRadius, direction, biteDistance, LayerMask.GetMask("Player"));
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out IDamageable victim) && victim.EnableDamage)
            {
                victim.TakeDamage(monsterData.GetStatValue("AttackDamage"));
                Debug.Log($"Bite hit: {hit.collider.name}");
            }
        }
        IsAttacking = false;
        OnFinishAttack?.Invoke();
        yield break;
    }
    public IEnumerator DashAttack(Vector3 direction, float force, float duration)
    {
        float startTime = Time.time;
        IsAttacking = true;

        while (Time.time < startTime + duration)
        {
            // 1. คำนวณระยะที่ต้องการจะไปในเฟรมนี้
            float frameDistance = force * Time.deltaTime;
            Vector3 nextPos = transform.position + direction * frameDistance;

            // 2. ใช้ Linecast ตรวจสอบเส้นทางจากจุดปัจจุบันไปจุดถัดไป
            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.up, nextPos + Vector3.up, out hit))
            {
                if (!hit.collider.isTrigger)
                {
                    // หากเจอสิ่งกีดขวาง ให้เลื่อนตัวไปหยุดที่หน้าสิ่งนั้นพอดี
                    transform.position = hit.point - direction * 0.2f;
                    Debug.Log("Dash Blocked by: " + hit.collider.name);
                    break;
                }
            }

            // 3. หากไม่มีอะไรขวาง ค่อยใช้ Move เพื่อให้ติดขอบ NavMesh
            aiAgent.Move(direction * frameDistance);
            yield return null;
        }
        OnFinishAttack?.Invoke();
    }
    private void OnDrawGizmosSelected()
    {
        if (IsAttacking)
        {
            Gizmos.color = Color.cyan;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Vector3 origin = transform.position + (Vector3.up * yOffset);
        Vector3 targetLocation = origin + (transform.forward * biteDistance);

        // วาดทรงกลมที่จุดเริ่มและจุดจบ
        Gizmos.DrawWireSphere(origin, biteRadius);
        Gizmos.DrawWireSphere(targetLocation, biteRadius);

        // วาดเส้นเชื่อมระหว่างทรงกลมทั้งสอง
        Gizmos.DrawLine(origin, targetLocation);
    }
}
