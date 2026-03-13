using System.Collections;
using UnityEngine;

public class Bacteria : MonsterBase
{
    private const float biteRadius = 0.5f;
    private const float biteDistance = 1.0f;
    private const float yOffset = 0.5f;

    public override void AttackHandle()
    {
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

        Vector3 origin = transform.position + (Vector3.up * yOffset);

        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, biteRadius, direction, biteDistance, LayerMask.GetMask("Player"));
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out IDamageable victim) && victim.EnableDamage)
            {
                victim.TakeDamage(monsterData.GetStatValue("AttackDamage"), gameObject);
                Debug.Log($"Bite hit: {hit.collider.name}");
            }
        }
        IsAttacking = false;
        OnFinishAttack?.Invoke();
        yield break;
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

        Gizmos.DrawWireSphere(origin, biteRadius);
        Gizmos.DrawWireSphere(targetLocation, biteRadius);

        Gizmos.DrawLine(origin, targetLocation);
    }
}
