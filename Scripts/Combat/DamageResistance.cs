using System;
using System.Collections.Generic;

namespace Combat
{
    /// <summary>
    /// Damage modifier that applies resistance/weakness to specific damage types.
    /// Implements Strategy Pattern for damage modification.
    /// </summary>
    [Serializable]
    public class DamageResistance : IDamageModifier
    {
        private Dictionary<DamageType, float> resistanceMultipliers;

        public DamageResistance()
        {
            resistanceMultipliers = new Dictionary<DamageType, float>();
        }

        /// <summary>
        /// Set resistance multiplier for a damage type.
        /// 1.0 = normal damage, 0.5 = 50% resistance, 1.5 = 50% weakness
        /// </summary>
        public void SetResistance(DamageType type, float multiplier)
        {
            resistanceMultipliers[type] = multiplier;
        }

        public float GetResistance(DamageType type)
        {
            return resistanceMultipliers.TryGetValue(type, out float multiplier) ? multiplier : 1.0f;
        }

        public float ModifyDamage(DamageInfo damageInfo)
        {
            float multiplier = GetResistance(damageInfo.Type);
            return damageInfo.Amount * multiplier;
        }
    }
}

