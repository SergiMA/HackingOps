using UnityEngine;

namespace HackingOps.Characters.Common
{
    public class CharacterRagdollController : MonoBehaviour
    {
        [SerializeField] private Transform _ragdollParent;

        private Rigidbody[] _rigidbodies;
        private Collider[] _colliders;

        private void Awake()
        {
            _rigidbodies = _ragdollParent.GetComponentsInChildren<Rigidbody>();
            _colliders = _ragdollParent.GetComponentsInChildren<Collider>();

            DeactivateRagdoll();
        }

        private void DeactivateRagdoll()
        {
            foreach (Rigidbody rb in _rigidbodies) { rb.isKinematic = true; }
            foreach (Collider c in _colliders) { c.enabled = false; }
        }

        public void ActivateRagdoll()
        {
            foreach (Rigidbody rb in _rigidbodies) { rb.isKinematic = false; }
            foreach (Collider c in _colliders) { c.enabled = true; }
        }
    }
}