using UnityEngine;

namespace HackingOps.Animations.IK.EnvironmentInteractions
{
    public class EnvironmentInteractorEventForwarder : MonoBehaviour
    {
        [SerializeField] private EnvironmentInteractor[] _environmentInteractorsArms;

        public void OnStartedRunning()
        {
            foreach (EnvironmentInteractor arm in _environmentInteractorsArms)
                arm.OnStartedRunning();
        }

        public void OnStartedWalking() 
        {
            foreach (EnvironmentInteractor arm in _environmentInteractorsArms)
                arm.OnStartedWalking();
        }

        public void OnStoppedMoving() {
            foreach (EnvironmentInteractor arm in _environmentInteractorsArms)
                arm.OnStoppedMoving();
        }

        public void OnStartedCrouching() {
            foreach (EnvironmentInteractor arm in _environmentInteractorsArms)
                arm.OnStartedCrouching();
        }

        public void OnStoppedCrouching()
        {
            foreach (EnvironmentInteractor arm in _environmentInteractorsArms)
                arm.OnStoppedCrouching();
        }
    }
}