using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GameObjectPickUp : MonoBehaviourPun,IInteractable
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

    public virtual void OnBeginIntereact(GameObject player)
    {
        if (PhotonNetwork.InRoom)
        {
            int getViewID = player.GetComponent<PhotonView>().ViewID;
            photonView.RPC(nameof(RPC_OnPickedUp), RpcTarget.AllBuffered, getViewID);
        }
        else
        {
            OnInterected(player);
        }
    }

    [PunRPC]
    public void RPC_OnPickedUp(int playerViewID)
    {
        PhotonView targetPv = PhotonView.Find(playerViewID);
        if (targetPv != null)
        {
            GameObject player = targetPv.gameObject;

            // 1. ซ่อนวัตถุในทุกเครื่องทันที
            LocalHide();

            // 2. สั่ง OnInterected (ซึ่งจะไปเรียก OnPlayerEquipped ในคลาสลูก)
            OnInterected(player);
        }
    }

    public virtual void OnInterected(GameObject player)
    {
        if (!PhotonNetwork.InRoom)
        {
            LocalHide();
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
