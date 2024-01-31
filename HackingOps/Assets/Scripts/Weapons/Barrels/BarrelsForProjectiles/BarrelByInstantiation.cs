using HackingOps.Audio.Impacts;
using HackingOps.Utilities.Audio;
using HackingOps.Utilities.Extensions;
using HackingOps.VFX.Particles;
using HackingOps.Weapons.Projectiles;
using UnityEngine;
using UnityEngine.Pool;

namespace HackingOps.Weapons.Barrels.BarrelsForProjectiles
{
    public class BarrelByInstantiation : Barrel
    {
        [Header("Projectiles bindings")]
        [SerializeField] private Projectile _prefabToInstantiate;
        [SerializeField] private Transform _shootPoint;

        [Header("Projectiles settings")]
        [SerializeField] private float _launchSpeed = 10f;
        [Range(0f, 1f)][SerializeField] private float _spread = 0.2f;

        [Header("Projectiles particles bindings (optional)")]
        [SerializeField] private PooledParticle _projectileParticlePrefab;
        [SerializeField] private PooledImpactAudio _impactAudioPrefab;

        [Header("Audio (optional)")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip[] _shotSounds;
        [SerializeField] private float _basePitch = 1f;
        [SerializeField] private float _pitchVariation = 0.2f;

        private ObjectPool<Projectile> _projectilesPool;
        private ParticlePoolController _particlePoolController;
        private ImpactAudioPoolController _impactAudioPoolController;

        private Vector3 _debugShootingDirection;
        private Quaternion _debugShootingRotation;
        private float _debugLinesLength = 1f;

        private void Awake()
        {
            _projectilesPool = new ObjectPool<Projectile>(CreateProjectile, OnTakeProjectileFromPool, OnReturnProjectileToPool);

            if (_projectileParticlePrefab != null)
                _particlePoolController = gameObject.GetOrAdd<ParticlePoolController>();

            if (_impactAudioPrefab != null)
                _impactAudioPoolController = gameObject.GetOrAdd<ImpactAudioPoolController>();
        }

        private void Start()
        {
            if (_projectileParticlePrefab != null) _particlePoolController.SetParticlePrefab(_projectileParticlePrefab);
            if (_impactAudioPrefab != null) _impactAudioPoolController.SetImpactAudioPrefab(_impactAudioPrefab);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_shootPoint.position, _debugShootingDirection * _debugLinesLength);

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(_shootPoint.position, _debugShootingRotation.eulerAngles * _debugLinesLength);
        }


        private void SpawnProjectile()
        {
            Vector3 shootingDirection = GetShootingDirection();
            Quaternion shootingRotation = Quaternion.LookRotation(shootingDirection);

            _debugShootingDirection = shootingDirection;
            _debugShootingRotation = shootingRotation;

            Projectile projectile = _projectilesPool.Get();
            projectile.transform.SetPositionAndRotation(_shootPoint.position, shootingRotation);
            projectile.SetOriginTransform(transform);
            projectile.SetLaunchSpeed(_launchSpeed);
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * _launchSpeed, ForceMode.VelocityChange);
        }

        private void PlayShotSound()
        {
            AudioClip clip = AudioHelper.GetRandomClip(_shotSounds);
            _audioSource.pitch = AudioHelper.RandomizePitch(_basePitch, _pitchVariation);
            _audioSource.PlayOneShot(clip);
        }

        #region Projectile pool management
        private Projectile CreateProjectile()
        {
            var projectile = Instantiate(_prefabToInstantiate);
            projectile.SetPool(_projectilesPool);

            GameObject parent = gameObject.FindOrCreate("Projectiles");
            projectile.transform.SetParent(parent.transform);
            return projectile;
        }

        private void OnTakeProjectileFromPool(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
            projectile.ResetBullet();
        }

        private void OnReturnProjectileToPool(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);

            if (_projectileParticlePrefab != null)
            {
                _particlePoolController.Get().transform.position = projectile.transform.position;
            }

            if (_impactAudioPrefab != null)
            {
                _impactAudioPoolController.Get().transform.position = projectile.transform.position;
            }
        }
        #endregion

        private Vector3 GetShootingDirection()
        {
            Vector3 shootingDirection = _shootPoint.forward;
            shootingDirection.x += Random.Range(-_spread, _spread);
            shootingDirection.y += Random.Range(-_spread, _spread);
            shootingDirection.z += Random.Range(-_spread, _spread);

            return shootingDirection;
        }

        #region Inherited from Barrel
        protected override void InternalShot()
        {
            SpawnProjectile();

            if (_shotSounds.Length > 0) PlayShotSound();
        }

        protected override void InternalStartShooting()
        {
        }

        protected override void InternalStopShooting()
        {
        }
        #endregion
    }
}