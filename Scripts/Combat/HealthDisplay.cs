using UnityEngine;
using UnityEngine.UI;

namespace Combat
{
    /// <summary>
    /// Example implementation of IHealthObserver for UI display.
    /// Demonstrates Observer Pattern usage.
    /// </summary>
    public class HealthDisplay : MonoBehaviour, IHealthObserver
    {
        [Header("References")]
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private Slider healthBar;
        [SerializeField] private Text healthText;

        [Header("Settings")]
        [SerializeField] private bool showPercentage = false;

        private void Start()
        {
            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }
            if (healthComponent != null)
            {
                healthComponent.AddObserver(this);
                UpdateDisplay(healthComponent.CurrentHealth, healthComponent.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            if (healthComponent != null)
            {
                healthComponent.RemoveObserver(this);
            }
        }

        public void OnHealthChanged(float currentHealth, float maxHealth, float delta)
        {
            UpdateDisplay(currentHealth, maxHealth);
        }

        public void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth)
        {
            throw new System.NotImplementedException();
        }

        public void OnDamageTaken(DamageInfo damageInfo)
        {
           // Debug.Log($"{gameObject.name} took {damageInfo.Amount} {damageInfo.Type} damage.");
        }

        public void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            Debug.Log($"{gameObject.name} died from {finalDamage.Type} damage!");
            // Handle death visuals here
        }

        private void UpdateDisplay(float currentHealth, float maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
            }
            if (healthText != null)
            {
                if (showPercentage)
                {
                    healthText.text = $"{(currentHealth / maxHealth * 100):F0}%";
                }
                else
                {
                    healthText.text = $"{currentHealth:F0} / {maxHealth:F0}";
                }
            }
        }
    }
}

