using UnityEngine;
using UnityEngine.Pool;

namespace HackingOps.VFX.Particles
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticle : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private ObjectPool<PooledParticle> _pool;

        private void Awake() => _particleSystem = GetComponent<ParticleSystem>();

        private void Start()
        {
            var main = _particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        public void SetPool(ObjectPool<PooledParticle> pool) => _pool = pool;

        private void OnParticleSystemStopped()
        {
            _pool.Release(this);
        }
    }
}