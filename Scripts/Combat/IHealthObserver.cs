using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Observer interface for health change notifications.
    /// Implements Observer Pattern for health events.
    /// </summary>
    public interface IHealthObserver
    {
        void OnHealthChanged(float currentHealth, float maxHealth, float delta);
        void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth);
        void OnDeath(GameObject dead, DamageInfo finalDamage);
    }
}

