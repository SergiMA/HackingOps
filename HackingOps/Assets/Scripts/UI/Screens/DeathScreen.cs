using HackingOps.Characters.Common;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Utilities;
using UnityEngine;

namespace HackingOps.UI.Screens
{
    public class DeathScreen : MonoBehaviour, IEventObserver
    {
        [Header("UI elements")]
        [SerializeField] private CanvasGroup _deathScreen;

        [Header("Bindings - Conditions to show the screen")]
        [SerializeField] private CharacterId[] _characterIds;

        [Header("Settings - Scenes to go")]
        [SerializeField] private string _menuSceneName = "MenuScene";
        [SerializeField] private string _gameSceneName = "DemoScene";

        // Services
        private SceneLoader _sceneLoader;
        private IEventQueue _eventQueue;
        private CursorLocker _cursorLocker;

        private void Awake()
        {
            _sceneLoader = ServiceLocator.Instance.GetService<SceneLoader>();
            _eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            _cursorLocker = ServiceLocator.Instance.GetService<CursorLocker>();
        }

        private void Start() => _eventQueue.Subscribe(EventIds.CharacterDied, this);

        private bool ValidateId(string idToCheck)
        {
            foreach (CharacterId id in _characterIds)
            {
                if (idToCheck == id.Value) return true;
            }
            return false;
        }

        private void LoadScene(string sceneName) => _sceneLoader.Load(sceneName);

        private void Show()
        {
            _deathScreen.alpha = 1.0f;
            _deathScreen.blocksRaycasts = true;
        }

        public void OnTryAgainPressed() => LoadScene(_gameSceneName);
        public void OnExitPressed() => LoadScene(_menuSceneName);

        #region IEventObserver implementation
        void IEventObserver.Process(EventData eventData)
        {
            if (eventData.EventId != EventIds.CharacterDied) return;

            CharacterDiedData data = eventData as CharacterDiedData;
            if (ValidateId(data.Id) == false) return;

            Show();
            _cursorLocker.FreeCursor();
        }
        #endregion
    }
}