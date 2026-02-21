using NaughtyAttributes;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class PlayerEquipment : MonoBehaviourPun
{
    public static Action<int, WeaponData> OnSetNewWeapon;
    [BoxGroup("Socket Weapon Attach (One-Handed Melee)")]
    [SerializeField] private Transform OneMeleeHanded_POS;
    [BoxGroup("Socket Weapon Attach (One-Handed)")]
    [SerializeField] private SheathedWeaponSocket[] OneMeleeSheathSocket;
    [BoxGroup("Socket Weapon Attach (Two-Handed)")]
    [SerializeField] private SheathedWeaponSocket[] TwoMeleeSheathSocket;

    [Tooltip("When no weapon use unarmed")]
    [SerializeField] private GameObject unarmedWeapon;

    private WeaponScript currentWeaponOnHanded;
    private WeaponScript[] currentCarriedWeapons = new WeaponScript[2];

    public WeaponScript CurrentWeaponOnHanded { get { return currentWeaponOnHanded; } }
    public WeaponScript[] CurrentCarriedWeapons { get { return currentCarriedWeapons; } }
    public WeaponScript UnarmedWeapon { get { return unarmedWeapon.GetComponent<WeaponScript>(); } }
    private int indexCarriedWeapon = 0;


    [BoxGroup("Class References")]
    [SerializeField] private PlayerCombat playerCombat;
    [BoxGroup("Class References")]
    [SerializeField] private PlayerAnimation playerAnimation;


    public void OnPlayerEquipped(EquipmentScript tEquipment)
    {
        switch (tEquipment)
        {
            case WeaponScript weapon:
                HandleEquippedWeapon(weapon);
                break;
        }
    }

    private void HandleEquippedWeapon(WeaponScript getWeapon)
    {
        // 1. Check สำหรับ slot ที่ว่าง
        bool slotIsFree = false;
        for (int i = 0; i < currentCarriedWeapons.Length; i++)
        {
            if (currentCarriedWeapons[i] == null)
            {
                // เจอ ให้ Set
                SetWeaponSlot(i, getWeapon);
                slotIsFree = true;
                break;
            }
        }
        // เจอ Slot ที่ว่าง ให้ return ไปเลยไม่ต้องทำอะไรต่อ
        if (slotIsFree) return;

        // 2. ถ้าไม่เจอ Slot ที่ว่าง ให้ใช้ Slot index ที่ถืออาวุธอยู่ล่าสุด
        SetWeaponSlot(indexCarriedWeapon, getWeapon);
    }
    public void SetNewHandedWeapon(WeaponScript weapon = null)
    {
        if (weapon != null)
            currentWeaponOnHanded = weapon;
        else
            currentWeaponOnHanded = UnarmedWeapon;
    }
    public IEnumerator SwapWeapon(int index)
    {
        if (!currentCarriedWeapons[index].IsShethed)
        {
            playerCombat.OnStartSheath();
            while (playerCombat.isSheathing)
            {
                yield return null;
            }
        }
        playerCombat.currentIndexWeaponSlotNumber = index;
        playerCombat.OnStartDrawedWeapon();

        string nameWeapon = currentWeaponOnHanded.WeaponData.Name;
        Debug.Log($"Current Holding Weapon {nameWeapon} at index[{index}]");
        yield return null;
    }

    public void SetWeaponSlot(int indexSlot, WeaponScript getWeapon)
    {
        //ถ้า Slot นั้นมีอาวุธอยู่แล้ว ให้เอาอาวุธนั้นไปเก็บไว้ในฝักก่อน
        currentCarriedWeapons[indexSlot] = getWeapon;
        currentCarriedWeapons[indexSlot].IndexSlotNumber = indexSlot;
        currentCarriedWeapons[indexSlot].IsShethed = true;
        Debug.Log($"Set Weapon {indexSlot} = {getWeapon.WeaponData.Name}");

        switch (getWeapon.WeaponData.weaponType)
        {
            case UtilityDev.WeaponType.OneHandedMelee:
                SheathedWeaponSocket sheathSocket;
                if (OneMeleeSheathSocket[0].CheckSocketIsFree())
                {
                    sheathSocket = OneMeleeSheathSocket[0];
                    sheathSocket.SetWeaponInSocket(getWeapon);
                }
                else if(OneMeleeSheathSocket[1].CheckSocketIsFree())
                {
                    sheathSocket = OneMeleeSheathSocket[1];
                    sheathSocket.SetWeaponInSocket(getWeapon);
                }

                SetWeaponSheathedPosition(indexSlot, getWeapon);
                break;
        }
    }


    public void SetWeaponSheathedPosition(int indexSlot, WeaponScript getWeapon)
    {
        // 1. เสก Visual ครั้งเดียวและเก็บไว้ใน Array
        GameObject instanceWeapon = getWeapon.gameObject;
        if (instanceWeapon == null) return;

        // 2. จัดการเรื่อง UI ที่นี่เลย
        WeaponData weaponData = getWeapon.WeaponData;
        OnSetNewWeapon?.Invoke(indexSlot, weaponData); 

        // 3. ตั้งค่าตำแหน่งให้ติดกับฝัก
        instanceWeapon.transform.SetParent(getWeapon.SheathedSocket.SocketTransform, false);
        instanceWeapon.transform.localPosition = Vector3.zero;
        instanceWeapon.transform.localRotation = Quaternion.identity;

        if (getWeapon != null)
        {
            getWeapon.OnSheathedWeapon(); // เรียกใช้ฟังก์ชันปิด Rigidbody/Collider ที่คุณเขียนไว้
            getWeapon.IndexSlotNumber = indexSlot;
        }
    }

    public void SetWeaponDrawPosition(int indexSlot)
    {
        // ดึงตัวละครที่อยู่ใน Slot นั้นมา (ตัวที่เรา Instantiate ไว้ตอนแรก)
        WeaponScript weapon = currentCarriedWeapons[indexSlot];
        if (weapon == null) return;

        currentWeaponOnHanded = weapon;

        // แทนที่จะ Instantiate ใหม่ ให้ย้าย Parent ไปที่มือแทน
        weapon.transform.SetParent(OneMeleeHanded_POS, false);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.OnDrawedWeapon(); // เปลี่ยนสถานะ IsShethed เป็น false

    }

    public void OnHandedCallShethedWeapon(int indexSlot)
    {
        WeaponScript weapon = currentWeaponOnHanded;
        if (weapon == null) return;

        // แทนที่จะ Instantiate ใหม่ ให้ย้าย Parent ไปที่มือแทน
        weapon.transform.SetParent(weapon.SheathedSocket.SocketTransform, false);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        weapon.OnSheathedWeapon(); // เปลี่ยนสถานะ IsShethed เป็น false
    }
}

[System.Serializable]
public class SheathedWeaponSocket
{
    public WeaponScript CurrentWeaponInSocket { get; private set; }
    public Transform SocketTransform;

    public bool CheckSocketIsFree()
    {
        return CurrentWeaponInSocket == null;
    }

    public void SetWeaponInSocket(WeaponScript weapon)
    {
        weapon.SheathedSocket = this;
        CurrentWeaponInSocket = weapon;
    }
}
