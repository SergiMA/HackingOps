using HackingOps.CombatSystem;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public class CombatModeInstaller : Installer
    {
        [SerializeField] private CombatMode _combatMode;

        public override void Install(ServiceLocator serviceLocator)
        {
            ServiceLocator.Instance.RegisterService(_combatMode);
        }

        public override void Uninstall(ServiceLocator serviceLocator)
        {
            ServiceLocator.Instance.DeregisterService(_combatMode);
        }
    }
}