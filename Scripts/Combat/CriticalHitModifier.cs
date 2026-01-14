using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Damage modifier that applies critical hit multipliers.
    /// Example of extending the system with randomized damage modification.
    /// </summary>
    public class CriticalHitModifier : IDamageModifier
    {
        private float criticalChance;
        private float criticalMultiplier;
        private System.Random random;

        public CriticalHitModifier(float critChance = 0.1f, float critMultiplier = 2.0f)
        {
            criticalChance = Mathf.Clamp01(critChance);
            criticalMultiplier = Mathf.Max(1f, critMultiplier);
            random = new System.Random();
        }

        public void SetCriticalChance(float chance)
        {
            criticalChance = Mathf.Clamp01(chance);
        }

        public void SetCriticalMultiplier(float multiplier)
        {
            criticalMultiplier = Mathf.Max(1f, multiplier);
        }

        public float ModifyDamage(DamageInfo damageInfo)
        {
            float roll = (float)random.NextDouble();
            if (roll < criticalChance)
            {
                return damageInfo.Amount * criticalMultiplier;
            }
            return damageInfo.Amount;
        }
    }
}

