using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.InteractionSystem
{
    public class ButtonInteractable : MonoBehaviour, IInteractable
    {
        public UnityEvent OnInteracted;

        [SerializeField] private string _interactionText = "interaction_elevator_control_panel";

        private bool _isInteractable = true;

        private void ProcessInteraction()
        {
            if (!_isInteractable) return;

            OnInteracted.Invoke();
        }

        #region IInteractable implementation
        public bool CanBeInteracted() => _isInteractable;

        public void EnableInteractions() => _isInteractable = true;

        public void DisableInteractions() => _isInteractable = false;

        public string GetInteractableText() => _interactionText;

        public Transform GetTransform() => transform;

        public void Interact(Interactor interactor)
        {
            ProcessInteraction();
        }
        #endregion
    }
}