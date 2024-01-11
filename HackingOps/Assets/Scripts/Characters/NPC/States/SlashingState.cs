using UnityEngine;
using HackingOps.Characters.NPC.Senses;
using HackingOps.Characters.Entities;
using HackingOps.Characters.Common;

namespace HackingOps.Characters.NPC.States
{
    public class SlashingState : State
    {
        [SerializeField] private float _maxAttackCooldown = 5f;
        [SerializeField] private float _minAttackCooldown = 3f;

        private EntityWeapons _entityWeapons;
        private CharacterCombat _characterCombat;

        private float _currentAttackCooldown = 3f;

        private void Update()
        {
            LookAtTarget();

            if (IsAttackInCooldown())
                DecreaseAttackCooldown();
            else
            {
                _characterCombat.Attack();
                _currentAttackCooldown = Random.Range(_minAttackCooldown, _maxAttackCooldown);
            }
        }

        private void LookAtTarget()
        {
            if (_entity.DecisionMaker.CurrentTarget != null)
            {
                IVisible currentTarget = _entity.DecisionMaker.CurrentTarget;

                Vector3 direction = currentTarget.GetTransform().position - _entity.transform.position;
                Vector3 directionOnPlane = Vector3.ProjectOnPlane(direction, Vector3.up);
                float angularDistance = Vector3.SignedAngle(_entity.transform.forward, directionOnPlane, Vector3.up);

                float angleToApply = _entity.AngularSpeed * Time.deltaTime;
                angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));
                _entity.transform.Rotate(new(0f, Mathf.Sign(angularDistance) * angleToApply, 0f));
            }
        }

        void DecreaseAttackCooldown()
        {
            _currentAttackCooldown -= Time.deltaTime;
            _currentAttackCooldown = Mathf.Max(0, _currentAttackCooldown);
        }

        private bool IsAttackInCooldown() => _currentAttackCooldown > 0;

        #region State implementation
        protected override void StateAwake()
        {
            _entityWeapons = GetComponent<EntityWeapons>();
            _characterCombat = GetComponent<CharacterCombat>();
        }

        protected override void StateOnEnable()
        {
            _entity.Agent.destination = _entity.transform.position;
            _entityWeapons.MustShoot();


            _currentAttackCooldown = Random.Range(_minAttackCooldown, _maxAttackCooldown);
        }

        protected override void StateOnDisable()
        {
            _entityWeapons.MustNotShoot();
        }
        #endregion
    }
}