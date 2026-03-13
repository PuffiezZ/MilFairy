using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    public bool EnableDamage 
    { 
        get => _enableDamage; 
        set => _enableDamage = value; 
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        
        // ตรวจสอบ AudioSource หากไม่ได้ตั้งค่าไว้ใน Inspector จะพยายามดึงจากเครื่องตัวเอง
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        // ลงทะเบียนรถเข้า Minimap ทันทีที่เริ่มเกม
        MinimapMilfairy mm = FindObjectOfType<MinimapMilfairy>();
    }

    /// <summary>
    /// ฟังก์ชันรับดาเมจตาม Interface IDamageable
    /// </summary>
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
            // ส่ง RPC ให้ทุกเครื่องประมวลผลดาเมจและแสดงผล Effect พร้อมกัน
            photonView.RPC(nameof(RPC_ApplyDamage), RpcTarget.All, damage, attackerName);
        }
        else
        {
            // กรณีเล่นคนเดียว (Offline)
            ApplyDamageLogic(damage, attackerName);
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
        }
    }
}