using UnityEngine;
using UnityEngine.Playables;

namespace HackingOps.CutsceneSystem
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PlayableDirectorUpdater : MonoBehaviour
    {
        private PlayableDirector _playableDirector;

        private void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
        }

        public void SetWrapModeToNone()
        {
            _playableDirector.extrapolationMode = DirectorWrapMode.None;
        }

        public void SetWrapModeToHold()
        {
            _playableDirector.extrapolationMode = DirectorWrapMode.Hold;
        }
    }
}