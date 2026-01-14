using UnityEngine;

namespace Waves
{
    public class WaveSystemStarter : MonoBehaviour
    {
        [Header("Wave System")]
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private WaveUIController uiController;
        
        [Header("Trigger Type")]
        [SerializeField] private TriggerType triggerType = TriggerType.Automatic;
        
        [Header("Trigger Components")]
        [SerializeField] private UIButtonWaveTrigger buttonTrigger;
        [SerializeField] private AutomaticWaveTrigger automaticTrigger;
        [SerializeField] private TimeBasedWaveTrigger timeBasedTrigger;
        
        [Header("Start Settings")]
        [SerializeField] private bool startFirstWaveOnAwake = true;
        
        private void Start()
        {
            SetupWaveSystem();
            
            if (startFirstWaveOnAwake)
            {
                waveManager.StartNextWave();
            }
        }
        
        private void SetupWaveSystem()
        {
            WaveTrigger selectedTrigger = null;
            
            switch (triggerType)
            {
                case TriggerType.UIButton:
                    selectedTrigger = buttonTrigger;
                    break;
                case TriggerType.Automatic:
                    selectedTrigger = automaticTrigger;
                    break;
                case TriggerType.TimeBased:
                    selectedTrigger = timeBasedTrigger;
                    break;
            }
            
            if (selectedTrigger != null)
            {
                waveManager.SetWaveTrigger(selectedTrigger);
            }
            
            SubscribeToWaveEvents();
        }
        
        private void SubscribeToWaveEvents()
        {
            if (waveManager != null && waveManager.StateManager != null && uiController != null)
            {
                waveManager.StateManager.OnWaveStarted += (wave, data) => 
                {
                    uiController.UpdateWaveDisplay(wave, data);
                    uiController.UpdateEnemyCount(data.TotalEnemyCount, data.TotalEnemyCount);
                };
                
                waveManager.StateManager.OnEnemyKilled += (wave) => 
                {
                    var stateManager = waveManager.StateManager;
                    uiController.UpdateEnemyCount(stateManager.EnemiesRemaining, stateManager.CurrentWaveData.TotalEnemyCount);
                };
            }
        }
        
        private enum TriggerType
        {
            UIButton,
            Automatic,
            TimeBased
        }
    }
}

