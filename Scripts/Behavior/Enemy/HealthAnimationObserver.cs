using Combat;
using UnityEngine;
using UnityEngine.Serialization;

namespace Behavior.Enemy
{
    public class HealthAnimationObserver : MonoBehaviour, IHealthObserver
    {
        [SerializeField] private Animator animator;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private string deathBoolName = "Death";
        
        private int DeathBool => Animator.StringToHash(deathBoolName);

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }
            healthComponent.AddObserver(this);
        }
        
        public void OnHealthChanged(float current, float max, float delta)
        {
            if (animator != null && delta > 0)
            {
                animator.SetBool(DeathBool, false);
            }
        }

        public void OnDamageTaken(DamageInfo info, float currentHealth, float maxHealth)
        {
            return;
        }

        public void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            if (animator != null)
            {
                animator.SetBool(DeathBool, true);
            }
        }
    }
}