using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class HostGame : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField servername_TMPinput;
    [SerializeField] private TMP_InputField password_TMPinput;

    [Header("UI Page References")]
    [SerializeField] private RectTransform lobbyPage_Rect;
    [SerializeField] private RectTransform lobbyLoading_Rect;
    [SerializeField] private RectTransform lobbyFailed_Rect;
    [SerializeField] private RectTransform lobbyHostConfig_Rect;
    [SerializeField] private RectTransform lobbyCreatingRoom_Rect;

    [SerializeField] private PlayerLobbyList playerLobbyList;
    [SerializeField] private MainMenu mainmenu;

    public void ConnectToPhotonServer()
    {
        mainmenu.OnClickConnect();
        if (!PhotonNetwork.IsConnected)
        {
            lobbyPage_Rect.gameObject.SetActive(false);
            lobbyFailed_Rect.gameObject.SetActive(false);
            lobbyHostConfig_Rect.gameObject.SetActive(false);
            lobbyCreatingRoom_Rect.gameObject.SetActive(false);

            lobbyLoading_Rect.gameObject.SetActive(true);
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting...");
        }
        else
        {
            ConnectedToPhotonServer();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        ConnectedToPhotonServer();
    }

    private void ConnectedToPhotonServer()
    {
        lobbyPage_Rect.gameObject.SetActive(false);
        lobbyFailed_Rect.gameObject.SetActive(false);
        lobbyLoading_Rect.gameObject.SetActive(false);
        lobbyCreatingRoom_Rect.gameObject.SetActive(false);

        lobbyHostConfig_Rect.gameObject.SetActive(true);
        Debug.Log("Host Connected To PhotonServer!");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(servername_TMPinput.text)) return;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4; // กำหนดจำนวนผู้เล่น
            options.IsVisible = true; // ให้คนอื่นเห็นใน Lobby
            options.IsOpen = true;

            lobbyPage_Rect.gameObject.SetActive(false);
            lobbyFailed_Rect.gameObject.SetActive(false);
            lobbyLoading_Rect.gameObject.SetActive(false);
            lobbyCreatingRoom_Rect.gameObject.SetActive(true);
            lobbyHostConfig_Rect.gameObject.SetActive(false);

            PhotonNetwork.CreateRoom(servername_TMPinput.text, options);
        }
        else
        {
            Debug.LogWarning("ยังไม่พร้อมเชื่อมต่อ กรุณารอสักครู่");
        }
    }

    // เมื่อสร้างสำเร็จ Photon จะพาเข้าห้องให้อัตโนมัติ (ไม่ต้องเรียก Join ซ้ำ)
    public override void OnJoinedRoom()
    {
        Debug.Log("Host Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        gameObject.SetActive(false);
        lobbyFailed_Rect.gameObject.SetActive(false);
        lobbyLoading_Rect.gameObject.SetActive(false);
        lobbyCreatingRoom_Rect.gameObject.SetActive(false);
        lobbyPage_Rect.gameObject.SetActive(true);

        playerLobbyList.OnJoinedRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("สร้างห้องล้มเหลว: " + message);
        lobbyPage_Rect.gameObject.SetActive(false);
        lobbyLoading_Rect.gameObject.SetActive(false);
        lobbyCreatingRoom_Rect.gameObject.SetActive(false);

        lobbyFailed_Rect.gameObject.SetActive(true);
        // เพิ่ม UI แจ้งเตือนผู้เล่นตรงนี้ เช่น "ชื่อห้องนี้มีคนใช้แล้ว"
    }

    public void CreateRoomWithRandomCode()
    {
        // สุ่มตัวอักษร 6 หลัก
        string randomCode = GenerateRandomCode(6);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = false; // ถ้าต้องการให้จอยผ่านรหัสเท่านั้น ไม่ต้องโชว์ในรายชื่อห้องสาธารณะ

        PhotonNetwork.CreateRoom(randomCode, options);
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] stringChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[Random.Range(0, chars.Length)];
        }
        return new string(stringChars);
    }
}
