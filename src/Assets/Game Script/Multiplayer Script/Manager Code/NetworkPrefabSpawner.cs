using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPrefabSpawner : MonoBehaviour
{
    public static NetworkPrefabSpawner Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    public void SpawnResource(string namePrefab,PhotonView photonView)
    {
        // 1. ถ้าออนไลน์อยู่ ให้เช็ค MasterClient เท่านั้น
        if (PhotonNetwork.InRoom)
        {
            if (!photonView.IsMine) return;
            // ถ้าไม่ใช่ Master ให้หยุดทำงานทันที ไม่ต้องไปทำ else
            if (!PhotonNetwork.IsMasterClient) return;

            Vector3 spawnPosition = photonView.transform.position + (photonView.transform.forward * 2f) + (photonView.transform.up * 2f);
            Quaternion spawnRotation = photonView.transform.rotation;
            // เฉพาะ MasterClient เท่านั้นที่รันบรรทัดนี้
            PhotonNetwork.InstantiateRoomObject(namePrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"<color=cyan>[NetworkSpawner]</color> Master Spawned {namePrefab}");
        }
        else
        {
            // 2. ถ้าเล่น Offline (Solo) จริง ๆ ถึงจะทำส่วนนี้
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            if (pool.ResourceCache.TryGetValue(namePrefab, out GameObject prefab))
            {
                Vector3 spawnPosition = photonView.transform.position + (photonView.transform.forward * 2f) + (photonView.transform.up * 2f);
                Instantiate(prefab, spawnPosition, Quaternion.identity);
                Debug.Log($"<color=yellow>[NetworkSpawner]</color> Offline Spawned {prefab.name}");
            }
        }
    }
}
