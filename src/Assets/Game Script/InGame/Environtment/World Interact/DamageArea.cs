using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DamageArea : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("Damage ที่จะให้กับ Player แต่ละครั้ง")]
    [SerializeField] private float damageAmount = 10f;
    
    [Tooltip("ระยะเวลา (วินาที) ระหว่างการทำดาเมจแต่ละครั้ง")]
    [SerializeField] private float damageInterval = 1f;
    
    private const string playerTag = "Player";
    private const string payloadTag = "Payload";
    

    // เก็บเวลาที่จะทำดาเมจครั้งต่อไปของแต่ละ Collider (รองรับผู้เล่นหลายคน)
    private Dictionary<Collider, float> nextDamageTime = new Dictionary<Collider, float>();

    private void OnTriggerStay(Collider other)
    {
        // ตรวจสอบว่า Collider ที่เข้ามามี Tag เป็น Player หรือไม่
        if (other.CompareTag(playerTag) || other.CompareTag(payloadTag))
        {
            // ถ้าเพิ่งเข้ามาครั้งแรก หรือถึงเวลาที่ต้องรับดาเมจรอบถัดไป
            if (!nextDamageTime.ContainsKey(other) || Time.time >= nextDamageTime[other])
            {
                // ดึง Component IDamageable จาก GameObject ที่ Collider อยู่
                IDamageable damageable = other.GetComponent<IDamageable>();

                // ตรวจสอบว่า GameObject มี Component IDamageable หรือไม่
                if (damageable != null)
                {
                    // เรียกใช้ฟังก์ชัน TakeDamage
                    ApplyDamage(damageable, other.gameObject);
                    
                    // ตั้งเวลาสำหรับการทำดาเมจครั้งถัดไป
                    nextDamageTime[other] = Time.time + damageInterval;
                }
            }
        }
    }

    private void ApplyDamage(IDamageable damageable, GameObject player)
    {
        // ตรวจสอบว่าเป็น Multiplayer หรือ Singleplayer
        if (PhotonNetwork.InRoom)
        {
            // ดึง PhotonView จาก Player
            PhotonView pv = player.GetComponent<PhotonView>();

            // ตรวจสอบว่ามี PhotonView และเป็นของผู้เล่นคนนี้หรือไม่
            if (pv != null && pv.IsMine)
            {
                // Apply Damage โดยตรง (เนื่องจาก IDamageable ไม่ได้เป็น MonoBehaviour)
                damageable.TakeDamage(damageAmount, gameObject);
            }
        }
        else
        {
            // ถ้าเป็น Singleplayer ก็ Apply Damage ได้เลย
            damageable.TakeDamage(damageAmount, gameObject);
        }
    }
    
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag(playerTag))
        {
            // ลบข้อมูลออกเมื่อ Player ออกจากพื้นที่
            if (nextDamageTime.ContainsKey(other))
            {
                nextDamageTime.Remove(other);
            }
            Debug.Log("Player exited the damage area.");
        }
    }
}