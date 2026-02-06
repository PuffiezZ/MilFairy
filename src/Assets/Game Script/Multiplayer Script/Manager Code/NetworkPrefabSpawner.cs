using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPrefabSpawner : MonoBehaviour, IPunPrefabPool
{
    public static NetworkPrefabSpawner Instance;
    // เก็บ Prefab ที่จะใช้เป็นต้นแบบ
    [SerializeField] private GameObject monsterPrefab;
    private Stack<GameObject> pool = new Stack<GameObject>();

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
    public GameObject SpawnEntity(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            return PhotonNetwork.Instantiate(prefab.name, position, rotation);
        }
        else
        {
            return this.Instantiate(prefab.name, position, rotation);
        }
    }
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Pop();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true); // ปลุกมอนสเตอร์ให้กลับมาทำงาน
        }
        else
        {
            // ถ้าใน Pool หมด ให้สร้างใหม่
            obj = Instantiate(monsterPrefab, position, rotation);
        }
        return obj;
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false); // ปิดการทำงานแทนการลบทิ้ง
        pool.Push(gameObject); // เก็บเข้า Stack เพื่อรอใช้ใหม่
    }
}
