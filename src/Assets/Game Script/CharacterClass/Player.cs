using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UtilityDev;
using static UtilityDev.ResourceType;

public class Player : CharacterBase,IPickupable
{
    // ใช้ Event ที่เราคุยกันก่อนหน้าเพื่ออัปเดต UI
    public static event Action<float, float> OnPlayerHealthChanged;
    public static event Action<UtilityDev.ResourceType, float, int> OnResourceValueChanged;

    private float[] percentageProgressResource = new float[6];
    private int[] amountResource = new int[6];

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

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage); // เรียกใช้ Logic ลดเลือดจากตัวแม่
        CallUpdatePlayerUIHealth();


    }
    private void CallUpdatePlayerUIHealth()
    {
        // ถ้าเป็นตัวละครของเรา ให้บอก UI Manager ด้วย
        if (photonView.IsMine && PhotonNetwork.InRoom)
        {
            photonView.RPC(nameof(RPC_UpdatePlayerHealthUI), RpcTarget.All, currentHealth, maxHealth);
        }
        else
        {
            LocalUpdatePlayerHealthUI(currentHealth, maxHealth);
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
        // Logic การเกิดใหม่ หรือกลายเป็นศพ
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
        // ล็อคไว้เลยว่า "เฉพาะเจ้าของตัวละครนี้เท่านั้น" ที่จะเปลี่ยนค่าในตัวแปรและอัปเดต UI
        if (!photonView.IsMine && PhotonNetwork.InRoom) return;

        percentageProgressResource[(int)resourceType] += percentage;

        if (percentageProgressResource[(int)resourceType] >= 1.00f)
        {
            percentageProgressResource[(int)resourceType] -= 1f;
            amountResource[(int)resourceType]++;
        }

        // ตอนนี้ไม่ต้องแยก if-else สำหรับ InRoom แล้ว เพราะ IsMine ครอบคลุมหมด
        OnResourceValueChanged?.Invoke(resourceType, percentageProgressResource[(int)resourceType], amountResource[(int)resourceType]);

        Debug.Log($"Resource Updated for {gameObject.name}: {resourceType}, Amount: {amountResource[(int)resourceType]}");
    }
    #endregion
}
