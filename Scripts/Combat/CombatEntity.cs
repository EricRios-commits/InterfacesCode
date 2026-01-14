using System.Collections;
using UnityEngine;
using Utility;

namespace Combat
{
    /// <summary>
    /// Example entity that uses the health system.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class CombatEntity : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Optional: Use a stats profile to configure this entity")]
        [SerializeField] private EntityStatsProfile statsProfile;

        [Header("Entity Settings")]
        [SerializeField] private string entityName = "Combat Entity";
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private float destroyDelay = 5f;
        
        private HealthComponent healthComponent;
        private AudioSource audioSource;

        public string EntityName => entityName;
        public HealthComponent Health => healthComponent;
        public EntityStatsProfile StatsProfile => statsProfile;
        
        private void Start()
        {
            healthComponent = GetComponent<HealthComponent>();
            audioSource = GetComponent<AudioSource>();
            ApplyStatsProfile(statsProfile);
        }

        /// <summary>
        /// Apply a complete stats profile to this entity.
        /// </summary>
        private void ApplyStatsProfile(EntityStatsProfile profile)
        {
            if (profile == null) return;
            if (entityName == "Combat Entity" && !string.IsNullOrEmpty(profile.EntityTypeName))
            {
                entityName = profile.EntityTypeName;
            }
            if (profile.DeathEffectPrefab != null)
            {
                deathEffectPrefab = profile.DeathEffectPrefab;
            }
            destroyDelay = profile.DestroyDelay;
            profile.ApplyToEntity(this);
        }
        
        public void HandleDeath(DamageInfo finalDamage)
        {
            if (deathEffectPrefab != null)
            {
                var effect = PoolManager.GetObjectOfType(deathEffectPrefab);
                if (effect != null)
                {
                    effect.transform.position = transform.position;
                    effect.transform.rotation = Quaternion.identity;
                }
            }
            if (audioSource != null && statsProfile != null && statsProfile.DeathSound != null)
            {
                audioSource.PlayOneShot(statsProfile.DeathSound);
            }
        }
        
        private IEnumerator DisableAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}
