using HackingOps.Common.Services;
using HackingOps.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace HackingOps.UI
{
    public class CreditsManager : MonoBehaviour
    {
        [SerializeField] private string _sceneName = "MenuScene";

        private SceneLoader _sceneLoader;

        private void Awake()
        {
            _sceneLoader = ServiceLocator.Instance.GetService<SceneLoader>();
        }

        private void Update()
        {
            if (Keyboard.current != null)
            {
                if (Keyboard.current.anyKey.wasPressedThisFrame)
                    ChangeScene();
            }

            if (Gamepad.current != null)
            {
                // https://forum.unity.com/threads/how-can-i-know-that-any-button-on-gamepad-is-pressed.757322/
                if (Gamepad.current.allControls.Any((x => x is ButtonControl button && x.IsPressed() && !x.synthetic)))
                    ChangeScene();
            }
        }

        public void ChangeScene() => _sceneLoader.Load(_sceneName);
        public void ChangeScene(string sceneName) => _sceneLoader.Load(sceneName);
    }
}