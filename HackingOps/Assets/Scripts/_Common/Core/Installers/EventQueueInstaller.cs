using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.Core.Installers
{
    public class EventQueueInstaller : Installer
    {
        [SerializeField] private EventQueue _eventQueue;

        public override void Install(ServiceLocator serviceLocator)
        {
            serviceLocator.RegisterService<IEventQueue>(_eventQueue);
        }
    }
}