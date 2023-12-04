using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HackingOps.Utilities
{
    public class SceneLoader : MonoBehaviour
    {
        [Tooltip("Optional. Assign the reference if you have a loading screen")]
        [SerializeField] private CanvasGroup _loadingScreen;

        private float _loadingProgress;

        public void Load(string sceneName)
        {
            StartCoroutine(LoadSceneAsynchronously(sceneName));
        }

        public void LoadNext()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (sceneIndex == SceneManager.sceneCountInBuildSettings) sceneIndex = 0;

            StartCoroutine(LoadSceneAsynchronously(sceneIndex));
        }

        IEnumerator LoadSceneAsynchronously(string sceneName)
        {
            if (_loadingScreen) _loadingScreen.alpha = 1f;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            while (!operation.isDone)
            {
                _loadingProgress = Mathf.Clamp01(operation.progress / 0.9f);

                yield return null;
            }

            if (_loadingScreen) _loadingScreen.alpha = 0f;
        }

        IEnumerator LoadSceneAsynchronously(int sceneIndex)
        {
            if (_loadingScreen) _loadingScreen.alpha = 1f;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

            while (!operation.isDone)
            {
                _loadingProgress = Mathf.Clamp01(operation.progress / 0.9f);

                yield return null;
            }

            if (_loadingScreen) _loadingScreen.alpha = 0f;
        }

        /// <returns>Returns the loading scene progress divided by 0.9 and clamped between 0 and 1</returns>
        public float GetLoadingProgress() => _loadingProgress;
    }
}