using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using UnityEngine;

public class IsPlayerInSight : Conditional
{
    private MonsterPerception monsterCtrl;
    public override void OnStart()
    {
        monsterCtrl = GetComponent<MonsterPerception>();
    }

    public override TaskStatus OnUpdate()
    {
        //if (monsterCtrl == null)
        //{
        //    Debug.LogWarning("monsterController is Null!");
        //    return TaskStatus.Failure;
        //}

        //if (monsterCtrl.FindVisibleTargets())
        //{
        //    return TaskStatus.Success;
        //}
        //else
        //{
        //    return TaskStatus.Failure;
        //}

        return TaskStatus.Success;
    }
}
