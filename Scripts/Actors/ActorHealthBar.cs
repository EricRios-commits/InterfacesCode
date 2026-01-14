using Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Actors
{
    /// <summary>
    /// Actor-specific health bar implementation using a UI Slider.
    /// Implements IHealthObserver following Observer Pattern.
    /// Single Responsibility: Only manages visual representation of health via slider.
    /// </summary>
    public class ActorHealthBar : MonoBehaviour, IHealthObserver
    {
        [Header("Health Component Reference")]
        [SerializeField] private HealthComponent healthComponent;

        [Header("UI References")]
        [SerializeField] private Slider healthSlider;
        
        [Header("Optional Visual Settings")]
        [SerializeField] private bool hideWhenFull;
        [SerializeField] private bool hideWhenDead = true;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Optional Fill Color")]
        [SerializeField] private bool useColorGradient = true;
        [SerializeField] private Image fillImage;
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color damagedColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private float criticalThreshold = 0.25f;
        
        private IHealthBarView _view;

        private void Awake()
        {
            InitializeView();
        }

        private void Start()
        {
            RegisterToHealthComponent();
            InitializeHealthDisplay();
        }

        private void OnDestroy()
        {
            UnregisterFromHealthComponent();
        }

        /// <summary>
        /// Initialize the view strategy based on available components.
        /// Demonstrates Strategy Pattern for different visualization approaches.
        /// </summary>
        private void InitializeView()
        {
            if (healthSlider != null)
            {
                _view = new SliderHealthBarView(healthSlider, fillImage, canvasGroup, 
                    useColorGradient, healthyColor, damagedColor, criticalColor, 
                    criticalThreshold, hideWhenFull, hideWhenDead);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: No health slider assigned to ActorHealthBar!");
            }
        }

        private void RegisterToHealthComponent()
        {
            if (healthComponent == null)
            {
                healthComponent = GetComponentInParent<HealthComponent>();
            }

            if (healthComponent != null)
            {
                healthComponent.AddObserver(this);
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: No HealthComponent found for ActorHealthBar!");
            }
        }

        private void UnregisterFromHealthComponent()
        {
            if (healthComponent != null)
            {
                healthComponent.RemoveObserver(this);
            }
        }

        private void InitializeHealthDisplay()
        {
            if (healthComponent != null && _view != null)
            {
                _view.UpdateHealth(healthComponent.CurrentHealth, healthComponent.MaxHealth);
            }
        }

        private void Update()
        {
            if (Camera.main != null)
            {
                var direction = transform.position - Camera.main.transform.position;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        
        public void OnHealthChanged(float currentHealth, float maxHealth, float delta)
        {
            _view?.UpdateHealth(currentHealth, maxHealth);
        }

        public void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth)
        {
            _view?.UpdateHealth(currentHealth, maxHealth);
        }

        public void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            _view?.OnActorDeath();
        }

        /// <summary>
        /// Public method to manually set the health component reference.
        /// Follows Dependency Injection principle.
        /// </summary>
        public void SetHealthComponent(HealthComponent component)
        {
            UnregisterFromHealthComponent();
            healthComponent = component;
            RegisterToHealthComponent();
            InitializeHealthDisplay();
        }
    }
}