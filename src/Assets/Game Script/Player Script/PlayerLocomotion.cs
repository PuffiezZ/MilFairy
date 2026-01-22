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

        public bool EnableToAttack { get; set; }
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
            print(MovementInput);
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
            bool isGround = GetComponent<PlayerMovement>().IsGround;  
            if(context.performed && !OnJumping && isGround)
            {
                OnJumping = true;
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            // ใช้เฉพาะ context.performed เพื่อให้โค้ดทำงานแค่ 1 ครั้งต่อการกด 1 รอบ
            if (context.performed)
            {
                Debug.Log("On Attack Locomotion Invoke (Performed Only)");
                EnableToAttack = true;
            }
            else
            {
                EnableToAttack = false;
            }
        }

        public void OnWeaponToggle(InputAction.CallbackContext context)
        {
            // ใช้ performed เพื่อให้รันแค่ครั้งเดียวเมื่อกดปุ่มลง
            if (context.performed)
            {
                int weaponValue = Mathf.Abs(Mathf.RoundToInt(context.ReadValue<float>()));
                int finalValue = weaponValue - 1;

                // หยุดการสลับอาวุธครั้งก่อนหน้า (ถ้ามี) เพื่อไม่ให้ Logic ตีกัน
                if (weaponSwapCoroutine != null) StopCoroutine(weaponSwapCoroutine);

                // เริ่มต้นการสลับอาวุธครั้งใหม่
                weaponSwapCoroutine = StartCoroutine(PlayerEquipment.SwapWeapon(finalValue));
            }
        }
    }
}

