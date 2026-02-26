using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "MilFairy/Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    
    [Header("Requirements")]
    // ลิสต์ชื่อไอเทมที่ต้องใช้ (ต้องตรงกับชื่อใน HoldableObject)
    public List<string> requiredItemNames;

    [Header("Result")]
    // Prefab ของไอเทมที่จะสร้างออกมา (ต้องอยู่ในโฟลเดอร์ Resources สำหรับ Photon)
    public GameObject resultPrefab;  
}
