using HackingOps.Audio;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace HackingOps.Common.Settings
{
    public class VolumeSetting : MonoBehaviour, ISetting, ISaveable, ISliderSetting
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private AudioMixer _audioMixer;

        [SerializeField] private AudioMixerGroupId _mixerGroupId;

        [SerializeField] private bool _applyOnChange;

        [Range(0.0001f, 1f)]
        [SerializeField] private float _defaultValue = 1f;

        private float _previousValue;
        private float _blueprintValue;
        private float _currentValue;

        private void Start()
        {
            Recover();
            ChangeSliderValue(_currentValue);
        }

        private void ChangeSliderValue(float value)
        {
            _slider.value = value;
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
            _audioMixer.SetFloat(_mixerGroupId.Value, Mathf.Log10(_currentValue) * 20);
        }

        public void ApplyAsBlueprint()
        {
            _audioMixer.SetFloat(_mixerGroupId.Value, Mathf.Log10(_blueprintValue) * 20);
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
            PlayerPrefs.SetFloat(_mixerGroupId.Value, _currentValue);
        }

        public void Recover()
        {
            _currentValue = PlayerPrefs.GetFloat(_mixerGroupId.Value, _defaultValue);
            _previousValue = _currentValue;
            _blueprintValue = _currentValue;
        }
        #endregion
    }
}