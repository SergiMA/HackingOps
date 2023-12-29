using HackingOps.UI;
using UnityEngine;

namespace HackingOps.Common.Settings.Video
{
    public class ResolutionSetting : MonoBehaviour, ISetting, ISaveable
    {
        [SerializeField] private Selector _selector;
        [SerializeField] private FullscreenSetting _fullscreenSetting;

        [SerializeField] private bool _applyOnChange;

        Resolution[] _resolutions;

        private int _defaultResolutionIndex;
        private int _previousResolutionIndex;
        private int _currentResolutionIndex;
        private int _blueprintResolutionIndex;

        private void Awake()
        {
            GetResolutions();
            SendResolutionsToSelector();
        }

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
            GetDefaultResolutionIndex();
            MoveSelectorTo(_currentResolutionIndex);
        }

        private void SubscribeToSelectorEvents()
        {
            _selector.OnChanged += SetBlueprintValue;
        }

        private void UnsubscribeFromSelectorEvents()
        {
            _selector.OnChanged -= SetBlueprintValue;
        }

        private void GetResolutions() => _resolutions = Screen.resolutions;

        private void SendResolutionsToSelector()
        {
            foreach (Resolution resolution in _resolutions)
            {
                string resolutionString = ConvertResolutionToString(resolution);
                _selector.Add(resolutionString);
            }
        }

        private void MoveSelectorTo(int index) => _selector.MoveTo(index);

        /// <summary>
        /// The default resolution index is the resolution detected of the active screen
        /// </summary>
        private void GetDefaultResolutionIndex()
        {
            for (int i = 0; i < _resolutions.Length; i++)
            {
                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    _defaultResolutionIndex = i;
                    return;
                }
            }
        }

        #region Resolution-string conversions
        /// <summary>
        /// The resolution format returned is <br></br><i>[resolution.width] x [resolution.height]</i>
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        private string ConvertResolutionToString(Resolution resolution)
        {
            return $"{resolution.width} x {resolution.height}";
        }

        /// <summary>
        /// The string to receive must have the following format <br></br><i>[resolution.width] x [resolution.height]</i>
        /// </summary>
        /// <param name="resolutionString"></param>
        /// <returns></returns>
        private Resolution ConvertStringToResolution(string resolutionString)
        {
            string[] resolutionSplit = resolutionString.Trim(' ').Split('x');

            foreach (Resolution resolution in _resolutions)
            {
                if (resolution.width == int.Parse(resolutionSplit[0]) && resolution.height == int.Parse(resolutionSplit[1]))
                {
                    return resolution;
                }
            }

            Debug.LogError($"Resolution {resolutionString} not found");
            return _resolutions[_defaultResolutionIndex];
        }
        #endregion

        #region ISetting implementation
        public void Apply()
        {
            Screen.SetResolution(
                _resolutions[_currentResolutionIndex].width,
                _resolutions[_currentResolutionIndex].height, 
                _fullscreenSetting.CurrentValue);
        }

        public void ApplyAsBlueprint()
        {
            Screen.SetResolution(
                _resolutions[_blueprintResolutionIndex].width,
                _resolutions[_blueprintResolutionIndex].height,
                _fullscreenSetting.BlueprintValue);
        }

        public void ApplyBlueprint()
        {
            _currentResolutionIndex = _blueprintResolutionIndex;
            _previousResolutionIndex = _blueprintResolutionIndex;

            Apply();
        }

        public void ApplyPrevious()
        {
            DiscardValue();
            Apply();
        }

        public void DiscardValue()
        {
            _currentResolutionIndex = _previousResolutionIndex;
            _blueprintResolutionIndex = _previousResolutionIndex;

            MoveSelectorTo(_previousResolutionIndex);

            if (_applyOnChange) ApplyAsBlueprint();
        }

        public void ResetValue()
        {
            _currentResolutionIndex = _defaultResolutionIndex;
            _previousResolutionIndex = _defaultResolutionIndex;
            _blueprintResolutionIndex = _defaultResolutionIndex;

            MoveSelectorTo(_defaultResolutionIndex);
        }

        public void SetBlueprintValue<T>(T blueprintValue)
        {
            _blueprintResolutionIndex = (int)(object)blueprintValue;

            if (!_applyOnChange) ApplyAsBlueprint();
        }
        #endregion

        #region ISaveable implementation
        public void Recover()
        {
            _currentResolutionIndex = PlayerPrefs.GetInt("Resolution", _defaultResolutionIndex);
            _previousResolutionIndex = _currentResolutionIndex;
            _blueprintResolutionIndex = _currentResolutionIndex;

            MoveSelectorTo(_currentResolutionIndex);
        }

        public void Save()
        {
            PlayerPrefs.SetInt("Resolution", _currentResolutionIndex);
        }
        #endregion
    }
}