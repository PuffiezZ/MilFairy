using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentScript : GameObjectPickUp
{
    public override void OnBeginIntereact(GameObject player,bool setActive = false)
    {
        setActive = true;
        if (PhotonNetwork.InRoom)
        {
            int playerID = player.GetComponent<PhotonView>().ViewID;
            photonView.RPC("RPC_EquipToPlayer", RpcTarget.AllBuffered, playerID, setActive);
        }
        else
        {
            PlayerEquipment playerEquipment = player.GetComponent<PlayerEquipment>();
            playerEquipment.OnPlayerEquipped(this);
            base.OnBeginIntereact(player, setActive);
        }
    }

    [PunRPC]
    public void RPC_EquipToPlayer(int playerViewID,bool setActive)
    {
        // 1. ค้นหา GameObject ของผู้เล่นจาก ViewID ที่ส่งมา
        PhotonView targetPv = PhotonView.Find(playerViewID);

        if (targetPv != null)
        {
            GameObject playerObj = targetPv.gameObject;
            // ทำ Logic การสวมใส่ต่อด้วย playerObj
            PlayerEquipment playerEquipment = playerObj.GetComponent<PlayerEquipment>();
            playerEquipment.OnPlayerEquipped(this);

            base.OnBeginIntereact(playerObj, setActive);
        }
    }

}
