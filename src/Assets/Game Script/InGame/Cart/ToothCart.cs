using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(typeof(PhotonView))]
public class ToothCart : MonoBehaviourPunCallbacks, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 500f;
    [SerializeField] private float currentHealth;
    [SerializeField] private bool _enableDamage = true;

    [Header("Feedback Effects")]
    [SerializeField] private GameObject hitVfxPrefab;
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioSource audioSource;

    [Header("Minimap Settings")]
    [SerializeField] private Sprite cartIcon;
    
    [Header("Tooth Models")]
    [SerializeField] private GameObject normalTooth;
    [SerializeField] private GameObject decayTooth;

    public event Action<float, float> OnPayloadHealthChanged;
    public bool EnableDamage 
    { 
        get => _enableDamage; 
        set => _enableDamage = value; 
    }
    public override void OnEnable() 
    {
        UIManager uiM = FindObjectOfType<UIManager>();
        
        if(uiM == null) return;
        
        uiM.RegisterPayloadHealthBar(this);
    }

    private void Awake()
    {
        currentHealth = maxHealth;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        // ลงทะเบียนรถเข้า Minimap ทันทีที่เริ่มเกม
        MinimapMilfairy mm = FindObjectOfType<MinimapMilfairy>();
        
        if(PhotonNetwork.InRoom)
        {
            if(PhotonNetwork.IsMasterClient)
                photonView.RPC(nameof(RPC_ChangeModelByHP), RpcTarget.All, currentHealth);
        }
        else
        {
            OnLocalChangeModelByHP(currentHealth);
        }
    }
    public void TakeDamage(float damage, GameObject source = null)
    {
        if (EnableDamage == false)
        {
            return;
        }

        // กรองดาเมจ: หาก Source คือผู้เล่น (ผ่าน Tag "Player") จะไม่ได้รับดาเมจ
        if (source != null)
        {
            if (source.CompareTag("Player"))
            {
                return;
            }
        }

        string attackerName = "Unknown/Environment";
        if (source != null)
        {
            attackerName = source.name;
        }

        if (PhotonNetwork.InRoom)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(RPC_ApplyDamage), RpcTarget.All, damage, attackerName);
                photonView.RPC(nameof(RPC_ChangeModelByHP), RpcTarget.All, currentHealth);
            }
        }
        else
        {
            // กรณีเล่นคนเดียว (Offline)
            ApplyDamageLogic(damage, attackerName);
            OnLocalChangeModelByHP(currentHealth);
        }
    }

    [PunRPC]
    private void RPC_ApplyDamage(float damage, string attackerName)
    {
        ApplyDamageLogic(damage, attackerName);
    }

    private void ApplyDamageLogic(float damage, string attackerName)
    {
        Debug.Log($"[ToothCart] ถูกโจมตีโดย: {attackerName} เป็นจำนวน {damage} dmg");
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        OnPayloadHealthChanged?.Invoke(currentHealth, maxHealth);     

        // เล่น VFX (Instantiate Prefab) ณ ตำแหน่งของรถ
        if (hitVfxPrefab != null)
        {
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);
        }

        // เล่นเสียง SFX
        if (hitSfx != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSfx);
        }

        if (currentHealth <= 0)
        {
            Debug.Log("ToothCart is destroyed!");
            // คุณสามารถเพิ่ม Logic เมื่อรถพังตรงนี้ เช่น เรียก Game Over
            
            RoomManager.Instance.TriggerLoseCondition();
        }
    }
    
    private void OnLocalChangeModelByHP(float currentHP)
    {
        float halfMaxHP = maxHealth / 2f;
        if(currentHealth > halfMaxHP)
        {
            normalTooth.SetActive(true);
            decayTooth.SetActive(false);
        }
        else
        {
            normalTooth.SetActive(false);
            decayTooth.SetActive(true);
        }
    }
    
    [PunRPC]
    public void RPC_ChangeModelByHP(float currentHP)
    {
        OnLocalChangeModelByHP(currentHP);
    }
}