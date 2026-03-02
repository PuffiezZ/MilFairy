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
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject playerEntryPrefab; 

    [Scene]
    public string sceneName;
    
    private Dictionary<Photon.Realtime.Player, GameObject> playerListEntries = new Dictionary<Photon.Realtime.Player, GameObject>();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToGamePlayScene()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("Only the Host can start the game!");
            return;
        }

        Debug.Log("Host is starting the game...");

        PhotonNetwork.LoadLevel(sceneName);
    }

    public override void OnJoinedRoom()
    {
        foreach (GameObject entry in playerListEntries.Values) 
            Destroy(entry);
        playerListEntries.Clear();

        foreach (Photon.Realtime.Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            AddPlayerListing(player);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    { 
        AddPlayerListing(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (playerListEntries.ContainsKey(otherPlayer))
        {
            Destroy(playerListEntries[otherPlayer]);
            playerListEntries.Remove(otherPlayer);
        }
    }

    private void AddPlayerListing(Photon.Realtime.Player player)
    {
        GameObject entry = Instantiate(playerEntryPrefab, contentParent);
        if (entry != null)
        {

            TextMeshProUGUI text = entry.GetComponent<TextMeshProUGUI>();
            text.text = player.NickName;

            // ��Ҫ�����ҧ �������� Player + ID ᷹
            if (string.IsNullOrEmpty(player.NickName))
                text.text = "Player " + player.ActorNumber;

            // ����� Dictionary ���
            playerListEntries.Add(player, entry);
            Debug.Log($"Added Player: {player.NickName}");
        }
    }
}
