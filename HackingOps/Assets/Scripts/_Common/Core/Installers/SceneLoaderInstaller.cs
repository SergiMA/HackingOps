using HackingOps.Common.Services;
using HackingOps.Utilities;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public class SceneLoaderInstaller : Installer
    {
        [SerializeField] private SceneLoader _sceneLoader;

        public override void Install(ServiceLocator serviceLocator)
        {
            ServiceLocator.Instance.RegisterService(_sceneLoader);
        }

        public override void Uninstall(ServiceLocator serviceLocator)
        {
            ServiceLocator.Instance.DeregisterService(_sceneLoader);
        }
    }
}