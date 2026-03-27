using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TransitionCamera : MonoBehaviour
{
    public Camera cam;
    public Vector3 startPOS;
    public Cutscene cutscene;
    
    void Awake()
    {
        cam.transform.position = startPOS;
    }

    public void ExecuteCutsceneFadeIn() 
    {
        // ใน Multiplayer: เช็คว่าเราคือเจ้าของตัวละครไหม 
        // เพราะเราต้องการให้ UI Fade เฉพาะหน้าจอของคนเล่นคนนั้น
        if (GetComponent<PhotonView>() != null && !GetComponent<PhotonView>().IsMine) return;

        cutscene?.MakeTransition(1.5f, 0f);
    }

    // เปลี่ยนจาก OnFinish เป็นชื่อนี้
    public void ExecuteCutsceneFadeOut()
    {
        if (GetComponent<PhotonView>() != null && !GetComponent<PhotonView>().IsMine) return;

        cutscene?.MakeTransition(2f, 1f, true);
    }
}
