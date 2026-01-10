using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPrefabRegister : MonoBehaviour
{
    public static NetworkPrefabRegister Instance;

    [Header("Add your prefabs here")]
    [SerializeField] private List<GameObject> prefabsToRegister;

    private void Awake()
    {
        // ทำเป็น Singleton เพื่อให้เรียกใช้ง่าย
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        RegisterPrefabs();
    }

    private void RegisterPrefabs()
    {
        // เข้าถึง PrefabPool ของ Photon
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;

        if (pool != null)
        {
            foreach (GameObject prefab in prefabsToRegister)
            {
                if (prefab != null)
                {
                    string namePrefab = prefab.name;
                    // ลงทะเบียน Prefab เข้าไปในระบบโดยใช้ชื่อของมัน
                    // ตอนเรียก Spawn ต้องใช้ชื่อที่ตรงกัน (แนะนำ prefab.name)
                    if (!pool.ResourceCache.ContainsKey(namePrefab))
                    {
                        pool.ResourceCache.Add(namePrefab, prefab);
                        Debug.Log($"<color=green>Registered:</color> {namePrefab} to Photon Pool");
                    }
                }
            }
        }
    }

}
