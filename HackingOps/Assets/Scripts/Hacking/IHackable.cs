using System;

namespace HackingOps.Hacking
{
    public interface IHackable
    {
        event Action OnReceiveCandidateNotification;

        void BeginHacking();
        void StopHacking();
        bool IsControllable();
        void ReceiveCandidateNotification();
    }
}