using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CameraRegister[] register;
    public static Dictionary<string, Camera> camerasDict = new Dictionary<string, Camera>();
    private Camera onCurrentCamera;
    void Awake()
    {
        foreach (var each in register)
        {
            if(!camerasDict.ContainsKey(each.nameCamera))
            {
                camerasDict.Add(each.nameCamera, each.camera);
            }

        }
        
        ChangeCameraByName("Main Camera");
    } 
    public static void ChangeCameraByName(string nameCamera)
    {
        if(!camerasDict.ContainsKey(nameCamera)) return;
        
        foreach (var each in camerasDict)
        {
            each.Value.gameObject.SetActive(nameCamera == each.Key);
        }
    }
}

[System.Serializable]
public struct CameraRegister
{
    public string nameCamera;
    public Camera camera;
}
