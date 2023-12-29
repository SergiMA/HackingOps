using HackingOps.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace HackingOps.Common.Settings.Video
{
    public class AntialiasingSetting : MonoBehaviour, ISetting, ISaveable
    {
        [SerializeField] private Selector _selector;
        [SerializeField] private bool _applyOnChange;

        [SerializeField] HDAdditionalCameraData.AntialiasingMode _defaultAntialiasing;

        private List<HDAdditionalCameraData.AntialiasingMode> _antialiasings = new();
        private HDAdditionalCameraData _cameraData;

        private int _defaultAntialiasingIndex;
        private int _previousAntialiasingIndex;
        private int _currentAntialiasingIndex;
        private int _blueprintAntialiasingIndex;

        private void Awake()
        {
            GetAntialiasings();
            SendToSelector();
        }

        private void OnEnable()
        {
            GrabComponents();
            SubscribeToSelectorEvents();
        }

        private void OnDisable()
        {
            UnsubcribeFromSelectorEvents();
        }

        private void Start()
        {
            _defaultAntialiasingIndex = GetAntialiasingIndex(_defaultAntialiasing);
            MoveSelectorTo(_currentAntialiasingIndex);
        }

        private void SubscribeToSelectorEvents()
        {
            _selector.OnChanged += SetBlueprintValue;
        }

        private void UnsubcribeFromSelectorEvents()
        {
            _selector.OnChanged -= SetBlueprintValue;
        }

        private void GrabComponents()
        {
            _cameraData = Camera.main.GetComponent<HDAdditionalCameraData>();
        }

        private void GetAntialiasings()
        {
            foreach (HDAdditionalCameraData.AntialiasingMode antialiasing in Enum.GetValues(typeof(HDAdditionalCameraData.AntialiasingMode)))
            {
                _antialiasings.Add(antialiasing);
            }
        }

        private int GetAntialiasingIndex(HDAdditionalCameraData.AntialiasingMode antialiasing)
        {
            for (int i = 0;  i < _antialiasings.Count; i++)
            {
                if (_antialiasings[i] == antialiasing)
                {
                    return i;
                }
            }

            throw new Exception($"The antialiasing {antialiasing} could not be found in the antialiasing list");
        }

        private void SendToSelector()
        {
            foreach (HDAdditionalCameraData.AntialiasingMode antialiasing in _antialiasings)
            {
                _selector.Add(antialiasing.ToString());
            }
        }

        private void MoveSelectorTo(int index) => _selector.MoveTo(index);

        #region ISetting implementation
        public void Apply()
        {
            _cameraData.antialiasing = _antialiasings[_currentAntialiasingIndex];
        }

        public void ApplyAsBlueprint()
        {
            _cameraData.antialiasing = _antialiasings[_blueprintAntialiasingIndex];
        }

        public void ApplyBlueprint()
        {
            _currentAntialiasingIndex = _blueprintAntialiasingIndex;
            _previousAntialiasingIndex = _blueprintAntialiasingIndex;

            Apply();
        }

        public void ApplyPrevious()
        {
            DiscardValue();
            Apply();
        }

        public void DiscardValue()
        {
            _currentAntialiasingIndex = _previousAntialiasingIndex;
            _blueprintAntialiasingIndex = _previousAntialiasingIndex;

            MoveSelectorTo(_previousAntialiasingIndex);
        }

        public void ResetValue()
        {
            _currentAntialiasingIndex = _defaultAntialiasingIndex;
            _previousAntialiasingIndex = _defaultAntialiasingIndex;
            _blueprintAntialiasingIndex = _defaultAntialiasingIndex;

            MoveSelectorTo(_defaultAntialiasingIndex);

            if (_applyOnChange) ApplyAsBlueprint();
        }

        public void SetBlueprintValue<T>(T blueprintValue)
        {
            _blueprintAntialiasingIndex = (int)(object)blueprintValue;

            if (!_applyOnChange) ApplyAsBlueprint();
        }
        #endregion

        #region ISaveable implementation
        public void Recover()
        {
            _currentAntialiasingIndex = PlayerPrefs.GetInt("Antialiasing", _defaultAntialiasingIndex);
            _previousAntialiasingIndex = _currentAntialiasingIndex;
            _blueprintAntialiasingIndex = _currentAntialiasingIndex;

            MoveSelectorTo(_currentAntialiasingIndex);
        }

        public void Save()
        {
            PlayerPrefs.SetInt("Antialiasing", _currentAntialiasingIndex);
        }
        #endregion
    }
}