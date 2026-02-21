using NaughtyAttributes;
using Sausagecat.PlayerControlSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sausagecat.PlayerControlSystem.PlayerState;

public class PlayerRagdollMovement : MonoBehaviour
{
    [BoxGroup("Script References")]
    [SerializeField] private PlayerLocomotion playerLocomotion;
    [BoxGroup("Script References")]
    [SerializeField] private Rigidbody rb;
    [BoxGroup("Script References")]
    [SerializeField] private ConfigurableJoint configurableJoint;
    [BoxGroup("Script References")]
    [SerializeField] private PlayerState _playerState;

    [BoxGroup("Camera Reference")]
    [SerializeField] private Transform cameraTransform;

    [Space(3)]
    [BoxGroup("Player Setting")]
    [SerializeField] private float maxSpeed = 5f;
    [BoxGroup("Player Setting")]
    [SerializeField] private float acceration = 5f;
    [BoxGroup("Player Setting")]
    [SerializeField] private float jumpForce = 5f;
    [BoxGroup("Player Setting")]
    [SerializeField] private float turnSpeed = 250f;
    [BoxGroup("Player Setting")]
    [SerializeField] private float deceleration = 10f;

    public float MaxSpeed => maxSpeed;
    public bool IsGrounded { get; private set; }
    public bool IsMovementInput { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsSprinting { get; private set; }

    public Vector3 RBvelocity => rb.velocity;

    private RaycastHit[] raycastHits = new RaycastHit[10];
    private SyncJoint[] syncJoints;

    private void Awake()
    {
        syncJoints = GetComponentsInChildren<SyncJoint>();
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleRigidbodyHandler();
        RigidbodyJumpHandler();
        UpdateHandleMovementState();
        for (int i = 0; i < syncJoints.Length; i++)
        {
            syncJoints[i].UpdateJointFromAnimation();
        }


    }
    private void UpdateHandleMovementState()
    {
        IsMovementInput = playerLocomotion.MovementInput != Vector2.zero;
        IsMoving = RBvelocity.sqrMagnitude > 0.01f;
        IsSprinting = playerLocomotion.OnSprinting && IsMoving;

        if (IsSprinting)
        {
            _playerState.SetMovementPlayerState(PlayerMovementState.Sprint);
        }
        else
        {
            if (IsMoving || IsMovementInput)
            {
                _playerState.SetMovementPlayerState(PlayerMovementState.Run);
            }
            else
            {
                _playerState.SetMovementPlayerState(PlayerMovementState.Idle);
            }
        }

        if (!IsGrounded && RBvelocity.y > 0f)
        {
            _playerState.SetMovementPlayerState(PlayerMovementState.Jumping);
        }
        else if (!IsGrounded && RBvelocity.y <= 0f)
        {
            _playerState.SetMovementPlayerState(PlayerMovementState.Falling);
        }
    }

    private void CheckGround()
    {
        // --- Ground Check (เหมือนเดิม) ---
        IsGrounded = false;
        int numberOfHit = Physics.SphereCastNonAlloc(rb.position, 0.1f, Vector3.down, raycastHits, 0.5f); // ปรับทิศเป็น Vector3.down ชัดเจนกว่า

        for (int i = 0; i < numberOfHit; i++)
        {
            if (raycastHits[i].transform.root == transform) continue;
            IsGrounded = true;
            break;
        }

    }
    private void RigidbodyJumpHandler()
    {

        if (IsGrounded && playerLocomotion.OnJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            playerLocomotion.OnJumping = false; // อย่าลืม Reset สถานะกระโดด
        }
    }
    private void HandleRigidbodyHandler()
    {
        // --- Movement Logic (แก้ใหม่) ---
        float inputMagnitude = playerLocomotion.MovementInput.magnitude;
        if (inputMagnitude > 0.01f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // ตัดแกน Y ทิ้งเพื่อให้เดินราบไปกับพื้น (ไม่เหินฟ้า/จมดิน)
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // สูตรมาตรฐาน: (หน้ากล้อง * แกนตั้ง) + (ขวากล้อง * แกนนอน)
            Vector3 moveDir = (camForward * playerLocomotion.MovementInput.y) +(camRight * playerLocomotion.MovementInput.x);

            // 3. การหมุนตัว (Rotation)
            if (moveDir != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(moveDir, Vector3.up);

                // TRICK: ConfigurableJoint.targetRotation มักต้องใช้ค่า "Inverse" เพื่อหมุนตัวละครไปหาทิศ World ที่ต้องการ
                // หากตัวละครหันหลังให้ทิศที่กด ให้เอา Quaternion.Inverse() ออก
                Quaternion targetJointRot = Quaternion.Inverse(lookRot);
                Quaternion fromRotation = configurableJoint.targetRotation;
                configurableJoint.targetRotation = Quaternion.RotateTowards(fromRotation, targetJointRot,Time.fixedDeltaTime * turnSpeed);
            }

            // 4. การใส่แรง (Force Application)
            Vector3 localVelocityForward = transform.forward * Vector3.Dot(transform.forward, rb.velocity);

            // ใช้ moveDir ในการเช็คทิศทางแรง เพื่อให้กดปุ่มแล้วเดินไปทางนั้นทันที (Responsive)
            if (localVelocityForward.magnitude < maxSpeed)
            {
                // ใช้ moveDir แทน transform.forward เพื่อให้แรงส่งไปตามกล้องเสมอ แม้ตัวจะยังหมุนไม่เสร็จ
                rb.AddForce(moveDir.normalized * inputMagnitude * acceration);
            }
        }
        else
        {
            if (IsGrounded)
            {
                // ดึงความเร็วปัจจุบันเฉพาะแกนราบ (Horizontal Velocity)
                Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

                // ถ้ายังมีความเร็วอยู่ ให้ใส่แรงต้านทิศทางเดิม
                if (flatVelocity.magnitude > 0.1f)
                {
                    // ใส่แรงตรงข้ามกับความเร็ว (-flatVelocity) คูณด้วยแรงเบรค (deceleration)
                    rb.AddForce(-flatVelocity * deceleration, ForceMode.Acceleration);
                }
            }
        }
    }
}
