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

    [SerializeField] private LobbyHandler playerLobbyHandler;
    [SerializeField] private MainMenu mainmenu;

    private bool isCancellingRoom = false;

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

    public void CancelDuringConnecting()
    {
        // ��Ǩ�ͺ��ҡ��ѧ�������� �����������������������
        if (PhotonNetwork.IsConnected || PhotonNetwork.NetworkingClient.State == ClientState.ConnectingToMasterServer)
        {
            PhotonNetwork.Disconnect();
            Debug.Log("Canceling connection and Disconnecting...");
        }
    }

    public void CancenlCreatingRoom()
    {
        isCancellingRoom = true;

        lobbyCreatingRoom_Rect.gameObject.SetActive(false);
        lobbyHostConfig_Rect.gameObject.SetActive(true);
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(servername_TMPinput.text)) return;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4; // ��˹��ӹǹ������
            options.IsVisible = true; // ��餹������� Lobby
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
            Debug.LogWarning("�ѧ��������������� ��س����ѡ����");
        }
    }

    // ��������ҧ����� Photon �о������ͧ����ѵ��ѵ� (����ͧ���¡ Join ���)
    public override void OnJoinedRoom()
    {
        if (isCancellingRoom)
        {
            isCancellingRoom = false; // ����ʶҹ�
            PhotonNetwork.LeaveRoom(); // �������͡�ҡ��ͧ�ѹ��
            Debug.Log("Room was created but left due to cancellation.");
            return; // ����ͧ�ѹ Logic �ʴ���˹�� Lobby ���
        }

        // Logic ����ͧ�س
        Debug.Log("Host Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        gameObject.SetActive(false);
        lobbyCreatingRoom_Rect.gameObject.SetActive(false);
        lobbyPage_Rect.gameObject.SetActive(true);

        playerLobbyHandler.OnJoinedRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        isCancellingRoom = false;

        Debug.LogError("���ҧ��ͧ�������: " + message);
        lobbyPage_Rect.gameObject.SetActive(false);
        lobbyLoading_Rect.gameObject.SetActive(false);
        lobbyCreatingRoom_Rect.gameObject.SetActive(false);

        lobbyFailed_Rect.gameObject.SetActive(true);
        // ���� UI ����͹�����蹵ç��� �� "������ͧ����դ�������"
    }

    public void CreateRoomWithRandomCode()
    {
        // ��������ѡ�� 6 ��ѡ
        string randomCode = GenerateRandomCode(6);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = false; // ��ҵ�ͧ�������¼�ҹ������ҹ�� ����ͧ������ª�����ͧ�Ҹ�ó�

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
