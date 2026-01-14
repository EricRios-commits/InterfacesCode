using System;
using Behavior.Enemy;
using UnityEngine;

namespace Waves
{
    [Serializable]
    public class EnemyTypeDefinition
    {
        public string typeName;
        public GameObject enemyPrefab;
        public EnemyProfile profile;
        public float baseSpawnWeight = 1f;
        public int minWaveToAppear = 1;
        public EnemyTier tier;
    }
}

