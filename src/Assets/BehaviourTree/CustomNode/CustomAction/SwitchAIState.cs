using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using UnityEngine;
using static UtilityDev;

public class SwitchAIState : Action
{
    public EnemyState newState;
    private MonsterState monsterState;
    public override void OnAwake()
    {
        monsterState = GetComponent<MonsterState>();
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterState == null) return TaskStatus.Failure;

        // สั่งเปลี่ยนสถานะผ่านสคริปต์ที่คุณเขียน
        monsterState.CallChangeStateFunc(newState);
        //GetComponent<BehaviorTree>().RestartBehavior();
        return TaskStatus.Success;
    }
}
