using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    [SerializeField] private Image weaponImage;

    public void SetWeaponSlotUI(WeaponData weaponData = null)
    {
        if(weaponData == null)
        {
            weaponImage.color = new Color32(255, 255, 255, 0);
            weaponImage.sprite = null;
        }
        else
        {
            weaponImage.color = new Color32(255, 255, 255, 255);
            weaponImage.sprite = weaponData.weaponSprite;   
        }        
    }
}
