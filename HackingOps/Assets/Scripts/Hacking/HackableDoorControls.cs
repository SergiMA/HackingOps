using HackingOps.Doors.AnimatedDoors;
using UnityEngine;

namespace HackingOps.Hacking
{
    public class HackableDoorControls : MonoBehaviour, IHackable
    {
        [SerializeField] private AnimatedDoor[] _doors;

        #region IHackable implementation
        public void BeginHacking()
        {
            foreach (AnimatedDoor door in _doors)
                door.Open();
        }

        public void StopHacking() { }
        public bool IsControllable() => false;
        #endregion
    }
}