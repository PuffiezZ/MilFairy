using Sausagecat.PlayerControlSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using static UtilityDev.ResourceType;

public class UIManager : MonoBehaviour
{
    [BoxGroup("Resource UI")]
    [SerializeField] private RectTransform[] rectResource;

    [BoxGroup("Weapon Slot")]
    [SerializeField] private WeaponSlotUI[] weaponsSlot;

    [SerializeField] private Slider healthSlider;

    private void OnEnable()
    {
        // เริ่มติดตาม (Subscribe) เมื่อสคริปต์ทำงาน
        Player.OnPlayerHealthChanged += UpdateHealthBar;
        Player.OnResourceValueChanged += UpdateResource;

        PlayerEquipment.OnSetNewWeapon += UpdateWeaponSlot;
    }

    private void OnDisable()
    {
        // ยกเลิกการติดตาม (Unsubscribe) เมื่อปิดสคริปต์ เพื่อป้องกัน Memory Leak
        Player.OnPlayerHealthChanged -= UpdateHealthBar;
        Player.OnResourceValueChanged -= UpdateResource;

        PlayerEquipment.OnSetNewWeapon -= UpdateWeaponSlot;
    }

    private void UpdateHealthBar(float current, float max)
    {
        if(healthSlider != null)
        {
            healthSlider.value = current / max;
        }
        Debug.Log("UI Updated via Event!");
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
}
