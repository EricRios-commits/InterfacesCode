using UnityEngine;

namespace Behavior.Enemy
{
    public class EnemyWave
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private Transform[] spawnPoints;
        private Transform playerTransform;


        private void Awake()
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }
    }
}