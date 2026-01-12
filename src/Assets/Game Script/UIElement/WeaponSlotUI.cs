using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private TextMeshProUGUI weaponNameText_TMP;

    public void SetWeaponSlotUI(WeaponData weaponData)
    {
        weaponImage.sprite = weaponData.weaponSprite;
    }
}
