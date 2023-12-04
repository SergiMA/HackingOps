using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    [DefaultExecutionOrder(-10)]
    public abstract class Installer : MonoBehaviour
    {
        public abstract void Install(ServiceLocator serviceLocator);
        public abstract void Uninstall(ServiceLocator serviceLocator);
    }
}