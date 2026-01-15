using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Waves;

namespace LLMAnswer
{
    /// <summary>
    /// Provides wave/round context to the AI prompt system.
    /// Subscribes to WaveStateManager events and formats wave information for the AI.
    /// </summary>
    public class WavePromptContextProvider : MonoBehaviour, IPromptContextProvider
    {
        [SerializeField] private WaveManager waveManager;
        [SerializeField] private bool includeEnemyDetails = true;
        [SerializeField] private bool includeWaveType = true;
        [SerializeField] private bool includeDifficulty = true;
        
        private int currentWave;
        private GeneratedWaveData currentWaveData;
        private int enemiesKilled;
        private int totalEnemies;
        
        private void Awake()
        {
            if (waveManager == null)
            {
                waveManager = FindFirstObjectByType<WaveManager>();
            }
            if (waveManager != null && waveManager.StateManager != null)
            {
                waveManager.StateManager.OnWaveStarted += OnWaveStarted;
                waveManager.StateManager.OnEnemyKilled += OnEnemyKilled;
                waveManager.StateManager.OnWaveCompleted += OnWaveCompleted;
            }
            else
            {
                Debug.LogWarning("[WavePromptContextProvider] WaveManager or StateManager not found!");
            }
        }
        
        private void OnDestroy()
        {
            if (waveManager != null && waveManager.StateManager != null)
            {
                waveManager.StateManager.OnWaveStarted -= OnWaveStarted;
                waveManager.StateManager.OnEnemyKilled -= OnEnemyKilled;
                waveManager.StateManager.OnWaveCompleted -= OnWaveCompleted;
            }
        }
        
        private void OnWaveStarted(int waveNumber, GeneratedWaveData waveData)
        {
            currentWave = waveNumber;
            currentWaveData = waveData;
            totalEnemies = waveData.TotalEnemyCount;
            enemiesKilled = 0;
            Debug.Log($"[WavePromptContextProvider] Wave {waveNumber} started with {totalEnemies} enemies");
        }
        
        private void OnEnemyKilled(int waveNumber)
        {
            enemiesKilled++;
        }
        
        private void OnWaveCompleted(int waveNumber)
        {
            Debug.Log($"[WavePromptContextProvider] Wave {waveNumber} completed");
        }
        
        public string GetContext()
        {
            if (currentWaveData == null)
            {
                return null;
            }
            var context = new StringBuilder();
            context.AppendLine("\n--- CURRENT GAME STATE ---");
            context.AppendLine($"Wave: {currentWave}");
            context.AppendLine($"Enemies killed this wave: {enemiesKilled}/{totalEnemies}");
            if (includeDifficulty)
            {
                context.AppendLine($"Difficulty multiplier: {currentWaveData.DifficultyMultiplier:F1}x");
            }
            if (includeWaveType)
            {
                context.AppendLine($"Wave type: {currentWaveData.Type}");
            }
            if (includeEnemyDetails && currentWaveData.EnemiesToSpawn != null)
            {
                var enemyTypes = new Dictionary<string, int>();
                foreach (var entry in currentWaveData.EnemiesToSpawn)
                {
                    if (entry.EnemyPrefab != null)
                    {
                        string enemyName = entry.EnemyPrefab.name;
                        if (enemyTypes.ContainsKey(enemyName))
                        {
                            enemyTypes[enemyName] += entry.Count;
                        }
                        else
                        {
                            enemyTypes[enemyName] = entry.Count;
                        }
                    }
                }
                if (enemyTypes.Count > 0)
                {
                    context.AppendLine("Enemy composition:");
                    foreach (var kvp in enemyTypes)
                    {
                        context.AppendLine($"  - {kvp.Key}: {kvp.Value}");
                    }
                }
            }
            return context.ToString();
        }
    }
}

