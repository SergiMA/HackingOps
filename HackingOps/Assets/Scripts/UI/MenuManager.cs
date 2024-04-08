using HackingOps.Common.Services;
using HackingOps.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.UI
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private List<CanvasGroup> _screens = new();

        [Header("Scene names")]
        [SerializeField] private string _creditsScene = "CreditsScene";

        private SceneLoader _sceneLoader;

        private void Start()
        {
            _sceneLoader = ServiceLocator.Instance.GetService<SceneLoader>();
        }

        public void OnTabButtonPressed(CanvasGroup attachedScreen)
        {
            UserInterfaceUtils.ChangeToScreen(_screens, attachedScreen);
        }

        public void OnPlayButtonPressed() => _sceneLoader.LoadNext();
        public void OnCreditsButtonPressed() => _sceneLoader.Load(_creditsScene);
        public void OnExitButtonPressed() => Utils.ExitGame();
    }
}