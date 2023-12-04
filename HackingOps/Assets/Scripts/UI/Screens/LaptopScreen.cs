using DG.Tweening;
using HackingOps.Common.CommandSystem;
using HackingOps.Common.CommandSystem.Commands;
using HackingOps.Common.Core.Managers;
using HackingOps.Common.Services;
using HackingOps.Screens.UI.ChatScreenElements;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace HackingOps.Screens.UI
{
    public class LaptopScreen : MonoBehaviour
    {
        public UnityEvent OnResumeButtonPressed;
        public UnityEvent OnMenuLoaded;

        [Header("Screens bindings")]
        [SerializeField] private CanvasGroup _pauseMenuScreen;
        [SerializeField] private CanvasGroup _chatScreenContainer;

        [Header("UI elements animations settings")]
        [SerializeField] private float _progressiveHideDurationInSeconds = 0.5f;
        [SerializeField] private float _progressiveShowDurationInSeconds = 0.5f;
        [SerializeField] private float _delayToStartSwapElements = 0.5f;

        private ChatScreen[] _chatScreens;
        private List<CanvasGroup> _mainScreens = new();

        private void Awake()
        {
            _chatScreens = GetComponentsInChildren<ChatScreen>(true);
        }

        private void Start()
        {
            _mainScreens.Add(_pauseMenuScreen);
            _mainScreens.Add(_chatScreenContainer);

            HideAllScreens();
        }

        private ChatScreen GetChatScreenByDialogue(DialogueId dialogueId) 
        {
            foreach (ChatScreen chatScreen in _chatScreens)
            {
                if (chatScreen.GetDialogue() == dialogueId)
                    return chatScreen;
            }

            return null;
        }

        public void OnCloseButtonPressed()
        {
            ServiceLocator.Instance.GetService<CommandQueue>().AddCommand(new StopUsingLaptopCommand());
            HideAllScreens(_progressiveHideDurationInSeconds);
            OnResumeButtonPressed.Invoke();
        }

        public void OnExitButtonPressed()
        {
            ServiceLocator.Instance.GetService<GameManager>().Exit();
        }

        public void OnLeaveHackingMode()
        {
            ServiceLocator.Instance.GetService<CommandQueue>().AddCommand(new StopUsingLaptopAfterHackingCommand());
        }

        public void LoadDialogue(DialogueId dialogueId)
        {
            HideAllScreens();
            ShowUIElementUsingCanvasGroup(_chatScreenContainer);

            ChatScreen chatScreen = GetChatScreenByDialogue(dialogueId);
            Assert.IsNotNull(chatScreen, $"<b>{name}</b> (LaptopScreen) can't get the <b>Chat Screen</b> for <b>{dialogueId}</b>. Make sure there's a Chat Screen with that ID");
            chatScreen.Show();
        }

        public void LoadMenu()
        {
            HideAllScreens();
            ShowUIElementUsingCanvasGroup(_pauseMenuScreen);

            DOVirtual.DelayedCall(_progressiveHideDurationInSeconds, () => { OnMenuLoaded.Invoke(); });
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

        public void HideAllScreens()
        {
            foreach (CanvasGroup screen in _mainScreens)
                HideUIElementUsingCanvasGroup(screen);
        }

        public void HideAllScreens(float duration)
        {
            foreach (CanvasGroup screen in _mainScreens)
                HideUIElementUsingCanvasGroup(screen, duration);
        }
    }
}