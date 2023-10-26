using UnityEngine;

namespace HackingOps.Common.QuestSystem.Quests
{
    public class CompletionQuestObjectStateChanger : MonoBehaviour, ICompletionQuestAction
    {
        [SerializeField] private Quest _quest;
        [SerializeField] private GameObject[] _objectsToDeactivate;
        [SerializeField] private GameObject[] _objectsToActivate;

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
            foreach (GameObject target in _objectsToDeactivate)
                target.SetActive(false);

            foreach (GameObject target in _objectsToActivate)
                target.SetActive(true);
        }
    }
}