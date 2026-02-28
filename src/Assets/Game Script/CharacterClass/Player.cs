using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System;
using UnityEngine;
using UnityEngine.Events;
using static UtilityDev;
using static UtilityDev.ResourceType;

public class Player : CharacterBase,IPickupable
{

    [Header("Throw Settings")]
    public float minThrowForce = 5f;
    public float maxThrowForce = 25f;
    public float maxChargeTime = 2f;
    public Vector3 throwDirectionOffset = new Vector3(0, 0.5f, 1f); // ���ҧ仢�ҧ˹�������§�����硹���

    // �� Event �����Ҥ�¡ѹ��͹˹�������ѻവ UI
    public static event Action<float, float> OnPlayerHealthChanged;
    public static event Action<UtilityDev.ResourceType, float, int> OnResourceValueChanged;
    public static event Action OnMainActionCalled;

    private float[] percentageProgressResource = new float[6];
    private int[] amountResource = new int[6];

    private PlayerCombat playerCombat;

    private void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        SetActionLeftClick(playerCombat.OnInvokeAttack);
    }
    public static bool CheckboolActionLeftClick(Action getAction)
    {
        return OnMainActionCalled == getAction;
    }
    public static void SetActionLeftClick(Action getAction = null)
    {
        OnMainActionCalled = null;
        OnMainActionCalled = getAction;
    }
    private void Update()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            NetworkPrefabSpawner.Instance.SpawnResource(ResourceType.Scrap.ToString(), photonView);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            NetworkPrefabSpawner.Instance.SpawnResource(ResourceType.Stick.ToString(), photonView);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            NetworkPrefabSpawner.Instance.SpawnResource(ResourceType.Electronic.ToString(), photonView);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            NetworkPrefabSpawner.Instance.SpawnResource(ResourceType.Oil.ToString(), photonView);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            NetworkPrefabSpawner.Instance.SpawnResource(ResourceType.Clothes.ToString(), photonView);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NetworkPrefabSpawner.Instance.SpawnResource("Sword", photonView);
        }

    }
    public void InvokeCallOnMainActionCalled()
    {
        OnMainActionCalled?.Invoke();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // ���¡�� Logic Ŵ���ʹ�ҡ������
        CallUpdatePlayerUIHealth();
    }
    private void CallUpdatePlayerUIHealth()
    {
        // ����繵���Фâͧ��� ���͡ UI Manager ����
        if (photonView.IsMine && PhotonNetwork.InRoom)
        {
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

        // ปิดการ Sync ตำแหน่งชั่วคราว เพื่อให้เคลื่อนที่ตาม Parent (Payload) ได้โดยไม่โดนขัดขวางจากข้อมูล Network
        if (TryGetComponent<PhotonTransformView>(out var ptv))
        {
            ptv.enabled = false;
        }
    }
    public void OnDismountingPayload()
    {
        PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.SetEnableCharacterMovement(true);
        pm.SwitchingMovement(false);
        pm.IsMounting = false;

        // เปิดการ Sync ตำแหน่งกลับมาเมื่อลงจากรถ
        if (TryGetComponent<PhotonTransformView>(out var ptv))
        {
            ptv.enabled = true;
        }
    }
    [PunRPC]
    public void RPC_UpdatePlayerHealthUI(float currentHealthValue, float maxHealthValue)
    {
        LocalUpdatePlayerHealthUI(currentHealthValue, maxHealth);
    }
    private void LocalUpdatePlayerHealthUI(float currentHealthValue, float maxHealthValue)
    {
        OnPlayerHealthChanged?.Invoke(currentHealthValue, maxHealthValue);
    }

    protected override void Die()
    {
        Debug.Log("Player Died! Show GameOver UI");
        // Logic ����Դ���� ���͡�����Ⱦ
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
