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
            rb.isKinematic = true;    // �Դ Kinematic ��������Ѻ��� Parent ��ҹ��
            rb.useGravity = false;   // �Դ�ç�����ǧ
            rb.detectCollisions = false; // �Դ��ê�����������Һ仴մ�Ѻ����Ф�
        }

        if (bc != null)
        {
            bc.enabled = false; // �Դ Collider �ͧ�Һ (��� Raycast ���� Trigger �¡᷹�͹����)
        }
    }
    public override void OnBeginIntereact(GameObject player, bool setActive = false)
    {
        if(PlayerTransform != null) return;
        
        PlayerTransform = player.transform;
        base.OnBeginIntereact(player, setActive);
        photonView.RequestOwnership();
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
