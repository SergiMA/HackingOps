namespace HackingOps.Utilities.Timers
{
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning && _time > 0)
                _time -= deltaTime;

            if (IsRunning && _time <= 0)
                Stop();
        }

        public bool IsFinished => _time <= 0;
        public void Reset() => _time = _initialTime;
        public void Reset(float newTime)
        {
            _initialTime = newTime;
            Reset();
        }
    }
}