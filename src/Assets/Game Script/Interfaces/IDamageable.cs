using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool EnableDamage { get; set; }
    void TakeDamage(float damage, GameObject source = null);
}
