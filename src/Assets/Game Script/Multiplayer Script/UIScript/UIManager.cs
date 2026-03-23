using System.Collections;
using Sausagecat.PlayerControlSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using static UtilityDev.ResourceType;
using System;

public class UIManager : MonoBehaviour
{
    [BoxGroup("Resource UI")]
    [SerializeField] private RectTransform[] rectResource;

    [BoxGroup("Weapon Slot")]
    [SerializeField] private WeaponSlotUI[] weaponsSlot;
    [BoxGroup("UI Bars")]
    [SerializeField] private Slider healthSlider;
    [BoxGroup("UI Bars")]
    [SerializeField] private Slider payloadHealthSlider;
    [BoxGroup("UI Bars")]
    [SerializeField] private Image handlePayloadHealhBar;
    [BoxGroup("UI Bars")]
    [SerializeField] private Sprite[] handlePayloadSprite;

    [BoxGroup("UI Parent")]
    [SerializeField] private RectTransform gameplayUI;
    [BoxGroup("UI Parent")]
    [SerializeField] private RectTransform winUI;
    [BoxGroup("UI Parent")]
    [SerializeField] private RectTransform loseUI;

    private Coroutine smoothHealthCoroutine;

    private void OnEnable()
    {
        // ������Դ��� (Subscribe) �����ʤ�Ի��ӧҹ
        Player.OnPlayerHealthChanged += UpdateHealthBar;
        Player.OnResourceValueChanged += UpdateResource;

        PlayerEquipment.OnSetNewWeapon += UpdateWeaponSlot;

        RoomManager.OnWinTriggered += WinUIHandler;
        RoomManager.OnLoseTriggered += LoseUIHandler;
    }

    private void OnDisable()
    {
        // ¡��ԡ��õԴ��� (Unsubscribe) ����ͻԴʤ�Ի�� ���ͻ�ͧ�ѹ Memory Leak
        Player.OnPlayerHealthChanged -= UpdateHealthBar;
        Player.OnResourceValueChanged -= UpdateResource;

        PlayerEquipment.OnSetNewWeapon -= UpdateWeaponSlot;
        RoomManager.OnWinTriggered -= WinUIHandler;
        RoomManager.OnLoseTriggered -= LoseUIHandler;
    }
    public void RegisterPayloadHealthBar(ToothCart cart)
    {
        cart.OnPayloadHealthChanged += UpdatePayloadHealthBar;
    }
    public void UpdateHandlePayload(float current, float max)
    {
        float halfAmountHP = max / 2f;
        
        if(current > halfAmountHP)
        {
            handlePayloadHealhBar.sprite = handlePayloadSprite[0];
        }
        else
        {
            handlePayloadHealhBar.sprite = handlePayloadSprite[1];
        }
    }
    private void UpdatePayloadHealthBar(float current, float max)
    {
        if(healthSlider != null)
        {
            float targetValue = current / max;
            UpdateHandlePayload(current, max);
            if (smoothHealthCoroutine != null)
            {
                StopCoroutine(smoothHealthCoroutine);
            }
            smoothHealthCoroutine = StartCoroutine(SmoothHealthBarPayloadRoutine(targetValue));
        }
        Debug.Log("UI Updated via Event!");
    }

    private void UpdateHealthBar(float current, float max)
    {
        if(healthSlider != null)
        {
            float targetValue = current / max;
            if (smoothHealthCoroutine != null)
            {
                StopCoroutine(smoothHealthCoroutine);
            }
            smoothHealthCoroutine = StartCoroutine(SmoothHealthBarRoutine(targetValue));
        }
        Debug.Log("UI Updated via Event!");
    }
    private IEnumerator SmoothHealthBarPayloadRoutine(float targetValue)
    {
        float elapsedTime = 0f;
        float duration = 0.25f; // สามารถปรับเปลี่ยนระยะเวลาความสมูทได้ที่นี่ (หน่วยเป็นวินาที)
        float startValue = payloadHealthSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            payloadHealthSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }
        
        payloadHealthSlider.value = targetValue;
    }

    private IEnumerator SmoothHealthBarRoutine(float targetValue)
    {
        float elapsedTime = 0f;
        float duration = 0.25f; // สามารถปรับเปลี่ยนระยะเวลาความสมูทได้ที่นี่ (หน่วยเป็นวินาที)
        float startValue = healthSlider.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }
        
        healthSlider.value = targetValue;
    }

    private void UpdateResource(UtilityDev.ResourceType resourceType,float percentage,int amountOfResource)
    {
        int index = (int)resourceType;
        RectTransform currentRect = rectResource[index];

        Debug.Log($"UI Update Resource At {index}");

        Slider circleSlider = currentRect.GetComponentInChildren<Slider>();
        TMP_Text textValue = currentRect.GetComponentInChildren<TMP_Text>();

        if(circleSlider == null || textValue == null) return;

        circleSlider.value = percentage / 1;
        textValue.text = amountOfResource.ToString();
    }

    public void UpdateWeaponSlot(int indexSlot,WeaponData weaponData)
    {
        weaponsSlot[indexSlot].SetWeaponSlotUI(weaponData);
    }

    public void WinUIHandler()
    {
        gameplayUI.gameObject.SetActive(false);
        winUI.gameObject.SetActive(true);
    }
    
    public void LoseUIHandler()
    {
        gameplayUI.gameObject.SetActive(false);
        loseUI.gameObject.SetActive(true);
    }
}
