using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using TMPro;

public class LobbyHandler : MonoBehaviourPunCallbacks
{
    [Header("Player Model Stand Spawn Position")]
    [SerializeField] private Transform[] playerStandPOSs;
    
    [Header("Player Model Prefab")]
    [SerializeField] private GameObject playerEntryPrefab;
    
    [Header("Target Scene")]
    [Scene] public string sceneName;
    private PlayerCharacterInLobby localPlayerCharacterInLobby;
    private Dictionary<Photon.Realtime.Player, GameObject> playerListEntries = new Dictionary<Photon.Realtime.Player, GameObject>();
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    public override void OnJoinedRoom()
    {
        CameraManager.ChangeCameraByName("Lobby Main Camera");
        foreach (GameObject entry in playerListEntries.Values) 
            Destroy(entry);
            
        playerListEntries.Clear();
        Photon.Realtime.Player localPlayer = PhotonNetwork.LocalPlayer;

        Debug.Log($"On Joined Room has Player Amount = {PhotonNetwork.CurrentRoom.Players.Values.Count}");
        for (int i = 0; i < PhotonNetwork.CurrentRoom.Players.Values.Count; i++)
        {
            Transform currentPOS = playerStandPOSs[i];
            Photon.Realtime.Player player = PhotonNetwork.PlayerList[i];
            Debug.Log($"Player: {player.NickName} has join at index: {i}");
            
            if(player.ActorNumber == localPlayer.ActorNumber)
            {
                AddPlayerListing(player, currentPOS,localPlayer);
            }
            else
            {
                AddPlayerListing(player, currentPOS);
            }
        }
        
        Debug.Log("Client Joined Room!");
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    { 
        AddPlayerListing(newPlayer,playerStandPOSs[newPlayer.ActorNumber - 1]);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (playerListEntries.ContainsKey(otherPlayer))
        {
            Destroy(playerListEntries[otherPlayer]);
            playerListEntries.Remove(otherPlayer);
        }
    }
    private void AddPlayerListing(Photon.Realtime.Player player, Transform posStanding,Photon.Realtime.Player localPlayer = null)
    {
        GameObject entry = Instantiate(playerEntryPrefab, posStanding);
        entry.transform.localPosition = Vector3.zero;
        entry.transform.localRotation = Quaternion.identity;
        Debug.Log("Player Model Has Instamctaite");
        
        if (entry != null)
        {
            PlayerCharacterInLobby pc = entry.GetComponent<PlayerCharacterInLobby>();
            TextMeshProUGUI text = pc.GetTextName;
            text.text = player.NickName;

            if (string.IsNullOrEmpty(player.NickName))
                text.text = "Player " + player.ActorNumber;

            playerListEntries.Add(player, entry);
            Debug.Log($"Added Player: {player.NickName}");
            
            if(localPlayer != null)
            {
                pc.CurrentPlayerControl = localPlayer;
                localPlayerCharacterInLobby = pc;
                
                Debug.Log($"Current Local player is {localPlayer.NickName}");
            }
        }
    }
    
    public void SetClothesCharacter(PlayerCharacterInLobby.SelectCharacter selectCharacter)
    {
        if(localPlayerCharacterInLobby == null) return;
    }
}
