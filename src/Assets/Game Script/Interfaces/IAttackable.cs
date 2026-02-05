using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public bool IsAttacking { get; }
    void OnCallAttack();

    void AttackHandle();
}
