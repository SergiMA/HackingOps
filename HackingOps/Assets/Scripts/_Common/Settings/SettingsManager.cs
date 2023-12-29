using UnityEngine;

namespace HackingOps.Common.Settings
{
    public class SettingsManager : MonoBehaviour
    {
        private ISetting[] _settings;
        private ISaveable[] _saveable;

        private void Awake()
        {
            SearchSettings();
            SearchSaveable();
        }

        private void Start()
        {
            foreach (ISaveable saveable in _saveable)
                saveable.Recover();

            foreach (ISetting setting in _settings)
                setting.Apply();
        }

        private void SearchSettings() => _settings = GetComponentsInChildren<ISetting>();
        private void SearchSaveable() => _saveable = GetComponentsInChildren<ISaveable>();

        public void ApplySettings()
        {
            foreach (ISetting setting in _settings)
                setting.ApplyBlueprint();

            foreach (ISaveable saveable in _saveable)
                saveable.Save();
        }

        public void DiscardSettings()
        {
            foreach (ISetting setting in _settings)
                setting.ApplyPrevious();

            foreach (ISaveable saveable in _saveable)
                saveable.Save();
        }

        public void ResetSettings()
        {
            foreach (ISetting setting in _settings)
            {
                setting.ResetValue();
                setting.Apply();

                foreach (ISaveable saveable in _saveable)
                    saveable.Save();
            }
        }
    }
}