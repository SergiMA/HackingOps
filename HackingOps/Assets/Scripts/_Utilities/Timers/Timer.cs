using System;

namespace HackingOps.Utilities.Timers
{
    public abstract class Timer
    {
        public event Action OnStart;
        public event Action OnStop;

        public bool IsRunning { get; protected set; }
        public float Progress => _time / _initialTime;

        protected float _initialTime;
        protected float _time { get; set; }

        protected Timer(float value)
        {
            _initialTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            if (IsRunning) return;

            _time = _initialTime;
            IsRunning = true;

            OnStart?.Invoke();
        }

        public void Stop()
        {
            if (!IsRunning) return;

            IsRunning = false;
            OnStop?.Invoke();
        }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        public abstract void Tick(float deltaTime);
    }
}