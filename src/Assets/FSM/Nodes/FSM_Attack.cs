using System;
using UnityEngine;
using NodeCanvas.Framework; // ������ BBParameter
using UnityEngine.AI;

public class FSM_Attack : ActionTask<MonsterBase>
{
    protected override void OnExecute()
    {
        if(agent.OnFinishAttack == null)
            agent.OnFinishAttack += EndAction;
            
        agent.OnCallAttack();
    }
}
