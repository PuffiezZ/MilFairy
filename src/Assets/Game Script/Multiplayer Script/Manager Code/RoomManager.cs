using Photon.Pun;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerObject; // ต้องอยู่ในโฟลเดอร์ Resources เท่านั้น
    [SerializeField] private Transform[] spawnPoints; // ใช้ array เพื่อสุ่มจุดเกิด

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
        }
    }
    void Start()
    {
        // ถ้าเราเปลี่ยน Scene มาโดยที่ยังอยู่ในห้อง (Joined Room แล้ว)
        if (PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
        else
        {
            Transform selectedPoint = spawnPoints[0];

            GameObject player = Instantiate(playerObject, selectedPoint.position, Quaternion.identity);

            player.GetComponent<PlayerSetup>().IsLocalPlayer();
        }
    }

    // กรณีที่อาจจะโหลด Scene มาก่อนเข้าห้องสำเร็จ
    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // สุ่มจุดเกิดไม่ให้ทับกัน
        Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // สร้างตัวละครผ่าน Network
        GameObject player = PhotonNetwork.Instantiate(playerObject.name, selectedPoint.position, Quaternion.identity);

        // เรียก Setup เพื่อเปิดกล้องเฉพาะเครื่องเรา
        player.GetComponent<PlayerSetup>().IsLocalPlayer();
    }
}