using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    [DefaultExecutionOrder(-2)]
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

        protected abstract void InternalStart();
        
        private void InstallDependencies()
        {
            foreach (Installer installer in _installers)
                installer.Install(ServiceLocator.Instance);

            InternalInstallDependencies();
        }

        protected abstract void InternalInstallDependencies();
    }
}