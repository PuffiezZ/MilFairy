using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponScript : EquipmentScript
{
    public WeaponData WeaponData;
    public Transform PlayerTransform { get; set; }
    public SheathedWeaponSocket SheathedSocket { get; set; }
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
    public override void OnBeginIntereact(GameObject player, bool setActive = false)
    {
        PlayerTransform = player.transform;
        base.OnBeginIntereact(player, setActive);
    }
    public void OnDrawedWeapon()
    {
        IsShethed = false;
    }

    public virtual void WeaponTrigger()
    {

    }

    public virtual void WeaponAnimationEventTrigger()
    {

    }
}
