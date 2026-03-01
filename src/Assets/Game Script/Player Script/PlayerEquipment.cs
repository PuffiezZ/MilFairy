using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
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
    [BoxGroup("Socket Weapon Attach (One-Handed Melee)")]
    [SerializeField] private Transform RangeWeaponHanded_POS;
    [BoxGroup("Range Weapon Sheath Socket")]
    [SerializeField] private SheathedWeaponSocket[] RangeWeaponSheathSocket;

    [Tooltip("When no weapon use unarmed")]
    [SerializeField] private GameObject unarmedWeapon;

    [BoxGroup("Hold Slot")]
    [SerializeField] private Transform holdSlot;
    public Transform HoldSlot { get { return holdSlot; } }

    private WeaponScript spawnedUnarmed;

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

    private void Start()
    {
        if (currentWeaponOnHanded == null)
        {
            SetNewHandedWeapon(null);
        }
    }
    public void OnPlayerEquipped(EquipmentScript tEquipment,PhotonView playerPv)
    {
        switch (tEquipment)
        {
            case WeaponScript weapon:
                HandleEquippedWeapon(weapon,playerPv);
                break;
        }
    }

    private void HandleEquippedWeapon(WeaponScript getWeapon,PhotonView playerPv)
    {
        if(PhotonNetwork.InRoom && playerPv.IsMine)
        {
            int weaponID = getWeapon.photonView.ViewID;
            playerPv.RPC(nameof(RPC_HandleEquippedWeapon), RpcTarget.AllBuffered,weaponID);
        }
        else
        {
            LocalHandleEquippedWeapon(getWeapon);
        }
    }
    [PunRPC]
    protected void RPC_HandleEquippedWeapon(int weaponID)
    {
        PhotonView targetPv = PhotonView.Find(weaponID);
        WeaponScript weaponScript= targetPv.GetComponent<WeaponScript>();
        LocalHandleEquippedWeapon(weaponScript);
    }
    private void LocalHandleEquippedWeapon(WeaponScript getWeapon)
    {
        bool slotIsFree = false;
        for (int i = 0; i < currentCarriedWeapons.Length; i++)
        {
            if (currentCarriedWeapons[i] == null)
            {

                SetWeaponSlot(i, getWeapon);
                slotIsFree = true;
                break;
            }
        }

        if (slotIsFree) return;
        SetWeaponSlot(indexCarriedWeapon, getWeapon);
    }
    public void SetNewHandedWeapon(WeaponScript weapon = null)
    {
        if (weapon != null)
        {
            currentWeaponOnHanded = weapon;
        }
        else
        {

            if (spawnedUnarmed == null)
            {
                // ����ѧ����� ������ҧ�͡�����١�ͧ����Ф� (transform)
                spawnedUnarmed = Instantiate(unarmedWeapon, transform).GetComponent<WeaponScript>();
                spawnedUnarmed.name = "Unarmed_Hand";
            }


            currentWeaponOnHanded = spawnedUnarmed;
            currentWeaponOnHanded.PlayerTransform = transform; // �� Transform ����Ф�����ʤ�Ի�����ظ

            if (currentWeaponOnHanded.TryGetComponent<MeleeWeapon>(out MeleeWeapon punch))
            {
                punch.RegisterHitbox();
            }

            Debug.Log($"<color=cyan>Unarmed Spawned and Registered:</color> {currentWeaponOnHanded.PlayerTransform.name}");
        }
    }
    public IEnumerator SwapWeapon(int index)
    {
        if(currentCarriedWeapons[index] == null && currentCarriedWeapons[index].GetComponent<WeaponScript>().WeaponData.weaponType != UtilityDev.WeaponType.Unarmed)
        {
            Debug.Log($"No Weapon in Slot[{index}] to Swap");
            yield break;
        }
        if (currentWeaponOnHanded.IndexSlotNumber == index && currentWeaponOnHanded.GetComponent<WeaponScript>().WeaponData.weaponType != UtilityDev.WeaponType.Unarmed)
        {
            Debug.Log($"Already Holding Weapon at index[{index}]");
            yield break;
        }
        if (!currentWeaponOnHanded.IsShethed && currentWeaponOnHanded.GetComponent<WeaponScript>().WeaponData.weaponType != UtilityDev.WeaponType.Unarmed)
        {
            playerCombat.OnStartSheath();
            while (playerCombat.isSheathing)
            {
                yield return null;
            }
        }
        playerCombat.currentIndexWeaponSlotNumber = index;
        playerCombat.OnStartDrawedWeapon(playerCombat.currentIndexWeaponSlotNumber);

        string nameWeapon = currentWeaponOnHanded.WeaponData.Name;
        Debug.Log($"Current Holding Weapon {nameWeapon} at index[{index}]");
        yield return null;
    }

    public void SetWeaponSlot(int indexSlot, WeaponScript getWeapon)
    {
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
            case UtilityDev.WeaponType.SlingshotOrBow:
                SheathedWeaponSocket rangeSheathSocket;
                if(RangeWeaponSheathSocket[0].CheckSocketIsFree())
                {
                    rangeSheathSocket = RangeWeaponSheathSocket[0];
                    rangeSheathSocket.SetWeaponInSocket(getWeapon);
                }
                else if(RangeWeaponSheathSocket[1].CheckSocketIsFree())
                {
                    rangeSheathSocket = RangeWeaponSheathSocket[1];
                    rangeSheathSocket.SetWeaponInSocket(getWeapon);
                }  
                SetWeaponSheathedPosition(indexSlot, getWeapon);   
                break;
        }
    }


    public void SetWeaponSheathedPosition(int indexSlot, WeaponScript getWeapon)
    {
        // 1. �ʡ Visual ����������������� Array
        GameObject instanceWeapon = getWeapon.gameObject;
        if (instanceWeapon == null) return;

        // 2. �Ѵ�������ͧ UI ��������
        WeaponData weaponData = getWeapon.WeaponData;
        OnSetNewWeapon?.Invoke(indexSlot, weaponData); 

        // 3. ��駤�ҵ��˹����Դ�Ѻ�ѡ
        instanceWeapon.transform.SetParent(getWeapon.SheathedSocket.SocketTransform, false);
        instanceWeapon.transform.localPosition = Vector3.zero;
        instanceWeapon.transform.localRotation = Quaternion.identity;

        if (getWeapon != null)
        {
            getWeapon.OnSheathedWeapon(); // ���¡��ѧ��ѹ�Դ Rigidbody/Collider ���س��¹���
            getWeapon.IndexSlotNumber = indexSlot;
        }
    }

    public void SetWeaponDrawPosition(int indexSlot,UtilityDev.WeaponType weaponType)
    {
        // �֧����Ф÷������� Slot ����� (��Ƿ����� Instantiate ���͹�á)
        WeaponScript weapon = currentCarriedWeapons[indexSlot];
        if (weapon == null) return;

        currentWeaponOnHanded = weapon;

        Transform handPOS = null;
        switch (weaponType)
        {
            case UtilityDev.WeaponType.OneHandedMelee:
                handPOS = OneMeleeHanded_POS;
                break;
            case UtilityDev.WeaponType.SlingshotOrBow:
                handPOS = RangeWeaponHanded_POS; 
                break;
        }            
        weapon.transform.SetParent(handPOS, false);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.OnDrawedWeapon(); // ����¹ʶҹ� IsShethed �� false

    }

    public void OnHandedCallShethedWeapon(int indexSlot)
    {
        WeaponScript weapon = currentWeaponOnHanded;
        if (weapon == null) return;

        // ᷹���� Instantiate ���� ������� Parent 价�����᷹
        weapon.transform.SetParent(weapon.SheathedSocket.SocketTransform, false);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        weapon.OnSheathedWeapon(); // ����¹ʶҹ� IsShethed �� false
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
