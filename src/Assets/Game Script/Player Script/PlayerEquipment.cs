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
                playerCombat.currentIndexWeaponSlotNumber = i;
                playerCombat.OnInvokeAttack();
                slotIsFree = true;
                break;
            }
        }

        if (slotIsFree) return;
        SetNewWeaponSlot(indexCarriedWeapon, getWeapon);
        playerCombat.currentIndexWeaponSlotNumber = indexCarriedWeapon;
        playerCombat.OnInvokeAttack();
    }
    public void SetNewHandedWeapon(WeaponScript weapon = null)
    {
        currentWeaponOnHanded = weapon;
    }
    public IEnumerator SwapWeapon(int index)
    {
        if (currentWeaponOnHanded.IndexSlotNumber == index) yield break;

        if (!currentWeaponOnHanded.IsShethed)
        {
            playerCombat.OnStartSheath();
            while (playerCombat.isSheathing)
            {
                yield return null;
            }
        }
        playerCombat.currentIndexWeaponSlotNumber = index;
        playerCombat.OnInvokeAttack();

        string nameWeapon = currentWeaponOnHanded.WeaponData.Name;
        Debug.Log($"Current Holding Weapon {nameWeapon} at index[{index}]");
        yield return null;
    }

    public void SetNewWeaponSlot(int indexSlot, WeaponScript getWeapon)
    {
        currentCarriedWeapons[indexSlot] = getWeapon;
        currentCarriedWeapons[indexSlot].IndexSlotNumber = indexSlot;

        Debug.Log($"Set Weapon {indexSlot} = {getWeapon.WeaponData.Name}");

        switch (getWeapon.WeaponData.weaponType)
        {
            case UtilityDev.WeaponType.OneHandedMelee:
                SheathedWeaponSocket sheathSocket;
                if (OneMeleeSheathSocket[0].CheckSocketIsFree())
                {
                    sheathSocket = OneMeleeSheathSocket[0];
                }
                else if(OneMeleeSheathSocket[1].CheckSocketIsFree())
                {
                    sheathSocket = OneMeleeSheathSocket[1];
                }
                else
                {
                    Debug.LogWarning("All One-Handed Melee Sheath Sockets are occupied!");
                    return;
                }
                sheathSocket.SetWeaponInSocket(getWeapon);
                SetWeaponSheathedPosition(indexSlot, getWeapon);
                break;
        }
    }


    public void SetWeaponSheathedPosition(int indexSlot, WeaponScript getWeapon)
    {
        // 1. เสก Visual ครั้งเดียวและเก็บไว้ใน Array
        GameObject instanceWeapon = getWeapon.gameObject;
        if (instanceWeapon == null) return;
        currentWeaponVisualActive[indexSlot] = instanceWeapon;

        WeaponScript weaponInSlot = instanceWeapon.GetComponent<WeaponScript>();
        currentCarriedWeapons[indexSlot] = weaponInSlot;

        WeaponData weaponData = weaponInSlot.WeaponData;
        OnSetNewWeapon?.Invoke(indexSlot, weaponData);

        // 3. ตั้งค่าตำแหน่งให้ติดกับฝัก
        instanceWeapon.transform.SetParent(weaponInSlot.SheathedSocket.SocketTransform, false);
        instanceWeapon.transform.localPosition = Vector3.zero;
        instanceWeapon.transform.localRotation = Quaternion.identity;
        instanceWeapon.transform.localScale = instanceWeapon.transform.localScale;

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
