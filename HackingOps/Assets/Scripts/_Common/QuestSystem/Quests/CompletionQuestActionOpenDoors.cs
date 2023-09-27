using HackingOps.Doors.AnimatedDoors;
using UnityEngine;

namespace HackingOps.Common.QuestSystem.Quests
{
    public class CompletionQuestActionOpenDoors : MonoBehaviour, ICompletionQuestAction
    {
        [SerializeField] private Quest _quest;
        [SerializeField] private AnimatedDoor[] _doors;

        private void OnEnable()
        {
            _quest.OnQuestCompleted += ExecuteAction;
        }

        private void OnDisable()
        {
            _quest.OnQuestCompleted -= ExecuteAction;
        }

        public void ExecuteAction()
        {
            foreach (AnimatedDoor door in _doors)
                door.Open();
        }
    }
}