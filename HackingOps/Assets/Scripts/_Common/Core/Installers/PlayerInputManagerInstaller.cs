using HackingOps.Common.Services;
using HackingOps.Input;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    [DefaultExecutionOrder(-10)]
    public class PlayerInputManagerInstaller : Installer
    {
        [SerializeField] private PlayerInputManager _playerInputManager;

        public override void Install(ServiceLocator serviceLocator)
        {
            serviceLocator.RegisterService(_playerInputManager);
        }

        public override void Uninstall(ServiceLocator serviceLocator)
        {
            serviceLocator.DeregisterService(_playerInputManager);
        }
    }
}