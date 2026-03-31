using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapSetup : MonoBehaviour
{
    [SerializeField] private Camera minimapCam;
    public void OnMinimapSetup(Transform playerPOS)
    {
        if (minimapCam == null) return;

        int targetLayer = LayerMask.NameToLayer("Marker");
        
        // ค้นหา Transform ทั้งหมดในฉาก (ใส่ true เพื่อให้ค้นหาตัวที่ถูก SetActive(false) ซ่อนไว้ด้วย)
        Transform[] allTransforms = FindObjectsOfType<Transform>(true);
        List<GameObject> markerList = new List<GameObject>();

        foreach (Transform t in allTransforms)
        {
            if (t.gameObject.layer == targetLayer)
            {
                markerList.Add(t.gameObject);
                
                Canvas canvas = t.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.worldCamera = minimapCam; // ตั้งค่า Event Camera ให้กับ Canvas
                }
                
                Marker markerScript = t.GetComponent<Marker>();
                if (markerScript == null) 
                {
                    markerScript = t.gameObject.AddComponent<Marker>();
                }
                
                // ให้ Marker รู้จักพ่อ (ตัวละคร) ของมันเพื่อเอาค่าการหมุนมาใช้
                if (t.parent != null)
                {
                    markerScript.SetTarget(playerPOS);
                }
            }
        }
    }
}
