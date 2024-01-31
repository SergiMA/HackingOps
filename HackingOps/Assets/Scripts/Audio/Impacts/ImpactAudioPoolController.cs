using HackingOps.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace HackingOps.Audio.Impacts
{
    public class ImpactAudioPoolController : MonoBehaviour
    {
        private PooledImpactAudio _impactAudioPrefab;
        private ObjectPool<PooledImpactAudio> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<PooledImpactAudio>(CreateImpactAudioItem, OnTakeImpactAudioFromPool, OnReturnImpactAudioToPool);
        }

        private PooledImpactAudio CreateImpactAudioItem()
        {
            PooledImpactAudio _impactAudio = Instantiate(_impactAudioPrefab);
            _impactAudio.SetPool(_pool);

            GameObject parent = gameObject.FindOrCreate("Impact audios");
            _impactAudio.transform.SetParent(parent.transform);

            return _impactAudio;
        }

        private void OnTakeImpactAudioFromPool(PooledImpactAudio impactAudio) => impactAudio.gameObject.SetActive(true);
        private void OnReturnImpactAudioToPool(PooledImpactAudio impactAudio) => impactAudio.gameObject.SetActive(false);
        
        public void SetImpactAudioPrefab(PooledImpactAudio impactAudioPrefab)
        {
            _impactAudioPrefab = impactAudioPrefab;
        }

        public PooledImpactAudio Get() => _pool.Get();
    }
}