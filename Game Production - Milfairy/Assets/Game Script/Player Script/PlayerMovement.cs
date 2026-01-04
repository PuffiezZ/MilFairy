using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sausagecat.PlayerControlSystem
{
    [DefaultExecutionOrder(-1)]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Camera _playerCamera;

        [Header("Movement Setting")]
        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float drag = 0.1f;

        [Header("Camera Sensitivity")]
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private float cameraSmoothTime = 0.12f;
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;
        public float turnSmoothTime = 0.1f;

        private float originTurnsmooth;
        private float _turnSmoothVelocity;
        private float _targetRotationX; // เก็บค่าก้ม-เงย
        private float _targetRotationY; // เก็บค่าซ้าย-ขวา
        private PlayerLocomotion playerLocomotion;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerTargetRotation = Vector2.zero;
        private bool lockRotating = false;

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
            HandleMovement();
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

        public void HandleCharacterRotation(Vector3 movementDirection)
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
                    float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }
            }
        }

        private void HandleMovement()
        {
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * playerLocomotion.MovementInput.x + cameraForwardXZ * playerLocomotion.MovementInput.y;

            HandleCharacterRotation(movementDirection);

            Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
            Vector3 newVelocity = characterController.velocity + movementDelta;

            //Add Drag หน่วงตอนขยับ
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = CheckMove(newVelocity, currentDrag);
            newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

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

