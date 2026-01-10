using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayeLobbyConnectHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private RectTransform lobbyPage_Rect;
    [SerializeField] private RectTransform mainMenu_Rect;
    // ฟังก์ชันสำหรับผูกกับปุ่ม "Leave"
    public void OnClickLeaveButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // ถ้าเป็น Host ให้ส่งคำสั่ง RPC ไปหาทุกคนในห้อง (รวมตัวเองด้วย)
            // ให้ทุกคนรันฟังก์ชันที่ชื่อว่า "EveryoneLeaveRoom"
            this.photonView.RPC("EveryoneLeaveRoom", RpcTarget.All);
        }
        else
        {
            // ถ้าเป็น Client ทั่วไป ให้กดออกคนเดียวปกติ
            PhotonNetwork.LeaveRoom();
        }
    }

    [PunRPC]
    public void EveryoneLeaveRoom()
    {
        Debug.Log("Host has left, closing the room for everyone...");
        PhotonNetwork.LeaveRoom();
    }

    // เมื่อออกจากห้องสำเร็จ (ทั้ง Host และ Client)
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("Left the room successfully.");

        // กลับไปจัดการ UI หน้าหลัก
        // เช่น ปิดหน้า Lobby และเปิดหน้า Main Menu กลับมา
        lobbyPage_Rect.gameObject.SetActive(false);
        mainMenu_Rect.gameObject.SetActive(true);
        // พาผู้เล่นกลับไปหน้าเมนูหลัก หรือเปิด UI หน้าแรก
        Debug.Log("Left room successfully");
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // ตัวอย่างการโหลดซีนใหม่
    }
}
