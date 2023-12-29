using HackingOps.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace HackingOps.Common.Settings.Game
{
    public class LanguageSetting : MonoBehaviour, ISetting, ISaveable
    {
        [SerializeField] private Selector _selector;
        [SerializeField] private bool _applyOnChange;
        [SerializeField] private int _defaultLanguageIndex;

        List<Locale> _languages = new();

        private int _previousLanguageIndex;
        private int _blueprintLanguageIndex;
        private int _currentLanguageIndex;

        private void OnEnable()
        {
            SubscribeToSelectorEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromSelectorEvents();
        }

        private void Start()
        {
            var init = LocalizationSettings.InitializationOperation;
            init.Completed += _ =>
            {
                GetLanguages();
                SendLanguagesToSelector();
                MoveSelectorTo(_currentLanguageIndex);
            };
        }

        private void SubscribeToSelectorEvents()
        {
            _selector.OnChanged += SetBlueprintValue;
        }

        private void UnsubscribeFromSelectorEvents()
        {
            _selector.OnChanged -= SetBlueprintValue;
        }

        private void GetLanguages()
        {
            _languages = LocalizationSettings.AvailableLocales.Locales;
        }

        private void SendLanguagesToSelector()
        {
            foreach (Locale language in _languages)
            {
                _selector.Add(language.Identifier.CultureInfo.DisplayName);
            }
        }

        private void MoveSelectorTo(int index)
        {
            _selector.MoveTo(index);
        }

        #region ISetting implementation
        public void ResetValue()
        {
            _currentLanguageIndex = _defaultLanguageIndex;
            _previousLanguageIndex = _defaultLanguageIndex;
            _blueprintLanguageIndex = _defaultLanguageIndex;

            MoveSelectorTo(_defaultLanguageIndex);

            if (_applyOnChange) ApplyAsBlueprint();
        }

        public void SetBlueprintValue<T>(T blueprintIndex)
        {
            _blueprintLanguageIndex = (int)(object)blueprintIndex;

            if (_applyOnChange) ApplyAsBlueprint();
        }

        public void ApplyPrevious()
        {
            DiscardValue();
            Apply();
        }

        public void ApplyBlueprint()
        {
            _currentLanguageIndex = _blueprintLanguageIndex;
            _previousLanguageIndex = _blueprintLanguageIndex;

            MoveSelectorTo(_blueprintLanguageIndex);

            Apply();
        }

        public void DiscardValue()
        {
            _currentLanguageIndex = _previousLanguageIndex;
            _blueprintLanguageIndex = _previousLanguageIndex;

            MoveSelectorTo(_previousLanguageIndex);
        }

        /// <summary>
        /// Start using the current language. Nothing will be applied until Localization is initialized
        /// </summary>
        public void Apply()
        {
            var init = LocalizationSettings.InitializationOperation;
            init.Completed += _ =>
            {
                if (_languages.Count == 0) GetLanguages();
                LocalizationSettings.SelectedLocale = _languages[_currentLanguageIndex];
            };
        }

        public void ApplyAsBlueprint()
        {
            LocalizationSettings.SelectedLocale = _languages[_blueprintLanguageIndex];
        }
        #endregion

        #region ISaveable implementation
        public void Save()
        {
            PlayerPrefs.SetInt("Language", _currentLanguageIndex);
        }

        public void Recover()
        {
            _currentLanguageIndex = PlayerPrefs.GetInt("Language", _defaultLanguageIndex);
            _previousLanguageIndex = _currentLanguageIndex;
            _blueprintLanguageIndex = _currentLanguageIndex;
        }
        #endregion
    }
}