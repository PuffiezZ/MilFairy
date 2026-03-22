using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class Platform : MonoBehaviour
{
    [Header("Platform Settings")]
    [Tooltip("หาก Platform มี Scale ไม่ใช่ 1,1,1 แนะนำให้สร้าง GameObject เปล่าที่เป็นลูกของแพลตฟอร์ม (ตั้งค่า Scale 1,1,1) แล้วลากมาใส่ช่องนี้ เพื่อป้องกันโมเดลผู้เล่นเพี้ยน")]
    [SerializeField] private Transform mountPoint;

    // เก็บค่า Parent เดิมของ Player เพื่อตอนเดินออกจะได้กลับไปอยู่ Parent เดิม (หรือ Root)
    private Dictionary<Transform, Transform> originalParents = new Dictionary<Transform, Transform>();

    private void Awake()
    {
        // ถ้าไม่ได้ตั้งค่า mountPoint ไว้ ให้ใช้ตัว Platform เอง
        if (mountPoint == null)
        {
            mountPoint = transform;
        }

        // แจ้งเตือนหากลืมตั้ง Collider เป็น Trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"[Platform] Collider ของ {gameObject.name} ควรตั้งค่า Is Trigger เป็น true", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            
            // ให้ยึดติดแพลตฟอร์ม เฉพาะ: 
            // 1. ไม่มี PhotonView (เกม Offline แท้)
            // 2. ไม่ได้อยู่ใน Room (Offline ยืนยัน)
            // 3. เป็นผู้เล่นของเราเอง (IsMine) 
            if (pv == null || !PhotonNetwork.InRoom || pv.IsMine)
            {
                if (!originalParents.ContainsKey(other.transform))
                {
                    originalParents[other.transform] = other.transform.parent;
                }
                
                other.transform.SetParent(mountPoint);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView pv = other.GetComponent<PhotonView>();
            
            if (pv == null || !PhotonNetwork.InRoom || pv.IsMine)
            {
                if (originalParents.TryGetValue(other.transform, out Transform originalParent))
                {
                    other.transform.SetParent(originalParent);
                    originalParents.Remove(other.transform);
                }
                else
                {
                    other.transform.SetParent(null);
                }
            }
        }
    }
}
