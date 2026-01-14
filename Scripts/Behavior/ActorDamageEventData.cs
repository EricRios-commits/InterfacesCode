using System;
using Combat;
using Unity.Properties;

namespace Behavior
{
    /// <summary>
    /// Data container for actor damage events.
    /// Packages damage information with health context for behavior tree events.
    /// </summary>
    [Serializable, GeneratePropertyBag]
    public class ActorDamageEventData
    {
        public DamageInfo DamageInfo;
        public float CurrentHealth;
        public float MaxHealth;

        public ActorDamageEventData(DamageInfo damageInfo, float currentHealth, float maxHealth)
        {
            DamageInfo = damageInfo;
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }

        public float HealthPercent => MaxHealth > 0 ? CurrentHealth / MaxHealth : 0f;
    }
}