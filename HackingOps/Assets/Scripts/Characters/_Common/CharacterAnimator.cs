using HackingOps.CombatSystem.HitHurtBox;
using HackingOps.Utilities;
using UnityEngine;

namespace HackingOps.Characters.Common
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] private float _movementSmoothingSpeed = 1f;
        [SerializeField] private float _crouchingTransitionDuration = 0.5f;
        [SerializeField] private float _blockingTransitionDuration = 0.5f;
        [SerializeField] private int _blockAnimationsAmount = 3;
        [Range(0, 50)] [SerializeField] private float _locomotionAnimationSmoothness = 10f;

        private Animator _animator;
        private IMovementReadable _movementReadable;
        private CrouchController _crouchController;
        private IAttackReadable _attackReadable;
        private HurtBox _hurtBox;

        private int _crouchingLayerIndex;
        private int _standingLayerIndex;

        private bool _isCrouching;
        private bool _isBlocking;

        private int _currentBlockingAnimationIndex;
        private int _lastBlockingAnimationIndex;

        private float _smoothedForwardVelocity;
        private float _smoothedSidewardVelocity;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _movementReadable = GetComponent<IMovementReadable>();
            _crouchController = GetComponent<CrouchController>();
            _attackReadable = GetComponent<IAttackReadable>();
            _hurtBox = GetComponent<HurtBoxWithLife>();
        }

        private void OnEnable()
        {
            _attackReadable.OnMustAttack += UpdateAttackAnimation;
            _hurtBox.OnNotifyHitWithLifeAndDirection.AddListener(UpdateHitAnimation);
        }

        private void OnDisable()
        {
            _attackReadable.OnMustAttack -= UpdateAttackAnimation;
        }

        private void Start()
        {
            _crouchingLayerIndex = _animator.GetLayerIndex("CrouchingLayer");
            _standingLayerIndex = _animator.GetLayerIndex("MovementLayer");
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

            _smoothedForwardVelocity = Mathf.SmoothStep(_smoothedForwardVelocity,
                                                        currentLocalCharacterVelocity.z,
                                                        Time.deltaTime * _locomotionAnimationSmoothness);

            _smoothedSidewardVelocity = Mathf.SmoothStep(_smoothedSidewardVelocity,
                                                         currentLocalCharacterVelocity.x,
                                                         Time.deltaTime * _locomotionAnimationSmoothness);

            _animator.SetFloat("ForwardVelocity", _smoothedForwardVelocity);
            _animator.SetFloat("SidewardVelocity", _smoothedSidewardVelocity);
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

        private void UpdateHitAnimation(float currentLife, float maxLife, Vector3 damageDealerLocation)
        {
            Vector3 damageDealerLocationFlatened = new(damageDealerLocation.x, transform.position.y, damageDealerLocation.z);
            Vector3 directionToDamageDealer = (damageDealerLocationFlatened - transform.position).normalized;

            float theta = CalculateTheta(transform.forward, directionToDamageDealer);
            theta = ApplyLateralDistinction(theta, directionToDamageDealer);

            if (theta >= -45f && theta < 45f) _animator.SetTrigger("HitFromFront");
            else if (theta >= -135f && theta < -45f) _animator.SetTrigger("HitFromLeft");
            else if (theta >= 45f && theta < 135f) _animator.SetTrigger("HitFromRight");
            else _animator.SetTrigger("HitFromBack");
        }

        private float CalculateTheta(Vector3 forwardDirection, Vector3 targetDirection)
        {
            float cosTheta = Vector3.Dot(forwardDirection, targetDirection);
            float theta = Mathf.Acos(cosTheta);
            return Mathf.Rad2Deg * theta;
        }

        private float ApplyLateralDistinction(float theta, Vector3 targetDirection)
        {
            Vector3 crossProduct = Vector3.Cross(transform.forward, targetDirection);

            if (crossProduct.y < 0) theta *= -1f;

            return theta;
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

            float startingCrouchingWeight = _animator.GetLayerWeight(_crouchingLayerIndex);
            float targetCrouchingWeight = _movementReadable.GetIsCrouched() ? 1 : 0;
            AnimationUtils.UpdateAnimationWeight(_animator,
                                                 _crouchingLayerIndex,
                                                 startingCrouchingWeight,
                                                 targetCrouchingWeight,
                                                 _crouchingTransitionDuration);

            float startingStandingWeight = _animator.GetLayerWeight(_standingLayerIndex);
            float targetStandingWeight = _movementReadable.GetIsCrouched() ? 0 : 1;
            AnimationUtils.UpdateAnimationWeight(_animator,
                                                 _standingLayerIndex,
                                                 startingStandingWeight,
                                                 targetStandingWeight,
                                                 _crouchingTransitionDuration);
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