using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Transform spawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Conecttting...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("testroom",null,null);
        Debug.Log("Now players joined to lobby");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        GameObject player = PhotonNetwork.Instantiate(playerObject.name, spawnPoint.position, Quaternion.identity);

    }
}
