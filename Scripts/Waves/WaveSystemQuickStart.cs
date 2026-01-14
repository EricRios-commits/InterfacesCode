using UnityEngine;

namespace Waves
{
    public class WaveSystemQuickStart : MonoBehaviour
    {
        [Header("Required Components")]
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private WaveTrigger waveTrigger;
        [Tooltip("Puede ser AlbertoTrigger, TimeBasedWaveTrigger, AutomaticWaveTrigger, etc.")]
        
        [Header("Settings")]
        [SerializeField] private bool startFirstWaveImmediately = true;
        
        private void Start()
        {
            if (waveManager == null)
            {
                Debug.LogError("[WaveSystemQuickStart] WaveManager not assigned!");
                return;
            }
            if (waveTrigger == null)
            {
                Debug.LogError("[WaveSystemQuickStart] WaveTrigger not assigned!");
                return;
            }
            
            Debug.Log($"[WaveSystemQuickStart] Configurando trigger: {waveTrigger.GetType().Name}");
            waveManager.SetWaveTrigger(waveTrigger);
            
            if (startFirstWaveImmediately)
            {
                StartFirstWave();
            }
        }
        
        private void StartFirstWave()
        {
            Debug.Log("[WaveSystemQuickStart] Starting first wave...");
            waveManager.StartNextWave();
        }
    }
}

