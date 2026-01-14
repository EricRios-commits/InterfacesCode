using System.Collections.Generic;
using System.Linq;
using Behavior.Enemy;
using Combat;
using Oculus.Interaction;
using UnityEngine;

namespace Waves
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Generation")]
        [SerializeField] private WaveGenerationProfile generationProfile;
        
        [Header("Components")]
        [SerializeField] private EnemySpawner spawner;
        
        private IWaveGenerator waveGenerator;
        private WaveStateManager stateManager;
        [SerializeField] private WaveTrigger currentTrigger;
        
        public WaveStateManager StateManager => stateManager;
        
        private GeneratedWaveData currentWave;
        private Queue<EnemySpawnEntry> spawnQueue;
        private float nextSpawnTime;
        
        private void Awake()
        {
            waveGenerator = new StandardWaveGenerator();
            stateManager = new WaveStateManager();
            stateManager.OnWaveCompleted += OnWaveCompleted;
        }
        
        public void StartNextWave()
        {
            int nextWaveNumber = stateManager.CurrentWave + 1;
            Debug.Log($"<color=cyan>🌊 [WaveManager] StartNextWave() llamado - Iniciando Wave {nextWaveNumber}</color>");
            
            currentWave = waveGenerator.GenerateWave(nextWaveNumber, generationProfile);
            Debug.Log($"[WaveManager] Generated Wave {nextWaveNumber}: {currentWave.TotalEnemyCount} enemies, " +
                      $"Type: {currentWave.Type}, Difficulty: {currentWave.DifficultyMultiplier:F2}x");
            spawnQueue = BuildSpawnQueue(currentWave);
            stateManager.StartWave(currentWave);
            nextSpawnTime = Time.time;
        }
        
        private Queue<EnemySpawnEntry> BuildSpawnQueue(GeneratedWaveData waveData)
        {
            var queue = new Queue<EnemySpawnEntry>();
            foreach (var entry in waveData.EnemiesToSpawn)
            {
                for (int i = 0; i < entry.Count; i++)
                {
                    queue.Enqueue(entry);
                }
            }
            return new Queue<EnemySpawnEntry>(queue.OrderBy(x => Random.value));
        }
        
        private void Update()
        {
            if (stateManager.State == WaveState.Spawning)
            {
                UpdateSpawning();
            }
        }
        
        private void UpdateSpawning()
        {
            if (Time.time >= nextSpawnTime && spawnQueue.Count > 0)
            {
                if (spawner.GetAliveCount() < currentWave.MaxSimultaneous)
                {
                    SpawnNextEnemy();
                    nextSpawnTime = Time.time + currentWave.SpawnInterval;
                }
            }
        }
        
        private void SpawnNextEnemy()
        {
            if (spawnQueue.Count == 0)
                return;
            var spawnEntry = spawnQueue.Dequeue();
            Vector3 spawnPos = spawner.GetRandomSpawnPoint();
            GameObject enemy = spawner.SpawnEnemy(spawnEntry.EnemyPrefab, spawnPos);
            Debug.Log("Spawned enemy: " + enemy.name);
            if (enemy != null)
            {
                spawner.ApplyDifficultyMultiplier(enemy, currentWave.DifficultyMultiplier);
                stateManager.RegisterEnemySpawned();
                var health = enemy.GetComponent<HealthComponent>();
                health.AddObserver(stateManager);
            }
        }
        
        private void OnWaveCompleted(int waveNumber)
        {
            Debug.Log($"<color=green>✅ [WaveManager] Wave {waveNumber} completed!</color>");
            
            if (currentTrigger != null)
            {
                Debug.Log($"[WaveManager] Habilitando trigger: {currentTrigger.GetType().Name}");
                currentTrigger.Enable();
            }
            else
            {
                Debug.LogError("[WaveManager] ❌ currentTrigger es null, no se puede habilitar");
            }
        }
        
        public void SetWaveTrigger(WaveTrigger trigger)
        {
            if (currentTrigger != null)
            {
                currentTrigger.OnTriggerActivated -= StartNextWave;
                currentTrigger.Disable();
            }
            currentTrigger = trigger;
            if (currentTrigger != null)
            {
                currentTrigger.Initialize(this);
                currentTrigger.OnTriggerActivated += StartNextWave;
            }
        }
        
        public GeneratedWaveData PreviewWave(int waveNumber)
        {
            return waveGenerator.PreviewWave(waveNumber, generationProfile);
        }
    }
}

