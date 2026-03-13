using System.Collections.Generic;
using NaughtyAttributes;
using Photon.Pun;
using TMPro;
using UnityEngine;
using ExitGames.Client.Photon;

public class PlayerCharacterInLobby : MonoBehaviourPun
{
    public SelectCharacter selectCharacter;
    public ObjectSender osSender;
    [SerializeField] private List<ModifyCharacterMesh> modifyCharacterMeshes = new List<ModifyCharacterMesh>();
    private Dictionary<SelectCharacter,List<SkinnedMeshRenderer>> DictskinnedMeshRenderers = new Dictionary<SelectCharacter,List<SkinnedMeshRenderer>>();
    
    [SerializeField] private TextMeshProUGUI textName;
    public TextMeshProUGUI GetTextName => textName;
    public Photon.Realtime.Player CurrentPlayerControl { get; set; }

    private const string SKIN_PROP_KEY = "SkinIndex";
 
    
    public enum SelectCharacter
    {
        Character1,
        Character2,
        Character3,
        Character4
    }

    private void Awake()
    {
        InitializeDictionary();
    }

    private void Start()
    {
        // ตั้งค่าชื่อจาก Photon Network โดยตรง เพื่อให้แสดงผลถูกต้องบนทุกเครื่อง
        if (photonView != null && photonView.Owner != null)
        {
            textName.text = photonView.Owner.NickName;
        }

        // สำหรับ Client ที่เข้าห้องมาทีหลัง: ตรวจสอบว่าเจ้าของตัวละครนี้เลือก Skin อะไรไว้ใน Custom Properties
        if (photonView != null && photonView.Owner != null && photonView.Owner.CustomProperties.TryGetValue(SKIN_PROP_KEY, out object skinIndex))
        {
            int savedIndex = (int)skinIndex;
            selectCharacter = (SelectCharacter)savedIndex;
            SetPlayerSkin(selectCharacter);
        }
    }

    private void InitializeDictionary()
    {
        DictskinnedMeshRenderers.Clear();
        
        // เช็ค IsMine ก่อนสั่ง Clear เพื่อไม่ให้ตัวละคร Remote มาล้างข้อมูลเรา
        if (osSender != null && photonView != null && photonView.IsMine) {
            osSender.ClearList(photonView);
        }
        
        foreach (var each in modifyCharacterMeshes)
        {
            if (!DictskinnedMeshRenderers.ContainsKey(each.SetSelectCharacter))
            {
                DictskinnedMeshRenderers.Add(each.SetSelectCharacter, each.skinClothes);
            }
        }
    }
    [Button("Reset Skin")]
    public void ResetSkin()
    {
        if(PhotonNetwork.InRoom)
        {
            Debug.Log("RPC call SetPlayerSkin");
            int selectIndex = (int)selectCharacter;

            // 1. บันทึกค่าลงใน Custom Properties ของ Player เพื่อให้คนมาทีหลังอ่านค่าได้จาก Start()
            Hashtable props = new Hashtable();
            props.Add(SKIN_PROP_KEY, selectIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            // 2. ส่ง RPC แบบธรรมดา (RpcTarget.All) เพื่ออัปเดตคนในห้องปัจจุบันทันที
            photonView.RPC(nameof(RPC_SetPlayerSkin), RpcTarget.All, selectIndex);
        }
        else
        {
            Debug.Log("Singleplayer call SetPlayerSkin");
            SetPlayerSkin(selectCharacter);
        }
    }
    [PunRPC]
    private void RPC_SetPlayerSkin(int selectCharacterIndex)
    {
        SetPlayerSkin((SelectCharacter)selectCharacterIndex);
    }
    private void SetPlayerSkin(SelectCharacter getSelectCharacter)
    {
        if (DictskinnedMeshRenderers.Count == 0)
        {
            Debug.LogWarning("Dictionary is empty! Initializing now...");
            InitializeDictionary();
        }

        // อัปเดตตัวแปรภายในให้ตรงกับค่าที่รับมา เพื่อป้องกันการ Reset กลับเป็นค่า Default
        this.selectCharacter = getSelectCharacter;

        // สั่งล้าง List ใน ObjectSender ทุกครั้งที่เริ่มเปลี่ยนชุด (เฉพาะตัวเราเอง)
        if (osSender != null && photonView != null && photonView.IsMine) {
            osSender.ClearList(photonView);
        }

        foreach(var each in DictskinnedMeshRenderers)
        {
            List<SkinnedMeshRenderer> getSkin = each.Value;
            
            foreach(var skin in getSkin)
            {
                bool isMatched = getSelectCharacter == each.Key;
                skin.gameObject.SetActive(isMatched);
                
                if(isMatched)
                {
                    if (osSender != null) 
                        osSender.AddObject(skin,photonView);   
                }
            }
        }
        
        Debug.Log($"Model player set clothes: {getSelectCharacter}");
    }
}
[System.Serializable]
public class ModifyCharacterMesh
{
    public PlayerCharacterInLobby.SelectCharacter SetSelectCharacter;
    public List<SkinnedMeshRenderer> skinClothes;
}
