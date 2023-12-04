using HackingOps.Common.Core.Managers;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public class GameManagerInstaller : Installer
    {
        [SerializeField] private GameManager _gameManager;

        public override void Install(ServiceLocator serviceLocator)
        {
            serviceLocator.RegisterService(_gameManager);
        }

        public override void Uninstall(ServiceLocator serviceLocator)
        {
            serviceLocator.DeregisterService(_gameManager);
        }
    }
}