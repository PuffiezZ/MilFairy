using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

public class CraftingManager : MonoBehaviourPunCallbacks,IParticleSystemFunction
{
    [Header("Settings")]
    [SerializeField] private CraftingZone craftingZone;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<CraftingRecipe> recipes;
    
    [Header("Visuals & Animation")]
    [SerializeField] private Animator craftingAnimator;
    [SerializeField] private string animatorBoolParam = "IsMatched";
    [SerializeField] private ParticleSystem successParticleEffect;

    public bool IsMatched {get; set;} = false;

    // ฟังก์ชันนี้ให้เอาไปใส่ใน OnClick() หรือ OnSwitchActivate() ของ EnvironmentSwitch
    public void TryCraft()
    {
        if (PhotonNetwork.InRoom)
        {
            // ถ้าเป็น Multiplayer ส่งคำขอไปให้ Master Client เป็นคนตัดสินใจ (เพื่อความ Sync)
            photonView.RPC("RPC_RequestCraft", RpcTarget.MasterClient);
        }
        else
        {
            // ถ้า Singleplayer รันฟังก์ชันคราฟโดยตรง
            ExecuteCrafting();
        }
    }
    
    [PunRPC]
    private void RPC_RequestCraft()
    {
        // เฉพาะ Master Client เท่านั้นที่จะเป็นคนคำนวณและสร้าง/ลบไอเทม
        if (PhotonNetwork.IsMasterClient)
        {
            ExecuteCrafting();
        }
    }

    private void ExecuteCrafting()
    {
        // ค้นหาสูตรที่ตรงกัน
        CraftingRecipe matchingRecipe = GetMatchingRecipe();

        if (matchingRecipe != null)
        {
            PerformCraft(matchingRecipe);
        }
        else
        {
            UpdateMatchState(false);
            Debug.Log("No matching recipe found!");
        }
    }

    /// <summary>
    /// ฟังก์ชันสำหรับเรียกใช้ผ่าน Animation Event เพื่อตรวจสอบว่าไอเทมในโซนยังตรงตามสูตรหรือไม่
    /// </summary>
    public void ValidateRecipe()
    {
        if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient) return;

        bool hasMatch = GetMatchingRecipe() != null;
        UpdateMatchState(hasMatch);
    }

    /// <summary>
    /// Helper function สำหรับค้นหาสูตรที่ตรงกับไอเทมปัจจุบันในโซน
    /// </summary>
    private CraftingRecipe GetMatchingRecipe()
    {
        craftingZone.CleanUpItems();

        List<HoldableObject> currentItems = craftingZone.itemsInRange
            .Where(i => i != null)
            .ToList();

        if (currentItems.Count == 0) return null;

        return recipes.FirstOrDefault(recipe => IsRecipeMatch(recipe, currentItems));
    }

    /// <summary>
    /// ฟังก์ชันสำหรับปรับเปลี่ยนค่า IsMatched และอัปเดต Animator
    /// </summary>
    private void UpdateMatchState(bool state)
    {
        if (PhotonNetwork.InRoom)
        {
            // ส่ง RPC ไปให้ทุกคนเพื่ออัปเดตแอนิเมชันให้ตรงกัน
            photonView.RPC(nameof(RPC_UpdateMatchState), RpcTarget.All, state);
        }
        else
        {
            ApplyMatchState(state);
        }
    }

    [PunRPC]
    private void RPC_UpdateMatchState(bool state) => ApplyMatchState(state);

    private void ApplyMatchState(bool state)
    {
        IsMatched = state;
        
        if (craftingAnimator != null)
            craftingAnimator.SetBool(animatorBoolParam, state);
    }

    private bool IsRecipeMatch(CraftingRecipe recipe, List<HoldableObject> currentItems)
    {
        if (recipe.requiredItemNames.Count != currentItems.Count) return false;

        // ดึง ItemID จาก Prefab ที่กำหนดไว้ในสูตร
        List<string> requiredIDs = recipe.requiredItemNames
            .Select(go => go.GetComponent<HoldableObject>().ItemID)
            .ToList();

        // ตรวจสอบกับ ItemID ของไอเทมที่อยู่ใน Zone
        foreach (HoldableObject item in currentItems)
        {
            if (!requiredIDs.Remove(item.ItemID)) return false;
        }
        return requiredIDs.Count == 0;
    }

    private void PerformCraft(CraftingRecipe recipe)
    {
        // เมื่อคราฟต์สำเร็จ ให้เปลี่ยนสถานะเป็น True
        UpdateMatchState(true);

        // 1. ทำลายวัตถุดิบ
        foreach (var item in craftingZone.itemsInRange)
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.Destroy(item.gameObject);
            else
                Destroy(item.gameObject);
        }

        craftingZone.itemsInRange.Clear();

        // 2. สร้างไอเทมใหม่
        if (PhotonNetwork.InRoom)
        {
            // ต้องมั่นใจว่า Prefab อยู่ใน Resources folder
            NetworkPrefabSpawner.Instance.SpawnResource(recipe.resultPrefab.name, photonView);
        }
        else
        {
            GameObject prefab = recipe.resultPrefab;
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }

        Debug.Log($"Crafted: {recipe.recipeName}");
    }
    
    public void StartParticleEffect()
    {
        if (successParticleEffect != null)
        {
            successParticleEffect.Play();
        }
    }
    
    public void StopParticleEffect()
    {
        
    }
}
