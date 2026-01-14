using System;
using UnityEngine;

namespace Waves
{
    [Serializable]
    public abstract class WaveTrigger : MonoBehaviour
    {
        public event Action OnTriggerActivated;
        public abstract void Initialize(WaveManager waveManager);
        public abstract void Enable();
        public abstract void Disable();
        public abstract bool CanTrigger();

        protected void InvokeTriggerActivated()
        {
            OnTriggerActivated?.Invoke();
        }
    }
}
