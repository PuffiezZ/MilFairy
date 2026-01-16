using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponScript : EquipmentScript
{
    public WeaponData WeaponData;

    public int IndexSlotNumber { get; set; }
    public bool IsShethed { get; set; }
    public void OnSheathedWeapon()
    {
        IsShethed = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        BoxCollider bc = GetComponent<BoxCollider>();

        if (rb != null)
        {
            rb.isKinematic = true;    // เปิด Kinematic เพื่อให้ขยับตาม Parent เท่านั้น
            rb.useGravity = false;   // ปิดแรงโน้มถ่วง
            rb.detectCollisions = false; // ปิดการชนเพื่อไม่ให้ดาบไปดีดกับตัวละคร
        }

        if (bc != null)
        {
            bc.enabled = false; // ปิด Collider ของดาบ (ไปใช้ Raycast หรือ Trigger แยกแทนตอนโจมตี)
        }
    }

    public void OnDrawedWeapon()
    {
        IsShethed = false;
    }
}
