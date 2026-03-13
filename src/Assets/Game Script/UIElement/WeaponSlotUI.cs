using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;

    public void SetWeaponSlotUI(WeaponData weaponData = null)
    {
        if (weaponImage == null)
        {
            Debug.LogError("ไม่ได้ลาก Image (weaponImage) ใส่ใน Inspector ของ " + gameObject.name);
            return;
        }

        if (weaponData == null)
        {
            // กรณีไม่มีข้อมูลอาวุธ ให้ทำให้รูปโปร่งใส (Alpha = 0)
            Debug.Log("[WeaponSlotUI] กำลังตั้งค่าเป็นว่าง (Hide Image)");
            weaponImage.sprite = null;
            weaponImage.color = Color.clear; // ค่าเดียวกับ new Color(0,0,0,0)
        }
        else
        {
            // กรณีมีข้อมูลอาวุธ ให้แสดงรูปปกติ (Alpha = 255)
            Debug.Log("[WeaponSlotUI] แสดงอาวุธ: " + weaponData.name);
            weaponImage.sprite = weaponData.weaponSprite;
            weaponImage.color = Color.white; // ค่าเดียวกับ new Color(1,1,1,1) หรือ Alpha 255
        }
    }
}
