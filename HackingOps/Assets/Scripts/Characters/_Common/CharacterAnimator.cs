using UnityEngine;

namespace HackingOps.Characters
{
    public class CharacterAnimator : MonoBehaviour
    {
        [SerializeField] float _movementSmoothingSpeed = 1f;

        Animator _animator;
        IMovementReadable _movementReadable;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _movementReadable = GetComponent<IMovementReadable>();
        }

        Vector3 currentLocalCharacterVelocity = Vector3.zero;
        private void Update()
        {
            UpdatePlaneMovementAnimation();
            UpdateVerticalMovementAnimation();
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
            if (s < -_movementReadable.GetWalkSpeed())
            {
                s = -Mathf.InverseLerp(-_movementReadable.GetWalkSpeed(), -_movementReadable.GetRunSpeed(), s);
                s -= 1f;
            }
            else if (s > _movementReadable.GetWalkSpeed())
            {
                s = Mathf.InverseLerp(_movementReadable.GetWalkSpeed(), _movementReadable.GetRunSpeed(), s);
                s += 1f;
            }
            else
            {
                s = Mathf.InverseLerp(-_movementReadable.GetWalkSpeed(), _movementReadable.GetWalkSpeed(), s);
                s *= 2f;
                s -= 1f;
            }

            return s;
        }
    }
}