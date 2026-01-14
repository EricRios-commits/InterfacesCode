using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Scriptable Object that defines complete entity stats including health and damage modifiers.
    /// </summary>
    [CreateAssetMenu(fileName = "New Entity Stats", menuName = "Combat/Entity Stats Profile")]
    public class EntityStatsProfile : ScriptableObject
    {
        [Header("Entity Identity")]
        [SerializeField] private string entityTypeName = "Entity";
        [TextArea(2, 4)]
        [SerializeField] private string description = "Describe this entity type...";

        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool startsInvulnerable = false;

        [Header("Stagger Settings")]
        [Tooltip("Number of hits required to trigger a stagger. Set to 0 to disable stagger.")]
        [SerializeField] private int hitsToStagger = 3;

        [Header("Resistance Profile")]
        [SerializeField] private DamageResistanceProfile resistanceProfile;

        [Header("Additional Modifiers")]
        [SerializeField] private bool useCriticalHitModifier = false;
        [SerializeField] private float criticalChance = 0.1f;
        [SerializeField] private float criticalMultiplier = 2.0f;

        [Header("Visual/Audio")]
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private float destroyDelay = 2f;

        public string EntityTypeName => entityTypeName;
        public string Description => description;
        public float MaxHealth => maxHealth;
        public bool StartsInvulnerable => startsInvulnerable;
        public int HitsToStagger => hitsToStagger;
        public DamageResistanceProfile ResistanceProfile => resistanceProfile;
        public bool UseCriticalHitModifier => useCriticalHitModifier;
        public float CriticalChance => criticalChance;
        public float CriticalMultiplier => criticalMultiplier;
        public GameObject DeathEffectPrefab => deathEffectPrefab;
        public AudioClip DeathSound => deathSound;
        public float DestroyDelay => destroyDelay;

        /// <summary>
        /// Apply all stats from this profile to an entity.
        /// </summary>
        public void ApplyToEntity(CombatEntity entity)
        {
            if (entity == null) return;
            var health = entity.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.SetMaxHealth(maxHealth);
                health.SetInvulnerable(startsInvulnerable);
                if (resistanceProfile != null)
                {
                    resistanceProfile.ApplyToHealthComponent(health);
                }
                if (useCriticalHitModifier)
                {
                    var critModifier = new CriticalHitModifier(criticalChance, criticalMultiplier);
                    health.AddDamageModifier(critModifier);
                }
            }
        }
    }
}
