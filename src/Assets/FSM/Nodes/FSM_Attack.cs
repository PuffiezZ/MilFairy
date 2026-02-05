using System;
using UnityEngine;
using NodeCanvas.Framework; // เพื่อใช้ BBParameter
using UnityEngine.AI;

public class FSM_Attack : ActionTask<MonsterBase>
{
    protected override void OnExecute()
    {
        agent.OnCallAttack();
    }
    protected override void OnUpdate()
    {
        //if (agent.IsAttackRotating)
        //{
        //    GameObject target = agent.MonsterState.FSMblackboard.GetVariableValue<GameObject>("TargetObject");

        //    if (target != null)
        //    {
        //        // คำนวณทิศทางไปยังเป้าหมายในแนวระนาบ (XZ Plane)
        //        Vector3 direction = (target.transform.position - agent.transform.position).normalized;
        //        direction.y = 0; // ป้องกันไม่ให้มอนสเตอร์แหงนหน้าขึ้นลง

        //        if (direction != Vector3.zero)
        //        {
        //            // สร้าง Rotation และใช้ Slerp เพื่อให้การหมุนดูนุ่มนวล (Smooth Rotation)
        //            Quaternion lookRotation = Quaternion.LookRotation(direction);
        //            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * 5f);
        //        }
        //    }
        //}
        if (!agent.IsAttacking)
        {
            // เมื่อโจมตีจบ ให้จบ Action นี้เพื่อส่งสัญญาณให้ FSM เปลี่ยนไป State ถัดไป
            EndAction();
        }
    }
    protected override void OnStop()
    {
        //GameObject target = agent.MonsterState.FSMblackboard.GetVariableValue<GameObject>("TargetObject");
        //float targetDistance = Vector3.Distance(agent.transform.position, target.transform.position);

        //if(targetDistance > agent.NavAIMesh.stoppingDistance)
        //{
        //    OnHandle_OutDistance();
        //}
        //else
        //{
        //    OnHandle_InDistance();
        //}
    }

    private void OnHandle_OutDistance()
    {
        GameObject target = agent.MonsterState.FSMblackboard.GetVariableValue<GameObject>("TargetObject");
        if (target.GetComponent<PayloadScript>())
        {
            agent.MonsterState.CallChangeStateFunc(UtilityDev.EnemyState.ChasePayload);
        }
        else if(target.GetComponent<Player>())
        {
            agent.MonsterState.CallChangeStateFunc(UtilityDev.EnemyState.Chase);
        }
    }

    private void OnHandle_InDistance()
    {
        agent.MonsterState.CallChangeStateFunc(UtilityDev.EnemyState.Idle);
    }
}
