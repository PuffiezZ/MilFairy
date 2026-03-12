using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class WalkFx_hamter : MonoBehaviourPun
{
    [Header("VFX Settings")]
    [Tooltip("ลาก Prefab ของควันเดินมาใส่ที่นี่")]
    [SerializeField] private GameObject footStepVFXPrefab;
    
    [Tooltip("ความสูงจากพื้นเล็กน้อย เพื่อไม่ให้ VFX จมดิน")]
    [SerializeField] private float groundOffset = 0.05f;

    [Header("Audio Settings")]
    [Tooltip("ลากไฟล์เสียงเดินของสัตว์มาใส่ที่นี่")]
    [SerializeField] private AudioClip footstepSFX;

    [Header("4-Legged Foot Bones")]
    [SerializeField] private Transform frontLeftFoot;
    [SerializeField] private Transform frontRightFoot;
    [SerializeField] private Transform backLeftFoot;
    [SerializeField] private Transform backRightFoot;

    [Header("Logic Settings")]
    [Tooltip("VFX จะไม่ทำงานถ้าค่า Magnitude ใน Animator ต่ำกว่านี้")]
    [SerializeField] private float speedThreshold = 0.1f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // --- ฟังก์ชันสำหรับ Animation Events (เรียกจากตัวละคร 4 ขา) ---
    public void OnFrontLeftStep() => TriggerVFX(0);
    public void OnFrontRightStep() => TriggerVFX(1);
    public void OnBackLeftStep() => TriggerVFX(2);
    public void OnBackRightStep() => TriggerVFX(3);

    private void TriggerVFX(int footIndex)
    {
        // ตรวจสอบความเร็ว Magnitude เพื่อไม่ให้ควันออกตอนกำลังหยุดเดิน (Blend Tree issue)
        if (animator != null)
        {
            float currentSpeed = animator.GetFloat("Magnitude");
            if (currentSpeed < speedThreshold) return;
        }

        // จัดการระบบ Network (PUN2)
        if (PhotonNetwork.InRoom)
        {
            // ส่ง RPC เฉพาะเครื่องที่เป็นเจ้าของตัวละคร (IsMine)
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(RPC_SpawnFootstepVFX_Hamter), RpcTarget.All, footIndex);
            }
        }
        else
        {
            // ถ้าเล่นคนเดียว (Offline)
            RPC_SpawnFootstepVFX_Hamter(footIndex);
        }
    }

    [PunRPC]
    private void RPC_SpawnFootstepVFX_Hamter(int footIndex)
    {
        Transform targetFoot = null;
        
        // เลือกว่าเท้าข้างไหนย่ำพื้น
        switch (footIndex)
        {
            case 0: targetFoot = frontLeftFoot; break;
            case 1: targetFoot = frontRightFoot; break;
            case 2: targetFoot = backLeftFoot; break;
            case 3: targetFoot = backRightFoot; break;
        }

        SpawnVFX(targetFoot);
    }

    private void SpawnVFX(Transform footTransform)
    {
        if (footStepVFXPrefab == null || footTransform == null) return;

        Vector3 spawnPosition = footTransform.position;
        spawnPosition.y += groundOffset;

        // สร้าง VFX
        GameObject vfxInstance = Instantiate(footStepVFXPrefab, spawnPosition, Quaternion.identity);
        
        // เรียกใช้ Sound Manager ส่วนกลาง (main)
        if (Main.Instance != null && footstepSFX != null)
        {
            Main.Instance.PlaySFX(footstepSFX);
        }

        // ทำลายอัตโนมัติใน 2 วินาที (หรือตั้งค่าที่ Particle System: Stop Action -> Destroy)
        Destroy(vfxInstance, 2f);
    }
}
