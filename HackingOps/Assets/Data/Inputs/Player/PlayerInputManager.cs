using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HackingOps.Input
{
    public class PlayerInputManager : MonoBehaviour
    {
        public event Action OnJump;
        public event Action OnStartCrouching;
        public event Action OnStopCrouching;

        public Vector2 MoveInput { get; private set; }
        public bool IsRunning { get; private set; }

        Player_InputActions _inputActions;

        private void Awake()
        {
            _inputActions = new Player_InputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();

            _inputActions.ThirdPersonCharacter_ActionMap.Move.performed += Move;
            _inputActions.ThirdPersonCharacter_ActionMap.Move.canceled += Move;

            _inputActions.ThirdPersonCharacter_ActionMap.Run.performed += Run;
            _inputActions.ThirdPersonCharacter_ActionMap.Run.canceled += Run;

            _inputActions.ThirdPersonCharacter_ActionMap.Jump.performed += Jump;

            _inputActions.ThirdPersonCharacter_ActionMap.Crouch.performed += StartCrouching;
            _inputActions.ThirdPersonCharacter_ActionMap.Crouch.canceled += StopCrouching;
        }

        private void OnDisable()
        {
            _inputActions.ThirdPersonCharacter_ActionMap.Move.performed -= Move;
            _inputActions.ThirdPersonCharacter_ActionMap.Move.canceled -= Move;

            _inputActions.ThirdPersonCharacter_ActionMap.Run.performed -= Run;
            _inputActions.ThirdPersonCharacter_ActionMap.Run.canceled -= Run;

            _inputActions.ThirdPersonCharacter_ActionMap.Jump.performed -= Jump;

            _inputActions.ThirdPersonCharacter_ActionMap.Crouch.performed -= StartCrouching;
            _inputActions.ThirdPersonCharacter_ActionMap.Crouch.canceled -= StopCrouching;

            _inputActions.Disable();
        }

        private void Move(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }

        private void Run(InputAction.CallbackContext ctx)
        {
            IsRunning = ctx.ReadValue<float>() > 0.1f;
        }

        private void Jump(InputAction.CallbackContext ctx)
        {
            OnJump?.Invoke();
        }

        private void StartCrouching(InputAction.CallbackContext ctx)
        {
            OnStartCrouching?.Invoke();
        }

        private void StopCrouching(InputAction.CallbackContext ctx)
        {
            OnStopCrouching?.Invoke();
        }
    }
}