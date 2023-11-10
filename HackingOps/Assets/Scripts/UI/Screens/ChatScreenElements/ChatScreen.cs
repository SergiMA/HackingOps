using DG.Tweening;
using UnityEngine;

namespace HackingOps.Screens.UI.ChatScreenElements
{
    public class ChatScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _chatScreenCanvasGroup;
        [SerializeField] private CanvasGroup _continueButtonCanvasGroup;
        [SerializeField] private CanvasGroup _finishButtonCanvasGroup;

        [SerializeField] private DialogueId _dialogueId;

        [Header("UI elements animations settings")]
        [SerializeField] private float _progressiveHideDurationInSeconds = 1f;
        [SerializeField] private float _progressiveShowDurationInSeconds = 1f;
        [SerializeField] private float _delayToStartSwapElements = 1f;

        [SerializeField] private float _fadingChatScreenDurationInSeconds = 0.3f;

        [SerializeField] private Dialogue _dialogue;
        private string _currentDialogueId;

        [Header("Debug")]
        [SerializeField] private bool _debugShowNextMessage;

        private void OnValidate()
        {
            if (_debugShowNextMessage)
            {
                _debugShowNextMessage = false;
                Continue();
            }
        }

        private void OnEnable()
        {
            _dialogue.OnLastMessageShown += OnLastMessageShown;
        }

        private void OnDisable()
        {
            _dialogue.OnLastMessageShown -= OnLastMessageShown;
        }

        private void Start()
        {
            HideUIElementUsingCanvasGroup(_finishButtonCanvasGroup);
            HideUIElementUsingCanvasGroup(_continueButtonCanvasGroup);
        }

        private void OnLastMessageShown()
        {
            HideUIElementUsingCanvasGroup(_continueButtonCanvasGroup, _progressiveHideDurationInSeconds, 0f);
            ShowUIElementUsingCanvasGroup(_finishButtonCanvasGroup, _progressiveShowDurationInSeconds, _delayToStartSwapElements);
        }

        public void LoadDialogue(DialogueId dialogueId)
        {
            _currentDialogueId = dialogueId.Value;

            ShowUIElementUsingCanvasGroup(_chatScreenCanvasGroup, _fadingChatScreenDurationInSeconds);
            ShowUIElementUsingCanvasGroup(_continueButtonCanvasGroup, _fadingChatScreenDurationInSeconds);
        }

        public void Continue()
        {
            _dialogue.ShowNextMessage();
        }

        public void Close()
        {
            DOVirtual.Float(_chatScreenCanvasGroup.alpha, 0f, _fadingChatScreenDurationInSeconds, (alpha) =>
            {
                _chatScreenCanvasGroup.alpha = alpha;
            }).OnComplete(() =>
            {
                _dialogue.HideAllMessages();
                HideUIElementUsingCanvasGroup(_finishButtonCanvasGroup, 0f);
                HideUIElementUsingCanvasGroup(_continueButtonCanvasGroup, 0f);
            });

        }

        public void ShowFinishButton()
        {
            ShowUIElementUsingCanvasGroup(_finishButtonCanvasGroup);
        }

        public void ShowUIElementUsingCanvasGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        public void ShowUIElementUsingCanvasGroup(CanvasGroup canvasGroup, float duration, float delay = 0)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                DOVirtual.Float(canvasGroup.alpha, 1f, duration, (alpha) =>
                {
                    canvasGroup.alpha = alpha;
                }).OnComplete(() => canvasGroup.blocksRaycasts = true);
            });
        }

        public void HideUIElementUsingCanvasGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        public void HideUIElementUsingCanvasGroup(CanvasGroup canvasGroup, float duration, float delay = 0)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                canvasGroup.blocksRaycasts = false;

                DOVirtual.Float(canvasGroup.alpha, 0f, duration, (alpha) =>
                {
                    canvasGroup.alpha = alpha;
                });
            });
        }

        public void Show()
        {
            ShowUIElementUsingCanvasGroup(_chatScreenCanvasGroup);
            ShowUIElementUsingCanvasGroup(_continueButtonCanvasGroup);
        }

        public void Hide()
        {
            HideUIElementUsingCanvasGroup(_chatScreenCanvasGroup);
        }

        public DialogueId GetDialogue() => _dialogueId;
    }
}