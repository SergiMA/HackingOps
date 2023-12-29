using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.Utilities
{
    public static class UserInterfaceUtils
    {
        /// <summary>
        /// Make the screen invisible and stop receiving raycasts
        /// </summary>
        /// <param name="screen">Screen to close</param>
        public static void CloseScreen(CanvasGroup screen)
        {
            screen.alpha = 0f;
            screen.blocksRaycasts = false;
        }

        /// <summary>
        /// Make the screens invisible and stop receiving raycasts
        /// </summary>
        /// <param name="screens">List of screens to close</param>
        public static void CloseScreen(List<CanvasGroup> screens)
        {
            foreach (CanvasGroup screen in screens)
                CloseScreen(screen);
        }

        /// <summary>
        /// Make the screen visible and start receiving raycasts
        /// </summary>
        /// <param name="screen">Screen to open</param>
        public static void OpenScreen(CanvasGroup screen)
        {
            screen.alpha = 1f;
            screen.blocksRaycasts = true;
        }

        /// <summary>
        /// Make the screens visible and start receiving raycasts
        /// </summary>
        /// <param name="screens">List of screens to open</param>
        public static void OpenScreen(List<CanvasGroup> screens)
        {
            foreach (CanvasGroup screen in screens)
                OpenScreen(screen);
        }

        /// <summary>
        /// Change to a specific screen. Close the other screens received and open the desired one
        /// </summary>
        public static void ChangeToScreen(List<CanvasGroup> screensToClose, CanvasGroup screenToOpen)
        {
            CloseScreen(screensToClose);
            OpenScreen(screenToOpen);
        }
    }
}