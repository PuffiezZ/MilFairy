using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UtilityDev;

public class MonsterBase : MonoBehaviourPunCallbacks,IDamageable,IKnockback,IAttackable
{
    [Header("Base Settings")]
    public MonsterData monsterData;
    protected float currentHP;
    public string monsterName;

    [Header("Script Refernce")]
    [SerializeField] private HealthBar healthBarUI;
    [SerializeField] protected NavMeshAgent aiAgent;
    [SerializeField] private MonsterState monsterState;

    [Header("Monster Character Setting")]
    public float gravity = 9.81f;

    private Vector3 impact = Vector3.zero;
    private float verticalVelocity = 0f;
    public float StopDistanceToTarget { get; private set; }
    public float MaxHP { get; private set; }
    public bool IsAttacking { get; set; }
    public bool IsAttackRotating { get; set; }
    public NavMeshAgent NavAIMesh { get { return aiAgent; } }
    public MonsterState MonsterState { get { return monsterState; } }

    protected virtual void Start()
    {
        OnDefaultSetData(monsterData);
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(MaxHP, currentHP);
        }
        if(aiAgent != null)
        {
            aiAgent.stoppingDistance = StopDistanceToTarget;
            aiAgent.speed = monsterData.GetStatValue("MoveSpeed");
        }
    }

    public void OnDefaultSetData(MonsterData mData)
    {
        MaxHP = mData.GetStatValue("MaxHP");
        currentHP = MaxHP;

        StopDistanceToTarget = mData.GetStatValue("StopDistance");
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
        // 1. หยุดเดินทันทีที่โดนตี
        aiAgent.isStopped = true;
        aiAgent.velocity = direction * force;
        // 2. ใช้ Coroutine จัดการการฟื้นตัวแทนการเขียนใน Update
        StopAllCoroutines();
        StartCoroutine(RecoverFromKnockback());
    }
    public IEnumerator RecoverFromKnockback()
    {
        // รอให้แรงส่ง (Impact) ค่อยๆ หายไป
        while (aiAgent.velocity.magnitude > 0.2f)
        {
            aiAgent.velocity = Vector3.Lerp(aiAgent.velocity, Vector3.zero, 5 * Time.deltaTime);
            yield return null;
        }

        // 3. ปลดล็อกเพื่อให้ Behavior Tree กลับมาสั่งเดินได้อีกครั้ง
        aiAgent.isStopped = false;
        Debug.Log("AI recovered and ready to move.");
    }
    [PunRPC]
    public virtual void RPC_TakeDamage(float damage)
    {
        localTakeDamage(damage);
    }
    private void localTakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{monsterName} took {damage} damage. Current HP: {currentHP}/{MaxHP}");
        // เล่น Effect เลือดกระเด็น หรือแอนิเมชันโดนตี
        if (healthBarUI != null)
        {
            healthBarUI.UpdateHealthBar(MaxHP, currentHP);
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

    public void OnCallAttack()
    {
        if (PhotonNetwork.InRoom)
        {
            // ส่ง RPC เพื่อให้ทุกคนเล่นแอนิเมชันโจมตี
            photonView.RPC(nameof(RPC_AttackHandle), RpcTarget.All);
        }
        else
        {
            AttackHandle();
        }
    }
    [PunRPC]
    private void RPC_AttackHandle()
    {
        AttackHandle();
    }

    public virtual void AttackHandle()
    {
        
    }
}
