using System;
using UnityEngine;

namespace Waves
{
    public class AutomaticWaveTrigger : WaveTrigger
    {
        [SerializeField] private float delayBetweenWaves = 3f;
        
        private WaveManager waveManager;
        private float enabledTime;
        private bool isEnabled;
        
        public override void Initialize(WaveManager manager)
        {
            waveManager = manager;
        }
        
        public override void Enable()
        {
            isEnabled = true;
            enabledTime = Time.time;
        }
        
        public override void Disable()
        {
            isEnabled = false;
        }
        
        public override bool CanTrigger()
        {
            return isEnabled && Time.time >= enabledTime + delayBetweenWaves;
        }
        
        private void Update()
        {
            if (CanTrigger())
            {
                InvokeTriggerActivated();
                Disable();
            }
        }
    }
}

