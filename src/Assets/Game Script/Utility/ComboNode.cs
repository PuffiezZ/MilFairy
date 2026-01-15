using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Combo", menuName = "Combo/New Combo Node")]
public class ComboNode : ScriptableObject
{
    public AnimatorOverrideController AnimationOverrideCtrl;
    public float normalizedTimeToNext = 0.95f;
    public float attackSpeedCombo = 1f;
    public float delayToNextCombo = 0f;

    [Button("Auto Fetch Length")]
    public float GetComboClipLength()
    {
        if (AnimationOverrideCtrl != null)  
        {
            var clip = AnimationOverrideCtrl["Attack"];
            return clip.length;
        }
        else
        {
            return 0f;
        }
    }
}
