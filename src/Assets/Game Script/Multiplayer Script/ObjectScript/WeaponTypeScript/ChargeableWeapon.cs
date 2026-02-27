using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeableWeapon : MonoBehaviour
{
    [Header("Charge Settings")]
    public float maxChargeTime = 2f;
    public float minPower = 10f;
    public float maxPower = 50f;

    private float currentChargeTime;
    private bool isCharging;

    public bool IsCharging => isCharging;

    public void StartCharging()
    {
        isCharging = true;
        currentChargeTime = 0f;
    }

    public void UpdateCharge()
    {
        if (!isCharging) return;
        currentChargeTime += Time.deltaTime;
        currentChargeTime = Mathf.Min(currentChargeTime, maxChargeTime);
    }

    public float ReleaseCharge()
    {
        isCharging = false;
        float chargePercent = currentChargeTime / maxChargeTime;
        float finalPower = Mathf.Lerp(minPower, maxPower, chargePercent);
        currentChargeTime = 0f;
        return finalPower;
    }

    public float GetChargeProgress()
    {
        return Mathf.Clamp01(currentChargeTime / maxChargeTime);
    }
    
    
}
