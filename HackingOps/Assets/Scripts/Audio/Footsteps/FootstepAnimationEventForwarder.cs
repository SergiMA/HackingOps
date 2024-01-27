using UnityEngine;

namespace HackingOps.Audio.Footsteps
{
    public class FootstepAnimationEventForwarder : MonoBehaviour
    {
        [SerializeField] private FootstepsPlayer[] _footstepsPlayer;

        /// <summary>
        /// Called by Animation Events. Make a footstep player play a step
        /// </summary>
        /// <param name="index">Footstep Player index</param>
        private void Step(int index)
        {
            _footstepsPlayer[index].Step();
        }
    }
}