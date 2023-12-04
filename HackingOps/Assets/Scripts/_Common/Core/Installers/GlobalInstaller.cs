using HackingOps.Common.CommandSystem;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    [DefaultExecutionOrder(-10)]
    public class GlobalInstaller : GeneralInstaller
    {
        protected override void InternalInstallDependencies()
        {
            ServiceLocator.Instance.RegisterService(CommandQueue.Instance);
        }

        protected override void InternalUninstallDependencies()
        {
            ServiceLocator.Instance.DeregisterService(CommandQueue.Instance);
        }

        protected override void InternalStart()
        {
            
        }
    }
}