using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

public class CraftingManager : MonoBehaviourPunCallbacks
{
    [Header("Settings")]
    [SerializeField] private CraftingZone craftingZone;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<CraftingRecipe> recipes;

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
        craftingZone.CleanUpItems();

        // ดึงชื่อไอเทมทั้งหมดที่มีอยู่ใน Zone
        // *สมมติว่าใน HoldableObject มีตัวแปรชื่อ itemName นะครับ ถ้าเป็นอย่างอื่นให้แก้ตรงนี้*
        List<string> currentItemNames = craftingZone.itemsInRange
            .Select(i => i.name.Replace("(Clone)", "").Trim()) // ป้องกันชื่อติด (Clone)
            .ToList();

        foreach (var recipe in recipes)
        {
            if (IsRecipeMatch(recipe, currentItemNames))
            {
                PerformCraft(recipe);
                return;
            }
        }

        Debug.Log("No matching recipe found!");
    }

    private bool IsRecipeMatch(CraftingRecipe recipe, List<string> currentItems)
    {
        if (recipe.requiredItemNames.Count != currentItems.Count) return false;

        // เช็คว่าไอเทมครบตามสูตรหรือไม่ (ไม่สนลำดับการโยน)
        var recipeItems = new List<string>(recipe.requiredItemNames);
        foreach (var item in currentItems)
        {
            if (!recipeItems.Remove(item)) return false;
        }
        return recipeItems.Count == 0;
    }

    private void PerformCraft(CraftingRecipe recipe)
    {
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
}
