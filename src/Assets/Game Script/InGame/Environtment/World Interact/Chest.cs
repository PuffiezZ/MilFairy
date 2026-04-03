using UnityEngine;
using Photon.Pun;

public class Chest : MonoBehaviourPun
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab ที่ต้องการสร้าง (ต้องอยู่ในโฟลเดอร์ Resources สำหรับ PUN2)")]
    [SerializeField] private GameObject objectToSpawn;
    
    [Tooltip("จุดที่ต้องการให้ Object ปรากฏ")]
    [SerializeField] private Transform spawnPoint;

    [Header("Offsets")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    public void SpawnObject()
    {
        if (objectToSpawn == null)
        {
            Debug.LogWarning($"[Chest] {gameObject.name} ไม่มี GameObject ในช่อง objectToSpawn!");
            return;
        }

        if(spawnPoint == null)
        {
            Debug.LogWarning($"[Chest] doesn't have a spawn point!");
            return;
        }
        Transform targetPoint = spawnPoint;

        // คำนวณตำแหน่งและหมุนโดยรวมค่า Offset
        Vector3 finalPosition = targetPoint.position + targetPoint.TransformDirection(positionOffset);
        Quaternion finalRotation = targetPoint.rotation * Quaternion.Euler(rotationOffset);

        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            // Multiplayer: ใช้ชื่อ Prefab ในการสร้างเพื่อให้ Sync ทุกเครื่อง
            PhotonNetwork.Instantiate(objectToSpawn.name, finalPosition, finalRotation);
        }
        else
        {
            // Solo / Offline: ใช้ Instantiate ปกติ
            Instantiate(objectToSpawn, finalPosition, finalRotation);
        }
    }
}
