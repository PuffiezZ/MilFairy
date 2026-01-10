using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sausagecat.PlayerControlSystem
{
    public class PlayerState : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idle;
        public enum PlayerMovementState
        {
            Idle = 0,
            Run = 1,    
            Sprint = 2,
            Jumping = 3,
            Falling = 4,
        }

        public void SetMovementPlayerState(PlayerMovementState state)
        {
            CurrentPlayerMovementState = state;
        }
    }
}

