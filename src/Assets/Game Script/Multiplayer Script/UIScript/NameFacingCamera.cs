using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NameFacingCamera : MonoBehaviour
{

    private void Update()
    {
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("FacingCamera"))
        {
            go.transform.LookAt(transform.position);
        }
    }
}
