using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NaughtyAttributes;
using Photon.Pun;

public class LoadingScnene : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update    void Start()
    [Header("UI")]
    [Tooltip("ลาก Slider UI ที่จะใช้แสดงความคืบหน้ามาใส่")]
    [SerializeField] private Slider loadingSlider;

    // ตัวแปร static สำหรับเก็บชื่อ Scene ที่จะโหลดต่อไป
    // ควรจะถูกตั้งค่าจาก Scene ก่อนหน้า (เช่น Lobby)
    [Header("Target Scene")]
    [Scene] public string sceneName;

    [Header("Settings")]
    [Tooltip("เวลาดีเลย์ (วินาที) หลังจากโหลดเสร็จก่อนที่จะย้ายฉาก")]
    [SerializeField] private float transitionDelay = 1.5f;

    public override void OnEnable()
    {
        // เริ่ม Coroutine สำหรับโหลด Scene แบบ Async
        StartCoroutine(LoadSceneAsync());
    }
    
    private IEnumerator LoadSceneAsync()
    {
        
        // ถ้าไม่ได้กำหนด Scene ที่จะโหลด ให้แสดง Error แล้วหยุดทำงาน
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("ไม่ได้กำหนด Scene ที่จะโหลด! กรุณาตั้งค่า LoadingScnene.sceneToLoad ก่อน");
            yield break;
        }

        if (PhotonNetwork.InRoom)
        {
            // สำหรับโหมด Multiplayer (PUN 2)
            // ให้ MasterClient เป็นคนสั่งโหลดฉาก (เครื่องอื่นจะโหลดตามอัตโนมัติถ้า AutomaticallySyncScene = true)
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(sceneName);
            }

            // อัปเดต Slider ตามความคืบหน้าของการโหลดฉากผ่านเครือข่าย Photon
            while (PhotonNetwork.LevelLoadingProgress < 0.99f)
            {
                if (loadingSlider != null)
                {
                    loadingSlider.value = PhotonNetwork.LevelLoadingProgress;
                }
                yield return null;
            }
            
            if (loadingSlider != null) loadingSlider.value = 1f;
            
            // หมายเหตุ: สำหรับโหมด Multiplayer ระบบ PUN2 จะบังคับเปลี่ยนฉากทันทีเมื่อโหลดเสร็จ (ถ้าเปิด AutomaticallySyncScene ไว้)
            // คำสั่ง delay นี้จะแสดงผลเล็กน้อยก่อนที่ฉากเก่าจะถูกทำลาย
            yield return new WaitForSeconds(transitionDelay);
        }
        else
        {
            // สำหรับโหมด Offline (Singleplayer)
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            
            // ป้องกันไม่ให้เปลี่ยนฉากทันทีที่โหลดเสร็จ
            operation.allowSceneActivation = false;

            // เมื่อ allowSceneActivation = false ค่า progress จะโหลดไปหยุดสูงสุดแค่ 0.9
            while (operation.progress < 0.9f)
            {
                float progressValue = Mathf.Clamp01(operation.progress / 0.9f);

                if (loadingSlider != null)
                {
                    loadingSlider.value = progressValue;
                }
                yield return null;
            }
            
            if (loadingSlider != null) loadingSlider.value = 1f;

            // รอเวลา Delay ก่อนย้ายฉากตามที่กำหนด
            yield return new WaitForSeconds(transitionDelay);

            // อนุญาตให้ย้ายไปฉากใหม่ได้
            operation.allowSceneActivation = true;
        }
    }
}