using HackingOps.CombatSystem.HitHurtBox;
using System;
using UnityEngine;
using UnityEngine.Pool;

namespace HackingOps.Weapons.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : HitBoxWithDamage
    {
        [SerializeField] private bool _destroyOnCollision = true;
        [SerializeField] private string[] _bouncingTags = { "Block" };
        [SerializeField] private int _bounces; // Amount of bounces allowed
        [SerializeField] private float _bounceSpread = 0.5f;
        [SerializeField] private float _activeDuration = 5f;    // Max time outside the pool

        private Rigidbody _rigidbody;
        private Transform _origin;
        private int _bouncesLeft;
        private float _launchSpeed;

        private float _currentActiveDuration;

        private IObjectPool<Projectile> _pool;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            _origin = transform;
        }

        private void Start()
        {
            _bouncesLeft = _bounces;
        }

        private void Update()
        {
            _currentActiveDuration -= Time.deltaTime;

            if (_currentActiveDuration <= 0)
            {
                _pool.Release(this);
            }
        }

        private bool HasCollidedWithBouncingTag(Collider collider)
        {
            foreach (string tag in _bouncingTags)
            {
                if (collider.CompareTag(tag))
                    return true;
            }

            return false;
        }

        private Vector3 GetBounceDirection()
        {
            Vector3 direction = transform.forward * -1;
            direction.x += UnityEngine.Random.Range(-_bounceSpread, _bounceSpread);
            direction.y += UnityEngine.Random.Range(-_bounceSpread, _bounceSpread);
            direction.z += UnityEngine.Random.Range(-_bounceSpread, _bounceSpread);

            return direction;
        }

        private void ResetTimer()
        {
            _currentActiveDuration = _activeDuration;
        }

        public override void DeliverHit(Collider collider)
        {
            if (collider.TryGetComponent(out HurtBox hurtBox))
                hurtBox?.NotifyHit(_damage, _origin);

            if (_bouncesLeft > 0 && HasCollidedWithBouncingTag(collider)) 
            {
                _bouncesLeft--;

                Vector3 bounceDirection = GetBounceDirection();
                _rigidbody.AddForce(bounceDirection * (_launchSpeed * 2f), ForceMode.VelocityChange); // [_launchSpeed * 2] because it needs to counter the initial speed
                ResetTimer();
            }
            else
                _pool.Release(this);
        }

        public void SetOriginTransform(Transform origin)
        {
            _origin = origin;
        }

        public void SetLaunchSpeed(float launchSpeed)
        {
            _launchSpeed = launchSpeed;
        }

        public void SetPool(IObjectPool<Projectile> pool) => _pool = pool;

        public void ResetBullet()
        {
            _rigidbody.velocity = Vector3.zero;
            ResetTimer();
            _bouncesLeft = _bounces;
        }
    }
}