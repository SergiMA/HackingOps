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

        private ObjectPool<Projectile> _projectilesPool;
        private ParticlePoolController _particlePoolController;

        private Vector3 _lastImpactPosition;

        private Vector3 _debugShootingDirection;
        private Quaternion _debugShootingRotation;
        private float _debugLinesLength = 1f;

        private void Awake()
        {
            _projectilesPool = new ObjectPool<Projectile>(CreateProjectile, OnTakeProjectileFromPool, OnReturnProjectileToPool);

            if (_projectileParticlePrefab != null)
                _particlePoolController = gameObject.GetOrAdd<ParticlePoolController>();
        }

        private void Start()
        {
            if (_projectileParticlePrefab == null)
                return;

            _particlePoolController.SetParticlePrefab(_projectileParticlePrefab);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_shootPoint.position, _debugShootingDirection * _debugLinesLength);

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(_shootPoint.position, _debugShootingRotation.eulerAngles * _debugLinesLength);
        }

        private void OnProjectileImpact(Vector3 impactPosition)
        {
            _lastImpactPosition = impactPosition;
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

        protected override void InternalStartShooting()
        {
        }

        protected override void InternalStopShooting()
        {
        }
        #endregion
    }
}