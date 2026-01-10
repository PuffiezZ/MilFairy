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
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool OnSprinting { get; private set; }
        public bool OnJumping { get; set; }
        private void OnEnable()
        {
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
    }
}

