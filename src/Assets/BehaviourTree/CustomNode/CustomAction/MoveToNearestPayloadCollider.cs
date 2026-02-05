using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using UnityEngine;
using UnityEngine.AI;

public class MoveToNearestPayloadCollider : Action
{
    public SharedVariable<GameObject> payloadGO;

    private NavMeshAgent navMeshAgent;
    private float reachThreshold = 0.1f;
    private MonsterBase monsterBase;
    public override void OnStart()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        monsterBase = GetComponent<MonsterBase>();

        if (navMeshAgent != null)
        {
            reachThreshold = monsterBase.StopDistanceToTarget;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (navMeshAgent == null || payloadGO.Value == null) return TaskStatus.Failure;

        BoxCollider boxCollider = payloadGO.Value.GetComponent<BoxCollider>();
        Vector3 getPOS = boxCollider.ClosestPoint(transform.position);

        // แก้ไข: สั่ง SetDestination เฉพาะเมื่อเป้าหมายขยับไปไกลพอ (เช่น 0.2 เมตร) 
        // เพื่อลดอาการเดินหน่วงจากการคำนวณ Path ใหม่ทุกเฟรม
        if (Vector3.Distance(navMeshAgent.destination, getPOS) > 0.2f)
        {
            navMeshAgent.SetDestination(getPOS);
        }

        // เช็คระยะห่างโดยใช้ PathPending ป้องกันค่า remainingDistance เป็น 0 ตอนเริ่มเฟรมแรก
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= reachThreshold)
        {
            navMeshAgent.isStopped = true;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
