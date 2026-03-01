using Sausagecat.PlayerControlSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        //[SerializeField] private float cameraSmoothTime = 0.12f;
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        private float originTurnsmooth;
        private float _turnSmoothVelocity;
        private float _targetRotationX; // �纤�ҡ��-��
        private float _targetRotationY; // �纤�ҫ���-���
        private float verticalVelocity = 0f;

        private PlayerLocomotion playerLocomotion;

        public bool lockRotating { get; private set; } = false;
        public bool IsMovementInput { get; private set; }
        public bool IsMoving { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsGround {  get; private set; }
        public bool IsMounting { get; set; }

        public Vector3 CurrentVelocity => characterController.velocity;
        public UnityAction MovementImplement;
        public PayloadScript plScript;

        private void Awake()
        {
            playerLocomotion = GetComponent<PlayerLocomotion>();
            originTurnsmooth = turnSmoothTime;

            MovementImplement += DefaultMovement;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                lockRotating = !lockRotating;
            }
            
            MovementImplement?.Invoke();
        }
        private void LateUpdate()
        {
            // 1. �Ѻ Input ���������ҡ����ع
            _targetRotationY += playerLocomotion.LookInput.x * lookSenseH;
            _targetRotationX -= playerLocomotion.LookInput.y * lookSenseV;

            // 2. �ӡѴ������-�� (Clamp)
            _targetRotationX = Mathf.Clamp(_targetRotationX, -lookLimitV, lookLimitV);

            // 3. �����ع CameraPivot (��ع�ͺ����Ф�)
            // ᡹ X ��͡����, ᡹ Y �����ع�ͺ���
            _cameraPivot.rotation = Quaternion.Euler(_targetRotationX, _targetRotationY, 0f);
        }

        public void SwitchingMovement(bool isMounting)
        {
            MovementImplement = null;
            if (isMounting)
            {
                MovementImplement = plScript.OnPayloadMoveAction;
            }
            else
            {
                MovementImplement = DefaultMovement;
            }
        }
        private void DefaultMovement()
        {
            if (playerLocomotion.OnSprinting && CurrentVelocity.sqrMagnitude > 0.01f)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, 10f * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed / 2, 10f * Time.deltaTime);
            }

            HandleMovement();
            HandleVerticleVelocity();
            UpdateHandleMovementState();
        }

        public void SetEnableCharacterMovement(bool getBoolean)
        {
            characterController.Move(Vector3.zero); // ��ش�������͹���ѹ��
            characterController.enabled = getBoolean;
        }

        private void HandleVerticleVelocity()
        {
            bool isGround = characterController.isGrounded;

            if (isGround && verticalVelocity < 0f)
            {
                // ��ҵԴź��硹��ª������ CharacterController ��Ǩ�ͺ isGrounded ���ʶ��â��
                verticalVelocity = -2f;
            }

            // �ӹǳ�ç���ⴴ (��÷���������躹�����ҹ��)
            if (isGround && playerLocomotion.OnJumping)
            {
                // �ٵ��ç���ⴴ: v = sqrt(h * 2 * g)
                verticalVelocity = Mathf.Sqrt(jumpingForce * 2f * gravity);
                playerLocomotion.OnJumping = false;
            }

            // �ç�����ǧ�ӧҹ��ʹ����
            verticalVelocity -= gravity * Time.deltaTime;
        }

        private void UpdateHandleMovementState()
        {
            IsMovementInput = playerLocomotion.MovementInput != Vector2.zero;
            IsMoving = characterController.velocity.sqrMagnitude > 0.01f;
            IsSprinting = playerLocomotion.OnSprinting && IsMoving;
            IsGround = characterController.isGrounded;

            if (IsMounting)
            {
                _playerState.SetMovementPlayerState(PlayerMovementState.Mounting);
                return;
            }
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
                    // Atan2 �Ф׹��������ͧ�� (0-360) �¤ӹǳ�ҡ��� x ��� z 
                    // �ҡ x=1, z=1 (�Թ��§��Һ�) targetAngle ���� 45 ͧ�����ѵ��ѵ�
                    float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;

                    // ����� ��ع��ҷ�ȹ�鹴��¤����������
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);

                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                }
            }
        }

        private void HandleMovement()
        {
            // 1. �֧��ȷҧ Forward ��� Right �ͧ���ͧ��
            Vector3 forward = _playerCamera.transform.forward;
            Vector3 right = _playerCamera.transform.right;

            // 2. ����������йҺ (᡹ Y �� 0) �������������Фá���µ͹�Թ
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            // 3. �ӹǳ��ȷҧ����͹��� (����ͨش��������Դ�����§)
            Vector3 movementDirection = (forward * playerLocomotion.MovementInput.y) + (right * playerLocomotion.MovementInput.x);

            // 4. ������ѧ��ѹ��ع���
            HandleCharacterRotation(movementDirection);

            Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
            Vector3 newVelocity = characterController.velocity + movementDelta;
            newVelocity.y = 0; // ��ҧ��� Y ����͡��͹�ӹǳ�ç�Թ
            newVelocity += movementDelta;

            //Add Drag ˹�ǧ�͹��Ѻ
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = CheckMove(newVelocity, currentDrag);
            newVelocity = Vector3.ClampMagnitude(newVelocity, currentSpeed);

            newVelocity.y = verticalVelocity;
            //Unity �Ѿവ Move 1 frame ���¡ 1 characterController.Move
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

