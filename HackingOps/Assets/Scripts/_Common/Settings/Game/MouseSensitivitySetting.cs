using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace HackingOps.Common.Settings.Game
{
    public class MouseSensitivitySetting : MonoBehaviour, ISetting, ISaveable, ISliderSetting
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private CinemachineFreeLook _freeLookCamera;

        [SerializeField] private bool _applyOnChange;

        [SerializeField] private float _defaultValue = 1f;

        private float _previousValue;
        private float _blueprintValue;
        private float _currentValue;

        private float _originalXAxisMaxSpeed;
        private float _originalYAxisMaxSpeed;

        private void Awake()
        {
            GetOriginalValues();
        }

        private void Start()
        {
            Recover();
            ChangeSliderValue(_currentValue);
        }

        private void ChangeSliderValue(float value)
        {
            _slider.value = value;
        }

        private void GetOriginalValues()
        {
            if (!_freeLookCamera) return;

            _originalXAxisMaxSpeed = _freeLookCamera.m_XAxis.m_MaxSpeed;
            _originalYAxisMaxSpeed = _freeLookCamera.m_YAxis.m_MaxSpeed;
        }

        #region ISliderSetting implementation
        public void OnValueChanged(float value)
        {
            SetBlueprintValue(value);

            if (_applyOnChange) ApplyAsBlueprint();
        }
        #endregion

        #region ISetting implementation
        public void Apply()
        {
            if (!_freeLookCamera) return;

            _freeLookCamera.m_XAxis.m_MaxSpeed = _originalXAxisMaxSpeed * _currentValue;
            _freeLookCamera.m_YAxis.m_MaxSpeed = _originalYAxisMaxSpeed * _currentValue;
        }

        public void ApplyAsBlueprint()
        {
            if (!_freeLookCamera) return;

            _freeLookCamera.m_XAxis.m_MaxSpeed = _originalXAxisMaxSpeed * _blueprintValue;
            _freeLookCamera.m_YAxis.m_MaxSpeed = _originalYAxisMaxSpeed * _blueprintValue;
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

            ChangeSliderValue(_previousValue);

            if (_applyOnChange) ApplyAsBlueprint();
        }

        public void ResetValue()
        {
            _currentValue = _defaultValue;
            _previousValue = _defaultValue;
            _blueprintValue = _defaultValue;

            ChangeSliderValue(_defaultValue);
        }

        public void SetBlueprintValue<T>(T blueprintValue)
        {
            _blueprintValue = (float)(object)blueprintValue;

            if (_applyOnChange) ApplyAsBlueprint();
        }
        #endregion


        #region ISaveable implementation
        public void Save()
        {
            PlayerPrefs.SetFloat("MouseSensitivity", _currentValue);
        }

        public void Recover()
        {
            _currentValue = PlayerPrefs.GetFloat("MouseSensitivity", _defaultValue);
            _previousValue = _currentValue;
            _blueprintValue = _currentValue;

            ChangeSliderValue(_currentValue);
        }
        #endregion
    }
}