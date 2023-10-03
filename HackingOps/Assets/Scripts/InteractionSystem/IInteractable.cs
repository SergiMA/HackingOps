using UnityEngine;

namespace HackingOps.InteractionSystem
{
    public interface IInteractable
    {
        void Interact(Interactor interactor);
        Transform GetTransform();
        bool CanBeInteracted();
        string GetInteractableText();
        void EnableInteractions();
        void DisableInteractions();
    }
}