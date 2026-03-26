using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toothy : MonsterBase
{
    public override void AttackHandle()
    {
        
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
}
