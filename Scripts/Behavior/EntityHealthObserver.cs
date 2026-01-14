using UnityEngine;

namespace Combat
{
    public abstract class EntityHealthObserver : MonoBehaviour, IHealthObserver
    {
        [SerializeField] protected HealthComponent healthComponent;
        [SerializeField] protected CombatEntity combatEntity;

        protected virtual void Awake()
        {
            if (healthComponent == null)
                healthComponent = GetComponent<HealthComponent>();
            if (combatEntity == null)
                combatEntity = GetComponent<CombatEntity>();
            healthComponent?.AddObserver(this);
        }

        protected virtual void OnDestroy()
        {
            healthComponent?.RemoveObserver(this);
        }

        public abstract void OnHealthChanged(float current, float max, float delta);

        public abstract void OnDamageTaken(DamageInfo info, float currentHealth, float maxHealth);

        public virtual void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            if (combatEntity != null)
                combatEntity.HandleDeath(finalDamage);
        }
    }
}