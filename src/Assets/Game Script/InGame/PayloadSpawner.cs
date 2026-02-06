using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.FilePathAttribute;
// using NaughtyAttributes; // ถ้าใช้ BoxGroup ให้ uncomment บรรทัดนี้

public class PayloadSpawner : MonoBehaviourPunCallbacks
{
    [Header("Prefab Settings")]
    public GameObject enemyPrefab;

    [Header("Spawn Location Settings")]
    public Transform spawnPointParent; // ลากตัวแม่ที่เก็บจุดเกิดทั้งหมดมาใส่
    [Tooltip("ระยะห่างต่ำสุดจาก Payload (ห้ามเกิดใกล้กว่านี้)")]
    public float minSpawnRadius = 10f;
    [Tooltip("ระยะห่างสูงสุดจาก Payload (ห้ามเกิดไกลกว่านี้)")]
    public float maxSpawnRadius = 25f;

    [Header("Spawn Logic Timing")]
    public float spawnInterval = 5f;
    public int maxEnemies = 32;

    private List<Transform> allSpawnPoints = new List<Transform>();
    private float timer;

    void Start()
    {
        // เก็บจุดเกิดทั้งหมดไว้ใน List ตั้งแต่เริ่ม
        if (spawnPointParent != null)
        {
            // ดึงลูกๆ ทั้งหมดที่เป็นจุดเกิดมาเก็บไว้
            allSpawnPoints = spawnPointParent.GetComponentsInChildren<Transform>().Where(t => t != spawnPointParent.transform).ToList();
        }
    }

    void Update()
    {
        // ทำงานเฉพาะบน Master Client เท่านั้น
        if (!PhotonNetwork.IsMasterClient) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            // เช็คจำนวน Enemy ทั้งหมดในฉาก (รองรับ Object Pooling ถ้า Enemy ที่ตายแล้วถูก SetActive(false))
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
        // 1. กรองจุดเกิดที่อยู่ในระยะวงแหวนรอบๆ Payload
        Vector3 currentPos = transform.position;
        var nearbyPoints = allSpawnPoints.Where(p => {
            float dist = Vector3.Distance(currentPos, p.position);
            return dist >= minSpawnRadius && dist <= maxSpawnRadius;
        }).ToList();

        // 2. ถ้ามีจุดที่เข้าเงื่อนไข ให้สุ่มเกิด
        if (nearbyPoints.Count > 0)
        {
            Transform selectedPoint = nearbyPoints[Random.Range(0, nearbyPoints.Count)];
            NetworkPrefabSpawner.Instance.SpawnEntity(enemyPrefab, selectedPoint.position,selectedPoint.rotation);
        }
    }

    // ==================================================
    // ส่วนของ Debug Gizmos (แสดงผลเมื่อคลิกที่ตัว PayloadSpawner)
    // ==================================================
    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position;

        // 1. วาด WireSphere แสดงรัศมีต่ำสุด (สีแดง)
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // แดงโปร่งแสง
        Gizmos.DrawWireSphere(center, minSpawnRadius);

        // 2. วาด WireSphere แสดงรัศมีสูงสุด (สีฟ้า)
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f); // ฟ้าโปร่งแสง
        Gizmos.DrawWireSphere(center, maxSpawnRadius);

        // 3. วาดเส้นเชื่อมไปยังจุดเกิดทั้งหมด เพื่อดูว่าจุดไหนอยู่ในระยะบ้าง
        if (spawnPointParent != null)
        {
            // ถ้ายังไม่ได้กด Play ใช้การดึงสดจาก Parent มาโชว์ก่อน
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
                    // จุดที่อยู่ในระยะ (Valid): สีเขียว
                    Gizmos.color = Color.green; Gizmos.DrawLine(center, point.position);
                    Gizmos.DrawSphere(point.position, 0.5f); // วาดจุดกลมๆ
                }
                else
                {
                    // จุดที่อยู่นอกระยะ (Invalid): สีเทาจางๆ
                    Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                    Gizmos.DrawLine(center, point.position);
                    Gizmos.DrawWireSphere(point.position, 0.3f); // วาดแค่วงกลมบางๆ
                }
            }
        }
    }
}