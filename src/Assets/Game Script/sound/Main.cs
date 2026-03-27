using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // นี่คือการทำ Instance (Singleton) เพื่อให้สคริปต์อื่นเรียกใช้ main.Instance ได้จากทุกที่
    public static Main Instance;

    [Header("Audio Sources")]
    [Tooltip("AudioSource สำหรับเพลงประกอบ (BGM) - ควรติ๊ก Loop ใน Component นี้ด้วย")]
    [SerializeField] private AudioSource bgmSource;
    
    [Tooltip("AudioSource สำหรับเสียงเอฟเฟกต์ (SFX) เช่น เสียงกดปุ่ม")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Default UI Sounds")]
    [Tooltip("ลากไฟล์เสียงคลิกปุ่มมาใส่ที่นี่")]
    [SerializeField] private AudioClip buttonClickSFX;

    [Header("Auto Play")]
    [Tooltip("ลากเพลงที่ต้องการให้เล่นทันทีเมื่อเข้าเกม")]
    [SerializeField] private AudioClip startBGM;

    private void Awake()
    {
        // Singleton: ทำให้ Sound Manager มีตัวเดียวตลอดทั้งเกม แม้จะเปลี่ยนฉาก
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (startBGM != null) PlayBGM(startBGM);
    }

    // ฟังก์ชันเล่นเสียงเอฟเฟกต์สั้นๆ (เรียกใช้จาก Code อื่นได้ เช่น main.Instance.PlaySFX(clip))
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null) sfxSource.PlayOneShot(clip);
    }

    // ฟังก์ชันสำหรับปุ่ม (UI Button): ลากไปใส่ในช่อง OnClick ของปุ่มใน Unity ได้เลย
    // เพิ่ม parameter ให้สามารถรับ AudioClip อื่นมาเล่นได้
    public void PlayButtonSound(AudioClip customClip = null)
    {
        AudioClip clipToPlay = (customClip != null) ? customClip : buttonClickSFX;
        PlaySFX(clipToPlay);
    }

    // ฟังก์ชันสำหรับเปลี่ยนเพลงประกอบ (BGM)
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null || bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
}
