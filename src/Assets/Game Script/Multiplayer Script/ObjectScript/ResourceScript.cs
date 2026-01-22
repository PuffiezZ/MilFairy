using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ResourceScript : GameObjectPickUp
{
    [SerializeField] private ResourceData resourceData;
    public ResourceData GetResourceData {  get { return resourceData; } }
    public override void OnBeginIntereact(GameObject player, bool setActive = false)
    {
        Player getPlayerComponent = player.GetComponent<Player>();

        if (getPlayerComponent == null) return;
        
        UtilityDev.ResourceType resourceType = resourceData.resourceType;
        float persentOfresource = resourceData.getPercent;
        getPlayerComponent.OnPickResourceInvoke(resourceData.resourceType, persentOfresource);

        base.OnBeginIntereact(player, setActive);
    }

    public override void OnGameObjectSpawn()
    {
        base.OnGameObjectSpawn();

    }
}
