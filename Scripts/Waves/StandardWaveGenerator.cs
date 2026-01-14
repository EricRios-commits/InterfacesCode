using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Waves
{
    public class StandardWaveGenerator : IWaveGenerator
    {
        public GeneratedWaveData GenerateWave(int waveNumber, WaveGenerationProfile profile)
        {
            var waveData = new GeneratedWaveData
            {
                WaveNumber = waveNumber,
                Type = DetermineWaveType(waveNumber, profile),
                EnemiesToSpawn = new List<EnemySpawnEntry>()
            };
            int totalEnemies = CalculateTotalEnemies(waveNumber, profile);
            waveData.DifficultyMultiplier = CalculateDifficultyMultiplier(waveNumber, profile, waveData.Type);
            waveData.EnemiesToSpawn = GenerateEnemyComposition(waveNumber, totalEnemies, profile, waveData.Type);
            waveData.SpawnInterval = CalculateSpawnInterval(waveNumber, profile);
            waveData.MaxSimultaneous = profile.maxSimultaneousEnemies;
            return waveData;
        }
        
        public GeneratedWaveData PreviewWave(int waveNumber, WaveGenerationProfile profile)
        {
            return GenerateWave(waveNumber, profile);
        }
        
        private WaveType DetermineWaveType(int waveNumber, WaveGenerationProfile profile)
        {
            if (waveNumber % profile.bossWaveInterval == 0)
                return WaveType.Boss;
            if (waveNumber % profile.eliteWaveInterval == 0)
                return WaveType.Elite;
            return WaveType.Normal;
        }
        
        private int CalculateTotalEnemies(int waveNumber, WaveGenerationProfile profile)
        {
            float curveValue = profile.enemyCountGrowthCurve.Evaluate(waveNumber / 100f) * 10f;
            int enemyCount = Mathf.RoundToInt(profile.baseEnemyCount + curveValue);
            // Debug.Log($"Calculating total enemies for wave {waveNumber}: Base {profile.baseEnemyCount} + Curve {curveValue} = {enemyCount}");
            return Mathf.Clamp(enemyCount, profile.baseEnemyCount, profile.maxEnemyCountPerWave);
        }
        
        private float CalculateDifficultyMultiplier(int waveNumber, WaveGenerationProfile profile, WaveType type)
        {
            float baseMultiplier = profile.baseDifficultyMultiplier;
            float growthMultiplier = 1f + (waveNumber * profile.difficultyGrowthRate);
            float curveMultiplier = profile.difficultyScalingCurve.Evaluate(waveNumber);
            float finalMultiplier = baseMultiplier * growthMultiplier * curveMultiplier;
            if (type == WaveType.Elite)
                finalMultiplier += profile.eliteWaveDifficultyBonus;
            if (type == WaveType.Boss)
                finalMultiplier += profile.eliteWaveDifficultyBonus * 2f;
            return finalMultiplier;
        }
        
        private List<EnemySpawnEntry> GenerateEnemyComposition(int waveNumber, int totalEnemies, WaveGenerationProfile profile, WaveType waveType)
        {
            var composition = new List<EnemySpawnEntry>();
            var availableEnemies = profile.availableEnemyTypes
                .Where(e => e.minWaveToAppear <= waveNumber)
                .ToList();
            if (availableEnemies.Count == 0)
                return composition;
            if (waveType == WaveType.Boss)
            {
                composition.Add(SelectBossEnemy(availableEnemies));
                totalEnemies -= 1;
            }
            int remainingEnemies = totalEnemies;
            while (remainingEnemies > 0)
            {
                var selectedEnemy = SelectEnemyByWeight(waveNumber, availableEnemies, profile, waveType);
                var existing = composition.FirstOrDefault(e => e.EnemyPrefab == selectedEnemy.enemyPrefab);
                if (existing != null)
                {
                    existing.Count++;
                }
                else
                {
                    composition.Add(new EnemySpawnEntry
                    {
                        EnemyPrefab = selectedEnemy.enemyPrefab,
                        Profile = selectedEnemy.profile,
                        Count = 1,
                        Tier = selectedEnemy.tier
                    });
                }
                remainingEnemies--;
            }
            return composition;
        }
        
        private EnemyTypeDefinition SelectEnemyByWeight(int waveNumber, List<EnemyTypeDefinition> enemies, WaveGenerationProfile profile, WaveType waveType)
        {
            var weightedEnemies = enemies.Select(e => new
            {
                Enemy = e,
                Weight = CalculateAdjustedWeight(e, waveNumber, profile, waveType)
            }).ToList();
            float totalWeight = weightedEnemies.Sum(w => w.Weight);
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;
            foreach (var weighted in weightedEnemies)
            {
                currentWeight += weighted.Weight;
                if (randomValue <= currentWeight)
                    return weighted.Enemy;
            }
            return weightedEnemies.Last().Enemy;
        }
        
        private float CalculateAdjustedWeight(EnemyTypeDefinition enemy, int waveNumber, WaveGenerationProfile profile, WaveType waveType)
        {
            float baseWeight = enemy.baseSpawnWeight;
            
            switch (enemy.tier)
            {
                case EnemyTier.Basic:
                    return baseWeight * profile.basicEnemyWeightCurve.Evaluate(waveNumber);
                case EnemyTier.Advanced:
                    return baseWeight * profile.advancedEnemyWeightCurve.Evaluate(waveNumber);
                case EnemyTier.Elite:
                    return waveType == WaveType.Elite ? baseWeight * 2f : baseWeight * 0.5f;
                case EnemyTier.Boss:
                    return 0f;
                default:
                    return baseWeight;
            }
        }
        
        private EnemySpawnEntry SelectBossEnemy(List<EnemyTypeDefinition> enemies)
        {
            var bossEnemies = enemies.Where(e => e.tier == EnemyTier.Boss).ToList();
            var selectedBoss = bossEnemies.Count > 0 
                ? bossEnemies[Random.Range(0, bossEnemies.Count)]
                : enemies[Random.Range(0, enemies.Count)];
            
            return new EnemySpawnEntry
            {
                EnemyPrefab = selectedBoss.enemyPrefab,
                Profile = selectedBoss.profile,
                Count = 1,
                Tier = selectedBoss.tier
            };
        }
        
        private float CalculateSpawnInterval(int waveNumber, WaveGenerationProfile profile)
        {
            float speedMultiplier = profile.spawnSpeedCurve.Evaluate(waveNumber);
            float interval = profile.baseSpawnInterval / speedMultiplier;
            return Mathf.Max(interval, profile.minSpawnInterval);
        }
    }
}

