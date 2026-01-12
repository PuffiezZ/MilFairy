using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityDev
{
    public enum ResourceType
    {
        Scrap = 0,
        Stick = 1,
        Electronic = 2,
        Oil = 3,
        Clothes = 4,
        Battery = 5
    }

    public enum ItemType
    {
        Weapon = 0,
        Armor = 1
    }

    public enum WeaponType
    {
        // Melee Group
        OneHandedMelee = 0, // ดาบมือเดียว, แปรงสีฟัน, กิ่งไม้
        TwoHandedMelee = 1, // ดาบสองมือ, ไม้ถูพื้นใหญ่
        Spear = 2,          // หอก (มีท่าแทงเฉพาะตัว)

        // Range Group
        SlingshotOrBow = 3, // ธนูไหมขัดฟัน (ต้องใช้สองมือดึง)
        PistolStyle = 4     // อาวุธยิงมือเดียว (ถ้ามีในอนาคต)
    }

    public enum ArmorType
    {
        Helmet = 0,
        BodyArmor = 1,
        LowerBodyArmor = 2,
    }

    public enum DrawOrSheath
    {
        Draw = 0,
        Sheath = 1,
    }

}
