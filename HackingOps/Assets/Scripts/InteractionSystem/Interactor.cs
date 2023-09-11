using HackingOps.Common.Events;
using HackingOps.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.InteractionSystem
{
    public class Interactor : MonoBehaviour
    {
        public UnityEvent<IInteractable> OnInteractWithObject;

        [SerializeField] PlayerInputManager _inputManager;
        [SerializeField] Transform _interactionPoint;

        [SerializeField] private float _interactionRadius = 2f;

        private List<IInteractable> _interactables = new();

        private void OnEnable()
        {
            _inputManager.OnInteract += OnInteract;
        }

        private void OnDisable()
        {
            _inputManager.OnInteract -= OnInteract;
        }

        private void OnInteract()
        {
            _interactables.Clear();

            FillInteractablesList();

            if (_interactables.Count == 0) return;

            IInteractable interactable = GetClosestInteractable();

            EventQueue.Instance.EnqueueEvent(new InteractionEventData(interactable, interactable.GetTransform()));
        }

        private void FillInteractablesList()
        {
            Collider[] colliders = Physics.OverlapSphere(_interactionPoint.position, _interactionRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out IInteractable interactable))
                {
                    _interactables.Add(interactable);
                }
            }
        }

        private IInteractable GetClosestInteractable()
        {
            IInteractable closestInteractable = null;

            foreach (IInteractable interactable in _interactables)
            {
                if (closestInteractable == null)
                    closestInteractable = interactable;
                else
                {
                    if (Vector3.Distance(_interactionPoint.position, interactable.GetTransform().position) <
                        Vector3.Distance(_interactionPoint.position, closestInteractable.GetTransform().position))
                    {
                        closestInteractable = interactable;
                    }
                }
            }

            return closestInteractable;
        }
    }
}