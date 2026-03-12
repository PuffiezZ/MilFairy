using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ลากไฟล์เพลง (AudioClip) ที่ต้องการให้เล่นในฉากนี้มาใส่")]
    [SerializeField] private AudioClip sceneBGM;

    private void Start()
    {
        // เมื่อฉากเริ่มทำงาน ให้สั่ง main.Instance (ที่เป็นของฉากแรก) ให้เปลี่ยนเพลง
        if (Main.Instance != null && sceneBGM != null)
        {
            Main.Instance.PlayBGM(sceneBGM);
        }
    }
}
