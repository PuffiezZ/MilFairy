using NUnit.Framework;
using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks;
using Opsive.BehaviorDesigner.Runtime.Tasks.Actions;
using Opsive.GraphDesigner.Runtime.Variables;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FindNearestPlayer : Action
{
    public SharedVariable<GameObject> targetGO;
    public float detectionRadius = 20f; // กำหนดระยะตรวจจับ
    public LayerMask playerLayer; // ตั้งเป็น Layer ของ Player

    private List<PhotonView> viewList = new List<PhotonView>(); 
    private MonsterState monsterState;
    public override void OnStart()
    {
        monsterState = GetComponent<MonsterState>();

        viewList.Clear();
        GameObject[] playerGameObject= GameObject.FindGameObjectsWithTag("Player");

        foreach(var go in playerGameObject)
        {
            if(go.TryGetComponent<PhotonView>(out PhotonView pv))
            {
                viewList.Add(pv);
            }
        }
    }
    public override TaskStatus OnUpdate()
    {
        // 1. หา Collider ของผู้เล่นในระยะรอบตัวเท่านั้น
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        if (hitPlayers.Length == 0) return TaskStatus.Failure;

        Transform nearestTransform = null;
        float minDistance = Mathf.Infinity;

        // 2. หาตัวที่ใกล้ที่สุด
        foreach (var hit in hitPlayers)
        {
            float dist = (hit.transform.position - transform.position).sqrMagnitude; // sqrMagnitude เร็วกว่า Distance
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestTransform = hit.transform;
            }
        }

        if (nearestTransform != null)
        {
            targetGO.Value = nearestTransform.gameObject;
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }   
}
