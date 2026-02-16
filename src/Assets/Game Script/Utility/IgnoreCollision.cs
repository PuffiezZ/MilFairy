using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField] private Collider thisCollider;
    [SerializeField] private Collider[] collidersToIgnore;

    private void Start()
    {
        foreach(var collider in collidersToIgnore)
        {
            Physics.IgnoreCollision(thisCollider, collider);
        }
    }
}
