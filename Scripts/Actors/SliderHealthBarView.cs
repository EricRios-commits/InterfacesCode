using UnityEngine;
using UnityEngine.UI;

namespace Actors
{
    /// <summary>
    /// Concrete implementation of IHealthBarView using Unity UI Slider.
    /// Follows Single Responsibility - only handles slider-based health visualization.
    /// Follows Open/Closed - can be extended for additional effects without modification.
    /// </summary>
    public class SliderHealthBarView : IHealthBarView
    {
        private readonly Slider _healthSlider;
        private readonly Image _fillImage;
        private readonly CanvasGroup _canvasGroup;
        
        private readonly bool _useColorGradient;
        private readonly Color _healthyColor;
        private readonly Color _damagedColor;
        private readonly Color _criticalColor;
        private readonly float _criticalThreshold;
        private readonly bool _hideWhenFull;
        private readonly bool _hideWhenDead;

        /// <summary>
        /// Constructor following Dependency Injection principle.
        /// All dependencies are provided explicitly.
        /// </summary>
        public SliderHealthBarView(
            Slider healthSlider,
            Image fillImage = null,
            CanvasGroup canvasGroup = null,
            bool useColorGradient = true,
            Color healthyColor = default,
            Color damagedColor = default,
            Color criticalColor = default,
            float criticalThreshold = 0.25f,
            bool hideWhenFull = false,
            bool hideWhenDead = true)
        {
            _healthSlider = healthSlider;
            _fillImage = fillImage;
            _canvasGroup = canvasGroup;
            _useColorGradient = useColorGradient;
            _healthyColor = healthyColor == default ? Color.green : healthyColor;
            _damagedColor = damagedColor == default ? Color.yellow : damagedColor;
            _criticalColor = criticalColor == default ? Color.red : criticalColor;
            _criticalThreshold = criticalThreshold;
            _hideWhenFull = hideWhenFull;
            _hideWhenDead = hideWhenDead;
            
            InitializeSlider();
        }

        private void InitializeSlider()
        {
            if (_healthSlider != null)
            {
                _healthSlider.minValue = 0f;
                _healthSlider.maxValue = 1f;
                _healthSlider.value = 1f;
            }
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            if (_healthSlider == null) return;
            float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            _healthSlider.value = healthPercentage;
            UpdateVisibility(healthPercentage);
            UpdateColor(healthPercentage);
        }

        public void OnActorDeath()
        {
            if (_hideWhenDead && _canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
            }
            else if (_hideWhenDead && _healthSlider != null)
            {
                _healthSlider.gameObject.SetActive(false);
            }
        }

        private void UpdateVisibility(float healthPercentage)
        {
            if (!_hideWhenFull) return;
            bool isFullHealth = Mathf.Approximately(healthPercentage, 1f);
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = isFullHealth ? 0f : 1f;
            }
            else if (_healthSlider != null)
            {
                _healthSlider.gameObject.SetActive(!isFullHealth);
            }
        }

        private void UpdateColor(float healthPercentage)
        {
            if (!_useColorGradient || _fillImage == null) return;
            Color targetColor;
            if (healthPercentage <= _criticalThreshold)
            {
                targetColor = _criticalColor;
            }
            else if (healthPercentage < 0.5f)
            {
                float t = (healthPercentage - _criticalThreshold) / (0.5f - _criticalThreshold);
                targetColor = Color.Lerp(_criticalColor, _damagedColor, t);
            }
            else
            {
                float t = (healthPercentage - 0.5f) / 0.5f;
                targetColor = Color.Lerp(_damagedColor, _healthyColor, t);
            }
            _fillImage.color = targetColor;
        }
    }
}

