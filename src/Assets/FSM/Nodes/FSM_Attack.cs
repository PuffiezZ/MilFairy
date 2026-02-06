using System;
using UnityEngine;
using NodeCanvas.Framework; // เพื่อใช้ BBParameter
using UnityEngine.AI;

public class FSM_Attack : ActionTask<MonsterBase>
{
    protected override void OnExecute()
    {
        agent.OnFinishAttack += EndAction;
        agent.OnCallAttack();
    }
}
