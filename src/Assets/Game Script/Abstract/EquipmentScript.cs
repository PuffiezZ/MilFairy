using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentScript : GameObjectPickUp
{
    public override void OnInterected(GameObject player)
    {
        PlayerEquipment playerEquipment = player.GetComponent<PlayerEquipment>();
        playerEquipment.OnPlayerEquiped(this);
        base.OnInterected(player);
    }

}
