using HackingOps.Common.Events;
using System;
using UnityEngine;

namespace HackingOps.Common.QuestSystem
{
    public abstract class Goal : MonoBehaviour, IEventObserver
    {
        public event Action OnGoalCompleted;

        [field: SerializeField] public string Description { get; protected set; }
        [field: SerializeField] public int RequiredAmount { get; protected set; } = 1;
        
        public int CurrentAmount { get; protected set; }
        public bool IsCompleted { get; protected set; }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        public virtual void Init()
        {
            SubscribeToEvents();
        }

        public virtual void SubscribeToEvents()
        {

        }

        public virtual void UnsubscribeFromEvents()
        {

        }

        public void Complete()
        {
            IsCompleted = true;
            OnGoalCompleted?.Invoke();
        }

        public void Evaluate()
        {
            if (CurrentAmount >= RequiredAmount)
                Complete();
        }

        public virtual void Process(EventData eventData)
        {
            
        }
    }
}