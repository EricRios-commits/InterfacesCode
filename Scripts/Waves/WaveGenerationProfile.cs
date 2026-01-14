using System.Collections.Generic;
using UnityEngine;

namespace Waves
{
    [CreateAssetMenu(menuName = "Wave System/Wave Generation Profile")]
    public class WaveGenerationProfile : ScriptableObject
    {
        [Header("Enemy Pool")]
        public List<EnemyTypeDefinition> availableEnemyTypes;
        
        [Header("Wave Composition Rules")]
        public int baseEnemyCount = 1;
        public int maxEnemyCountPerWave = 15;
        public int maxSimultaneousEnemies = 3;
        public AnimationCurve enemyCountGrowthCurve;
        
        [Header("Enemy Type Distribution")]
        public AnimationCurve basicEnemyWeightCurve;
        public AnimationCurve advancedEnemyWeightCurve;
        
        [Header("Spawn Timing")]
        public float baseSpawnInterval = 3f;
        public AnimationCurve spawnSpeedCurve;
        public float minSpawnInterval = 1f;
        
        [Header("Difficulty Scaling")]
        public float baseDifficultyMultiplier = 1.0f;
        public float difficultyGrowthRate = 0.1f;
        public AnimationCurve difficultyScalingCurve;
        
        [Header("Special Wave Rules")]
        public int eliteWaveInterval = 5;
        public float eliteWaveDifficultyBonus = 0.5f;
        public int bossWaveInterval = 10;
    }
}

