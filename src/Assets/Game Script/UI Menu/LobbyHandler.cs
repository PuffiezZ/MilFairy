using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyHandler : MonoBehaviourPunCallbacks
{
    [Header("Player Model Stand Spawn Position")]
    [SerializeField] private Transform[] playerStandPOSs;
    
    [Header("Player Model Prefab")]
    [SerializeField] private GameObject playerEntryPrefab;
    
    [SerializeField] private RectTransform loadingUI;
    [SerializeField] private RectTransform loopingBG;

    private PlayerCharacterInLobby localPlayerCharacterInLobby;
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

        photonView.RPC(nameof(RPC_ShowLoadingScreen), RpcTarget.All);
    }
    
    [PunRPC]
    private void RPC_ShowLoadingScreen()
    {
        loadingUI?.gameObject.SetActive(true);
        loopingBG?.gameObject.SetActive(false);
    }
    public void ChangeScene()
    {
        //SceneManager.LoadScene(sceneName);
        loadingUI.gameObject.SetActive(true);
    }
    public void ChangeCharacter(PlayerCharacterInLobby.SelectCharacter selectCharacter)
    {
        if (localPlayerCharacterInLobby == null) return;
        
        localPlayerCharacterInLobby.selectCharacter = selectCharacter;
        localPlayerCharacterInLobby.ResetSkin();
    }

    // วิธีที่ 2: รับผ่าน UnityEvent ที่ส่งค่าเป็น int (เช่น TMP_Dropdown.onValueChanged)
    public void ChangeCharacterInt(int index)
    {
        ChangeCharacter((PlayerCharacterInLobby.SelectCharacter)index);
    }

    public override void OnJoinedRoom()
    {
        CameraManager.ChangeCameraByName("Lobby Main Camera");
        loopingBG?.gameObject.SetActive(false);

        // ล้างข้อมูลเก่า
        playerListEntries.Clear();

        // ทุกคนจะ Spawn แค่ "ตัวเอง" เท่านั้นเมื่อเข้าห้อง
        // ใช้ ActorNumber ในการกำหนดจุดยืน เพื่อให้ตำแหน่งคงที่และไม่ซ้อนกัน
        int spawnIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % playerStandPOSs.Length;
        AddPlayerListing(PhotonNetwork.LocalPlayer, playerStandPOSs[spawnIndex]);
    }

    // ลบ OnPlayerEnteredRoom ออก เพราะ PhotonNetwork.Instantiate จะจัดการ Sync ตัวละครให้คนอื่นเห็นเองอัตโนมัติ

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        // ไม่ต้องสั่ง Destroy เอง เพราะ PUN จะลบ Networked Object ของคนที่ออกไปให้เอง
        if (playerListEntries.ContainsKey(otherPlayer))
        {
            playerListEntries.Remove(otherPlayer);
        }
    }

    private void AddPlayerListing(Photon.Realtime.Player player, Transform posStanding)
    {
        // ใช้ PhotonNetwork.Instantiate เสมอ เพื่อให้ Object นี้ Sync ไปยังเครื่องอื่น
        GameObject entry = PhotonNetwork.Instantiate(playerEntryPrefab.name, posStanding.position, posStanding.rotation);
        
        if (entry != null)
        {
            PlayerCharacterInLobby pc = entry.GetComponent<PlayerCharacterInLobby>();
            playerListEntries.Add(player, entry);
            
            if(player.IsLocal)
            {
                localPlayerCharacterInLobby = pc;
            }
        }
    }

}
