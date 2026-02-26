using UnityEngine;
using System.Collections.Generic;

public class CraftingZone : MonoBehaviour
{
    // ลิสต์เก็บ HoldableObject ที่อยู่ในพื้นที่
    public List<HoldableObject> itemsInRange = new List<HoldableObject>();
    
    private void OnTriggerEnter(Collider other) 
    {
        HoldableObject item = other.gameObject.GetComponent<HoldableObject>();
        if (item != null && !itemsInRange.Contains(item))
        {
            itemsInRange.Add(item);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        HoldableObject item = other.gameObject.GetComponent<HoldableObject>();
        if (item != null && itemsInRange.Contains(item))
        {
            itemsInRange.Remove(item);
        }
    }

    public void CleanUpItems()
    {
        // ลบไอเทมที่ถูกทำลายไปแล้วออกจากลิสต์
        itemsInRange.RemoveAll(item => item == null);
    }
}
