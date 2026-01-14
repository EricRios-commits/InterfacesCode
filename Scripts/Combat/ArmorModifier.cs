using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Advanced damage modifier that applies armor-like flat reduction.
    /// Example of extending the system with a new modifier type.
    /// </summary>
    public class ArmorModifier : IDamageModifier
    {
        private float armorValue;
        private float armorPenetrationResistance;

        public ArmorModifier(float armor, float penetrationResistance = 1.0f)
        {
            armorValue = armor;
            armorPenetrationResistance = penetrationResistance;
        }

        public void SetArmor(float armor)
        {
            armorValue = Mathf.Max(0, armor);
        }

        public float GetArmor() => armorValue;

        /// <summary>
        /// Reduces damage by flat armor value.
        /// Certain damage types (like Explosive) may ignore some armor.
        /// </summary>
        public float ModifyDamage(DamageInfo damageInfo)
        {
            float effectiveArmor = armorValue;

            // Explosive damage ignores 50% of armor
            if (damageInfo.Type == DamageType.Explosive)
            {
                effectiveArmor *= 0.5f;
            }
            // Magic damage ignores armor entirely
            else if (damageInfo.Type == DamageType.Magic)
            {
                effectiveArmor = 0f;
            }

            // Apply armor reduction
            float reducedDamage = Mathf.Max(0, damageInfo.Amount - effectiveArmor);
            
            return reducedDamage;
        }
    }
}

