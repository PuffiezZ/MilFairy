using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using NaughtyAttributes;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] uiPages;
    [SerializeField] private GameObject loadingText;
    [SerializeField] private GameObject joiningRoomText;
    [SerializeField] private GameObject lobbyPage;

    [BoxGroup("Instacne")]
    [SerializeField] private RectTransform roomViewContent;
    [BoxGroup("Instacne")]
    [SerializeField] private GameObject roomInstance;

    [BoxGroup("Game References")]
    [SerializeField] private LobbyHandler playerLobbyHandler;

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if(uiPages.Length > 0)
            {
                for (int i = 0; i < uiPages.Length; i++)
                {
                    uiPages[i].gameObject.SetActive(false);
                }
            }

            loadingText.SetActive(true);
            joiningRoomText.SetActive(false);
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Connecting...");
        }
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        loadingText.SetActive(false);

        PhotonNetwork.JoinLobby();

        for (int i = 0;i < uiPages.Length; i++)
        {
            uiPages[i].gameObject.SetActive(true);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ��ҧ UI ��ҷ���ҧ����� Content
        foreach (Transform child in roomViewContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            // �Ӥѭ�ҡ: �����ͧ�١ź仨ҡ Server ���� ����ͧ���ҧ����
            if (roomList[i].RemovedFromList) continue;

            GameObject newRoomGO = Instantiate(roomInstance, roomViewContent);
            Room room = newRoomGO.GetComponent<Room>();
            room.OnRoomListCalled(roomList[i].Name, this);
        }
    }

    public void JoinRoomOnListName(string nameServer)
    {
        for (int i = 0; i < uiPages.Length; i++)
        {
            uiPages[i].gameObject.SetActive(false);
        }
        loadingText.SetActive(false);
        joiningRoomText.gameObject.SetActive(true);

        Debug.Log("Client Joining the server...");
        PhotonNetwork.JoinRoom(nameServer);
    }

    public override void OnJoinedRoom()
    {
        joiningRoomText.gameObject.SetActive(false);
        for (int i = 0;i < uiPages.Length; i++)
        {
            uiPages[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
        lobbyPage.gameObject.SetActive(true);
        playerLobbyHandler.OnJoinedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room Failed: {message}"); // �к͡���˵تѴਹ� Console

        joiningRoomText.SetActive(false); // �Դ���˹ѧ�����Ŵ

        // �ʴ�˹�� UI ��ѡ��Ѻ�����������������͡����
        for (int i = 0; i < uiPages.Length; i++)
        {
            uiPages[i].gameObject.SetActive(true);
        }

        // ����͹�����蹼�ҹ Error Handler �������·����
    }
}
