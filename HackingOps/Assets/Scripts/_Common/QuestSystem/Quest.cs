using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HackingOps.Common.Services;
using HackingOps.Common.Events;
using System;

namespace HackingOps.Common.QuestSystem
{
    public class Quest : MonoBehaviour
    {
        public event Action OnQuestStarted;
        public event Action OnQuestCompleted;

        [SerializeField] private List<Goal> _goals;
        [SerializeField] private string _questName;
        [SerializeField] private string _description;

        private bool _isCompleted;

        private void OnEnable()
        {
            foreach (Goal goal in _goals)
            {
                goal.OnGoalCompleted += CheckGoals;
                goal.Init();
                OnQuestStarted?.Invoke();
            }
        }

        private void OnDisable()
        {
            foreach (Goal goal in _goals)
                goal.OnGoalCompleted -= CheckGoals;
        }

        public void CheckGoals()
        {
            _isCompleted = _goals.All(g => g.IsCompleted);

            if (_isCompleted)
            {
                ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new QuestCompletedData(this));
                OnQuestCompleted?.Invoke();
            }
        }
    }
}