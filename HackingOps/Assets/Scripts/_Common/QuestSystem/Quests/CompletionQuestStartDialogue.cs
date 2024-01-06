using DG.Tweening;
using HackingOps.Screens.UI;
using HackingOps.Screens.UI.ChatScreenElements;
using UnityEngine;

namespace HackingOps.Common.QuestSystem.Quests
{
    public class CompletionQuestStartDialogue : MonoBehaviour, ICompletionQuestAction
    {
        [Header("Component bindings")]
        [SerializeField] private Quest _quest;
        [SerializeField] private LaptopScreen _laptopScreen;

        [Header("Data bindings")]
        [SerializeField] private DialogueId _dialogueId;

        [Tooltip("Delay in seconds to make the laptop screen display the dialogue")]
        [SerializeField] private float _delay;

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
            DOVirtual.DelayedCall(_delay, () => _laptopScreen.LoadDialogue(_dialogueId));
        }
    }
}