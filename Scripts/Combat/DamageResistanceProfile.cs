using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Scriptable Object that defines damage resistance and weakness profiles.
    /// Can be shared across multiple entities.
    /// </summary>
    [CreateAssetMenu(fileName = "New Resistance Profile", menuName = "Combat/Damage Resistance Profile")]
    public class DamageResistanceProfile : ScriptableObject
    {
        [Header("Profile Info")]
        [SerializeField] private string profileName = "Default Profile";
        [TextArea(2, 4)]
        [SerializeField] private string description = "Describe the resistance profile...";

        [Header("Damage Type Resistances")]
        [Tooltip("1.0 = normal damage, 0.5 = 50% resistance, 1.5 = 50% weakness, 0 = immune")]
        [SerializeField] private List<DamageTypeResistance> resistances = new List<DamageTypeResistance>();

        [Header("Armor Settings")]
        [SerializeField] private bool hasArmor = false;
        [SerializeField] private float armorValue = 0f;

        [Header("Critical Hit Settings")]
        [SerializeField] private bool canReceiveCriticalHits = true;
        [SerializeField] private float criticalHitMultiplier = 2.0f;

        public string ProfileName => profileName;
        public string Description => description;
        public bool HasArmor => hasArmor;
        public float ArmorValue => armorValue;
        public bool CanReceiveCriticalHits => canReceiveCriticalHits;
        public float CriticalHitMultiplier => criticalHitMultiplier;

        /// <summary>
        /// Get the resistance multiplier for a specific damage type.
        /// </summary>
        public float GetResistance(DamageType type)
        {
            foreach (var resistance in resistances)
            {
                if (resistance.damageType == type)
                {
                    return resistance.multiplier;
                }
            }
            return 1.0f; // Default: no resistance
        }

        /// <summary>
        /// Get all configured resistances.
        /// </summary>
        public IEnumerable<DamageTypeResistance> GetAllResistances()
        {
            return resistances;
        }

        /// <summary>
        /// Apply this profile to a health component.
        /// </summary>
        public void ApplyToHealthComponent(HealthComponent health)
        {
            if (health == null) return;
            foreach (var resistance in resistances)
            {
                health.SetDamageResistance(resistance.damageType, resistance.multiplier);
            }
            if (hasArmor && armorValue > 0)
            {
                var armorModifier = new ArmorModifier(armorValue);
                health.AddDamageModifier(armorModifier);
            }
        }

        [System.Serializable]
        public struct DamageTypeResistance
        {
            public DamageType damageType;
            [Range(0f, 3f)]
            public float multiplier;

            public DamageTypeResistance(DamageType type, float mult)
            {
                damageType = type;
                multiplier = mult;
            }
        }
    }
}

