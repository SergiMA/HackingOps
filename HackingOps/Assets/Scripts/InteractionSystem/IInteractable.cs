using System;
using UnityEngine;

namespace HackingOps.InteractionSystem
{
    public interface IInteractable
    {
        event Action OnReceiveCandidateNotification;

        void Interact(Interactor interactor);
        Transform GetTransform();
        bool CanBeInteracted();
        string GetInteractableText();
        void EnableInteractions();
        void DisableInteractions();
        void ReceiveCandidateNotification();
    }
}