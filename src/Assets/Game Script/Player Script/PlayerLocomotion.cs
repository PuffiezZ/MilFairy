using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sausagecat.PlayerControlSystem
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotion : MonoBehaviour, PlayerControl.IPlayerLocomotionActions
    {
        public PlayerControl PlayerControls {  get; private set; }
        public PlayerEquipment PlayerEquipment { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool OnSprinting { get; private set; }
        public bool OnJumping { get; set; }

        public bool SendMainActionSignal { get; set; }
        private Coroutine weaponSwapCoroutine;

        private void OnEnable()
        {
            PlayerEquipment = GetComponent<PlayerEquipment>();
            PlayerControls = new PlayerControl();
            PlayerControls.Enable();

            PlayerControls.PlayerLocomotion.Enable();
            PlayerControls.PlayerLocomotion.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.PlayerLocomotion.Disable();
            PlayerControls.PlayerLocomotion.RemoveCallbacks(this);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
            //print(MovementInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSprinting = true;
            }
            else
            {
                OnSprinting = false;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {

            if (GetComponent<PlayerMovement>())
            {
                bool isGround = GetComponent<PlayerMovement>().IsGround;
                if (context.performed && !OnJumping && isGround)
                {
                    OnJumping = true;
                }
            }
            else
            {
                bool isGround = GetComponent<PlayerRagdollMovement>().IsGrounded;
                if (context.performed && !OnJumping && isGround)
                {
                    OnJumping = true;
                }
            }

        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            // ��੾�� context.performed ��������鴷ӧҹ�� 1 ���駵�͡�á� 1 �ͺ
            if (context.performed)
            {
                Debug.Log("On Attack Locomotion Invoke (Performed Only)");
                SendMainActionSignal = true;
            }
            if(context.canceled)
            {
                SendMainActionSignal = false;
            }
        }

        public void OnWeaponToggle(InputAction.CallbackContext context)
        {
            // �� performed ��������ѹ�������������͡�����ŧ
            if (context.performed)
            {
                int weaponValue = Mathf.Abs(Mathf.RoundToInt(context.ReadValue<float>()));
                int finalValue = weaponValue - 1;

                // ��ش�����Ѻ���ظ���駡�͹˹�� (�����) ���������� Logic �աѹ
                if (weaponSwapCoroutine != null) StopCoroutine(weaponSwapCoroutine);

                // ������鹡����Ѻ���ظ��������
                weaponSwapCoroutine = StartCoroutine(PlayerEquipment.SwapWeapon(finalValue));
            }
        }
    }
}

