using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.InteractionSystem
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] PlayerInputManager _inputManager;
        [SerializeField] Transform _interactionPoint;

        [SerializeField] private float _interactionRadius = 2f;
        [SerializeField] private float _dotThreshold = 0.7f;

        private List<IInteractable> _interactables = new();

        private void OnEnable()
        {
            _inputManager.OnInteract += OnInteract;
        }

        private void OnDisable()
        {
            _inputManager.OnInteract -= OnInteract;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(_interactionPoint.position, _interactionRadius);
        }

        private void OnInteract()
        {
            _interactables.Clear();

            FillInteractablesList();

            if (_interactables.Count == 0) return;

            IInteractable interactable = GetClosestInteractable();

            interactable.Interact(this);
            ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new InteractionEventData(interactable, interactable.GetTransform()));
        }

        private void FillInteractablesList()
        {
            Collider[] colliders = Physics.OverlapSphere(_interactionPoint.position, _interactionRadius);
            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent(out IInteractable interactable))
                {
                    if (Physics.Linecast(_interactionPoint.position, interactable.GetTransform().position))
                    {
                        if (IsInteractableInFront(interactable))
                        {
                            _interactables.Add(interactable);
                        }
                    }
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

        private bool IsInteractableInFront(IInteractable interactable)
        {
            Vector3 directionToInteractable = Vector3.Normalize(interactable.GetTransform().position - _interactionPoint.position);
            float dot = Vector3.Dot(_interactionPoint.forward, directionToInteractable);

            return dot >= _dotThreshold;
        }
    }
}