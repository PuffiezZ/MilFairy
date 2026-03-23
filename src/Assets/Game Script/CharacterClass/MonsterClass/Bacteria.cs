using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bacteria : MonsterBase
{
    [SerializeField] private float biteRadius = 0.5f;
    [SerializeField] private float biteDistance = 1.0f;
    [SerializeField] private float yOffset = 0.5f;

    private List<IDamageable> damagedTargets = new List<IDamageable>();
    public override void AttackHandle()
    {
        EnableHitBoxAttack = !EnableHitBoxAttack;
        if (EnableHitBoxAttack == false)
        {
            damagedTargets.Clear();
        }
    }
    private void  Update() 
    {
        if (EnableHitBoxAttack == false)
            return;  
        BittingAttack(); 
    }

    private void BittingAttack()
    {
        Vector3 origin = transform.position + (Vector3.up * yOffset);

        Vector3 direction = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, biteRadius, direction, biteDistance,LayerMask.GetMask("Player","Damageable"));
        foreach (var hit in hits)
        {
            IDamageable idamageableGO = hit.collider.GetComponent<IDamageable>();
            if (idamageableGO != null && !damagedTargets.Contains(idamageableGO))
            {
                damagedTargets.Add(idamageableGO);
                idamageableGO.TakeDamage(monsterData.GetStatValue("AttackDamage"), gameObject);
                Debug.Log($"Bite hit: {hit.collider.name}");
            }
        }
        IsAttacking = false;
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

        Gizmos.DrawWireSphere(origin, biteRadius);
        Gizmos.DrawWireSphere(targetLocation, biteRadius);

        Gizmos.DrawLine(origin, targetLocation);
    }
}
