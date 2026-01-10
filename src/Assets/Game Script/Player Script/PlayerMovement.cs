using Sausagecat.PlayerControlSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sausagecat.PlayerControlSystem.PlayerState;

namespace Sausagecat.PlayerControlSystem
{
    [DefaultExecutionOrder(-1)]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private PlayerState _playerState;

        [Header("Movement Setting")]
        public float runAcceleration = 0.25f;
        public float maxSpeed = 4f;
        public float drag = 0.1f;
        public float turnSmoothTime = 0.05f;
        public float jumpingForce = 20f;
        public float gravity = 9.81f;
        private float currentSpeed = 0f;

        [Header("Camera Sensitivity")]
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private float cameraSmoothTime = 0.12f;
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        private float originTurnsmooth;
        private float _turnSmoothVelocity;
        private float _targetRotationX; // เก็บค่าก้ม-เงย
        private float _targetRotationY; // เก็บค่าซ้าย-ขวา
        private PlayerLocomotion playerLocomotion;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerTargetRotation = Vector2.zero;
        private float _lastTargetAngle;
        private float verticalVelocity = 0f;

        public bool lockRotating { get; private set; } = false;
        public bool IsMovementInput { get; private set; }
        public bool IsMoving { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsGround {  get; private set; }
        public Vector3 CurrentVelocity => characterController.velocity;
        private void Awake()
        {
            playerLocomotion = GetComponent<PlayerLocomotion>();
            originTurnsmooth = turnSmoothTime;
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                lockRotating = !lockRotating;
            }

            if (playerLocomotion.OnSprinting && CurrentVelocity.sqrMagnitude > 0.01f)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, 10f * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed/2, 10f * Time.deltaTime);
            }

            HandleMovement();
            HandleVerticleVelocity();
            UpdateHandleMovementState();
        }
        private void LateUpdate()
        {
            // 1. รับ Input และสะสมค่าการหมุน
            _targetRotationY += playerLocomotion.LookInput.x * lookSenseH;
            _targetRotationX -= playerLocomotion.LookInput.y * lookSenseV;

            // 2. จำกัดมุมก้ม-เงย (Clamp)
            _targetRotationX = Mathf.Clamp(_targetRotationX, -lookLimitV, lookLimitV);

            // 3. สั่งหมุน CameraPivot (หมุนรอบตัวละคร)
            // แกน X คือก้มเงย, แกน Y คือหมุนรอบตัว
            _cameraPivot.rotation = Quaternion.Euler(_targetRotationX, _targetRotationY, 0f);
        }

        private void HandleVerticleVelocity()
        {
            bool isGround = characterController.isGrounded;

            if (isGround && verticalVelocity < 0f)
            {
                // ค่าติดลบเล็กน้อยช่วยให้ CharacterController ตรวจสอบ isGrounded ได้เสถียรขึ้น
                verticalVelocity = -2f;
            }

            // คำนวณแรงกระโดด (ควรทำเมื่ออยู่บนพื้นเท่านั้น)
            if (isGround && playerLocomotion.OnJumping)
            {
                // สูตรแรงกระโดด: v = sqrt(h * 2 * g)
                verticalVelocity = Mathf.Sqrt(jumpingForce * 2f * gravity);
                playerLocomotion.OnJumping = false;
            }

            // แรงโน้มถ่วงทำงานตลอดเวลา
            verticalVelocity -= gravity * Time.deltaTime;
        }

        private void UpdateHandleMovementState()
        {
            IsMovementInput = playerLocomotion.MovementInput != Vector2.zero;
            IsMoving = characterController.velocity.sqrMagnitude > 0.01f;
            IsSprinting = playerLocomotion.OnSprinting && IsMoving;
            IsGround = characterController.isGrounded;

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

            if(!IsGround && characterController.velocity.y > 0f)
            {
                _playerState.SetMovementPlayerState(PlayerMovementState.Jumping);
            }
            else if (!IsGround && characterController.velocity.y <= 0f)
            {
                _playerState.SetMovementPlayerState(PlayerMovementState.Falling);
            }
        }

        private void HandleCharacterRotation(Vector3 movementDirection)
        {
            if (lockRotating)
            {
                turnSmoothTime = 0f;
                float targetAngle = _playerCamera.transform.eulerAngles.y;

                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
            }
            else
            {
                turnSmoothTime = originTurnsmooth;
                if (movementDirection.sqrMagnitude > 0.01f)
                {
                    // Atan2 จะคืนค่ามุมเป็นองศา (0-360) โดยคำนวณจากทั้ง x และ z 
                    // หาก x=1, z=1 (เดินเฉียงขวาบน) targetAngle จะได้ 45 องศาโดยอัตโนมัติ
                    float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;

                    // ค่อยๆ หมุนไปหาทิศนั้นด้วยความนุ่มนวล
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);

                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }
            }
        }

        private void HandleMovement()
        {
            // 1. ดึงทิศทาง Forward และ Right ของกล้องมา
            Vector3 forward = _playerCamera.transform.forward;
            Vector3 right = _playerCamera.transform.right;

            // 2. ทำให้เป็นแนวระนาบ (แกน Y เป็น 0) เพื่อไม่ให้ตัวละครก้มเงยตอนเดิน
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // 3. คำนวณทิศทางเคลื่อนที่ (นี่คือจุดที่ทำให้เกิดมุมเฉียง)
            Vector3 movementDirection = (forward * playerLocomotion.MovementInput.y) + (right * playerLocomotion.MovementInput.x);

            // 4. ส่งไปให้ฟังก์ชันหมุนตัว
            HandleCharacterRotation(movementDirection);

            Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
            Vector3 newVelocity = characterController.velocity + movementDelta;
            newVelocity.y = 0; // ล้างค่า Y เก่าออกก่อนคำนวณแรงเดิน
            newVelocity += movementDelta;

            //Add Drag หน่วงตอนขยับ
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = CheckMove(newVelocity, currentDrag);
            newVelocity = Vector3.ClampMagnitude(newVelocity, currentSpeed);

            newVelocity.y = verticalVelocity;
            //Unity อัพเดต Move 1 frame เรียก 1 characterController.Move
            characterController.Move(newVelocity * Time.deltaTime);
        }

        private Vector3 CheckMove(Vector3 getNewVelocity,Vector3 getCurrentDrag)
        {
            if(getNewVelocity.magnitude > drag * Time.deltaTime)
            {
                return getNewVelocity - getCurrentDrag;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}

