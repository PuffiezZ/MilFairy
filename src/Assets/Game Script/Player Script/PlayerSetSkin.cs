using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using static PlayerCharacterInLobby;
using static ModifyCharacterMesh;
using ExitGames.Client.Photon;

public class PlayerSetSkin : MonoBehaviourPun
{
    [SerializeField] private List<ModifyCharacterMesh> modifyCharacterMeshes = new List<ModifyCharacterMesh>();
     private Dictionary<SelectCharacter,List<SkinnedMeshRenderer>> DictskinnedMeshRenderers = new Dictionary<SelectCharacter,List<SkinnedMeshRenderer>>();
     private const string SKIN_PROP_KEY = "SkinIndex";
    private void Awake()
    {
        InitializeDictionary();
    }

    private void Start()
    {
        ApplyInitialSkin();
    }

    private void InitializeDictionary()
    {
        DictskinnedMeshRenderers.Clear();
        
        foreach (var each in modifyCharacterMeshes)
        {
            if (!DictskinnedMeshRenderers.ContainsKey(each.SetSelectCharacter))
            {
                DictskinnedMeshRenderers.Add(each.SetSelectCharacter, each.skinClothes);
            }
        }
    }

    private void ApplyInitialSkin()
    {
        if(photonView.Owner != null && photonView.Owner.CustomProperties.TryGetValue(SKIN_PROP_KEY, out object skinIndex))
        {
            SelectCharacter selectCharacter = (SelectCharacter)skinIndex;
            ApplieSkin(selectCharacter);
        }
        else
        {
            Debug.LogWarning($"Not found Key {SKIN_PROP_KEY}");
        }
    }
    public void CallAppileSkinFunction(SelectCharacter selectCharacter)
    {
        if(PhotonNetwork.InRoom)
        {
            int indexSelectCharacter = (int) selectCharacter; 
            photonView.RPC(nameof(RPC_ApplieSkin), RpcTarget.All, indexSelectCharacter);
        }
        else
        {
            Debug.Log("Singleplayer call SetPlayerSkin");
            ApplieSkin(selectCharacter);
        }
    }
    private void ApplieSkin(SelectCharacter getSelectCharacter)
    {
        foreach(var each in DictskinnedMeshRenderers)
        {
            List<SkinnedMeshRenderer> getSkin = each.Value;
            
            foreach(var skin in getSkin)
            {
                bool isMatched = getSelectCharacter == each.Key;
                skin.gameObject.SetActive(isMatched);
            }
        }
    }
    
    [PunRPC]
    public void RPC_ApplieSkin(int selectCharacterIndex)
    {
        ApplieSkin((SelectCharacter)selectCharacterIndex);
    }
}
