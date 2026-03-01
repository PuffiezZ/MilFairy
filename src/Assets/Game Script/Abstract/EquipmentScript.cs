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
        PhotonView photonView = player.GetComponent<PhotonView>();
        if (PhotonNetwork.InRoom)
        {
            int playerID = photonView.ViewID;
            photonView.RPC("RPC_EquipToPlayer", RpcTarget.AllBuffered, playerID, setActive);
        }
        else
        {
            PlayerEquipment playerEquipment = player.GetComponent<PlayerEquipment>();
            playerEquipment.OnPlayerEquipped(this,photonView);
            base.OnBeginIntereact(player, setActive);
        }
    }

    [PunRPC]
    public void RPC_EquipToPlayer(int playerViewID,bool setActive)
    {

        PhotonView targetPv = PhotonView.Find(playerViewID);

        if (targetPv != null)
        {
            GameObject playerObj = targetPv.gameObject;

            PlayerEquipment playerEquipment = playerObj.GetComponent<PlayerEquipment>();
            playerEquipment.OnPlayerEquipped(this, targetPv);

            base.OnBeginIntereact(playerObj, setActive);
        }
    }

}
