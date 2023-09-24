using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public abstract class Installer : MonoBehaviour
    {
        public abstract void Install(ServiceLocator serviceLocator);
    }
}