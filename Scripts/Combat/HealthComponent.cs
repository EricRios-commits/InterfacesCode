using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Combat
{
    /// <summary>
    /// Core health management component.
    /// Follows Single Responsibility Principle - only manages health.
    /// Follows Open/Closed Principle - extensible through observers and modifiers.
    /// </summary>
    public class HealthComponent : MonoBehaviour, IDamageable, IResettable
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        [SerializeField] private bool invulnerable = false;

        [Header("Damage Modifiers")]
        [SerializeField] private bool useResistances = true;

        [Header("Debug")]
        [SerializeField] private float debugDamageAmount = 10f;
        [SerializeField] private DamageType debugDamageType = DamageType.Slash;

        private DamageResistance damageResistance;
        private List<IHealthObserver> observers = new List<IHealthObserver>();
        private List<IDamageModifier> damageModifiers = new List<IDamageModifier>();
        private bool isDead = false;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => !isDead && currentHealth > 0;
        public bool IsInvulnerable => invulnerable;

        private void Awake()
        {
            currentHealth = maxHealth;
            damageResistance = new DamageResistance();
            if (useResistances)
            {
                AddDamageModifier(damageResistance);
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (!IsAlive || invulnerable)
                return;
            float modifiedDamage = CalculateModifiedDamage(damageInfo);
            if (modifiedDamage <= 0)
                return;
            float previousHealth = currentHealth;
            currentHealth = Mathf.Max(0, currentHealth - modifiedDamage);
            float actualDelta = previousHealth - currentHealth;
            NotifyDamageTaken(damageInfo, actualDelta);
            NotifyHealthChanged(-actualDelta);
            if (currentHealth <= 0 && !isDead)
            {
                Die(damageInfo);
            }
        }

        private float CalculateModifiedDamage(DamageInfo damageInfo)
        {
            float damage = damageInfo.Amount;
            foreach (var modifier in damageModifiers)
            {
                damage = modifier.ModifyDamage(new DamageInfo(
                    damage, 
                    damageInfo.Type, 
                    damageInfo.Instigator, 
                    damageInfo.HitPoint, 
                    damageInfo.HitDirection
                ));
            }
            return damage;
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0)
                return;
            float previousHealth = currentHealth;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            float actualDelta = currentHealth - previousHealth;
            NotifyHealthChanged(actualDelta);
        }

        public void SetMaxHealth(float newMaxHealth)
        {
            float healthPercentage = currentHealth / maxHealth;
            maxHealth = Mathf.Max(1, newMaxHealth);
            currentHealth = maxHealth * healthPercentage;
            NotifyHealthChanged(0);
        }

        public void SetInvulnerable(bool value)
        {
            invulnerable = value;
        }

        private void Die(DamageInfo finalDamage)
        {
            isDead = true;
            NotifyDeath(finalDamage);
        }

        public void Revive(float healthAmount)
        {
            if (!isDead)
                return;
            isDead = false;
            currentHealth = Mathf.Min(healthAmount, maxHealth);
            NotifyHealthChanged(currentHealth);
        }
        
        public void AddObserver(IHealthObserver observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void RemoveObserver(IHealthObserver observer)
        {
            observers.Remove(observer);
        }

        private void NotifyHealthChanged(float delta)
        {
            foreach (var observer in observers)
            {
                observer.OnHealthChanged(currentHealth, maxHealth, delta);
            }
        }

        private void NotifyDamageTaken(DamageInfo damageInfo, float actualDamage)
        {
            DamageInfo actualDamageInfo = new DamageInfo(
                actualDamage,
                damageInfo.Type,
                damageInfo.Instigator,
                damageInfo.HitPoint,
                damageInfo.HitDirection
            );
            foreach (var observer in observers)
            {
                Debug.Log("Notifying observer of damage taken: " + observer);
                observer.OnDamageTaken(actualDamageInfo, currentHealth, maxHealth);
            }
        }

        private void NotifyDeath(DamageInfo finalDamage)
        {
            foreach (var observer in observers)
            {
                observer.OnDeath(gameObject, finalDamage);
            }
        }

        public void AddDamageModifier(IDamageModifier modifier)
        {
            if (!damageModifiers.Contains(modifier))
            {
                damageModifiers.Add(modifier);
            }
        }

        public void RemoveDamageModifier(IDamageModifier modifier)
        {
            damageModifiers.Remove(modifier);
        }

        public void SetDamageResistance(DamageType type, float multiplier)
        {
            damageResistance.SetResistance(type, multiplier);
        }


        #region Debug Methods

        /// <summary>
        /// Debug method to test taking damage from the inspector.
        /// Configure debugDamageAmount and debugDamageType in the inspector, then call this method.
        /// </summary>
        [ContextMenu("Debug: Take Damage")]
        public void DebugTakeDamage()
        {
            var damageInfo = new DamageInfo(
                debugDamageAmount,
                debugDamageType,
                gameObject, // Self as instigator for debug
                transform.position,
                Vector3.forward
            );
            TakeDamage(damageInfo);
            Debug.Log($"[HealthComponent Debug] Took {debugDamageAmount} {debugDamageType} damage. Health: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Debug method to fully heal from the inspector.
        /// </summary>
        [ContextMenu("Debug: Full Heal")]
        public void DebugFullHeal()
        {
            Heal(maxHealth);
            Debug.Log($"[HealthComponent Debug] Fully healed. Health: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Debug method to kill instantly from the inspector.
        /// </summary>
        [ContextMenu("Debug: Kill")]
        public void DebugKill()
        {
            var damageInfo = new DamageInfo(
                currentHealth + 1000f,
                DamageType.Magic,
                gameObject,
                transform.position,
                Vector3.zero
            );
            TakeDamage(damageInfo);
            Debug.Log($"[HealthComponent Debug] Killed. Health: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Debug method to revive from the inspector.
        /// </summary>
        [ContextMenu("Debug: Revive")]
        public void DebugRevive()
        {
            Revive(maxHealth);
            Debug.Log($"[HealthComponent Debug] Revived. Health: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Debug method to toggle invulnerability from the inspector.
        /// </summary>
        [ContextMenu("Debug: Toggle Invulnerable")]
        public void DebugToggleInvulnerable()
        {
            invulnerable = !invulnerable;
            Debug.Log($"[HealthComponent Debug] Invulnerable: {invulnerable}");
        }

        #endregion

        public float GetDamageResistance(DamageType type)
        {
            return damageResistance.GetResistance(type);
        }
        
        /// <summary>
        /// Reset the health component to its initial state.
        /// Used when returning objects to a pool for reuse.
        /// </summary>
        public void ResetState()
        {
            isDead = false;
            currentHealth = maxHealth;
            invulnerable = false;
            NotifyHealthChanged(currentHealth);
        }
    }
}
