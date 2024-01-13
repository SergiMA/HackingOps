using HackingOps.Characters.Entities;
using HackingOps.Characters.NPC.Senses.SightSense;
using UnityEngine;

namespace HackingOps.Characters.NPC.States
{
    public class ShootingStandingState : State
    {
        EntityWeapons _entityWeapons;

        protected override void StateAwake()
        {
            _entityWeapons = GetComponent<EntityWeapons>();
        }

        protected override void StateOnEnable()
        {
            _entity.Agent.destination = _entity.transform.position;
            _entityWeapons.MustShoot();
        }

        protected override void StateOnDisable()
        {
            _entityWeapons.MustNotShoot();
        }

        private void Update()
        {
            InternalPreUpdate();

            if (_entity.DecisionMaker.CurrentTarget != null)
            {
                //IVisible currentTarget = _entity.Sight.VisiblesInSight[0];

                Vector3 direction = _entity.DecisionMaker.CurrentTarget.position - _entity.transform.position;
                Vector3 directionOnPlane = Vector3.ProjectOnPlane(direction, Vector3.up);
                float angularDistance = Vector3.SignedAngle(_entity.transform.forward, directionOnPlane, Vector3.up);

                float angleToApply = _entity.AngularSpeed * Time.deltaTime;
                angleToApply = Mathf.Min(angleToApply, Mathf.Abs(angularDistance));
                _entity.transform.Rotate(new(0f, Mathf.Sign(angularDistance) * angleToApply, 0f));
            }

            InternalPostUpdate();
        }

        protected virtual void InternalPreUpdate() { }
        protected virtual void InternalPostUpdate() { }
    }
}