using UnityEngine;
using UnityEngine.UI;

namespace HackingOps.Common.Settings.Video
{
    public class FullscreenSetting : MonoBehaviour, ISetting, ISaveable, IToggleSetting
    {
        [SerializeField] private Toggle _toggle;

        [SerializeField] private bool _applyOnChange;

        [SerializeField] private bool _defaultValue;

        public bool PreviousValue { get; private set; }
        public bool BlueprintValue { get; private set; }
        public bool CurrentValue { get; private set; }

        void Start ()
        {
            Recover();
            ChangeToggleValue(CurrentValue);
        }

        private void ChangeToggleValue(bool value) => _toggle.isOn = value;

        #region IToggleSetting implementation
        public void OnValueChanged(bool value)
        {
            SetBlueprintValue(value);

            if (_applyOnChange) ApplyAsBlueprint();
        }
        #endregion

        #region ISetting implementation
        public void Apply()
        {
            // Fullscreen is applied by ResolutionSetting.
            //Screen.fullScreen = CurrentValue;
        }

        public void ApplyAsBlueprint()
        {
            // Fullscreen is applied by ResolutionSetting.
            //Screen.fullScreen = BlueprintValue;
        }

        public void ApplyBlueprint()
        {
            CurrentValue = BlueprintValue;
            PreviousValue = BlueprintValue;

            Apply();
        }

        public void ApplyPrevious()
        {
            DiscardValue();
            Apply();
        }

        public void DiscardValue()
        {
            CurrentValue = PreviousValue;
            BlueprintValue = PreviousValue;

            ChangeToggleValue(PreviousValue);
        }

        public void ResetValue()
        {
            CurrentValue = _defaultValue;
            PreviousValue = _defaultValue;
            BlueprintValue = _defaultValue;

            ChangeToggleValue(_defaultValue);

            if (_applyOnChange) ApplyAsBlueprint();
        }

        public void SetBlueprintValue<T>(T blueprintValue)
        {
            BlueprintValue = (bool)(object)blueprintValue;

            if (!_applyOnChange) ApplyAsBlueprint();
        }
        #endregion

        #region ISaveable implementation
        public void Recover()
        {
            CurrentValue = PlayerPrefs.GetInt("Fullscreen", _defaultValue ? 1 : 0) == 1;
            PreviousValue = CurrentValue;
            BlueprintValue = CurrentValue;

            ChangeToggleValue(CurrentValue);
        }

        public void Save()
        {
            PlayerPrefs.SetInt("Fullscreen", CurrentValue ? 1 : 0);
        }
        #endregion
    }
}