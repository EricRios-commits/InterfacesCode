using System;
using Combat;
using UnityEngine;

namespace Waves
{
    public class WaveStateManager : IHealthObserver
    {
        public int CurrentWave { get; private set; }
        public GeneratedWaveData CurrentWaveData { get; private set; }
        public int EnemiesRemaining { get; private set; }
        public int EnemiesSpawned { get; private set; }
        public int AliveEnemyCount { get; private set; }
        public WaveState State { get; private set; }
        
        public event Action<int, GeneratedWaveData> OnWaveStarted;
        public event Action<int> OnWaveCompleted;
        public event Action<int> OnEnemySpawned;
        public event Action<int> OnEnemyKilled;
        
        public void StartWave(GeneratedWaveData waveData)
        {
            CurrentWave++;
            CurrentWaveData = waveData;
            EnemiesRemaining = waveData.TotalEnemyCount;
            EnemiesSpawned = 0;
            State = WaveState.Spawning;
            OnWaveStarted?.Invoke(CurrentWave, waveData);
        }
        
        public void RegisterEnemySpawned()
        {
            EnemiesSpawned++;
            AliveEnemyCount++;
            OnEnemySpawned?.Invoke(CurrentWave);
            
            if (EnemiesSpawned >= CurrentWaveData.TotalEnemyCount)
            {
                State = WaveState.Active;
            }
        }
        
        public void RegisterEnemyKilled()
        {
            AliveEnemyCount--;
            EnemiesRemaining--;
            OnEnemyKilled?.Invoke(CurrentWave);
            Debug.Log($"State manager registered enemy killed. Enemies remaining: {EnemiesRemaining}");
            if (EnemiesRemaining <= 0 && State == WaveState.Active)
            {
                CompleteWave();
            }
        }
        
        private void CompleteWave()
        {
            State = WaveState.Complete;
            OnWaveCompleted?.Invoke(CurrentWave);
        }

        public void OnHealthChanged(float currentHealth, float maxHealth, float delta)
        {
        }

        public void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth)
        {
        }

        public void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            RegisterEnemyKilled();
        }
    }
}

