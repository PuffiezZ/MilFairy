using Photon.Pun;
using Sausagecat.PlayerControlSystem;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerAnimation : MonoBehaviourPun
{
    [SerializeField] private Animator animator;

    [SerializeField] private float simepleBlendSpeed = 20f;
    [SerializeField] private float locomotionBlendSpeed = 0.25f;

    private PlayerLocomotion playerLocomotion;
    private PlayerMovement playerMovement;
    private PlayerState playerState;

    private static int magnitudeHash = Animator.StringToHash("Magnitude");
    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");
    private static int isGroundedHash = Animator.StringToHash("isGrounded");
    private static int isJumpHash = Animator.StringToHash("isJump");
    private static int isFalling = Animator.StringToHash("isFalling");
    private static int verticalMagnitudeHash = Animator.StringToHash("VerticalMagnitude");
    private static int isUsingOneHandedWeapon = Animator.StringToHash("isUsingOneHanded");
    private static int isDrawOneHandedHash = Animator.StringToHash("isDrawOneHanded");
    private static int isSheathedOneHandedHash = Animator.StringToHash("isSheathedOneHanded");
    private static int animationFloatStateHash = Animator.StringToHash("animationFloatState");
    private static int lightAttackTriggerHash = Animator.StringToHash("lightAttackTrigger");
    private static int heavyAttackTriggerHash = Animator.StringToHash("heavyAttackTrigger");

    Vector3 locomotionMagnitude = Vector3.zero;
    Vector3 currentBlendInput = Vector3.zero;

    private float currentAnimationfloat = 0f;
    public float TargetAnimationfloat { get; set; }
    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerMovement = GetComponent<PlayerMovement>();
        playerState = GetComponent<PlayerState>();
    }
    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        if (PhotonNetwork.InRoom && !photonView.IsMine) return;

        Vector3 velocity = playerMovement.CurrentVelocity;
        velocity.y = 0;
        locomotionMagnitude = Vector3.Lerp(locomotionMagnitude, velocity, simepleBlendSpeed * Time.deltaTime);
        float speedRatio = locomotionMagnitude.magnitude / playerMovement.maxSpeed;
        animator.SetFloat(magnitudeHash, Mathf.Clamp01(speedRatio));

        Vector3 targetInput = playerLocomotion.MovementInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, targetInput, locomotionBlendSpeed * Time.deltaTime);
        animator.SetFloat(inputXHash,currentBlendInput.x);
        animator.SetFloat(inputYHash, currentBlendInput.y);

        animator.SetBool(isJumpHash, playerState.CurrentPlayerMovementState == PlayerState.PlayerMovementState.Jumping);
        animator.SetBool(isFalling, playerState.CurrentPlayerMovementState == PlayerState.PlayerMovementState.Falling);
        animator.SetBool(isGroundedHash, playerMovement.IsGround);

        float verticalVelocity = playerMovement.CurrentVelocity.y;
        float verticalRatio = verticalVelocity / playerMovement.maxSpeed;
        float finalVerticalValue = Mathf.Clamp(verticalRatio, 0f, 1f);
        animator.SetFloat(verticalMagnitudeHash, finalVerticalValue);

        currentAnimationfloat = Mathf.Lerp(currentAnimationfloat, TargetAnimationfloat, 10f * Time.deltaTime);
        animator.SetFloat(animationFloatStateHash, currentAnimationfloat);
    }

    public void SetOnUsingWeaponAnimation(bool isUsingBoolean)
    {
        animator.SetBool(isUsingOneHandedWeapon, isUsingBoolean);
    }

    public void OnTriggerDrawOrSheathed(UtilityDev.DrawOrSheath drawOrSheath)
    {
        if(drawOrSheath == UtilityDev.DrawOrSheath.Draw)
        {
            animator.SetTrigger(isDrawOneHandedHash);
            animator.ResetTrigger(isSheathedOneHandedHash);
            Debug.Log("Draw Animation Triggered");
        }
        else
        {
            animator.SetTrigger(isSheathedOneHandedHash);
            animator.ResetTrigger(isDrawOneHandedHash);
            Debug.Log("Sheathed Animation Triggered");
        }
    }

    public void PerformAttackAnimation(AnimatorOverrideController aoc)
    {
        // สมมติว่าใน Animator ของคุณ ท่าตีเบาใช้คลิปชื่อ "LightAttack_Base"
        animator.runtimeAnimatorController = aoc;
        animator.SetTrigger(lightAttackTriggerHash);
    }

    public float GetNormalizedTime(string nameTag, int layerIndex = 2)
    {

        if (animator.IsInTransition(layerIndex))
        {
            AnimatorStateInfo nextState = animator.GetNextAnimatorStateInfo(layerIndex);
            if (nextState.IsTag(nameTag)) return 0f;
            return -1f;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        if (stateInfo.IsTag(nameTag))
        {
            // แม้แอนิเมชันจะเร็วขึ้น แต่ normalizedTime จะยังคงวิ่งจาก 0 ไป 1 เสมอ
            return Mathf.Clamp01(stateInfo.normalizedTime);
        }

        return -1f;
    }

    public void SetAttackSpeed(float speed)
    {
        // ส่งค่าความเร็วที่ต้องการเข้าไปใน Parameter
        animator.SetFloat("AnimationAttackSpeed", speed);
    }
}
