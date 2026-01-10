using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviourPunCallbacks, IDamageable
{
    [Header("Base Stats")]
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float armor = 0f;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    // ฟังก์ชันหลักสำหรับรับดาเมจ
    public virtual void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - armor, 0);

        // ตรวจสอบว่าอยู่ในห้อง Network หรือไม่
        if (PhotonNetwork.InRoom)
        {
            // ถ้าเล่นออนไลน์ ให้ส่ง RPC ไปบอกทุกคน
            photonView.RPC("RPC_ApplyDamage", RpcTarget.All, finalDamage);
        }
        else
        {
            // ถ้าเล่นคนเดียว (Offline) ให้คำนวณในเครื่องตัวเองทันที
            ApplyDamageLogic(finalDamage);
        }
    }

    [PunRPC]
    protected virtual void RPC_ApplyDamage(float damage)
    {
        // เมื่อได้รับคำสั่งจาก RPC ให้รัน Logic เดียวกัน
        ApplyDamageLogic(damage);
    }

    // แยก Logic การลดเลือดออกมาเป็นฟังก์ชันกลาง เพื่อให้ใช้ได้ทั้ง Solo และ Multi
    protected virtual void ApplyDamageLogic(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // ป้องกันเลือดติดลบ

        Debug.Log($"{gameObject.name} Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected abstract void Die();
}
