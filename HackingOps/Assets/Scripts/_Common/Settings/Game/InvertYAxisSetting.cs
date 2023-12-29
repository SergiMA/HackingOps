using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

namespace HackingOps.Common.Settings.Game
{
    public class InvertYAxisSetting : MonoBehaviour, ISetting, ISaveable, IToggleSetting
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private CinemachineFreeLook _freeLookCamera;

        [SerializeField] private bool _applyOnChange;

        [SerializeField] private bool _defaultValue;

        private bool _previousValue;
        private bool _blueprintValue;
        private bool _currentValue;

        private void Start()
        {
            Recover();
            ChangeToggleValue(_currentValue);
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
            if (!_freeLookCamera) return;

            _freeLookCamera.m_YAxis.m_InvertInput = _currentValue;
        }

        public void ApplyAsBlueprint()
        {
            if (!_freeLookCamera) return;

            _freeLookCamera.m_YAxis.m_InvertInput = _blueprintValue;
        }

        public void ApplyBlueprint()
        {
            _currentValue = _blueprintValue;
            _previousValue = _blueprintValue;

            Apply();
        }

        public void ApplyPrevious()
        {
            DiscardValue();
            Apply();
        }

        public void DiscardValue()
        {
            _currentValue = _previousValue;
            _blueprintValue = _previousValue;

            ChangeToggleValue(_previousValue);
        }

        public void ResetValue()
        {
            _currentValue = _defaultValue;
            _previousValue = _defaultValue;
            _blueprintValue = _defaultValue;

            ChangeToggleValue(_defaultValue);

            if (_applyOnChange) ApplyAsBlueprint();
        }

        public void SetBlueprintValue<T>(T blueprintValue)
        {
            _blueprintValue = (bool)(object)blueprintValue;

            if (_applyOnChange) ApplyAsBlueprint();
        }
        #endregion

        #region ISaveable implementation
        public void Save()
        {
            PlayerPrefs.SetInt("InvertYAxis", _currentValue ? 1 : 0);
        }

        public void Recover()
        {
            _currentValue = PlayerPrefs.GetInt("InvertYAxis", _defaultValue ? 1 : 0) == 1;
            _previousValue = _currentValue;
            _blueprintValue = _currentValue;

            ChangeToggleValue(_currentValue);
        }
        #endregion
    }
}