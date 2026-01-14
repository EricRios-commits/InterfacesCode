using Combat;
using UnityEngine;

namespace Waves
{
    public class HealthDifficultyModifier : IWaveDifficultyModifier
    {
        public void ApplyToEnemy(GameObject enemy, float multiplier)
        {
            var health = enemy.GetComponent<HealthComponent>();
            if (health != null)
            {
                float newMaxHealth = health.MaxHealth * multiplier;
                health.SetMaxHealth(newMaxHealth);
            }
        }
    }
}

