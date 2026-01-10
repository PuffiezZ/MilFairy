using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    public void OnPickResourceInvoke(UtilityDev.ResourceType resourceType,float percentage);
}
