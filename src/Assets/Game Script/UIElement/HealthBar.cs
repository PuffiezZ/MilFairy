using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider sliderBar;

    public void UpdateHealthBar(float maxValue, float currentValue)
    {
        sliderBar.value = currentValue / maxValue;
    }   
}
