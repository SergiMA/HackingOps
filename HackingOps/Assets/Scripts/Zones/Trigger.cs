using System;
using UnityEngine;

namespace HackingOps.Zones
{
    public class Trigger : MonoBehaviour
    {
        public event Action<Trigger> OnEnter;
        public event Action<Trigger> OnStay;
        public event Action<Trigger> OnExit;

        [SerializeField] private string[] _targetTags = { "Player" };

        private Collider[] _colliders;
        private Collider _other;

        #region Unity methods
        private void Awake()
        {
            _colliders = GetComponents<Collider>();
            SetCollidersAsTrigger();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ValidateTag(other.tag) == false)
                return;

            _other = other;
            OnEnter?.Invoke(this);
        }

        private void OnTriggerStay(Collider other)
        {
            if (ValidateTag(other.tag) == false)
                return;

            _other = other;
            OnStay?.Invoke(this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (ValidateTag(other.tag) == false)
                return;

            _other = other;
            OnExit?.Invoke(this);
        }
        #endregion

        private void SetCollidersAsTrigger()
        {
            foreach (Collider collider in _colliders)
                collider.isTrigger = true;
        }

        private bool ValidateTag(string tag)
        {
            foreach (string targetTag in _targetTags)
            {
                if (tag == targetTag)
                    return true;
            }

            return false;
        }

        public Collider GetOtherCollider() => _other;
    }
}