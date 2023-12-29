using UnityEngine;

namespace HackingOps.Common.Settings
{
    public interface IToggleSetting
    {
        void OnValueChanged(bool value);
    }
}