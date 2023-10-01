using HackingOps.CutsceneSystem;
using UnityEngine;

namespace HackingOps.Common.QuestSystem.Quests
{
    public class CompletionQuestStartCutscene : MonoBehaviour, ICompletionQuestAction
    {
        [SerializeField] private Quest _quest;
        [SerializeField] private CutsceneSetup _cutsceneSetup;

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
            _cutsceneSetup.Play();
        }
    }
}