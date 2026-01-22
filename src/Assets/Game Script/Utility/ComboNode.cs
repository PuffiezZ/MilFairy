using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum HitboxType { Capsule, AOE_Sphere }
[CreateAssetMenu(fileName = "New Combo", menuName = "Combo/New Combo Node")]
public class ComboNode : ScriptableObject
{
    public AnimatorOverrideController AnimationOverrideCtrl;
    public HitboxType hitboxType; // เลือกว่าจะเป็น Capsule หรือ Sphere
    public float NormalizedTimeToNext = 0.95f;
    public float AttackSpeedCombo = 1f;
    public float DelayToNextCombo = 0f;
    public bool EnableAnimationTriggerHitbox = false;
    public PlayerCombat PlayerCombat { get; set; }

    [ShowIf("EnableAnimationTriggerHitbox")]
    [MinMax(0f,1f)] public float TimeHitboxTrigger = 0f;

}
