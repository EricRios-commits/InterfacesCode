using Combat;
using UnityEngine;
using UnityEngine.AI;
using Utility;

namespace Behavior.Enemy
{
    /// <summary>
    /// Handles enemy death behavior and state management.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class EnemyDeathHandler : MonoBehaviour, IHealthObserver, IResettable
    {
        [Header("Death Settings")]
        [SerializeField] private float deathDisableDelay = 5f;
        [SerializeField] private bool usePooling = true;
        
        [Header("Death Behavior Event")]
        [SerializeField] private OnActorDeath onDeathEvent;
        
        [Header("Components to Disable")]
        [SerializeField] private bool disableColliders = true;
        [SerializeField] private bool disableNavMeshAgent = true;
        [SerializeField] private bool disableAnimator = false;
        
        private HealthComponent healthComponent;
        private Collider[] colliders;
        private NavMeshAgent navMeshAgent;
        private Animator animator;
        private bool isDead = false;
        private float deathTimer = 0f;
        
        public bool IsDead => isDead;
        
        private void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            colliders = GetComponentsInChildren<Collider>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
        
        private void OnEnable()
        {
            if (healthComponent != null)
            {
                healthComponent.AddObserver(this);
            }
        }
        
        private void OnDisable()
        {
            if (healthComponent != null)
            {
                healthComponent.RemoveObserver(this);
            }
        }
        
        private void Update()
        {
            if (isDead && usePooling)
            {
                deathTimer += Time.deltaTime;
                if (deathTimer >= deathDisableDelay)
                {
                    ReturnToPool();
                }
            }
        }
        
        public void OnHealthChanged(float currentHealth, float maxHealth, float delta)
        {
            // Not needed for death handling
        }
        
        public void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth)
        {
            // Not needed for death handling
        }
        
        public void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            if (isDead) return;
            isDead = true;
            deathTimer = 0f;
            onDeathEvent?.SendEventMessage(gameObject, finalDamage);
            DisableGameplayComponents();
        }
        
        private void DisableGameplayComponents()
        {
            if (disableColliders && colliders != null)
            {
                foreach (var col in colliders)
                {
                    if (col != null)
                        col.enabled = false;
                }
            }
            if (disableNavMeshAgent && navMeshAgent != null && navMeshAgent.enabled)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.enabled = false;
            }
            if (disableAnimator && animator != null)
            {
                animator.enabled = false;
            }
        }
        
        private void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Reset the enemy to its initial state for reuse.
        /// Called by the spawner when an enemy is retrieved from the pool.
        /// </summary>
        public void ResetState()
        {
            isDead = false;
            deathTimer = 0f;
            if (disableColliders && colliders != null)
            {
                foreach (var col in colliders)
                {
                    if (col != null)
                        col.enabled = true;
                }
            }
            if (disableNavMeshAgent && navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.isStopped = false;
            }
            if (disableAnimator && animator != null)
            {
                animator.enabled = true;
            }
            if (healthComponent != null)
            {
                healthComponent.Revive(healthComponent.MaxHealth);
            }
        }
    }
}

