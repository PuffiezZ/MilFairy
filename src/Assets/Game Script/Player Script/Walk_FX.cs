using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Walk_FX : MonoBehaviourPun
{
    [Header("VFX Settings")]
    [Tooltip("ลาก Prefab ของ Particle System (เช่น ฝุ่นเดิน) มาใส่ที่นี่")]
    [SerializeField] private GameObject footStepVFXPrefab;
    
    [Tooltip("ความสูงจากพื้นเล็กน้อย เพื่อไม่ให้ VFX จมดิน (ค่าแนะนำ 0.05 - 0.1)")]
    [SerializeField] private float groundOffset = 0.05f;

    [Header("Foot Bones References")]
    [Tooltip("ลากกระดูกเท้าซ้าย (Left Foot Bone) จาก Model มาใส่")]
    [SerializeField] private Transform leftFootBone;
    
    [Tooltip("ลากกระดูกเท้าขวา (Right Foot Bone) จาก Model มาใส่")]
    [SerializeField] private Transform rightFootBone;

    [Header("Logic Settings")]
    [Tooltip("VFX จะไม่ทำงานถ้าค่า Magnitude ใน Animator ต่ำกว่าค่านี้ (แนะนำ 0.1)")]
    [SerializeField] private float speedThreshold = 0.1f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // ฟังก์ชันนี้จะถูกเรียกใช้โดย Animation Event
    public void OnLeftFootStep()
    {
        TriggerVFX(0); // 0 คือเท้าซ้าย
    }

    // ฟังก์ชันนี้จะถูกเรียกใช้โดย Animation Event
    public void OnRightFootStep()
    {
        TriggerVFX(1); // 1 คือเท้าขวา
    }

    private void TriggerVFX(int footIndex)
    {
        // ตรวจสอบความเร็วจาก Animator ก่อน 
        // ถ้า Magnitude ต่ำกว่า threshold ที่ตั้งไว้ (เช่น ช่วงที่กำลังหยุด) จะไม่ส่ง RPC และไม่สร้างควัน
        if (animator != null)
        {
            float currentSpeed = animator.GetFloat("Magnitude");
            if (currentSpeed < speedThreshold) return;
        }

        // ถ้าเล่นแบบ Online และเราเป็นเจ้าของตัวละคร
        if (PhotonNetwork.InRoom)
        {
            if (photonView.IsMine)
            {
                // ส่ง RPC ไปให้ทุกคน (รวมตัวเองด้วย) เพื่อสร้างควัน
                photonView.RPC(nameof(RPC_SpawnFootstepVFX), RpcTarget.All, footIndex);
            }
        }
        else
        {
            // ถ้าเล่นแบบ Offline ก็สร้างควันได้เลยทันที
            RPC_SpawnFootstepVFX(footIndex);
        }
    }

    [PunRPC]
    public void RPC_SpawnFootstepVFX(int footIndex)
    {
        // เลือกว่าจะใช้กระดูกชิ้นไหนตามที่ส่งมาใน RPC
        Transform targetFoot = (footIndex == 0) ? leftFootBone : rightFootBone;
        SpawnVFX(targetFoot);
    }


    private void SpawnVFX(Transform footTransform)
    {
        if (footStepVFXPrefab == null || footTransform == null) return;

        // --- การจัดการระบบ P2P (PUN2) และ Offline ---
        // สำหรับ VFX ที่เรียกผ่าน Animation Event เราไม่จำเป็นต้องใช้ RPC (Remote Procedure Call) 
        // เพราะ Animator ของทุกเครื่องจะเล่นท่าเดินพร้อมๆ กันอยู่แล้ว 
        // ดังนั้นโค้ดด้านล่างจะทำงานแบบ "Local" ในทุกเครื่องที่มองเห็นตัวละครนี้

        // หาตำแหน่งที่พื้น (ใต้เท้า)
        Vector3 spawnPosition = footTransform.position;
        spawnPosition.y += groundOffset; // ยกขึ้นนิดหน่อยกันจม

        // สร้าง VFX ขึ้นมา (Instantiate) โดยใช้ Rotation ของ World (Quaternion.identity) หรือจะใช้ของเท้าก็ได้
        GameObject vfxInstance = Instantiate(footStepVFXPrefab, spawnPosition, Quaternion.identity);

        // คำแนะนำสำหรับ Tech Art:
        // หากคุณต้องการเล่น Effect พิเศษ "เฉพาะเครื่องของผู้เล่นคนนั้น" 
        // เช่น Camera Shake (หน้าจอสั่นตอนวิ่ง) หรือเสียงลมข้างหู
        // ให้เขียนเช็คดังนี้:
        // if (photonView.IsMine) { 
        //    /* โค้ดที่ใส่ตรงนี้จะทำงานเฉพาะเจ้าของตัวละครเท่านั้น คนอื่นจะไม่เห็น/ไม่ได้ยิน */ 
        // }
        
        // แนะนำ: ให้ตั้งค่าใน Prefab ของ Particle ให้ "Stop Action" เป็น "Destroy" เพื่อให้มันทำลายตัวเองเมื่อเล่นจบ
        // หรือถ้าไม่มี ให้ใช้ Destroy(vfxInstance, 2f); เพื่อทำลายใน 2 วินาที
        Destroy(vfxInstance, 2f);
    }
}
