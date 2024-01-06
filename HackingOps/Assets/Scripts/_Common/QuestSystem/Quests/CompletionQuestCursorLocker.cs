using DG.Tweening;
using HackingOps.Common.Services;
using HackingOps.Utilities;
using UnityEngine;

namespace HackingOps.Common.QuestSystem.Quests
{
    public class CompletionQuestCursorLocker : MonoBehaviour, ICompletionQuestAction
    {
        [Header("Bindings")]
        [SerializeField] private Quest _quest;

        [Header("Settings")]
        [Tooltip("Delay in seconds to make switch to a specific CursorLockMode")]
        [SerializeField] private float _delay;

        [SerializeField] private CursorLockMode _cursorLockMode;

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
            DOVirtual.DelayedCall(_delay, () =>
            {
                CursorLocker cursorLocker = ServiceLocator.Instance.GetService<CursorLocker>();
                switch (_cursorLockMode)
                {
                    case CursorLockMode.None: cursorLocker.FreeCursor(); break;
                    case CursorLockMode.Locked: cursorLocker.LockCursor(); break;
                    case CursorLockMode.Confined: cursorLocker.ConfineCursor(); break;
                }
            });
        }
    }
}