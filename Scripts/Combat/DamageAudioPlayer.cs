using UnityEngine;
using System.Collections.Generic;

namespace Combat
{
    /// <summary>
    /// Simple IHealthObserver implementation that plays random audio clips when damage is taken.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DamageAudioPlayer : MonoBehaviour, IHealthObserver
    {
        [SerializeField]
        [Tooltip("List of audio clips to play randomly when damage is taken")]
        private List<AudioClip> damageAudioClips = new List<AudioClip>();

        [SerializeField]
        [Tooltip("Volume multiplier for damage sounds")]
        [Range(0f, 1f)]
        private float volume = 1f;
        
        [SerializeField] private HealthComponent healthComponent;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            if (healthComponent == null)
            {
                healthComponent = GetComponent<HealthComponent>();
            }
            if (healthComponent != null)
            {
                healthComponent.AddObserver(this);
            }
        }

        public void OnHealthChanged(float currentHealth, float maxHealth, float delta)
        {
        }

        public void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth)
        {
            PlayRandomDamageClip();
        }

        public void OnDeath(GameObject dead, DamageInfo finalDamage)
        {
            PlayRandomDamageClip();
        }

        private void PlayRandomDamageClip()
        {
            if (damageAudioClips == null || damageAudioClips.Count == 0)
            {
                Debug.LogWarning($"DamageAudioPlayer on {gameObject.name} has no audio clips assigned!");
                return;
            }
            int randomIndex = Random.Range(0, damageAudioClips.Count);
            AudioClip clipToPlay = damageAudioClips[randomIndex];
            if (clipToPlay != null)
            {
                audioSource.PlayOneShot(clipToPlay, volume);
            }
        }
    }
}

