using UnityEngine;

namespace HackingOps.Utilities
{
    public static class Utils
    {
        public static void ExitGame()
        {
# if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}