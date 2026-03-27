using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionCamera : MonoBehaviour
{
    public Camera cam;
    public Vector3 startPOS;
    public Cutscene cutscene;
    
    void Awake()
    {
        cam.transform.position = startPOS;
    }
    public void OnStart()
    {
        cutscene?.MakeTransition(1f,0f);
    }
    
    public void OnFinish()
    {
        cutscene?.MakeTransition(2f,1f,true);
    }
}
