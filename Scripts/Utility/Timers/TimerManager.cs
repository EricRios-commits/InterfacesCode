using System.Collections.Generic;
using UnityEngine;

namespace Utility.Timers {
    public static class TimerManager {
        static readonly List<Timer> timers = new();
        static readonly List<Timer> timersToRemove = new();
        
        public static void RegisterTimer(Timer timer)
        {
            if (!timers.Contains(timer))
            {
                timers.Add(timer);
            }
        }
        
        public static void DeregisterTimer(Timer timer)
        {
            timers.Remove(timer);
        }

        public static void UpdateTimers() {
            if (timers.Count == 0) return;

            for (int i = timers.Count - 1; i >= 0; i--)
            {
                if (i < timers.Count)
                {
                    timers[i].Tick(Time.deltaTime);
                }
            }
        }
        
        public static void Clear() {
            timersToRemove.Clear();
            timersToRemove.AddRange(timers);
            timers.Clear();
            
            foreach (var timer in timersToRemove) {
                timer.Dispose();
            }
            
            timersToRemove.Clear();
        }
    }
}
