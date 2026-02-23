using UnityEngine;
using NaughtyAttributes;
using Photon.Pun;

public class EnvironmentSpawner : MonoBehaviourPun
{
    public enum SpawnCondition { EveryHit, Chance, HealthPercentage }
    public enum NetworkMode { MasterClientOnly, AllClients }

    [Header("Spawn Settings")]
    [SerializeField] private GameObject prefabToSpawn;

    [Tooltip("ใส่จุดเกิดได้หลายจุด ระบบจะสุ่มเลือก 1 จุดในการเกิดแต่ละครั้ง")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Logic")]
    [SerializeField] private SpawnCondition condition = SpawnCondition.EveryHit;
    [SerializeField] private NetworkMode networkMode = NetworkMode.MasterClientOnly;

    [ShowIf(nameof(condition), SpawnCondition.Chance)]
    [Range(0, 100)]
    [SerializeField] private float spawnChance = 50f;

    [ShowIf(nameof(condition), SpawnCondition.HealthPercentage)]
    [Range(0, 100)]
    [SerializeField] private float thresholdPercentage = 50f;

    [Header("Random Offset")]
    [SerializeField] private bool useRandomOffset = true;
    [SerializeField] private float offsetRange = 0.5f;

    private EnvironmentDamageable _damageable;
    private bool _hasSpawnedThreshold = false;

    private void Awake()
    {
        _damageable = GetComponent<EnvironmentDamageable>();

        // ถ้าไม่มีการใส่จุดเกิดเลย ให้ใช้ตัวเองเป็นจุดเกิดสำรอง
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new Transform[] { transform };
        }
    }

    // ฟังก์ชันนี้ถูกเรียกจาก OnHit ของ EnvironmentDamageable
    public void OnHitTrigger()
    {
        if (prefabToSpawn == null) return;

        // ตรวจสอบสิทธิ์การ Spawn ในระบบ Network
        if (PhotonNetwork.InRoom)
        {
            if (networkMode == NetworkMode.MasterClientOnly && !PhotonNetwork.IsMasterClient)
                return; // ถ้าตั้งเป็น MasterOnly แต่เราไม่ใช่ Master ก็ไม่ต้องทำอะไร
        }

        // เช็คเงื่อนไข
        switch (condition)
        {
            case SpawnCondition.EveryHit:
                ExecuteSpawn();
                break;

            case SpawnCondition.Chance:
                if (Random.Range(0f, 100f) <= spawnChance)
                    ExecuteSpawn();
                break;

            case SpawnCondition.HealthPercentage:
                HandleHealthPercentageSpawn();
                break;
        }
    }

    private void HandleHealthPercentageSpawn()
    {
        if (_damageable == null || _hasSpawnedThreshold) return;

        float currentHealthPercent = (_damageable.CurrentHealth / _damageable.MaxHealth) * 100f;

        if (currentHealthPercent <= thresholdPercentage)
        {
            ExecuteSpawn();
            _hasSpawnedThreshold = true;
        }
    }

    private void ExecuteSpawn()
    {
        // --- ส่วนที่เพิ่มใหม่: สุ่ม Index ของจุดเกิด ---
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform selectedPoint = spawnPoints[randomIndex];

        // ตรวจสอบเผื่อกรณีมี Index ใน Array แต่ไม่มี Object (Missing)
        if (selectedPoint == null) 
            selectedPoint = transform;

        Vector3 finalPos = selectedPoint.position;
        if (useRandomOffset)
        {
            finalPos += new Vector3(Random.Range(-offsetRange, offsetRange), 0, Random.Range(-offsetRange, offsetRange));
        }

        if (PhotonNetwork.InRoom && networkMode == NetworkMode.MasterClientOnly)
        {
            // Photon ใช้ชื่อ Prefab ในการ Instantiate (ต้องอยู่ใน Resources)
            PhotonNetwork.Instantiate(prefabToSpawn.name, finalPos, selectedPoint.rotation);
        }
        else
        {
            Instantiate(prefabToSpawn, finalPos, selectedPoint.rotation);
        }
    }
}