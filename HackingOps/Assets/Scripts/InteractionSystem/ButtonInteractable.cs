using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.InteractionSystem
{
    public class ButtonInteractable : MonoBehaviour, IInteractable, IEventObserver
    {
        public UnityEvent OnInteracted;

        [SerializeField] private string _interactionText = "interaction_elevator_control_panel";

        private bool _isInteractable = true;

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.Interaction, this);
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.Interaction, this);
        }

        #region IInteractable implementation
        public bool CanBeInteracted() => _isInteractable;

        public void EnableInteractions() => _isInteractable = true;

        public void DisableInteractions() => _isInteractable = false;

        public string GetInteractableText() => _interactionText;

        public Transform GetTransform() => transform;

        public void Interact()
        {
            if (!_isInteractable) return;

            OnInteracted.Invoke();
        }
        #endregion

        #region IEventObserver implementation
        public void Process(EventData eventData)
        {
            if (eventData.EventId != EventIds.Interaction) return;

            InteractionEventData data = eventData as InteractionEventData;
            if (data.InteractableTransform != transform) return;

            Interact();
        }
        #endregion
    }
}