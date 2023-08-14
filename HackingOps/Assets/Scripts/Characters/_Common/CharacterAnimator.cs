using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace HackingOps.Characters.Common
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] float _movementSmoothingSpeed = 1f;
        [SerializeField] float _crouchingTransitionDuration = 0.5f;

        Animator _animator;
        IMovementReadable _movementReadable;

        int _crouchingLayerIndex;

        bool _previousIsCrouching;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _movementReadable = GetComponent<IMovementReadable>();
        }

        private void Start()
        {
            _crouchingLayerIndex = _animator.GetLayerIndex("CrouchingLayer");
        }

        Vector3 currentLocalCharacterVelocity = Vector3.zero;
        private void Update()
        {
            UpdatePlaneMovementAnimation();
            UpdateVerticalMovementAnimation();

            if (_previousIsCrouching != _movementReadable.GetIsCrouched())
            {
                UpdateCrouchingAnimation();
            }

            _previousIsCrouching = _movementReadable.GetIsCrouched();
        }

        private void UpdatePlaneMovementAnimation()
        {
            Vector3 characterVelocity = _movementReadable.GetVelocity();
            Vector3 localCharacterVelocity = transform.InverseTransformDirection(characterVelocity);
            Vector3 desiredLocalCharacterVelocity = Vector3.zero;

            desiredLocalCharacterVelocity.z = NormalizeSpeed(localCharacterVelocity.z);
            desiredLocalCharacterVelocity.x = NormalizeSpeed(localCharacterVelocity.x);

            float distance = (desiredLocalCharacterVelocity - currentLocalCharacterVelocity).magnitude;
            float distanceThatWouldNormallyBeApplied = _movementSmoothingSpeed * Time.deltaTime;

            currentLocalCharacterVelocity +=
                (desiredLocalCharacterVelocity - currentLocalCharacterVelocity).normalized *
                Mathf.Min(distanceThatWouldNormallyBeApplied, distance);

            _animator.SetFloat("ForwardVelocity", currentLocalCharacterVelocity.z);
            _animator.SetFloat("SidewardVelocity", currentLocalCharacterVelocity.x);
        }

        private void UpdateVerticalMovementAnimation()
        {
            Vector3 characterVelocity = _movementReadable.GetVelocity();
            float jumpProgress = Mathf.InverseLerp(_movementReadable.GetJumpSpeed(), -_movementReadable.GetJumpSpeed(), characterVelocity.y);

            _animator.SetBool("IsGrounded", _movementReadable.GetIsGrounded());
            _animator.SetFloat("JumpProgress", jumpProgress);
        }

        private float NormalizeSpeed(float s)
        {
            if (s < -_movementReadable.GetNormalSpeed())
            {
                s = -Mathf.InverseLerp(-_movementReadable.GetNormalSpeed(), -_movementReadable.GetAcceleratedSpeed(), s);
                s -= 1f;
            }
            else if (s > _movementReadable.GetNormalSpeed())
            {
                s = Mathf.InverseLerp(_movementReadable.GetNormalSpeed(), _movementReadable.GetAcceleratedSpeed(), s);
                s += 1f;
            }
            else
            {
                s = Mathf.InverseLerp(-_movementReadable.GetNormalSpeed(), _movementReadable.GetNormalSpeed(), s);
                s *= 2f;
                s -= 1f;
            }

            return s;
        }

        private void UpdateCrouchingAnimation()
        {
            _animator.SetBool("IsCrouching", _movementReadable.GetIsCrouched());

            float startingWeight = _animator.GetLayerWeight(_crouchingLayerIndex);
            float targetWeight = _movementReadable.GetIsCrouched() ? 1 : 0;

            DOVirtual.Float(startingWeight, targetWeight, _crouchingTransitionDuration, weight =>
            {
                _animator.SetLayerWeight(_crouchingLayerIndex, weight);
            });
        }
    }
}