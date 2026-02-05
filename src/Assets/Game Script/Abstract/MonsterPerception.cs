using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UtilityDev;

public class MonsterPerception : MonoBehaviour
{
    [BoxGroup("Controller Setting")]
    [SerializeField] private MonsterState monsterState;
    [BoxGroup("Controller Setting")]
    [SerializeField] private Vector3 radiusCastOffset = new Vector3(0f, 0.5f, 0f);
    [BoxGroup("Controller Setting")]
    [SerializeField] private LayerMask detectMask;
    [BoxGroup("Controller Setting")]
    [SerializeField] private LayerMask obstructionMask;

    [BoxGroup("Vision Controller Setting")]
    [SerializeField] private float detectRadius = 5f;
    [BoxGroup("Vision Controller Setting")]
    [SerializeField] private float fovAngle = 90f;

    private List<GameObject> visibleTargets = new List<GameObject>();
    private MonsterBase monster;
    private void Start()
    {
         monster = GetComponent<MonsterBase>();
    }
    private void Update()
    {
        visibleTargets.Clear();
        FindVisibleTargets();
        if (visibleTargets.Count > 0)
        {
            GameObject target = visibleTargets[0];

            if (target == null || monster == null)
            {
                return;
            }
            // เช็คว่าเป็นผู้เล่น และยังไม่ได้อยู่ในสเตท Chase
            if (target.GetComponent<Player>())
            {
                monsterState.FSMblackboard.SetVariableValue("PlayerInVision", target);
                GameObject currentPlayerInVision = monsterState.FSMblackboard.GetVariableValue<GameObject>("PlayerInVision");

                if (monsterState.FSMblackboard.GetVariableValue<GameObject>("FirstSeenPlayer") != currentPlayerInVision)
                {
                    monsterState.FSMblackboard.SetVariableValue("FirstSeenPlayer", currentPlayerInVision);
                    monsterState.FSMblackboard.SetVariableValue("TargetObject", currentPlayerInVision);
                }
                //monsterState.CallChangeStateFunc(EnemyState.Chase);
            }
            //else
            //{
            //    // ถ้าเจอเป้าหมายแต่ไม่ใช่ผู้เล่น (เช่น เจอรถ Payload) และยังไม่ได้ ChasePayload อยู่
            //    if (monsterState.CurrentState != EnemyState.ChasePayload)
            //    {
            //        GameObject payloadGB = monsterState.FSMblackboard.GetVariableValue<GameObject>("PayloadGameobject");
            //        monster.TargetObject = payloadGB;

            //        monsterState.FSMblackboard.SetVariableValue("TargetObject", monster.TargetObject);
            //        monsterState.CallChangeStateFunc(EnemyState.ChasePayload);
            //    }
            //}
        }
        else
        {
            monsterState.FSMblackboard.SetVariableValue("PlayerInVision", null);
        }
        //else
        //{
        //    // กรณีไม่เจออะไรเลย ให้กลับไปหา Payload (ถ้ายังไม่ทำอยู่)
        //    if (monsterState.CurrentState != EnemyState.ChasePayload)
        //    {
        //        GameObject payloadGB = monsterState.FSMblackboard.GetVariableValue<GameObject>("PayloadGameobject");
        //        monster.TargetObject = payloadGB;

        //        monsterState.FSMblackboard.SetVariableValue("TargetObject", monster.TargetObject);
        //        monsterState.CallChangeStateFunc(EnemyState.ChasePayload);
        //    }
        //}
    }
    public bool FindVisibleTargets()
    {
        // 1. ตรวจจับรอบตัวด้วย Sphere ก่อน
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position + radiusCastOffset, detectRadius, detectMask);

        foreach (Collider target in targetsInRadius)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            // 2. เช็คว่าอยู่ในมุมมอง (FOV) หรือไม่
            if (Vector3.Angle(transform.forward, directionToTarget) < fovAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 3. เช็คว่าไม่มีอะไรบัง (Line of Sight)
                if (!Physics.Raycast(transform.position + radiusCastOffset, directionToTarget, distanceToTarget, obstructionMask))
                {
                    visibleTargets.Add(target.gameObject);
                    return true;
                }
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        // วาดวงกลมระยะตรวจจับ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + radiusCastOffset, detectRadius);

        // วาดเส้นขอบเขต FOV
        Vector3 forward = transform.forward;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-fovAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(fovAngle / 2, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * forward;
        Vector3 rightRayDirection = rightRayRotation * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + radiusCastOffset, transform.position + radiusCastOffset + leftRayDirection * detectRadius);
        Gizmos.DrawLine(transform.position + radiusCastOffset, transform.position + radiusCastOffset + rightRayDirection * detectRadius);
    }
}
