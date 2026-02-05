using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "New Combo", menuName = "Combo/New Combo Node")]
public class ComboNode : ScriptableObject
{
    public AnimatorOverrideController AnimationOverrideCtrl;
    public float NormalizedTimeToNext = 0.95f;
    public float AttackSpeedCombo = 1f;
    public float DelayToNextCombo = 0f;
    public bool EnableAnimationTriggerHitbox = false;
    public PlayerCombat PlayerCombat { get; set; }

    [ShowIf("EnableAnimationTriggerHitbox")]
    [MinMax(0f,1f)] public float TimeHitboxTrigger = 0f;

}
