using System.Collections.Generic;
using System.Linq;

namespace Waves
{
    public class GeneratedWaveData
    {
        public int WaveNumber;
        public List<EnemySpawnEntry> EnemiesToSpawn;
        public int MaxSimultaneous;
        public float SpawnInterval;
        public float DifficultyMultiplier;
        public WaveType Type;
        
        public int TotalEnemyCount => EnemiesToSpawn.Sum(e => e.Count);
    }
}

