using HackingOps.InteractionSystem;
using UnityEngine;

namespace HackingOps.Common.Events
{
    public class EventData
    {
        public readonly EventIds EventId;

        public EventData(EventIds eventId)
        {
            EventId = eventId;
        }
    }

    public class InteractionEventData : EventData
    {
        public readonly IInteractable Interactable;
        public readonly Transform InteractableTransform;

        public InteractionEventData(IInteractable interactable, Transform interactableTransform) : base(EventIds.Interaction)
        {
            Interactable = interactable;
            InteractableTransform = interactableTransform;
        }
    }
}