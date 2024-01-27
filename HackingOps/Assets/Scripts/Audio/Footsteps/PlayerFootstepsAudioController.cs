using HackingOps.Characters.Common;
using HackingOps.Input;
using System.Collections;
using UnityEngine;

namespace HackingOps.Audio.Footsteps
{
    public class PlayerFootstepsAudioController : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private PlayerInputManager _inputManager;
        [SerializeField] private FootstepsPlayer[] _footstepsPlayers;

        [Header("Locomotion properties")]
        [SerializeField] private LocomotionPropertiesSO _standingProperties;
        [SerializeField] private LocomotionPropertiesSO _crouchingProperties;

        private bool _isCrouching;
        private bool _previousIsRunning;
        private bool _previousIsCrouching;

        private void Start() => ChangeFootstepsPlayersVolume(_standingProperties.VolumeNormal);

        private void Update()
        {
            bool isUsingWalkVolume = (HasSwitchedCrouching() || HasSwitchedRunning()) && !_inputManager.IsRunning && !_isCrouching;
            bool isUsingRunVolume = (HasSwitchedCrouching() || HasSwitchedRunning()) && _inputManager.IsRunning && !_isCrouching;
            bool isUsingCrouchingWalkVolume = (HasSwitchedCrouching() || HasSwitchedRunning()) && !_inputManager.IsRunning && _isCrouching;
            bool isUsingCrouchingRunVolume = (HasSwitchedCrouching() || HasSwitchedRunning()) && _inputManager.IsRunning && _isCrouching;

            if (isUsingWalkVolume) ChangeFootstepsPlayersVolume(_standingProperties.VolumeNormal);
            if (isUsingRunVolume) ChangeFootstepsPlayersVolume(_standingProperties.VolumeAccelerated);
            if (isUsingCrouchingWalkVolume) ChangeFootstepsPlayersVolume(_crouchingProperties.VolumeNormal);
            if (isUsingCrouchingRunVolume) ChangeFootstepsPlayersVolume(_crouchingProperties.VolumeAccelerated);

            SetPreviousValues();
        }

        private void SetPreviousValues()
        {
            _previousIsRunning = _inputManager.IsRunning;
            _previousIsCrouching = _isCrouching;
        }

        private bool HasSwitchedRunning() => _previousIsRunning != _inputManager.IsRunning;
        private bool HasSwitchedCrouching() => _previousIsCrouching != _isCrouching;
        
        private void ChangeFootstepsPlayersVolume(float volume)
        {
            foreach (FootstepsPlayer player in _footstepsPlayers)
                player.ChangeVolume(volume);
        }

        public void OnStartCrouching() => _isCrouching = true;  // Called by Unity Event
        public void OnStopCrouching() => _isCrouching = false;  // Called by Unity Event
    }
}