using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public class CutsceneCameraControllerInstaller : Installer
    {
        [SerializeField] private CutsceneCameraController _cutsceneCameraController;

        public override void Install(ServiceLocator serviceLocator)
        {
            serviceLocator.RegisterService(_cutsceneCameraController);
        }
    }
}