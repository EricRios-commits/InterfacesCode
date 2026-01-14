using System;
using UnityEngine;
using UnityEngine.UI;

namespace Waves
{
    public class UIButtonWaveTrigger : WaveTrigger
    {
        [SerializeField] private Button triggerButton;
        
        private WaveManager waveManager;
        
        public override void Initialize(WaveManager manager)
        {
            waveManager = manager;
            
            if (triggerButton != null)
            {
                triggerButton.onClick.AddListener(TriggerWave);
            }
        }
        
        public override void Enable()
        {
            if (triggerButton != null)
            {
                triggerButton.interactable = true;
            }
        }
        
        public override void Disable()
        {
            if (triggerButton != null)
            {
                triggerButton.interactable = false;
            }
        }
        
        public override bool CanTrigger()
        {
            return triggerButton != null && triggerButton.interactable;
        }
        
        private void TriggerWave()
        {
            InvokeTriggerActivated();
            Disable();
        }
    }
}

