using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
using UnityEngine;
using static UtilityDev;

public class StateCheck : Conditional
{
    // ตัวแปรนี้จะไปโชว์ใน Behavior Designer Inspector ให้เราเลือก Enum ได้
    public ComparisonStateType ComparisonType;
    public EnemyState stateToCheck;

    // อ้างอิงสคริปต์หลักของ AI (เช่น ตัวควบคุม FSM หรือตัวแปรกลาง)
    private MonsterState enemyState;

    public override void OnStart()
    {
        enemyState = GetComponent<MonsterState>();
    }

    public override TaskStatus OnUpdate()
    {
        if(enemyState == null)
        {
            Debug.LogWarning("StateCheck: EnemyState component not found!");
            return TaskStatus.Failure;
        }
        if (enemyState == null) return TaskStatus.Failure;

        switch(ComparisonType)
        {
            case ComparisonStateType.EqualTo:
                if (enemyState.CurrentState == stateToCheck)
                {
                    return TaskStatus.Success;
                }
                else
                {
                    return TaskStatus.Failure;
                }
            case ComparisonStateType.NotEqualTo:
                if (enemyState.CurrentState != stateToCheck)
                {
                    return TaskStatus.Success;
                }
                else
                {
                    return TaskStatus.Failure;
                }
            default:
                return TaskStatus.Failure;
        }

    }
}