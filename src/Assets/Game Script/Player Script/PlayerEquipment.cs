using NaughtyAttributes;
using Photon.Pun;
using System;
using UnityEngine;

public class PlayerEquipment : MonoBehaviourPun
{
    public static Action<int, WeaponData> OnSetNewWeapon;
    [BoxGroup("Socket Weapon Attach (One-Handed Melee)")]
    [SerializeField] private Transform OneMeleeHanded_POS;
    [BoxGroup("Socket Weapon Attach (One-Handed Melee)")]
    [SerializeField] private Transform OneMeleeSheath_POS;

    private WeaponScript currentWeaponOnHanded;
    private WeaponScript[] currentCarriedWeapons = new WeaponScript[2];

    public WeaponScript CurrentWeaponOnHanded { get { return currentWeaponOnHanded; } }
    public WeaponScript[] CurrentCarriedWeapons { get { return currentCarriedWeapons; } }
    private int indexCarriedWeapon = 0;

    private GameObject[] currentWeaponVisualActive = new GameObject[2];

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
        bool slotIsFree = false;
        for (int i = 0; i < currentCarriedWeapons.Length; i++)
        {
            if (currentCarriedWeapons[i] == null)
            {
                SetNewWeaponSlot(i, getWeapon);
                SwapWeapon(i);
                slotIsFree = true;
                break;
            }
        }

        if (slotIsFree) return;
        SetNewWeaponSlot(indexCarriedWeapon, getWeapon);
        SwapWeapon(indexCarriedWeapon);
    }
    private void HandleEquippedArmor()
    {

    }

    private void SwapWeapon(int index)
    {
        WeaponScript newWeapon = currentCarriedWeapons[index];
        currentWeaponOnHanded = newWeapon;

        playerCombat.OnInvokeAttack();

        string nameWeapon = currentWeaponOnHanded.WeaponData.Name;
        Debug.Log($"Current Holding Weapon {nameWeapon} at index[{index}]");
    }

    public void SetNewWeaponSlot(int indexSlot, WeaponScript getWeapon)
    {
        currentCarriedWeapons[indexSlot] = getWeapon;
        currentCarriedWeapons[indexSlot].IndexSlotNumber = indexSlot;

        Debug.Log($"Set Weapon {indexSlot} = {getWeapon.WeaponData.Name}");

        switch (getWeapon.WeaponData.weaponType)
        {
            case UtilityDev.WeaponType.OneHandedMelee:
                SetOneHandedSheathedPosition(indexSlot, getWeapon);
                break;
        }
    }


    public void SetOneHandedSheathedPosition(int indexSlot, WeaponScript getWeapon)
    {
        // 1. เสก Visual ครั้งเดียวและเก็บไว้ใน Array
        GameObject instanceWeapon = null;

        if (!PhotonNetwork.InRoom)
        {
            instanceWeapon = Instantiate(getWeapon.WeaponData.weaponPrefab, OneMeleeSheath_POS);
        }
        else
        {
            instanceWeapon = PhotonNetwork.Instantiate(getWeapon.WeaponData.weaponPrefab.name, OneMeleeSheath_POS.position, OneMeleeSheath_POS.rotation);
        }
        if (instanceWeapon == null) return;
        currentWeaponVisualActive[indexSlot] = instanceWeapon;

        WeaponScript weaponInSlot = instanceWeapon.GetComponent<WeaponScript>();
        currentCarriedWeapons[indexSlot] = weaponInSlot;

        WeaponData weaponData = weaponInSlot.WeaponData;
        OnSetNewWeapon?.Invoke(indexSlot, weaponData);

        // 3. ตั้งค่าตำแหน่งให้ติดกับฝัก
        instanceWeapon.transform.SetParent(OneMeleeSheath_POS, false);
        instanceWeapon.transform.localPosition = Vector3.zero;
        instanceWeapon.transform.localRotation = Quaternion.identity;

        if (weaponInSlot != null)
        {
            weaponInSlot.IndexSlotNumber = indexSlot;
            weaponInSlot.OnSheathedWeapon(); // เรียกใช้ฟังก์ชันปิด Rigidbody/Collider ที่คุณเขียนไว้
        }
    }

    public void SetDrawedEquipmentPOS(int indexSlot)
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

    public void SetSheathEquipmentPOS(int indexSlot)
    {
        WeaponScript weapon = currentWeaponOnHanded;
        if (weapon == null) return;

        // แทนที่จะ Instantiate ใหม่ ให้ย้าย Parent ไปที่มือแทน
        weapon.transform.SetParent(OneMeleeSheath_POS, false);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        weapon.OnSheathedWeapon(); // เปลี่ยนสถานะ IsShethed เป็น false
    }
}
