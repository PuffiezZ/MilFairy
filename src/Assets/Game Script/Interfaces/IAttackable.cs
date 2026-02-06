using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public Action OnStartAttack {  get; }
    public Action OnFinishAttack { get; }
    void OnCallAttack();

    void AttackHandle();
}
