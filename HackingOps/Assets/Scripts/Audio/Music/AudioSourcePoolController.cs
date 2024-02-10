using UnityEngine;
using UnityEngine.Pool;

namespace HackingOps.Audio.Music
{
    public class AudioSourcePoolController : MonoBehaviour
    {
        [SerializeField] private Transform _pooledAudioSourcesParent;
        [SerializeField] private PooledAudioSource _pooledAudioSourcePrefab;

        private ObjectPool<PooledAudioSource> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<PooledAudioSource>(CreateAudioSource,
                                                      OnTakeAudioSourceFromPool,
                                                      OnReturnAudioSourceToPool);
        }

        private PooledAudioSource CreateAudioSource()
        {
            PooledAudioSource _pooledAudioSource = Instantiate(_pooledAudioSourcePrefab);
            _pooledAudioSource.SetPool(_pool);
            _pooledAudioSource.transform.SetParent(_pooledAudioSourcesParent);

            return _pooledAudioSource;
        }

        private void OnTakeAudioSourceFromPool(PooledAudioSource pooledAudioSource)
        {
            pooledAudioSource.gameObject.SetActive(true);
        }

        private void OnReturnAudioSourceToPool(PooledAudioSource pooledAudioSource)
        {
            pooledAudioSource.gameObject.SetActive(false);
        }

        public void SetPooledAudioPrefab(PooledAudioSource pooledAudioSourcePrefab)
        {
            _pooledAudioSourcePrefab = pooledAudioSourcePrefab;
        }

        public PooledAudioSource Get() => _pool.Get();
    }
}