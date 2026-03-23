using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// using NaughtyAttributes; // ����� BoxGroup ��� uncomment ��÷Ѵ���

public class PayloadSpawner : MonoBehaviourPunCallbacks
{
    [Header("Prefab Settings")]
    public GameObject enemyPrefab;

    [Header("Spawn Location Settings")]
    public Transform spawnPointParent; // �ҡ���������纨ش�Դ�����������
    [Tooltip("������ҧ����ش�ҡ Payload (�����Դ�����ҹ��)")]
    public float minSpawnRadius = 10f;
    [Tooltip("������ҧ�٧�ش�ҡ Payload (�����Դ�š��ҹ��)")]
    public float maxSpawnRadius = 25f;

    [Header("Spawn Logic Timing")]
    public float spawnInterval = 5f;
    public int maxEnemies = 32;

    [Header("Spawn Limits & Toggles")]
    public bool isSpawningEnabled = true;
    [Tooltip("จำนวนครั้งทั้งหมดที่อนุญาตให้ Spawn ได้ (ตั้งเป็นค่าที่ต้องการ)")]
    public int maxTotalSpawns = 10;
    private int currentTotalSpawned = 0;

    private List<Transform> allSpawnPoints = new List<Transform>();
    private float timer;

    void Start()
    {
        // �纨ش�Դ���������� List ����������
        if (spawnPointParent != null)
        {
            // �֧�١� ����������繨ش�Դ�������
            allSpawnPoints = spawnPointParent.GetComponentsInChildren<Transform>().Where(t => t != spawnPointParent.transform).ToList();
        }
    }

    void Update()
    {
        // �ӧҹ੾�к� Master Client ��ҹ��
        if (!PhotonNetwork.IsMasterClient) return;

        // หยุดทำงานหากไม่ได้เปิดใช้งาน หรือ Spawn ครบจำนวนที่กำหนดแล้ว
        if (!isSpawningEnabled) return;
        if (currentTotalSpawned >= maxTotalSpawns) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            // �礨ӹǹ Enemy ������㹩ҡ (�ͧ�Ѻ Object Pooling ��� Enemy ��������Ƕ١ SetActive(false))
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Count(e => e.activeInHierarchy);

            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
            timer = 0;
        }
    }

    void SpawnEnemy()
    {
        // 1. ��ͧ�ش�Դ������������ǧ��ǹ�ͺ� Payload
        Vector3 currentPos = transform.position;
        var nearbyPoints = allSpawnPoints.Where(p => {
            float dist = Vector3.Distance(currentPos, p.position);
            return dist >= minSpawnRadius && dist <= maxSpawnRadius;
        }).ToList();

        // 2. ����ըش���������͹� ��������Դ
        if (nearbyPoints.Count > 0)
        {
            Transform selectedPoint = nearbyPoints[Random.Range(0, nearbyPoints.Count)];
            NetworkPrefabSpawner.Instance.SpawnEntity(enemyPrefab, selectedPoint.position,selectedPoint.rotation);
            
            // นับจำนวนการ Spawn ที่สำเร็จ
            currentTotalSpawned++;
        }
    }

    public void SetSpawnerActive(bool isActive)
    {
        isSpawningEnabled = isActive;
    }

    public void ResetSpawnCount()
    {
        currentTotalSpawned = 0;
        timer = 0f; // รีเซ็ตเวลาเริ่มต้นการ Spawn ใหม่ด้วย
    }

    // ==================================================
    // ��ǹ�ͧ Debug Gizmos (�ʴ�������ͤ�ԡ����� PayloadSpawner)
    // ==================================================
    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position;

        // 1. �Ҵ WireSphere �ʴ�����յ���ش (��ᴧ)
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // ᴧ����ʧ
        Gizmos.DrawWireSphere(center, minSpawnRadius);

        // 2. �Ҵ WireSphere �ʴ�������٧�ش (�տ��)
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f); // �������ʧ
        Gizmos.DrawWireSphere(center, maxSpawnRadius);

        // 3. �Ҵ����������ѧ�ش�Դ������ ���ʹ���Ҩش�˹��������к�ҧ
        if (spawnPointParent != null)
        {
            // ����ѧ����顴 Play ���ô֧ʴ�ҡ Parent ������͹
            var points  = allSpawnPoints.Count > 0 && Application.isPlaying
                ? allSpawnPoints
                : spawnPointParent.GetComponentsInChildren<Transform>().Where(t => t != spawnPointParent.transform).ToList();

            foreach (Transform point in points)
            {
                if (point == null) continue;

                float dist = Vector3.Distance(center, point.position);
                bool isInRange = dist >= minSpawnRadius && dist <= maxSpawnRadius;

                if (isInRange)
                {
                    // �ش������������ (Valid): ������
                    Gizmos.color = Color.green; Gizmos.DrawLine(center, point.position);
                    Gizmos.DrawSphere(point.position, 0.5f); // �Ҵ�ش����
                }
                else
                {
                    // �ش�������͡���� (Invalid): ���Ҩҧ�
                    Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                    Gizmos.DrawLine(center, point.position);
                    Gizmos.DrawWireSphere(point.position, 0.3f); // �Ҵ��ǧ����ҧ�
                }
            }
        }
    }
}