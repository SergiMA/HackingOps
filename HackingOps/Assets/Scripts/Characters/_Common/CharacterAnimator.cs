using UnityEngine;
using DG.Tweening;

namespace HackingOps.Characters.Common
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private float _movementSmoothingSpeed = 1f;
        [SerializeField] private float _crouchingTransitionDuration = 0.5f;
        [SerializeField] private float _blockingTransitionDuration = 0.5f;
        [SerializeField] private int _blockAnimationsAmount = 3;

        private Animator _animator;
        private IMovementReadable _movementReadable;
        private CrouchController _crouchController;
        private IAttackReadable _attackReadable;

        private int _crouchingLayerIndex;
        private int _blockingLayerIndex;

        private bool _isCrouching;
        private bool _isBlocking;

        private int _currentBlockingAnimationIndex;
        private int _lastBlockingAnimationIndex;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _movementReadable = GetComponent<IMovementReadable>();
            _crouchController = GetComponent<CrouchController>();
            _attackReadable = GetComponent<IAttackReadable>();
        }

        private void OnEnable()
        {
            _attackReadable.OnMustAttack += UpdateAttackAnimation;
        }

        private void OnDisable()
        {
            _attackReadable.OnMustAttack -= UpdateAttackAnimation;
        }

        private void Start()
        {
            _crouchingLayerIndex = _animator.GetLayerIndex("CrouchingLayer");
            _blockingLayerIndex = _animator.GetLayerIndex("BlockingLayer");
        }

        Vector3 currentLocalCharacterVelocity = Vector3.zero;
        private void Update()
        {
            UpdatePlaneMovementAnimation();
            UpdateVerticalMovementAnimation();
            UpdateAttackAnimation();
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

        private void UpdateAttackAnimation()
        {
            if (_attackReadable != null && _attackReadable.MustAttack())
            {
                _animator.SetTrigger("Attack");
            }
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

        private void UpdateBlockAnimationIndex()
        {
            _currentBlockingAnimationIndex = GetRandomUnrepeatedValue(0, _blockAnimationsAmount, _lastBlockingAnimationIndex);
            _lastBlockingAnimationIndex = _currentBlockingAnimationIndex;

            _animator.SetInteger("BlockIndex", _currentBlockingAnimationIndex);
        }

        private int GetRandomUnrepeatedValue(int minValue, int maxValue, int lastValue)
        {
            int randomValue;
            do
            {
                randomValue = Random.Range(minValue, maxValue);
            } while (randomValue == lastValue);

            return randomValue;
        }

        public void OnStartCrouching()
        {
            if (_isBlocking)
                return;

            if (_crouchController.TryCrouchDown(true))
            {
                UpdateCrouchingAnimation();
            }
        }

        public void OnStopCrouching()
        {
            if (_crouchController.TryStandUp(true))
            {
                UpdateCrouchingAnimation();
            }
        }

        public void OnStartBlocking()
        {
            if (_isCrouching)
                return;

            _isBlocking = true;

            UpdateBlockAnimationIndex();

            _animator.SetBool("IsBlocking", _isBlocking);
        }

        public void OnStopBlocking()
        {
            _isBlocking = false;
            _animator.SetBool("IsBlocking", _isBlocking);
        }

        public void OnNormalBlockPerformed()
        {
            if (!_isBlocking)
                return;

            UpdateBlockAnimationIndex();
            _animator.SetTrigger("OnBlockedHit");
        }

        public void OnPerfectParryPerformed()
        {
            if (_isBlocking)
                _animator.SetTrigger("OnPerfectParryPerformed");
        }

        public void OnParried()
        {
            _animator.SetTrigger("OnParried");
        }
    }
}