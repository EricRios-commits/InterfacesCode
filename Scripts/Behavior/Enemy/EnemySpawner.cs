using System.Collections.Generic;
using Combat;
using UnityEngine;
using Utility;

namespace Behavior.Enemy
{
    /// <summary>
    /// Spawns and manages enemy instances.
    /// Follows Single Responsibility Principle - only manages spawning and tracking.
    /// Uses PoolManager for centralized object pooling.
    /// </summary>
    public class EnemySpawner : MonoBehaviour, IHealthObserver
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform[] spawnPoints;
        
        [Header("Pooling Settings")]
        [SerializeField] private bool usePooling = true;
        [SerializeField] private int poolSize = 10;
        
        private List<GameObject> activeEnemies = new List<GameObject>();
        
        /// <summary>
        /// Spawns an enemy at the specified position.
        /// Uses object pooling if enabled via PoolManager, otherwise instantiates a new instance.
        /// </summary>
        public GameObject SpawnEnemy(GameObject prefab, Vector3 position)
        {
            if (prefab == null)
                return null;
            
            GameObject enemy;
            
            if (usePooling)
            {
                enemy = PoolManager.GetObjectOfType(prefab, poolSize);
                if (enemy == null)
                {
                    Debug.LogWarning($"[EnemySpawner] Pool exhausted for {prefab.name}, creating new instance");
                    enemy = Instantiate(prefab);
                }
                ResetEnemy(enemy);
                enemy.transform.position = position;
                enemy.transform.rotation = Quaternion.identity;
            }
            else
            {
                enemy = Instantiate(prefab, position, Quaternion.identity);
            }
            var health = enemy.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.AddObserver(this);
            }
            activeEnemies.Add(enemy);
            return enemy;
        }
        
        /// <summary>
        /// Resets all IResettable components on the enemy.
        /// </summary>
        private void ResetEnemy(GameObject enemy)
        {
            var resettables = enemy.GetComponents<IResettable>();
            foreach (var resettable in resettables)
            {
                resettable.ResetState();
            }
        }
        
        public Vector3 GetRandomSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
                return transform.position;
            return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        
        public void ApplyDifficultyMultiplier(GameObject enemy, float multiplier)
        {
            var health = enemy.GetComponent<HealthComponent>();
            if (health != null)
            {
                float newMaxHealth = health.MaxHealth * multiplier;
                health.SetMaxHealth(newMaxHealth);
            }
        }
        
        public int GetAliveCount()
        {
            CleanupDeadEnemies();
            return activeEnemies.Count;
        }
        
        public void CleanupDeadEnemies()
        {
            activeEnemies.RemoveAll(e => e == null);
        }
        
        private void OnEnemyDied(GameObject enemy)
        {
            activeEnemies.Remove(enemy);
        }
        
        private void OnDrawGizmos()
        {
            if (spawnPoints == null)
                return;
            Gizmos.color = Color.red;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                    Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + Vector3.up * 2f);
                }
            }
        }

        public void OnHealthChanged(float currentHealth, float maxHealth, float delta)
        {
        }

        public void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth)
        {
        }

        public void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            OnEnemyDied(dead);
        }
    }
}

