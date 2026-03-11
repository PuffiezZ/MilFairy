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
        PhotonView playerphotonView = player.GetComponent<PhotonView>();
        if (PhotonNetwork.InRoom)
        {
            if(playerphotonView== null)
            {
                Debug.LogWarning("Equipment Script not found Photonview on player parameter");
                return;
            }
            
            int playerID = playerphotonView.ViewID;
            this.photonView.RPC(nameof(RPC_EquipToPlayer), RpcTarget.All, playerID, setActive);
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
