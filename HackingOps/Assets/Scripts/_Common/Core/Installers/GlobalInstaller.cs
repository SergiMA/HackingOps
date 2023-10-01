using HackingOps.Common.CommandSystem;
using HackingOps.Common.Services;

namespace HackingOps.Common.Core.Installers
{
    public class GlobalInstaller : GeneralInstaller
    {
        protected override void InternalInstallDependencies()
        {
            ServiceLocator.Instance.RegisterService(CommandQueue.Instance);
        }

        protected override void InternalStart()
        {
            
        }
    }
}