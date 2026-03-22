using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using ExitGames.Client.Photon.StructWrapping;

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
        if(PlayerTransform == null) return;
        
        Vector3 position = firePoint.position;
        Vector3 targetPoint;

        // 1. ยิง Ray จากกึ่งกลางกล้องออกไป
        // Viewport Space: (0.5, 0.5) คือกึ่งกลางจอพอดี
        PlayerMovement pm = PlayerTransform.GetComponent<PlayerMovement>();
        
        if(pm == null || pm._playerCamera == null) return;
        
        Ray ray = pm._playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f)) // ระยะ 100 เมตร (ปรับได้)
        {
            // ถ้าชนอะไรบางอย่าง ให้จุดนั้นเป็นเป้าหมาย
            targetPoint = hit.point;
        }
        else
        {
            // ถ้าไม่ชนอะไรเลย (มองท้องฟ้า) ให้เอาจุดที่อยู่ห่างออกไป 100 เมตรตามทิศทางกล้อง
            targetPoint = ray.GetPoint(100f);
        }

        // 2. คำนวณทิศทางจากปากกระบอกปืน (FirePoint) ไปยังจุดที่ Raycast ชน (TargetPoint)
        Vector3 targetDirection = (targetPoint - position).normalized;
        Quaternion rotation = Quaternion.LookRotation(targetDirection);

        GameObject projectile;

        // 3. สร้าง Projectile (รองรับทั้ง Offline และ PUN 2)
        if (PhotonNetwork.InRoom)
        {;
            projectile = PhotonNetwork.Instantiate(localProjectilePrefab.name, position, rotation);
        }
        else
        {
            projectile = Instantiate(localProjectilePrefab, position, rotation);
        }

        // 4. ใส่แรงส่งด้วย Rigidbody
        if (projectile.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = targetDirection * power; // ใช้ velocity โดยตรงจะแม่นยำกว่าในบางกรณี
            // หรือใช้ rb.AddForce(targetDirection * power, ForceMode.Impulse);
        }
    }

    public ChargeableWeapon GetChargeSystem()
    {
        if (chargeSystem == null) 
            chargeSystem = GetComponent<ChargeableWeapon>();
        return chargeSystem;
    }
}
