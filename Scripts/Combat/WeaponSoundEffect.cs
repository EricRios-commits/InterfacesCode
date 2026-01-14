using UnityEngine;

namespace Combat
{
  /// <summary>
  /// Component that plays sound effects when a weapon hits a target.
  /// Detects collision/trigger contacts and plays sound at the impact point.
  /// Works with weapons that have the DamageDealer on the parent and the hitbox collider on a child.
  /// </summary>
  public class WeaponSoundEffect : MonoBehaviour
  {
    [Header("Sound Settings")]
    [SerializeField] private AudioClip impactSoundClip;
    [SerializeField] private bool enableSound = true;
    [SerializeField] private float soundVolume = 1f;

    private void Awake()
    {
      DamageDealer damageDealer = GetComponent<DamageDealer>();
      if (damageDealer == null)
      {
        Debug.LogError($"[WeaponSoundEffect] No se encontró DamageDealer en {gameObject.name}!");
        return;
      }

      Debug.Log($"[WeaponSoundEffect] Iniciado en {gameObject.name}");
    }

    /// <summary>
    /// Play impact sound when weapon hits a target.
    /// </summary>
    public void PlayImpactSoundEffect(Vector3 hitPoint, float damageAmount)
    {
      if (!enableSound || impactSoundClip == null)
        return;

      PlayImpactSound(hitPoint);
      Debug.Log($"<color=cyan>🔊 Sonido de impacto en {hitPoint}. Daño: {damageAmount:F1}</color>");
    }

    /// <summary>
    /// Play impact sound at the hit location.
    /// </summary>
    private void PlayImpactSound(Vector3 soundPosition)
    {
      AudioSource.PlayClipAtPoint(impactSoundClip, soundPosition, soundVolume);
    }
  }
}
