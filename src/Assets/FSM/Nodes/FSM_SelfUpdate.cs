using NodeCanvas.Framework;
using UnityEngine;
using static UtilityDev;

public class FSM_SelfUpdate : ActionTask
{
    public EnemyState stateToSync;

    protected override void OnExecute()
    {
        // เรียกผ่าน MonsterState ที่คุณทำระบบ RPC ไว้แล้ว
        var monsterState = agent.GetComponent<MonsterState>();
        if (monsterState != null)
        {
            //monsterState.CallUpdateStateFunc_FSM();
        }
        EndAction(true);
    }
}
