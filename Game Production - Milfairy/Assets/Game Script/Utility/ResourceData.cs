using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Data", menuName = "Inventory/New Resource Data")]
public class ResourceData : ScriptableObject
{
    public UtilityDev.ResourceType resourceType;
    public float getPercent;
    public string NameResource => resourceType.ToString();
}
