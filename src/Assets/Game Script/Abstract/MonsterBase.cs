using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System;
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
    public bool Hurt { get; set; }
    public bool IsAttackRotating { get; set; }
    public NavMeshAgent NavAIMesh { get { return aiAgent; } }
    public MonsterState MonsterState { get { return monsterState; } }

    public Action OnStartAttack {get;set;}
    public Action OnFinishAttack { get; set; }
    public Action OnMonsterDie { get; set; }

    protected virtual void Start()
    {
        OnDefaultSetData();
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

    public void OnDefaultSetData()
    {
        MaxHP = monsterData.GetStatValue("MaxHP");
        StopDistanceToTarget = monsterData.GetStatValue("StopDistance");

        currentHP = MaxHP;
        healthBarUI.UpdateHealthBar(MaxHP, currentHP);
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
    public void RPC_Knockback(Vector3 direction, float force)
    {
        LocalKnockback(direction, force);
    }
    private void LocalKnockback(Vector3 direction, float force)
    {
        // 1. หยุดเดินทันทีที่โดนตี
        if (aiAgent.isOnNavMesh)
        {
            aiAgent.isStopped = true;
            aiAgent.ResetPath(); // ล้างคำสั่งเดินที่ค้างอยู่ทั้งหมด
        }

        aiAgent.velocity = direction * force;
        // 2. ใช้ Coroutine จัดการการฟื้นตัวแทนการเขียนใน Update
        StopAllCoroutines();
        StartCoroutine(RecoverFromKnockback(direction,force));
    }
    public IEnumerator RecoverFromKnockback(Vector3 direction, float force)
    {
        //// รอให้แรงส่ง (Impact) ค่อยๆ หายไป
        //while (aiAgent.velocity.magnitude > 0.2f)
        //{
        //    aiAgent.velocity = Vector3.Lerp(aiAgent.velocity, Vector3.zero, 5 * Time.deltaTime);
        //    yield return null;
        //}

        //// 3. ปลดล็อกเพื่อให้ Behavior Tree กลับมาสั่งเดินได้อีกครั้ง
        //aiAgent.isStopped = false;
        //Debug.Log("AI recovered and ready to move.");
        float duration = 0.25f; // ระยะเวลาการกระเด็น (ปรับตามความเหมาะสม)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // คำนวณแรงที่ค่อยๆ เบาลงแบบ Linear Decay
            float strength = Mathf.Lerp(force, 0, elapsed / duration);

            // ใช้ Move แทนการเปลี่ยนตำแหน่งตรงๆ เพื่อความปลอดภัยบน NavMesh
            aiAgent.Move(direction * strength * Time.deltaTime);

            yield return null;
        }

        // 3. ปล่อยให้ AI กลับมาทำงานต่อ
        yield return new WaitForSeconds(0.1f); // รอให้นิ่งสักพักก่อนเดินต่อ

        if (aiAgent.isOnNavMesh)
        {
            aiAgent.isStopped = false;
        }
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
            monsterState.hurtSignal?.Invoke(transform,transform,false);
            healthBarUI.UpdateHealthBar(MaxHP, currentHP);
        }
        if (currentHP <= 0)
        {
            NavAIMesh.enabled = false;
            Die();
        }
    }

    protected virtual void Die()
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            OnMonsterDie?.Invoke();
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            OnMonsterDie?.Invoke();
            NetworkPrefabSpawner.Instance.Destroy(gameObject);
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
    public void RPC_AttackHandle()
    {
        AttackHandle();
    }

    public virtual void AttackHandle()
    {
        
    }
}
