using HackingOps.Common.Services;
using HackingOps.Utilities;
using UnityEngine;

namespace HackingOps.Common.QuestSystem.Quests
{
    public class CompletionQuestChangeScene : MonoBehaviour, ICompletionQuestAction
    {
        [SerializeField] private Quest _quest;
        [SerializeField] private string _sceneName;

        private void OnEnable() => _quest.OnQuestCompleted += ExecuteAction;

        private void OnDisable() => _quest.OnQuestCompleted -= ExecuteAction;

        public void ExecuteAction()
        {
            ServiceLocator.Instance.GetService<SceneLoader>().Load(_sceneName);
        }
    }
}