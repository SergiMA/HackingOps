using System;
using UnityEngine;

namespace HackingOps.Zones
{
    public class Trigger : MonoBehaviour
    {
        public event Action<Trigger> OnStepped;

        [SerializeField] private string[] _targetTags = { "Player" };

        Collider[] _colliders;

        private void Awake()
        {
            _colliders = GetComponents<Collider>();
            SetCollidersAsTrigger();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (ValidateTag(other.tag) == false)
                return;

            OnStepped?.Invoke(this);
        }

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
    }
}