using HackingOps.Common.Services;
using HackingOps.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.UI
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private CanvasGroup _mainMenuScreen;
        [SerializeField] private CanvasGroup _settingsScreen;
        [SerializeField] private CanvasGroup _creditsScreen;

        private SceneLoader _sceneLoader;
        private List<CanvasGroup> _screens = new();

        private void Start()
        {
            _sceneLoader = ServiceLocator.Instance.GetService<SceneLoader>();
            AddScreensToList();
        }

        private void AddScreensToList()
        {
            _screens.Add(_mainMenuScreen);
            _screens.Add(_settingsScreen);
            //_screens.Add(_creditsScreen);
        }

        private void ChangeToScreen(CanvasGroup screen)
        {
            CloseAllScreens();
            ShowScreen(screen);
        }

        private void CloseAllScreens()
        {
            foreach (CanvasGroup screen in _screens)
            {
                screen.alpha = 0f;
                screen.blocksRaycasts = false;
            }
        }

        private void ShowScreen(CanvasGroup screen)
        {
            screen.alpha = 1f;
            screen.blocksRaycasts = true;
        }

        public void OnPlayButtonPressed() => _sceneLoader.LoadNext();

        public void OnSettingsButtonPressed() => ChangeToScreen(_settingsScreen);

        public void OnCreditsButtonPressed() => ChangeToScreen(_creditsScreen);

        public void OnExitButtonPressed() => Utils.ExitGame();

        public void OnBackToMainMenuButtonPressed() => ChangeToScreen(_mainMenuScreen);
    }
}