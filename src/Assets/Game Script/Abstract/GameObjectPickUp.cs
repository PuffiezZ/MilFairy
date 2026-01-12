using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GameObjectPickUp : MonoBehaviour,IInteractable
{
    [SerializeField] protected GameObject intereactWorldUI;
    private void Start()
    {
        if (intereactWorldUI == null)
        {
            Debug.LogError($"[GameObjectPickUp] {gameObject.name} is missing UI Reference!");
        }
        OnGameObjectSpawn();
    }
    [PunRPC]
    public virtual void OnGameObjectSpawn()
    {
        HideWorldInterectUI();
    }

    public void HideWorldInterectUI()
    {
        if (intereactWorldUI == null) return;
        intereactWorldUI.SetActive(false);
    }

    public void ShowWorldInterectUI()
    {
        if (intereactWorldUI == null) return;
        intereactWorldUI.SetActive(true);
    }

    public virtual void Interact(GameObject player)
    {
        if (PhotonNetwork.InRoom)
        {
            // ส่ง RPC ไปบอกทุกคนว่าไอเทมนี้ถูกเก็บแล้ว
            GetComponent<PhotonView>().RPC(nameof(RPC_OnPickedUp), RpcTarget.MasterClient, player.GetComponent<PhotonView>().ViewID,player);
        }
        else
        {
            OnInterected(player);
        }
    }

    [PunRPC]
    public void RPC_OnPickedUp(int playerViewID,GameObject player)
    {
        // MasterClient ตรวจสอบความถูกต้องและสั่งลบ
        if (PhotonNetwork.IsMasterClient)
        {
            OnInterected(player);
        }
    }

    public virtual void OnInterected(GameObject player)
    {
        HideWorldInterectUI();
        if (PhotonNetwork.InRoom)
        {
            // ส่ง RPC บอกทุกคนให้ซ่อนวัตถุนี้ (ใช้ Buffered เพื่อให้คนที่มาทีหลังไม่เห็นของที่เก็บไปแล้ว)
            GetComponent<PhotonView>().RPC(nameof(RPC_HideObject), RpcTarget.AllBuffered);
        }
        else
        {
            // ใช้ Destroy ปกติสำหรับโหมด Offline
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void RPC_HideObject()
    {
        LocalHide();
    }

    private void LocalHide()
    {
        HideWorldInterectUI();
        gameObject.SetActive(false); // ซ่อนวัตถุแทนการ Destroy
    }
}
