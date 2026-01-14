using System;

namespace Utility.Timers
{
    public abstract class Timer : IDisposable
    {
        protected float initialTime;
        public float Time { get; set; }
        public bool IsRunning { get; protected set; }

        public float Progress => initialTime > 0 ? Time / initialTime : 0;

        public event Action OnTimerStart;
        public event Action OnTimerStop;

        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            Time = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart?.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                OnTimerStop?.Invoke();
            }
        }

        public void Resume()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
            }
        }
        
        public void Pause()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
            }
        }

        public abstract void Tick(float deltaTime);

        public void ClearCallbacks()
        {
            OnTimerStart = null;
            OnTimerStop = null;
        }

        private bool disposed;

        ~Timer()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                if (IsRunning)
                {
                    IsRunning = false;
                    TimerManager.DeregisterTimer(this);
                }
                ClearCallbacks();
            }
            disposed = true;
        }
    }
}
