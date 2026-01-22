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

    public virtual void OnBeginIntereact(GameObject player,bool setActive = false)
    {
        if (PhotonNetwork.InRoom)
        {
            int getViewID = player.GetComponent<PhotonView>().ViewID;
            photonView.RPC(nameof(RPC_OnPickedUp), RpcTarget.AllBuffered, getViewID, setActive);
        }
        else
        {
            OnInterected(player,setActive);
        }
    }

    [PunRPC]
    public void RPC_OnPickedUp(int playerViewID, bool getBoolean)
    {
        PhotonView targetPv = PhotonView.Find(playerViewID);
        if (targetPv != null)
        {
            GameObject player = targetPv.gameObject;

            // 1. ซ่อนวัตถุในทุกเครื่องทันที
            LocalSetGameObjectActive(getBoolean);

            // 2. สั่ง OnInterected (ซึ่งจะไปเรียก OnPlayerEquipped ในคลาสลูก)
            OnInterected(player, getBoolean);
        }
    }

    public virtual void OnInterected(GameObject player, bool getBoolean)
    {
        if (!PhotonNetwork.InRoom)
        {
            LocalSetGameObjectActive(getBoolean);
        }
    }

    private void LocalSetGameObjectActive(bool getBoolean)
    {
        HideWorldInterectUI();
        gameObject.SetActive(getBoolean);
    }
}
