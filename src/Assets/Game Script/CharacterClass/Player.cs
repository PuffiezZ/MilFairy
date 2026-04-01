using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Events;
using static UtilityDev;
using static UtilityDev.ResourceType;

public class Player : CharacterBase,IPickupable
{
    [SerializeField] private GameObject playerModel; // ���͹˹�������ѻƹ���
 
    [Header("Throw Settings")]
    public float minThrowForce = 0.1f;
    public float maxThrowForce = 25f;
    public float maxChargeTime = 2f;
    public Vector3 throwDirectionOffset = new Vector3(0, 0.5f, 1f); // ���ҧ仢�ҧ˹�������§�����硹���

    // �� Event �����Ҥ�¡ѹ��͹˹�������ѻവ UI
    public static event Action<float, float> OnPlayerHealthChanged;
    public static event Action<UtilityDev.ResourceType, float, int> OnResourceValueChanged;
    public static event Action OnMainActionCalled;
    public static event Action OnPlayerDie;
    public static event Action OnPlayerRespawn;

    private float[] percentageProgressResource = new float[6];
    private int[] amountResource = new int[6];

    private PlayerCombat playerCombat;
    private PhotonView pv;
    
    public HoldableObject CurrentHoldable {get; private set;}

    private void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        pv = GetComponent<PhotonView>();

        
        SetActionLeftClick(pv, playerCombat.OnInvokeAttack);
    }
    public static bool CheckboolActionLeftClick(Action getAction)
    {
        return OnMainActionCalled == getAction;
    }
    public static void SetActionLeftClick(PhotonView view, Action getAction = null)
    {
        // ตรวจสอบว่าเป็น P2P (InRoom) หรือ Offline
        if (PhotonNetwork.InRoom)
        {
            // ตรวจสอบความเป็นเจ้าของในแบบ static โดยใช้ PhotonView ที่ส่งเข้ามา
            // หากไม่ใช่เจ้าของ (Remote Player) จะไม่สามารถเปลี่ยนค่า Action ของเครื่องนี้ได้
            if (view != null && !view.IsMine) return;
        }

        OnMainActionCalled = null;
        OnMainActionCalled = getAction;
    }
    public void SetHoldableObject(PhotonView view,HoldableObject holdableObject)
    {
         // ตรวจสอบว่าเป็น P2P (InRoom) หรือ Offline
        if (PhotonNetwork.InRoom)
        {
            // ตรวจสอบความเป็นเจ้าของในแบบ static โดยใช้ PhotonView ที่ส่งเข้ามา
            // หากไม่ใช่เจ้าของ (Remote Player) จะไม่สามารถเปลี่ยนค่า Action ของเครื่องนี้ได้
            if (view != null && !view.IsMine) return;
        }
        CurrentHoldable = holdableObject;
    }
    
    private void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NetworkPrefabSpawner.Instance.SpawnResource("Sword", photonView);
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            RoomManager.Instance.TriggerWinCondition();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            RoomManager.Instance.TriggerLoseCondition();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TakeDamage(100f);
        }
    }
    public void InvokeCallOnMainActionCalled()
    {
        OnMainActionCalled?.Invoke();
    }

    public override void TakeDamage(float damage, GameObject source = null)
    {
        base.TakeDamage(damage, source); // ¡ Logic Ŵʹҡ
        CallUpdatePlayerUIHealth();
    }
    private void CallUpdatePlayerUIHealth()
    {
        if (PhotonNetwork.InRoom)
        {
            if(!photonView.IsMine) return; 
            photonView.RPC(nameof(RPC_UpdatePlayerHealthUI), RpcTarget.All, currentHealth, maxHealth);
        }
        else
        {
            LocalUpdatePlayerHealthUI(currentHealth, maxHealth);
        }
    }

    public void OnMountingPayload()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.SetEnableCharacterMovement(false);
        pm.SwitchingMovement(true);
        pm.IsMounting = true;
    }
    public void OnDismountingPayload()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.SetEnableCharacterMovement(true);
        pm.SwitchingMovement(false);
        pm.IsMounting = false;
    }
    [PunRPC]
    public void RPC_UpdatePlayerHealthUI(float currentHealthValue, float maxHealthValue)
    {
        if (!photonView.IsMine && PhotonNetwork.InRoom) return;
        LocalUpdatePlayerHealthUI(currentHealthValue, maxHealth);
    }
    private void LocalUpdatePlayerHealthUI(float currentHealthValue, float maxHealthValue)
    {
        OnPlayerHealthChanged?.Invoke(currentHealthValue, maxHealthValue);
    }

    protected override void Die()
    {
        Debug.Log("Player Died! Show GameOver UI");
                               
        if(PhotonNetwork.InRoom)
            if(!photonView.IsMine) return;
                
         PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.SetEnableCharacterMovement(false);
        playerModel.SetActive(false);    
        OnPlayerDie?.Invoke();

        // สั่งให้ RoomManager ทำการ Respawn ผู้เล่นคนนี้ (ส่งตัวเองไป และรอ 3 วินาที)
        RoomManager.Instance.RespawnPlayer(this, 3f);
    }

    public void Respawn()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine) return;

        // Reset เลือดให้เต็ม และอัปเดตไปที่ UI
        currentHealth = maxHealth;
        CallUpdatePlayerUIHealth();

        // เปิด Model และเปิดการเคลื่อนไหวให้กลับมาเดินได้ปกติ
        playerModel.SetActive(true);
        PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.SetEnableCharacterMovement(true);
        
        OnPlayerRespawn?.Invoke();
    }

    #region Pick Up Resource Handle
    public void OnPickResourceInvoke(UtilityDev.ResourceType resourceType, float percentage)
    {
        if (photonView.IsMine && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_ApplyPickResource), RpcTarget.All, resourceType, percentage);
        }
        else
        {
            ChangeResourceAmount(resourceType, percentage);
        }
    }

    [PunRPC]
    private void RPC_ApplyPickResource(UtilityDev.ResourceType resourceType, float percentage)
    {
        if (!photonView.IsMine && PhotonNetwork.InRoom) return;
        ChangeResourceAmount(resourceType, percentage);
    }

    private void ChangeResourceAmount(UtilityDev.ResourceType resourceType, float percentage)
    {
        // ��ͤ��������� "੾����Ңͧ����Фù����ҹ��" ��������¹���㹵��������ѻവ UI
        if (!photonView.IsMine && PhotonNetwork.InRoom) return;

        percentageProgressResource[(int)resourceType] += percentage;

        if (percentageProgressResource[(int)resourceType] >= 1.00f)
        {
            percentageProgressResource[(int)resourceType] -= 1f;
            amountResource[(int)resourceType]++;
        }

        // �͹�������ͧ�¡ if-else ����Ѻ InRoom ���� ���� IsMine ��ͺ�������
        OnResourceValueChanged?.Invoke(resourceType, percentageProgressResource[(int)resourceType], amountResource[(int)resourceType]);

        Debug.Log($"Resource Updated for {gameObject.name}: {resourceType}, Amount: {amountResource[(int)resourceType]}");
    }
    #endregion
}
