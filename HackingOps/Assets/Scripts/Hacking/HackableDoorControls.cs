using HackingOps.Doors.AnimatedDoors;
using System;
using UnityEngine;

namespace HackingOps.Hacking
{
    public class HackableDoorControls : MonoBehaviour, IHackable
    {
        public event Action OnReceiveCandidateNotification;

        [SerializeField] private AnimatedDoor[] _doors;

        #region IHackable implementation
        public void BeginHacking()
        {
            foreach (AnimatedDoor door in _doors)
                door.Open();
        }

        public void StopHacking() { }
        public bool IsControllable() => false;
        public void ReceiveCandidateNotification() => OnReceiveCandidateNotification?.Invoke();
        #endregion
    }
}