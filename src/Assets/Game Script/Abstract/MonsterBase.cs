using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBase : MonoBehaviourPunCallbacks,IDamageable,IKnockback
{
    [Header("Base Settings")]
    public float maxHP = 100f;
    protected float currentHP;
    public string monsterName;

    [Header("Health Bar References")]
    [SerializeField] private HealthBar healthBarUI;

    [Header("Monster Character Setting")]
    public float gravity = 9.81f;

    private CharacterController controller;
    private Vector3 impact = Vector3.zero;
    private float verticalVelocity = 0f;
    protected virtual void Start()
    {
        currentHP = maxHP;
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(maxHP, currentHP);
        }

        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        HandleKnockback();
        HandleVerticleVelocity();
    }
    private void HandleKnockback()
    {
        // ค่อยๆ ลดแรงกระแทกลงตามเวลา (Friction)
        if (impact.magnitude > 0.2f)
        {
            controller.Move(impact * Time.deltaTime);
        }
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

    }
    private void HandleVerticleVelocity()
    {
        // 1. คำนวณแรงโน้มถ่วง
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        verticalVelocity -= gravity * Time.deltaTime;

        // 2. คำนวณแรง Knockback (Friction)
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        // 3. รวมแรงทั้งหมด (Horizontal Impact + Vertical Gravity)
        Vector3 finalMove = impact + (Vector3.up * verticalVelocity);

        // 4. สั่งเคลื่อนที่เพียงครั้งเดียว
        controller.Move(finalMove * Time.deltaTime);
    }
    public virtual void TakeDamage(float damage)
    {
        float finalDamage = damage; // คุณอาจเพิ่มการคำนวณดาเมจที่นี่ เช่น ลดตามเกราะ
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_TakeDamage", RpcTarget.All, finalDamage);
        }
        else
        {
            localTakeDamage(finalDamage);
        }
    }

    public void Knockback(Vector3 direction, float force)
    {
        if(PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_Knockback), RpcTarget.All, direction, force);
        }
        else
        {
            LocalKnockback(direction,force);
        }
    }
    [PunRPC]
    private void RPC_Knockback(Vector3 direction, float force)
    {
        LocalKnockback(direction, force);
    }
    private void LocalKnockback(Vector3 direction, float force)
    {
        impact = direction * force;
    }
    [PunRPC]
    public virtual void RPC_TakeDamage(float damage)
    {
        localTakeDamage(damage);
    }
    private void localTakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{monsterName} took {damage} damage. Current HP: {currentHP}/{maxHP}");
        // เล่น Effect เลือดกระเด็น หรือแอนิเมชันโดนตี
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(maxHP, currentHP);
        }
        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
