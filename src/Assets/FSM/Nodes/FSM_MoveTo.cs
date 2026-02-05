using System;
using UnityEngine;
using NodeCanvas.Framework; // เพื่อใช้ BBParameter
using UnityEngine.AI;

[Serializable]
public class FSM_MoveTo : ActionTask
{
    public BBParameter<Vector3> targetPOS;
    public BBParameter<GameObject> targetGO;
    private NavMeshAgent aiAgentNav;
    bool isInRange;
    protected override void OnExecute()
    {
        aiAgentNav = agent.GetComponent<NavMeshAgent>();
        isInRange = false;
        UpdatePath();
    }

    protected override void OnUpdate()
    {
        // ตรวจสอบว่าตำแหน่งใน Blackboard เปลี่ยนไปจากที่ Agent กำลังมุ่งหน้าไปหรือไม่
        if (Vector3.Distance(aiAgentNav.transform.position, targetPOS.value) > aiAgentNav.stoppingDistance)
        {
            UpdatePath();
            EndAction();
        }
        else
        {
            aiAgentNav.isStopped = true;
            isInRange = true;
            OnReachToTarget();
            EndAction();
        }
    }
    protected override void OnStop()
    {
        isInRange = false;
    }

    private void UpdatePath()
    {
        if (targetGO.value != null)
        {
            aiAgentNav.SetDestination(targetGO.value.transform.position);
        }
    }

    private void OnReachToTarget()
    {
        MonsterState mState = agent.GetComponent<MonsterState>();
        if (mState == null) return;

        bool monsterIsStop = aiAgentNav.isStopped;
        Debug.Log($"Monster Check State Var: monsterIsStop == {monsterIsStop},IsInRange {isInRange}");
        if (monsterIsStop && isInRange)
        {
            mState.CallChangeStateFunc(UtilityDev.EnemyState.Attack);
        }
        else
        {
            mState.CallChangeStateFunc(UtilityDev.EnemyState.Idle);
        }

    }
}

