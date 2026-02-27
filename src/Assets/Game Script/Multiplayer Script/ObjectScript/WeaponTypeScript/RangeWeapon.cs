using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RangeWeapon : WeaponScript
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject localProjectilePrefab; // สำหรับ Singleplayer
    [SerializeField] private Transform firePoint;
    [SerializeField] private ChargeableWeapon chargeSystem;

    public override void WeaponTrigger()
    {
        // เรียกใช้ผ่าน Animation Event หรือ Input ตรงๆ
    }

    public void Fire(float power)
    {
        GameObject projectile;
        Vector3 position = firePoint.position;
        Quaternion rotation = firePoint.rotation;

        if (PhotonNetwork.InRoom)
        {
            // Multiplayer: ต้องใช้ชื่อไฟล์จาก Resources
            projectile = PhotonNetwork.Instantiate(localProjectilePrefab.name, position, rotation);
        }
        else
        {
            // Singleplayer
            projectile = Instantiate(localProjectilePrefab, position, rotation);
        }

        // ใส่แรงส่งให้ลูกธนู (สมมติว่าลูกธนูมี Rigidbody)
        if (projectile.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(firePoint.forward * power, ForceMode.Impulse);
        }
    }

    public ChargeableWeapon GetChargeSystem()
    {
        if (chargeSystem == null) 
            chargeSystem = GetComponent<ChargeableWeapon>();
        return chargeSystem;
    }
}
