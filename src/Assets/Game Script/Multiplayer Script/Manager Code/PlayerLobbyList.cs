using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLobbyList : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private Transform contentParent; // ลาก PlayerListContent มาใส่
    [SerializeField] private GameObject playerEntryPrefab; // ลาก Prefab ที่มี TMP มาใส่

    [Scene]
    public string sceneName;

    // ใช้ Dictionary เพื่อเก็บข้อมูลว่า Player คนไหนคู่กับ UI ชิ้นไหน (เพื่อจะได้ลบถูกคนเมื่อเขาออก)
    private Dictionary<Photon.Realtime.Player, GameObject> playerListEntries = new Dictionary<Photon.Realtime.Player, GameObject>();

    private void Awake()
    {
        // บรรทัดนี้สำคัญมาก ต้องตั้งค่าเป็น true
        // เพื่อให้ Client ทุกคนโหลด Scene ตาม Master Client อัตโนมัติ
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToGamePlayScene()
    {
        // 1. ตรวจสอบว่าเป็น Host (Master Client) หรือไม่
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the Host can start the game!");
            return;
        }

        // 2. ตรวจสอบว่าทุกคนโหลด Scene เสร็จหรือยัง (Optional แต่แนะนำ)
        // ในที่นี้เราจะสั่งโหลด Scene ชื่อ "Gameplay" (เปลี่ยนชื่อตาม Scene ของคุณ)
        Debug.Log("Host is starting the game...");

        // 3. ใช้คำสั่งของ Photon แทน SceneManager.LoadScene
        // คำสั่งนี้จะทำให้ทุกคนในห้องเปลี่ยน Scene ไปพร้อมกัน
        PhotonNetwork.LoadLevel(sceneName);
    }

    // เรียกทำงานเมื่อเรา (ตัวเอง) เข้าห้องสำเร็จ
    public override void OnJoinedRoom()
    {
        // เคลียร์รายชื่อเก่า (ถ้ามี)
        foreach (GameObject entry in playerListEntries.Values) 
            Destroy(entry);
        playerListEntries.Clear();

        // สร้างรายชื่อของทุกคนที่ "อยู่ในห้องก่อนแล้ว" รวมทั้งตัวเราเอง
        foreach (Photon.Realtime.Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            AddPlayerListing(player);
        }
    }

    // เรียกทำงานเมื่อ "คนอื่น" join เข้ามา
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //newPlayer.NickName =  
        AddPlayerListing(newPlayer);
    }

    // เรียกทำงานเมื่อ "คนอื่น" ออกจากห้อง
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (playerListEntries.ContainsKey(otherPlayer))
        {
            Destroy(playerListEntries[otherPlayer]);
            playerListEntries.Remove(otherPlayer);
        }
    }

    // ฟังก์ชันสำหรับสร้าง UI และตั้งชื่อ
    private void AddPlayerListing(Photon.Realtime.Player player)
    {
        GameObject entry = Instantiate(playerEntryPrefab, contentParent);
        if (entry != null)
        {
            // หา Component TextMeshPro ใน Prefab แล้วเปลี่ยนเป็นชื่อผู้เล่น
            TextMeshProUGUI text = entry.GetComponent<TextMeshProUGUI>();
            text.text = player.NickName;

            // ถ้าชื่อว่าง ให้ใส่เป็น Player + ID แทน
            if (string.IsNullOrEmpty(player.NickName))
                text.text = "Player " + player.ActorNumber;

            // เก็บใส่ Dictionary ไว้
            playerListEntries.Add(player, entry);
            Debug.Log($"Added Player: {player.NickName}");
        }
    }
}
