using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonConfiguration : MonoBehaviour
{
    [SerializeField] private bool scaleWidthByParentLayout = false;

    [Header("Button References")]
    [SerializeField] ContentSizeFitter csf;
    [SerializeField] private LayoutGroup layoutGroup;

    [Header("Audio Overrides")]
    [Tooltip("หากต้องการให้ปุ่มนี้มีเสียงเฉพาะตัว ให้ลากเสียงมาใส่ที่นี่ (ถ้าไม่ใส่จะใช้เสียง Default)")]
    [SerializeField] private AudioClip customClickSFX;

    private void Awake()
    {
        // ดึง Button component จาก GameObject นี้
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            // ผูก Event Listener เข้ากับฟังก์ชัน PlayClickSound
            btn.onClick.AddListener(PlayClickSound);
        }
    }

    private void OnValidate()
    {
        if(csf != null)
        {
            csf.enabled = !scaleWidthByParentLayout;
            csf.horizontalFit = scaleWidthByParentLayout ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;

            // ������ Layout Rebuild �ѹ�����������繼��˹�� Scene
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
        
    }
    public void PlayClickSound()
    {
        if (Main.Instance != null)
        {
            // ส่งเสียง customClickSFX ไปเป็น Parameter (ถ้าเป็น null ระบบจะจัดการให้เอง)
            Main.Instance.PlayButtonSound(customClickSFX);
        }
    }
}
