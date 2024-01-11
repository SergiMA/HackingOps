namespace HackingOps.Utilities.Timers
{
    public class StopwatchTimer : Timer
    { 
        public StopwatchTimer() : base(0) { }

        public override void Tick(float deltaTime)
        {
            if (!IsRunning) return;

            _time += deltaTime;
        }

        public void Reset() => _time = 0;
        public float GetTime() => _time;
    }
}