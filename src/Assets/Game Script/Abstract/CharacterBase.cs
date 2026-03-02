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

    [Header("General Setting")]
    [SerializeField] private bool enableDamage = true;
    public bool EnableDamage { get; set; }

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        EnableDamage = enableDamage;
    }

    public virtual void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - armor, 0);

        if (PhotonNetwork.InRoom)
        {
            photonView.RPC("RPC_ApplyDamage", RpcTarget.All, finalDamage);
        }
        else
        {
            ApplyDamageLogic(finalDamage);
        }
    }

    [PunRPC]
    protected virtual void RPC_ApplyDamage(float damage)
    {
        ApplyDamageLogic(damage);
    }

    protected virtual void ApplyDamageLogic(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); 

        Debug.Log($"{gameObject.name} Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected abstract void Die();
}
