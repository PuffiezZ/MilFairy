using NodeCanvas.Framework; // ������ BBParameter
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class FSM_FindPayloadPoint : ActionTask
{
    public BBParameter<GameObject> payloadGameObject;
    public BBParameter<Vector3> pointVectorTarget;
    public BBParameter<float> updateInterval = 0.2f; // ��Ѻ��������äӹǳ���ͻ����Ѵ CPU

    private BoxCollider payloadCollider;
    private float lastUpdateTime;
    private NavMeshAgent navMeshAgent;

    protected override void OnExecute()
    {
        if (payloadGameObject.value == null) { EndAction(false); return; }

        // Cache Collider ����������������������������ء���
        payloadCollider = payloadGameObject.value.GetComponent<BoxCollider>();
        navMeshAgent = agent.GetComponent<NavMeshAgent>();

        UpdateDestination();
    }

    protected override void OnUpdate()
    {
        // �ӹǳ���������� (�� �ء 0.2 �Թҷ�) ᷹��äӹǳ�ء��� �������� FPS
        if (Time.time - lastUpdateTime > updateInterval.value)
        {
            UpdateDestination();
            EndAction(true);
        }
    }

    private void UpdateDestination()
    {
        if (payloadCollider == null) return;

        lastUpdateTime = Time.time;

        Vector3 targetPoint = payloadCollider.ClosestPoint(agent.transform.position);

        // 2. ��������ҧ�����觤��� Blackboard
        if (Vector3.Distance(navMeshAgent.destination, targetPoint) > navMeshAgent.stoppingDistance)
        {
            pointVectorTarget.value = targetPoint;
            //Debug.Log($"Update pointVectorTarget: {pointVectorTarget.value}");
        }
    }
}
