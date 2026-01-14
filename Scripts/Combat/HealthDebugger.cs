using System.Collections;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Debug component that observes health changes and displays visual feedback in VR.
    /// Shows damage with color flashes and visual effects.
    /// </summary>
    [RequireComponent(typeof(HealthComponent))]
    public class HealthDebugger : MonoBehaviour, IHealthObserver
    {
        [Header("Visual Feedback Settings")]
        [SerializeField] private bool enableVisualFeedback = true;
        [SerializeField] private Color damageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        [SerializeField] private float flashDuration = 0.3f;
        [SerializeField] private bool scaleOnDamage = true;
        [SerializeField] private float damageScaleFactor = 1.2f;

        [Header("Death Settings")]
        [SerializeField] private bool disableOnDeath = true;
        [SerializeField] private float deathDelay = 1f;
        
        [Header("Debug Settings")]
        [SerializeField] private bool logHealthChanges = true;
        [SerializeField] private bool logDeath = true;
        [SerializeField] private bool showPercentage = true;

        private HealthComponent healthComponent;
        private Renderer[] renderers;
        private Color[] originalColors;
        private Material[] materials;
        private Vector3 originalScale;
        private bool isFlashing = false;

        private void Awake()
        {
            healthComponent = GetComponent<HealthComponent>();
            if (healthComponent == null)
            {
                Debug.LogError($"[HealthDebugger] No se encontró HealthComponent en {gameObject.name}!");
                return;
            }

            // Obtener todos los renderers para cambiar color
            renderers = GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                materials = new Material[renderers.Length];
                originalColors = new Color[renderers.Length];
                
                for (int i = 0; i < renderers.Length; i++)
                {
                    materials[i] = renderers[i].material;
                    originalColors[i] = materials[i].color;
                }
            }

            originalScale = transform.localScale;

            // Registrarse como observador
            healthComponent.AddObserver(this);
            Debug.Log($"[HealthDebugger] Iniciado en {gameObject.name}. Salud inicial: {healthComponent.CurrentHealth}/{healthComponent.MaxHealth}");
        }

        private void OnDestroy()
        {
            // Desregistrarse cuando se destruye
            if (healthComponent != null)
            {
                healthComponent.RemoveObserver(this);
            }
        }

        public void OnHealthChanged(float currentHealth, float maxHealth, float delta)
        {
            string deltaText = delta > 0 ? $"+{delta:F1}" : $"{delta:F1}";
            string percentageText = showPercentage ? $" ({(currentHealth / maxHealth * 100):F0}%)" : "";
            
            if (delta < 0)
            {
                if (logHealthChanges)
                    Debug.Log($"<color=red>💔 [{gameObject.name}] DAÑO RECIBIDO: {deltaText} HP | Salud: {currentHealth:F1}/{maxHealth:F1}{percentageText}</color>");
                
                if (enableVisualFeedback)
                {
                    StartCoroutine(FlashColor(damageColor));
                    if (scaleOnDamage)
                        StartCoroutine(ScalePulse());
                }
            }
            else if (delta > 0)
            {
                if (logHealthChanges)
                    Debug.Log($"<color=green>💚 [{gameObject.name}] CURACIÓN: {deltaText} HP | Salud: {currentHealth:F1}/{maxHealth:F1}{percentageText}</color>");
                
                if (enableVisualFeedback)
                    StartCoroutine(FlashColor(healColor));
            }
        }

        public void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth)
        {
            string instigatorName = damageInfo.Instigator != null ? damageInfo.Instigator.name : "Desconocido";
            string percentageText = showPercentage ? $" ({(currentHealth / maxHealth * 100):F0}%)" : "";
            
            if (logHealthChanges)
                Debug.Log($"<color=orange>⚔️ [{gameObject.name}] DAÑO RECIBIDO | Tipo: {damageInfo.Type} | Cantidad: {damageInfo.Amount:F1} | Causado por: {instigatorName} | Salud: {currentHealth:F1}/{maxHealth:F1}{percentageText}</color>");
        }

        public void OnDamageTaken(DamageInfo damageInfo)
        {
            string instigatorName = damageInfo.Instigator != null ? damageInfo.Instigator.name : "Desconocido";
            
            if (logHealthChanges)
                Debug.Log($"<color=orange>⚔️ [{gameObject.name}] DAÑO RECIBIDO | Tipo: {damageInfo.Type} | Cantidad: {damageInfo.Amount:F1} | Causado por: {instigatorName}</color>");
        }

        public void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            string instigatorName = finalDamage.Instigator != null ? finalDamage.Instigator.name : "Desconocido";
            
            if (logDeath)
                Debug.Log($"<color=red>☠️ [{gameObject.name}] MUERTE | Tipo de daño: {finalDamage.Type} | Causado por: {instigatorName} | Daño final: {finalDamage.Amount:F1}</color>");

            if (enableVisualFeedback)
            {
                StartCoroutine(DeathEffect());
            }
        }

        private IEnumerator FlashColor(Color flashColor)
        {
            if (isFlashing || materials == null || materials.Length == 0)
                yield break;

            isFlashing = true;

            // Cambiar a color de flash
            foreach (var mat in materials)
            {
                if (mat != null)
                    mat.color = flashColor;
            }

            yield return new WaitForSeconds(flashDuration);

            // Restaurar colores originales
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                    materials[i].color = originalColors[i];
            }

            isFlashing = false;
        }

        private IEnumerator ScalePulse()
        {
            Vector3 targetScale = originalScale * damageScaleFactor;
            float elapsed = 0f;
            float duration = flashDuration * 0.5f;

            // Aumentar escala
            while (elapsed < duration)
            {
                transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            // Volver a escala original
            while (elapsed < duration)
            {
                transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;
        }

        private IEnumerator DeathEffect()
        {
            // Flash rojo intenso
            foreach (var mat in materials)
            {
                if (mat != null)
                    mat.color = Color.red;
            }

            // Reducir escala progresivamente
            float elapsed = 0f;
            Vector3 targetScale = Vector3.zero;

            while (elapsed < deathDelay)
            {
                transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / deathDelay);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (disableOnDeath)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
