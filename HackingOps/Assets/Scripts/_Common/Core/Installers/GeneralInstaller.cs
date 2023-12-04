using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    [DefaultExecutionOrder(-10)]
    public abstract class GeneralInstaller : MonoBehaviour
    {
        [SerializeField] private Installer[] _installers;

        private void Awake()
        {
            InstallDependencies();
        }

        private void Start()
        {
            InternalStart();
        }

        private void OnDestroy()
        {
            UninstallDependencies();
            InternalUninstallDependencies();
        }

        protected abstract void InternalStart();

        private void InstallDependencies()
        {
            foreach (Installer installer in _installers)
                installer.Install(ServiceLocator.Instance);

            InternalInstallDependencies();
        }

        private void UninstallDependencies()
        {
            foreach (Installer installer in _installers)
                installer.Uninstall(ServiceLocator.Instance);
        }

        protected abstract void InternalInstallDependencies();
        protected abstract void InternalUninstallDependencies();
    }
}