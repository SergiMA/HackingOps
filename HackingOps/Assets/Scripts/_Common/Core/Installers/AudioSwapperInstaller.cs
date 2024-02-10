using HackingOps.Audio;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public class AudioSwapperInstaller : Installer
    {
        [SerializeField] private AudioSwapper _audioSwapper;

        public override void Install(ServiceLocator serviceLocator)
        {
            ServiceLocator.Instance.RegisterService(_audioSwapper);
        }

        public override void Uninstall(ServiceLocator serviceLocator)
        {
            ServiceLocator.Instance.DeregisterService(_audioSwapper);
        }
    }
}