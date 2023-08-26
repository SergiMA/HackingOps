using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace HackingOps.Characters.Common
{
    public class CrouchController : MonoBehaviour
    {
        [Header("Bindings")]

        [SerializeField] private Transform _headPosition;

        [Tooltip("This property will only be used if Use Character Controller Collider is unchecked")]
        [SerializeField] private CapsuleCollider _capsuleCollider;

        [Tooltip("This property will only be used if Use Character Controller Collider is checked")]
        [SerializeField] private CharacterController _characterController;

        [SerializeField] private Transform _weaponHeightReference;

        [Header("Settings - Check if head is free of obstacles")]

        [SerializeField] private float _ceilingCheckerRayDistance = 0.8f;
        [SerializeField] private LayerMask _obstaclesLayerMask = Physics.DefaultRaycastLayers;

        [Header("Settings - Collider")]

        [SerializeField] private float _changingColliderDuration = 0.5f;

        [SerializeField] private bool _useCharacterControllerCollider;

        [Space]

        [SerializeField] private float _colliderStandingHeight;
        [SerializeField] private Vector3 _colliderStandingCenter;

        [Space]

        [SerializeField] private float _colliderCrouchingHeight;
        [SerializeField] private Vector3 _colliderCrouchingCenter;

        [Header("Settings - Weapon Height Reference")]
        [SerializeField] private float _movingWeaponReferenceDuration = 0.5f;

        [SerializeField] private Vector3 _weaponReferenceStandingHeight;
        [SerializeField] private Vector3 _weaponReferenceCrouchingHeight;

        private void OnDrawGizmos()
        {
            /*
             * It should be used Gizmos.DrawRay(), but Physics.Raycast is the only way to change 
             * the ray color based on the impact. And it's used in an OnDrawGizmos because it's only
             * called in the Unity editor
             */

            if (Physics.Raycast(_headPosition.position, Vector3.up, _ceilingCheckerRayDistance, _obstaclesLayerMask))
            {
                Debug.DrawRay(_headPosition.position, Vector3.up * _ceilingCheckerRayDistance, Color.red);
            }
            else
            {
                Debug.DrawRay(_headPosition.position, Vector3.up * _ceilingCheckerRayDistance, Color.green);
            }

        }

        private bool IsObjectAboveHead()
        {
            return Physics.Raycast(_headPosition.position, Vector3.up, _ceilingCheckerRayDistance, _obstaclesLayerMask);
        }

        /// <summary>
        /// Check if the character is able to crouch. If they are, adjust
        /// the collider to match the crouching posture
        /// </summary>
        /// <returns>Returns true if the character is able to crouch down</returns>
        public bool TryCrouchDown(bool ignoreColliderAdjustment = false)
        {
            if (!ignoreColliderAdjustment)
            {
                if (_useCharacterControllerCollider)
                {
                    DOVirtual.Float(_characterController.height, _colliderCrouchingHeight, _changingColliderDuration, height => 
                    {
                        _characterController.height = height;
                    });

                    DOVirtual.Vector3(_characterController.center, _colliderCrouchingCenter, _changingColliderDuration, center =>
                    {
                        _characterController.center = center;
                    });
                }
                else
                {
                    DOVirtual.Float(_capsuleCollider.height, _colliderCrouchingHeight, _changingColliderDuration, height =>
                    {
                        _characterController.height = height;
                    });

                    DOVirtual.Vector3(_capsuleCollider.center, _colliderCrouchingCenter, _changingColliderDuration, center =>
                    {
                        _characterController.center = center;
                    });
                }

                _weaponHeightReference.DOLocalMove(_weaponReferenceCrouchingHeight, _movingWeaponReferenceDuration);
            }

            return true;
        }

        /// <summary>
        /// Check if there's something on top of the character to make sure if the character
        /// is able to stand up or not. If the head is free, adjust the collider
        /// to match the standing posture
        /// </summary>
        /// <returns>Returns true if the character can stand up. False otherwise</returns>

        public bool TryStandUp(bool ignoreColliderAdjustment = false)
        {
            bool isHeadFree = !IsObjectAboveHead();

            if (isHeadFree && !ignoreColliderAdjustment)
            {
                if (_useCharacterControllerCollider)
                {
                    DOVirtual.Float(_characterController.height, _colliderStandingHeight, _changingColliderDuration, height =>
                    {
                        _characterController.height = height;
                    });

                    DOVirtual.Vector3(_characterController.center, _colliderStandingCenter, _changingColliderDuration, center =>
                    {
                        _characterController.center = center;
                    });
                }
                else
                {
                    DOVirtual.Float(_capsuleCollider.height, _colliderStandingHeight, _changingColliderDuration, height =>
                    {
                        _characterController.height = height;
                    });

                    DOVirtual.Vector3(_capsuleCollider.center, _colliderStandingCenter, _changingColliderDuration, center =>
                    {
                        _characterController.center = center;
                    });
                }

                _weaponHeightReference.DOLocalMove(_weaponReferenceStandingHeight, _movingWeaponReferenceDuration);
            }

            return isHeadFree;
        }
    }
}