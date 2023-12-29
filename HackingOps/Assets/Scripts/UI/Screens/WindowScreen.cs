using HackingOps.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.UI.Screens
{
    public class WindowScreen : MonoBehaviour
    {
        [SerializeField] private List<CanvasGroup> _screens = new();

        public void OnTabButtonPressed(CanvasGroup attachedScreen)
        {
            UserInterfaceUtils.ChangeToScreen(_screens, attachedScreen);
        }
    }
}