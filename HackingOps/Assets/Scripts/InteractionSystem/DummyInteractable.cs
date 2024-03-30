using System;
using System.Collections;
using UnityEngine;

namespace HackingOps.InteractionSystem
{
    public class DummyInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool _canBeInteracted;

        private string _interactionText = "Interact";

        public event Action OnReceiveCandidateNotification;

        #region IInteractable implementation
        public bool CanBeInteracted() => _canBeInteracted;
        public void DisableInteractions() { }
        public void EnableInteractions() { }
        public string GetInteractableText() => _interactionText;
        public Transform GetTransform() => transform;

        public void Interact(Interactor interactor)
        {
            if (interactor == null) return;
            if (!_canBeInteracted) return;

            Debug.Log($"{name} received an interaction");
        }

        public void ReceiveCandidateNotification()
        {
            OnReceiveCandidateNotification?.Invoke();
        }
        #endregion
    }
}