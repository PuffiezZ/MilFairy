using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
public class PlayerCombat : MonoBehaviourPunCallbacks
{ 
    [SerializeField] private PlayerEquipment equipment;
    [SerializeField] private PlayerAnimation playerAnimation;

    private int currentIndexWeaponSlotNumber = 0;

    private void Update()
    {
        // ตรวจสอบว่าเป็นเครื่องของเรา และกดปุ่มโจมตี
        //if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnInvokeAttack();
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            currentIndexWeaponSlotNumber = 0;
            WeaponScript currentWeapon = equipment.CurrentWeaponOnHanded;

            if (currentWeapon == null) return;
            if (!currentWeapon.IsShethed)
            {
                OnStartSheath();
            }
        }
    }

    public void OnInvokeAttack()
    {
        WeaponScript weapon = equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber];

        if (weapon == null)
        {
            Debug.Log($"Weapon is null or empty ||");
            return;
        }

        if (weapon.IsShethed)
        {
            OnStartDrawedWeapon();
            return;
        }
        Debug.Log("Player Attack!");
    }

    public void OnInvokeDrawed()
    {
        playerAnimation.SetOnUsingWeaponAnimation(true);
        equipment.SetDrawedEquipmentPOS(currentIndexWeaponSlotNumber);
        //Play Animation Sheathed
    }
    public void OnStartDrawedWeapon()
    {
        playerAnimation.OnTriggerDrawOrSheathed(UtilityDev.DrawOrSheath.Draw);
    }

    public void OnInvokeSheathed()
    {
        equipment.SetSheathEquipmentPOS(currentIndexWeaponSlotNumber);
        playerAnimation.SetOnUsingWeaponAnimation(false);
    }

    public void OnStartSheath()
    {
        playerAnimation.OnTriggerDrawOrSheathed(UtilityDev.DrawOrSheath.Sheath);
    }

    public void WeaponIsSheathed()
    {
        WeaponScript weapon = equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber];
        weapon.IsShethed = true;
    }
    public void WeaponIsDrawed()
    {
        WeaponScript weapon = equipment.CurrentCarriedWeapons[currentIndexWeaponSlotNumber];
        weapon.IsShethed = false;
    }
}
