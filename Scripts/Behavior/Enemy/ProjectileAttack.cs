using Combat;
using UnityEngine;
using Utility;
using Utility.Timers;

namespace Behavior.Enemy
{
    /// <summary>
    /// Enemy attack that spawns a projectile towards the target.
    /// </summary>
    public class ProjectileAttack : EnemyAttack
    {
        [Header("Projectile Settings")] [SerializeField]
        private GameObject projectilePrefab;

        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float projectileLifetime = 5f;

        [Header("Attack Settings")] [SerializeField]
        private float shotCooldown = 1f;

        [SerializeField] private Transform shotPoint;

        [Header("Pool Settings")] [SerializeField]
        private int poolSize = 10;

        public override float Cooldown => shotCooldown;
        
        private CountdownTimer shotCooldownTimer;

        private void Awake()
        {
            shotCooldownTimer = new CountdownTimer(shotCooldown);
        }

        public override bool Perform(GameObject agent, GameObject target)
        {
            if (shotCooldownTimer.IsRunning)
                return false;
            ShootAtTarget(agent, target);
            shotCooldownTimer.Reset();
            return true;
        }
        
        public override bool Perform(GameObject agent, Vector3 targetPosition)
        {
            if (shotCooldownTimer.IsRunning)
                return false;
            ShootAtPosition(agent, targetPosition);
            shotCooldownTimer.Reset();
            return true;
        }
        
        private void ShootAtTarget(GameObject agent, GameObject target)
        {
            if (projectilePrefab == null || agent == null || target == null)
                return;
            var projectile = PoolManager.GetObjectOfType(projectilePrefab, poolSize);
            if (projectile != null)
            {
                var projectileComponent = projectile.GetComponent<Projectile>();
                if (projectileComponent != null)
                {
                    var spawnPosition = shotPoint != null ? shotPoint.position : agent.transform.position;
                    Vector3 direction = (target.transform.position - spawnPosition);
                    direction.y = 0;
                    direction.Normalize();
                    projectileComponent.Initialize(spawnPosition, direction, projectileSpeed, projectileLifetime);
                }
                else
                {
                    Debug.LogWarning($"Projectile prefab '{projectilePrefab.name}' is missing Projectile component");
                }
            }
        }
        
        private void ShootAtPosition(GameObject agent, Vector3 targetPosition)
        {
            if (projectilePrefab == null || agent == null)
                return;
            var projectile = PoolManager.GetObjectOfType(projectilePrefab, poolSize);
            if (projectile != null)
            {
                var projectileComponent = projectile.GetComponent<Projectile>();
                if (projectileComponent != null)
                {
                    var spawnPosition = shotPoint != null ? shotPoint.position : agent.transform.position;
                    Vector3 direction = (targetPosition - spawnPosition);
                    direction.y = 0;
                    direction.Normalize();
                    projectileComponent.Initialize(spawnPosition, direction, projectileSpeed, projectileLifetime);
                }
                else
                {
                    Debug.LogWarning($"Projectile prefab '{projectilePrefab.name}' is missing Projectile component");
                }
            }
        }
    }
}
