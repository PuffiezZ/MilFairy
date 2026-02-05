using NodeCanvas.Framework; // เพื่อใช้ BBParameter
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class FSM_FindPayloadPoint : ActionTask
{
    public BBParameter<GameObject> payloadGameObject;
    public BBParameter<Vector3> pointVectorTarget;
    public BBParameter<float> updateInterval = 0.2f; // ปรับความถี่การคำนวณเพื่อประหยัด CPU

    private BoxCollider payloadCollider;
    private float lastUpdateTime;
    private NavMeshAgent navMeshAgent;

    protected override void OnExecute()
    {
        if (payloadGameObject.value == null) { EndAction(false); return; }

        // Cache Collider ไว้ตั้งแต่เริ่มเพื่อไม่ให้หาใหม่ทุกเฟรม
        payloadCollider = payloadGameObject.value.GetComponent<BoxCollider>();
        navMeshAgent = agent.GetComponent<NavMeshAgent>();

        UpdateDestination();
    }

    protected override void OnUpdate()
    {
        // คำนวณใหม่เป็นระยะ (เช่น ทุก 0.2 วินาที) แทนการคำนวณทุกเฟรม เพื่อเพิ่ม FPS
        if (Time.time - lastUpdateTime > updateInterval.value)
        {
            UpdateDestination();
            EndAction(true);
        }
    }

    private void UpdateDestination()
    {
        if (payloadCollider == null) return;

        // 1. บันทึกเวลาที่ทำการ "พยายาม" อัปเดตครั้งนี้ก่อน
        lastUpdateTime = Time.time;

        Vector3 targetPoint = payloadCollider.ClosestPoint(agent.transform.position);

        // 2. เช็คระยะห่างเพื่อส่งค่าไป Blackboard
        if (Vector3.Distance(navMeshAgent.destination, targetPoint) > navMeshAgent.stoppingDistance)
        {
            pointVectorTarget.value = targetPoint;
            Debug.Log($"Update pointVectorTarget: {pointVectorTarget.value}");
            // หาก MoveTo ของคุณอ่านค่าจาก Blackboard ตัว AI จะเริ่มเดินใหม่ทันทีครับ
        }
    }
}
