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
        // �ӹǳ�ش������� (Origin) �ºǡ Offset ᡹ Y
        Vector3 origin = transform.position + (Vector3.up * yOffset);
        // ��ȷҧ�����ԧ Raycast �͡�
        Vector3 direction = transform.forward;

        // �� SphereCast �����礡�ûзе����ȷҧ��ҹ˹��
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
            // 1. �ӹǳ���з���ͧ��è���������
            float frameDistance = force * Time.deltaTime;
            Vector3 nextPos = transform.position + direction * frameDistance;

            // 2. �� Linecast ��Ǩ�ͺ��鹷ҧ�ҡ�ش�Ѩ�غѹ仨ش�Ѵ�
            RaycastHit hit;
            if (Physics.Linecast(transform.position + Vector3.up, nextPos + Vector3.up, out hit))
            {
                if (!hit.collider.isTrigger)
                {
                    // �ҡ����觡մ��ҧ �������͹������ش���˹����觹�鹾ʹ�
                    transform.position = hit.point - direction * 0.2f;
                    Debug.Log("Dash Blocked by: " + hit.collider.name);
                    break;
                }
            }

            // 3. �ҡ��������â�ҧ ������ Move �������Դ�ͺ NavMesh
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

        // �Ҵ�ç������ش�������Шش��
        Gizmos.DrawWireSphere(origin, biteRadius);
        Gizmos.DrawWireSphere(targetLocation, biteRadius);

        // �Ҵ�������������ҧ�ç�������ͧ
        Gizmos.DrawLine(origin, targetLocation);
    }
}
