using HackingOps.Common.Services;
using HackingOps.Utilities;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public class CursorLockerInstaller : Installer
    {
        [SerializeField] private CursorLocker _cursorLocker;

        public override void Install(ServiceLocator serviceLocator)
        {
            ServiceLocator.Instance.RegisterService(_cursorLocker);
        }

        public override void Uninstall(ServiceLocator serviceLocator)
        {
            serviceLocator.DeregisterService(_cursorLocker);
        }
    }
}