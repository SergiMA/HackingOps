using Cinemachine;
using DG.Tweening;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.CutsceneSystem
{
    public class CutsceneCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain _cinemachineBrain;
        [SerializeField] private int _maxCutsceneCameraPriority = 1000;
        [SerializeField] float _cameraTransitionBlendDurationInSeconds = 3f;

        private CinemachineVirtualCamera _cutsceneCamera;

        private ServiceLocator _serviceLocator = ServiceLocator.Instance;

        private float _cinemachineDefaultBlend;

        private void Start()
        {
            _cinemachineDefaultBlend = _cinemachineBrain.m_DefaultBlend.m_Time;
        }

        private void SetCameraDefaultBlendTime(float duration)
        {
            _cinemachineBrain.m_DefaultBlend.m_Time = duration;
        }

        public void SetCutsceneCamera(CinemachineVirtualCamera cutsceneCamera)
        {
            SetCameraDefaultBlendTime(_cameraTransitionBlendDurationInSeconds);
            _cutsceneCamera = cutsceneCamera;
            _cutsceneCamera.Priority = _maxCutsceneCameraPriority;
        }

        public void SetCutsceneCamera(CinemachineVirtualCamera cutsceneCamera, float cameraTransitionBlendDurationInSeconds)
        {
            SetCameraDefaultBlendTime(cameraTransitionBlendDurationInSeconds);
            _cutsceneCamera = cutsceneCamera;
            _cutsceneCamera.Priority = _maxCutsceneCameraPriority;
        }

        public void UnsetCutsceneCamera()
        {
            if (_cutsceneCamera == null)
                return;

            SetCameraDefaultBlendTime(_cameraTransitionBlendDurationInSeconds);
            _cutsceneCamera.Priority = 0;
            _cutsceneCamera = null;
            DOVirtual.DelayedCall(_cameraTransitionBlendDurationInSeconds, () =>
            {
                _cinemachineBrain.m_DefaultBlend.m_Time = _cinemachineDefaultBlend;
            });
        }
    }
}