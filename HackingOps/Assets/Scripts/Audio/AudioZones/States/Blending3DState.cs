using System.Linq;
using UnityEngine;

namespace HackingOps.Audio.AudioZones.States
{
    public class Blending3DState : AudioZoneBaseState
    {
        private bool _steppedInsideZone;
        private Transform _closestWaypoint;

        private Vector3 _startingPosition;

        public Blending3DState(AudioZone ctx, AudioZoneStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState()
        {
            _startingPosition = _ctx.AudioSource.transform.position;
            GetClosestWaypoint();
        }

        public override void UpdateState()
        {
            IncreaseBlending();
            MoveToWaypointSmoothly();
            CheckSwitchState();
        }

        public override void ExitState() => _steppedInsideZone = false;

        protected override void CheckSwitchState()
        {
            if (_steppedInsideZone)
                SwitchState(_factory.GetState(AudioZoneStateFactory.States.Blending2D));
            else if (_ctx.CurrentBlendingProgress == 1)
                SwitchState(_factory.GetState(AudioZoneStateFactory.States.Blended3D));
        }

        public override void OnTriggerStay(Collider other) => _steppedInsideZone = true;

        private void GetClosestWaypoint()
        {
            _closestWaypoint = _ctx.Waypoints.OrderBy(t => (t.position - _ctx.Follower.TargetToFollow.position).sqrMagnitude)
                                             .First();
        }

        private void IncreaseBlending()
        {
            IncreaseBlendingProgress();
            ApplyBlending();
        }

        private void IncreaseBlendingProgress()
        {
            _ctx.CurrentBlendingProgress += (1f / _ctx.BlendingDurationTo3D) * Time.deltaTime;
            _ctx.CurrentBlendingProgress = Mathf.Min(_ctx.CurrentBlendingProgress, 1f);
        }

        private void ApplyBlending() => _ctx.AudioSource.spatialBlend = _ctx.CurrentBlendingProgress;

        private void MoveToWaypointSmoothly()
        {
            _ctx.AudioSource.transform.position = Vector3.Lerp(_startingPosition,
                                                               _closestWaypoint.position,
                                                               _ctx.CurrentBlendingProgress);
        }
    }
}