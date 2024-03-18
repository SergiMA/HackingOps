using HackingOps.Utilities.Timers;
using UnityEngine;

namespace HackingOps.Audio.AudioZones.States
{
    public class Blending2DState : AudioZoneBaseState
    {
        private readonly CountdownTimer _countdownTimer;
        private bool _timerEnded;

        private Vector3 _startingPosition;

        public Blending2DState(AudioZone ctx, AudioZoneStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;

            _countdownTimer = new CountdownTimer(_ctx.MinDurationInsideZone);
            _countdownTimer.OnStop += () => _timerEnded = true;
        }

        public override void EnterState()
        {
            _countdownTimer.Start();
            _startingPosition = _ctx.AudioSource.transform.position;
        }

        public override void UpdateState()
        {
            _countdownTimer.Tick(Time.deltaTime);

            DecreaseBlending();
            MoveAudioSourceToTargetSmoothly();
            CheckSwitchState();
        }

        public override void ExitState() => _timerEnded = false;

        protected override void CheckSwitchState()
        {
            if (_timerEnded)
                SwitchState(_factory.GetState(AudioZoneStateFactory.States.Blending3D));
            else if (_ctx.CurrentBlendingProgress == 0)
                SwitchState(_factory.GetState(AudioZoneStateFactory.States.Blended2D));
        }

        public override void OnTriggerStay(Collider other) => _countdownTimer.Reset();

        private void DecreaseBlending()
        {
            DecreaseBlendingProgress();
            ApplyBlending();
        }

        private void DecreaseBlendingProgress()
        {
            _ctx.CurrentBlendingProgress -= (1f / _ctx.BlendingDurationTo2D) * Time.deltaTime;
            _ctx.CurrentBlendingProgress = Mathf.Max(_ctx.CurrentBlendingProgress, 0f);
        }

        private void ApplyBlending() => _ctx.AudioSource.spatialBlend = _ctx.CurrentBlendingProgress;

        private void MoveAudioSourceToTargetSmoothly()
        {
            _ctx.AudioSource.transform.position = Vector3.Lerp(_ctx.Follower.GetTargetPosition(),
                                                               _startingPosition,
                                                               _ctx.CurrentBlendingProgress);
        }
    }
}