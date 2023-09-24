using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HackingOps.Input
{
    public class PlayerInputManager : MonoBehaviour
    {
        public event Action OnJump;
        public event Action OnCrouchPressed;
        public event Action<Vector2> OnChangeWeaponDeltaUpdated;
        public event Action<int> OnSelectWeapon;
        public event Action<float> OnShoot;
        public event Action OnInteract;
        public event Action OnStartAiming;
        public event Action OnStopAiming;

        public Vector2 MoveInput { get; private set; }
        public Vector2 MouseInput { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsLocking { get; private set; }

        private PlayerInputActions _inputActions;

        private float _previousAimingInput;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();

            _inputActions.ThirdPersonCharacter_ActionMap.Move.performed += Move;
            _inputActions.ThirdPersonCharacter_ActionMap.Move.canceled += Move;

            _inputActions.ThirdPersonCharacter_ActionMap.Look.performed += Look;
            _inputActions.ThirdPersonCharacter_ActionMap.Look.canceled += Look;

            _inputActions.ThirdPersonCharacter_ActionMap.Run.performed += Run;
            _inputActions.ThirdPersonCharacter_ActionMap.Run.canceled += Run;

            _inputActions.ThirdPersonCharacter_ActionMap.Jump.performed += Jump;

            _inputActions.ThirdPersonCharacter_ActionMap.Crouch.performed += Crouch;

            _inputActions.ThirdPersonCharacter_ActionMap.Shoot.performed += Shoot;
            _inputActions.ThirdPersonCharacter_ActionMap.Shoot.canceled += Shoot;

            _inputActions.ThirdPersonCharacter_ActionMap.Lock.performed += Lock;
            _inputActions.ThirdPersonCharacter_ActionMap.Lock.canceled += Lock;

            _inputActions.ThirdPersonCharacter_ActionMap.ChangeWeapon.performed += ChangeWeapon;
            _inputActions.ThirdPersonCharacter_ActionMap.ChangeWeapon.canceled += ChangeWeapon;

            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon0.performed += SelectWeapon0;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon1.performed += SelectWeapon1;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon2.performed += SelectWeapon2;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon3.performed += SelectWeapon3;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon4.performed += SelectWeapon4;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon5.performed += SelectWeapon5;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon6.performed += SelectWeapon6;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon7.performed += SelectWeapon7;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon8.performed += SelectWeapon8;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon9.performed += SelectWeapon9;

            _inputActions.ThirdPersonCharacter_ActionMap.Interact.performed += Interact;
        }

        private void OnDisable()
        {
            _inputActions.ThirdPersonCharacter_ActionMap.Move.performed -= Move;
            _inputActions.ThirdPersonCharacter_ActionMap.Move.canceled -= Move;

            _inputActions.ThirdPersonCharacter_ActionMap.Look.performed -= Look;
            _inputActions.ThirdPersonCharacter_ActionMap.Look.canceled -= Look;

            _inputActions.ThirdPersonCharacter_ActionMap.Run.performed -= Run;
            _inputActions.ThirdPersonCharacter_ActionMap.Run.canceled -= Run;

            _inputActions.ThirdPersonCharacter_ActionMap.Jump.performed -= Jump;

            _inputActions.ThirdPersonCharacter_ActionMap.Crouch.performed -= Crouch;

            _inputActions.ThirdPersonCharacter_ActionMap.Shoot.performed -= Shoot;
            _inputActions.ThirdPersonCharacter_ActionMap.Shoot.canceled -= Shoot;

            _inputActions.ThirdPersonCharacter_ActionMap.Lock.performed -= Lock;
            _inputActions.ThirdPersonCharacter_ActionMap.Lock.canceled -= Lock;

            _inputActions.ThirdPersonCharacter_ActionMap.ChangeWeapon.performed -= ChangeWeapon;
            _inputActions.ThirdPersonCharacter_ActionMap.ChangeWeapon.canceled -= ChangeWeapon;

            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon0.performed -= SelectWeapon0;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon1.performed -= SelectWeapon1;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon2.performed -= SelectWeapon2;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon3.performed -= SelectWeapon3;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon4.performed -= SelectWeapon4;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon5.performed -= SelectWeapon5;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon6.performed -= SelectWeapon6;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon7.performed -= SelectWeapon7;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon8.performed -= SelectWeapon8;
            _inputActions.ThirdPersonCharacter_ActionMap.SelectWeapon9.performed -= SelectWeapon9;

            _inputActions.ThirdPersonCharacter_ActionMap.Interact.performed -= Interact;

            _inputActions.Disable();
        }

        private void Move(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }

        private void Look(InputAction.CallbackContext ctx)
        {
            MouseInput = ctx.ReadValue<Vector2>();
        }

        private void Run(InputAction.CallbackContext ctx)
        {
            IsRunning = ctx.ReadValue<float>() > 0.1f;
        }

        private void Jump(InputAction.CallbackContext ctx)
        {
            OnJump?.Invoke();
        }

        private void Crouch(InputAction.CallbackContext ctx)
        {
            OnCrouchPressed?.Invoke();
        }

        private void Shoot(InputAction.CallbackContext ctx)
        {
            OnShoot?.Invoke(ctx.ReadValue<float>());
        }

        private void Lock(InputAction.CallbackContext ctx)
        {
            IsLocking = ctx.ReadValue<float>() > 0f;
            float currentAimingInput = ctx.ReadValue<float>();

            if (_previousAimingInput != currentAimingInput)
            {
                if (currentAimingInput > 0f)
                    OnStartAiming?.Invoke();
                else
                    OnStopAiming?.Invoke();
            }

            _previousAimingInput = currentAimingInput;
        }

        private void Interact(InputAction.CallbackContext ctx)
        {
            OnInteract?.Invoke();
        }

        #region Switch weapon
        private void ChangeWeapon(InputAction.CallbackContext ctx)
        {
            OnChangeWeaponDeltaUpdated?.Invoke(ctx.ReadValue<Vector2>());
        }

        private void SelectWeapon0(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(-1);
        }

        private void SelectWeapon1(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(0);
        }

        private void SelectWeapon2(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(1);
        }

        private void SelectWeapon3(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(2);
        }

        private void SelectWeapon4(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(3);
        }

        private void SelectWeapon5(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(4);
        }

        private void SelectWeapon6(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(5);
        }

        private void SelectWeapon7(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(6);
        }

        private void SelectWeapon8(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(7);
        }

        private void SelectWeapon9(InputAction.CallbackContext ctx)
        {
            OnSelectWeapon?.Invoke(8);
        }
        #endregion
    }
}