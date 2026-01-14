using UnityEngine;

namespace Combat
{
  /// <summary>
  /// Component that spawns particle effects when a weapon hits a target.
  /// Detects collision/trigger contacts and plays particles at the impact point.
  /// Works with weapons that have the DamageDealer on the parent and the hitbox collider on a child.
  /// </summary>
  public class WeaponParticleEffect : MonoBehaviour
  {
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem impactParticlePrefab;
    [SerializeField] private bool enableParticles = true;
    [SerializeField] private float particleLifetime = 2f;
    [SerializeField] private float particleScale = 1.5f;

    [Header("Hitbox Configuration")]
    [SerializeField] private Transform hitboxTransform;
    [Tooltip("Si está vacío, buscará automáticamente el primer collider en los hijos")]

    [Header("Debug")]
    [SerializeField] private bool logParticleSpawn = true;

    private DamageDealer damageDealer;
    private Collider hitboxCollider;

    private void Awake()
    {
      damageDealer = GetComponent<DamageDealer>();
      if (damageDealer == null)
      {
        Debug.LogError($"[WeaponParticleEffect] No se encontró DamageDealer en {gameObject.name}!");
        return;
      }

      // Si no se asignó el hitbox, buscar automáticamente en los hijos
      if (hitboxTransform == null)
      {
        hitboxCollider = GetComponentInChildren<Collider>();
        if (hitboxCollider != null)
        {
          hitboxTransform = hitboxCollider.transform;
          Debug.Log($"[WeaponParticleEffect] Hitbox encontrado automáticamente en {hitboxCollider.gameObject.name}");
        }
      }
      else
      {
        hitboxCollider = hitboxTransform.GetComponent<Collider>();
      }

      if (hitboxCollider == null)
      {
        Debug.LogWarning($"[WeaponParticleEffect] No se encontró Collider en el hitbox de {gameObject.name}. Las partículas no se generarán.");
        return;
      }

      Debug.Log($"[WeaponParticleEffect] Iniciado en {gameObject.name} con hitbox en {hitboxTransform.gameObject.name}");
    }

    /// <summary>
    /// Spawn particle effect at the impact point when collision occurs.
    /// </summary>
    public void SpawnImpactParticles(Vector3 hitPoint, Vector3 hitNormal, float damageAmount)
    {
      if (!enableParticles || impactParticlePrefab == null)
        return;

      // Usar el punto de colisión, o la posición del hitbox si no es válido
      Vector3 spawnPosition = hitPoint != Vector3.zero ? hitPoint : hitboxTransform.position;

      // Instanciar las partículas
      ParticleSystem particleInstance = Instantiate(impactParticlePrefab, spawnPosition, Quaternion.identity);
      particleInstance.transform.localScale = Vector3.one * particleScale;

      // Orientar las partículas en la dirección del golpe
      if (hitNormal != Vector3.zero)
      {
        particleInstance.transform.rotation = Quaternion.LookRotation(hitNormal);
      }

      if (logParticleSpawn)
        Debug.Log($"<color=yellow>✨ Impacto de arma en {spawnPosition}. Daño: {damageAmount:F1}</color>");

      // Destruir después del tiempo de vida
      Destroy(particleInstance.gameObject, particleLifetime);
    }

  }
}
