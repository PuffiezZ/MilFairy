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
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        private PlayerLocomotion playerLocomotion;
        private Vector2 cameraRotation = Vector2.zero;
        private Vector2 playerTargetRotation = Vector2.zero;

        private void Awake()
        {
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }

        private void Update()
        {
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x,0f,_playerCamera.transform.right.z).normalized;
            Vector3 movementDirction = cameraRightXZ * playerLocomotion.MovementInput.x + cameraForwardXZ * playerLocomotion.MovementInput.y;

            Vector3 movementDelta = movementDirction * runAcceleration * Time.deltaTime;
            Vector3 newVelocity = characterController.velocity + movementDelta;

            //Add Drag หน่วงตอนขยับ
            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = CheckMove(newVelocity,currentDrag);
            newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

            //Unity อัพเดต Move 1 frame เรียก 1 characterController.Move
            characterController.Move(newVelocity * Time.deltaTime);
        }
        private void LateUpdate()
        {
            cameraRotation.x += lookSenseH * playerLocomotion.LookInput.x;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseV * playerLocomotion.LookInput.y, -lookLimitV,lookLimitV);
            
            playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * playerLocomotion.LookInput.x;

            transform.rotation = Quaternion.Euler(0f, playerTargetRotation.x, 0f);
            _playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y,cameraRotation.x, 0f);
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

