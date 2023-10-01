using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public class LaptopCutsceneInstaller : Installer
    {
        [SerializeField] private LaptopCutscene _laptopCutscene;

        public override void Install(ServiceLocator serviceLocator)
        {
            serviceLocator.RegisterService(_laptopCutscene);
        }
    }
}