using UnityEngine;
using UnityEngine.UI;

namespace Waves
{
    public class WaveUIController : MonoBehaviour
    {
        [SerializeField] private Text waveNumberText;
        [SerializeField] private Text waveTypeText;
        [SerializeField] private Text enemiesRemainingText;
        [SerializeField] private Text difficultyText;
        [SerializeField] private Slider waveProgressBar;
        
        public void UpdateWaveDisplay(int wave, GeneratedWaveData waveData)
        {
            if (waveNumberText != null)
                waveNumberText.text = $"Wave {wave}";
                
            if (waveTypeText != null)
            {
                waveTypeText.text = waveData.Type.ToString().ToUpper();
                
                switch (waveData.Type)
                {
                    case WaveType.Elite:
                        waveTypeText.color = Color.yellow;
                        break;
                    case WaveType.Boss:
                        waveTypeText.color = Color.red;
                        break;
                    default:
                        waveTypeText.color = Color.white;
                        break;
                }
            }
            
            if (difficultyText != null)
                difficultyText.text = $"Difficulty: {waveData.DifficultyMultiplier:F1}x";
        }
        
        public void UpdateEnemyCount(int remaining, int total)
        {
            if (enemiesRemainingText != null)
                enemiesRemainingText.text = $"{remaining}/{total}";
                
            if (waveProgressBar != null)
                waveProgressBar.value = 1f - ((float)remaining / total);
        }
    }
}


