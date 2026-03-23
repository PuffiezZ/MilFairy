using ExitGames.Client.Photon.StructWrapping;
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

    [Header("General Setting")]
    [SerializeField] private bool enableDamage = true;

    [Header("Monster Character Setting")]
    public float gravity = 9.81f;

    private Vector3 impact = Vector3.zero;
    private float verticalVelocity = 0f;
    public float StopDistanceToTarget { get; private set; }
    public float MaxHP { get; private set; }
    public float CurrentHP { get { return currentHP; } }
    public bool IsAttacking { get; set; }
    public bool Hurt { get; set; }
    public bool IsAttackRotating { get; set; }
    public bool EnableDamage { get; set; }
    public bool EnableHitBoxAttack { get; set; }
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

        EnableDamage = enableDamage;
        OnMonsterDie += () =>
        {
            NavAIMesh.enabled = false;
            Debug.Log($"{monsterName} has died.");
        };
    }

    public void OnDefaultSetData()
    {
        if(monsterData == null)
        {
            Debug.LogWarning("Monster Data is null. Fix it!");
            return;    
        }
        
        MaxHP = monsterData.GetStatValue("MaxHP");
        StopDistanceToTarget = monsterData.GetStatValue("StopDistance");

        currentHP = MaxHP;
        healthBarUI.UpdateHealthBar(MaxHP, currentHP);
    }

    public virtual void TakeDamage(float damage, GameObject source = null)
    {
        Debug.Log($"Monster get attack by {source}");
        float finalDamage = damage; 
        int pvSourceID = source.GetComponent<PhotonView>().ViewID;
        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_TakeDamage", RpcTarget.All, finalDamage, pvSourceID);
        }
        else
        {
            localTakeDamage(finalDamage, source);
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
        if (aiAgent.isOnNavMesh)
        {
            aiAgent.isStopped = true;
            aiAgent.ResetPath(); 
        }

        aiAgent.velocity = direction * force;

        StopAllCoroutines();
        StartCoroutine(RecoverFromKnockback(direction,force));
    }
    public IEnumerator RecoverFromKnockback(Vector3 direction, float force)
    {
        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float strength = Mathf.Lerp(force, 0, elapsed / duration);

            aiAgent.Move(direction * strength * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        if (aiAgent.isOnNavMesh)
        {
            aiAgent.isStopped = false;
        }
    }

    [PunRPC]
    public virtual void RPC_TakeDamage(float damage, int pvPlayerID)
    {
        // แก้ไข: ใช้ PhotonView.Find แทน TagObject เพื่อความแม่นยำในระบบ Network
        PhotonView targetPlayerView = PhotonView.Find(pvPlayerID);
        
        if (!PhotonNetwork.IsMasterClient) return;
        
        if(targetPlayerView == null)
        {
            Debug.LogWarning("RPC_TakeDamage not get targetPlayerView");
            return;
        } 
        
       localTakeDamage(damage, targetPlayerView.gameObject);

    }
    private void localTakeDamage(float damage, GameObject source)
    {
        if(PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_UpdateCurrentHP), RpcTarget.All, damage);
        }
        else
        {
            currentHP -= damage;
            
            if (healthBarUI != null)
            {
                monsterState.hurtSignal?.Invoke(transform,transform,false);
                healthBarUI.UpdateHealthBar(MaxHP, currentHP);
            }
        }

        MonsterPerception monsterPerception = GetComponent<MonsterPerception>();
        monsterPerception.MonsterGetHurt(source);
        
        Debug.Log($"{monsterName} took {damage} damage. Current HP: {currentHP}/{MaxHP}");

    }
    [PunRPC]
    public void RPC_UpdateCurrentHP(float damage)
    {
        currentHP -= damage;
        
        if (healthBarUI != null)
        {
            monsterState.hurtSignal?.Invoke(transform,transform,false);
            healthBarUI.UpdateHealthBar(MaxHP, currentHP);
        }
    }

    public virtual void Die()
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
            // �� RPC �������ء������͹����ѹ����
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
