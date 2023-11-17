using HackingOps.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace HackingOps.VFX.Particles
{
    public class ParticlePoolController : MonoBehaviour
    {
        [SerializeField] private PooledParticle _particlePrefab;

        private ObjectPool<PooledParticle> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<PooledParticle>(CreateParticleItem, OnTakeParticleFromPool, OnReturnParticleToPool);
        }

        private PooledParticle CreateParticleItem()
        {
            PooledParticle particle = Instantiate(_particlePrefab);
            particle.SetPool(_pool);

            GameObject parent = gameObject.FindOrCreate("Projectiles particles");
            particle.transform.SetParent(parent.transform);
            return particle;
        }

        private void OnTakeParticleFromPool(PooledParticle particle)
        {
            particle.gameObject.SetActive(true);
        }

        private void OnReturnParticleToPool(PooledParticle particle)
        {
            particle.gameObject.SetActive(false);
        }

        public void SetParticlePrefab(PooledParticle particlePrefab)
        {
            _particlePrefab = particlePrefab;
        }

        public PooledParticle Get() => _pool.Get();
    }
}