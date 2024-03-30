using HackingOps.InteractionSystem;
using HackingOps.Weapons.Common;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Weapons.WeaponFoundations
{
    public abstract class Weapon : MonoBehaviour, IInteractable
    {
        public event Action OnReceiveCandidateNotification;

        public UnityEvent OnStore;
        public UnityEvent OnGrab;
        public UnityEvent OnDrop;

        [field: Header("Settings")]

        [Tooltip("Slot is the space the weapon will occupy in the inventory.")]
        [SerializeField] public WeaponSlot Slot;

        [HideInInspector] public bool IsUsedByAI;

        [SerializeField] private string _interactionText = "interaction_grab";
        [SerializeField] private Collider[] _colliders;

        [Header("Debug")]
        [SerializeField] private bool _debugUse;
        [SerializeField] private bool _debugStartUsing;
        [SerializeField] private bool _debugStopUsing;

        [SerializeField] private Vector3 _holderOffset;

        private MeshRenderer[] _renderers;
        private Rigidbody[] _rigidbodies;
        private bool _canBeInteracted = true;


        private void OnValidate()
        {
            if (_debugUse)
            {
                _debugUse = false;
                Use();
            }

            if (_debugStartUsing)
            {
                _debugStartUsing = false;
                StartUsing();
            }

            if (_debugStopUsing)
            {
                StopUsing();
                _debugStopUsing = false;
            }
        }

        private void Awake()
        {
            GetWeaponEssentials();
            InternalAwake();
        }

        private void GetWeaponEssentials()
        {
            _renderers = GetComponentsInChildren<MeshRenderer>();
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
        }

        protected virtual void InternalAwake() { }

        public virtual void Use() { }
        public virtual void StartUsing() { }
        public virtual void StopUsing() { }
        public virtual void ResetWeapon() { }

        public abstract bool CanUse();
        public abstract bool CanContinuouslyUse();
        public abstract float GetEffectiveRange();

        public virtual void AdaptToVerticalAngle(Vector3 aimForward) { }
        public virtual void NotifyAimingAngles(float angleV, float angleH) { }
        public virtual Vector3 GetRotationPointPosition() { return Vector3.zero; }

        public virtual bool HasGrabPoints() { return false; }
        public virtual Transform GetLeftArmHint() { return null; }
        public virtual Transform GetLeftArmTarget() { return null; }
        public virtual Transform GetRightArmHint() { return null; }
        public virtual Transform GetRightArmTarget() { return null; }
        public virtual void ResetRotation() { transform.localRotation = Quaternion.identity; }

        public virtual void Drop()
        {
            foreach (MeshRenderer renderer in _renderers)
            {
                renderer.enabled = true;
            }

            foreach (Collider collider in _colliders)
            {
                collider.enabled = true;
            }

            foreach (Rigidbody rigidbody in _rigidbodies)
            {
                rigidbody.isKinematic = false;
            }

            _canBeInteracted = true;

            OnDrop.Invoke();
        }
        public virtual void Grab()
        {
            foreach (MeshRenderer renderer in _renderers)
            {
                renderer.enabled = true;
            }

            foreach (Collider collider in _colliders)
            {
                collider.enabled = false;
            }

            foreach (Rigidbody rigidbody in _rigidbodies)
            {
                rigidbody.isKinematic = true;
            }

            _canBeInteracted = false;

            OnGrab.Invoke();
        }
        public virtual void Store()
        {
            foreach (MeshRenderer renderer in _renderers)
            {
                renderer.enabled = true;
            }

            foreach (Collider collider in _colliders)
            {
                collider.enabled = false;
            }

            foreach (Rigidbody rigidbody in _rigidbodies)
            {
                rigidbody.isKinematic = true;
            }

            ResetRotation();

            _canBeInteracted = false;

            OnStore.Invoke();
        }

        public Vector3 GetHolderOffset() => _holderOffset;

        #region IInteractable implementation
        public void Interact(Interactor interactor)
        {
            if (interactor == null)
                return;

            if (!CanBeInteracted())
                return;

            if (interactor.TryGetComponent(out Inventory inventory))
            {
                inventory.AddWeapon(this);
            }
        }

        public bool CanBeInteracted() => _canBeInteracted;

        public Transform GetTransform() => transform;

        public string GetInteractableText() => _interactionText;

        public void EnableInteractions() { }
        public void DisableInteractions() { }

        public void ReceiveCandidateNotification()
        {
            OnReceiveCandidateNotification?.Invoke();
        }
        #endregion
    }
}