using Sausagecat.PlayerControlSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float simepleBlendSpeed = 20f;
    [SerializeField] private float locomotionBlendSpeed = 0.25f;

    private PlayerLocomotion playerLocomotion;
    private PlayerMovement playerMovement;

    private static int magnitudeHash = Animator.StringToHash("Magnitude");
    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");

    Vector3 locomotionMagnitude = Vector3.zero;
    Vector3 currentBlendInput = Vector3.zero;
    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerMovement = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        Vector3 velocity = playerMovement.CurrentVelocity;
        velocity.y = 0;
        locomotionMagnitude = Vector3.Lerp(locomotionMagnitude, velocity, simepleBlendSpeed * Time.deltaTime);
        float speedRatio = locomotionMagnitude.magnitude / playerMovement.runSpeed;
        animator.SetFloat(magnitudeHash, Mathf.Clamp01(speedRatio));

        Vector3 targetInput = playerLocomotion.MovementInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, targetInput, locomotionBlendSpeed * Time.deltaTime);
    }
}
