using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class PlayerCharacterInLobby : MonoBehaviour
{
    public SelectCharacter selectCharacter;
    [SerializeField] private List<ModifyCharacterMesh> modifyCharacterMeshes = new List<ModifyCharacterMesh>();
    private Dictionary<SelectCharacter,List<SkinnedMeshRenderer>> DictskinnedMeshRenderers = new Dictionary<SelectCharacter,List<SkinnedMeshRenderer>>();
    
    [SerializeField] private TextMeshProUGUI textName;
    public TextMeshProUGUI GetTextName => textName;
    public Photon.Realtime.Player CurrentPlayerControl { get; set; }
 
    
    public enum SelectCharacter
    {
        Character1,
        Character2
    }
    private void OnValidate()
    {
        foreach (var each in modifyCharacterMeshes)
        {
            if(DictskinnedMeshRenderers.ContainsKey(each.SetSelectCharacter))
                return;
                
            DictskinnedMeshRenderers.Add(each.SetSelectCharacter, each.skinClothes);
        }
    }
    [Button("Reset Skin")]
    public void ResetSkin()
    {
        SetPlayerSkin(selectCharacter);
    }
    private void SetPlayerSkin(SelectCharacter getSelectCharacter)
    {
        foreach(var each in DictskinnedMeshRenderers)
        {
            List<SkinnedMeshRenderer> getSkin = each.Value;
            
            foreach(var skin in getSkin)
            {
                skin.gameObject.SetActive(getSelectCharacter == each.Key);
            }
        }
    }
}
[System.Serializable]
public class ModifyCharacterMesh
{
    public PlayerCharacterInLobby.SelectCharacter SetSelectCharacter;
    public List<SkinnedMeshRenderer> skinClothes;
}

