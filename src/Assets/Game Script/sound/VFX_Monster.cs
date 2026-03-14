using UnityEngine;

public class VFX_Monster : MonoBehaviour
{
    [Header("VFX Settings")]
    [Tooltip("Prefab ของเอฟเฟคที่จะแสดงเวลาโดนโจมตี (เช่น Particle System)")]
    [SerializeField] private GameObject hitEffectPrefab;
    [Tooltip("จุดที่จะให้เอฟเฟคปรากฏ (ถ้าว่างไว้จะเกิดที่ตำแหน่งมอนสเตอร์)")]
    [SerializeField] private Transform effectSpawnPoint;
    [SerializeField] private float destroyDelay = 1.0f;

    private MonsterBase monsterBase;

    private void Awake()
    {
        monsterBase = GetComponent<MonsterBase>();
        if (monsterBase != null)
        {
            // ลงทะเบียนรอรับ Event เมื่อมอนสเตอร์โดนดาเมจ
            monsterBase.OnMonsterHurt += PlayHitEffect;
        }
    }

    private void OnDestroy()
    {
        if (monsterBase != null)
        {
            monsterBase.OnMonsterHurt -= PlayHitEffect;
        }
    }

    private void PlayHitEffect()
    {
        if (hitEffectPrefab != null)
        {
            Vector3 spawnPos = effectSpawnPoint != null ? effectSpawnPoint.position : transform.position;
            GameObject effect = Instantiate(hitEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(effect, destroyDelay);
        }
    }
}