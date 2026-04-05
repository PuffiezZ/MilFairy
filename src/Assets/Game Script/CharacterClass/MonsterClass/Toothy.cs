using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toothy : MonsterBase
{
    [SerializeField] private float attackRadius = 0.5f;
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
    private void Update() 
    {
        if (EnableHitBoxAttack == false)
            return;  
         
        SpinAttack();
    }

    private void SpinAttack()
    {
        Vector3 origin = transform.position + (Vector3.up * yOffset);

        // ใช้ OverlapSphere ในการตรวจจับรอบตัวแบบ 360 องศาสำหรับท่า Spin
        Collider[] hits = Physics.OverlapSphere(origin, attackRadius, LayerMask.GetMask("Player", "Damageable"));
        
        foreach (var hit in hits)
        {
            IDamageable idamageableGO = hit.GetComponent<IDamageable>();
            if (idamageableGO != null && !damagedTargets.Contains(idamageableGO))
            {
                damagedTargets.Add(idamageableGO);
                idamageableGO.TakeDamage(monsterData.GetStatValue("AttackDamage"), gameObject);
                Debug.Log($"Spin hit: {hit.name}");
            }
        }
        IsAttacking = false;
        OnFinishAttack?.Invoke();
    }

    public override void TakeDamage(float damage, GameObject source = null)
    {
        SoundFXManager.instance.PlayGlobalSound("tooty_hurt",this.transform.position);
        base.TakeDamage(damage, source);
    }
    public override void Die()
    {
        SoundFXManager.instance.PlayGlobalSound("tooty_die",this.transform.position);
        base.Die();
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
        Gizmos.DrawWireSphere(origin, attackRadius);
    }
}
