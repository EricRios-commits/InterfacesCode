using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    [System.Serializable]
    public class SoundEvent
    {
        public string eventName;
        public AudioClip baseAudioClip;
        public List<AudioClip> audioClips = new List<AudioClip>();
        [Range(0f, 1f)] public float volume = 1f;
    }

    [SerializeField] private List<SoundEvent> soundEvents = new List<SoundEvent>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(string eventName)
    {
        SoundEvent soundEvent = soundEvents.Find(s => s.eventName == eventName);

        if (soundEvent != null)
        {
            // Reproducir clip base si existe
            if (soundEvent.baseAudioClip != null)
            {
                audioSource.PlayOneShot(soundEvent.baseAudioClip, soundEvent.volume);
            }

            // Reproducir clip aleatorio si existen
            if (soundEvent.audioClips.Count > 0)
            {
                AudioClip randomClip = soundEvent.audioClips[Random.Range(0, soundEvent.audioClips.Count)];
                audioSource.PlayOneShot(randomClip, soundEvent.volume);
            }
        }
        else
        {
            Debug.LogWarning($"Sonido '{eventName}' no encontrado en {gameObject.name}");
        }
    }

    // Atajos para eventos comunes (opcional)
    public void PlayFootstep() => PlaySound("Footstep");
    public void PlayLightAttack() => PlaySound("LightAttack");
    public void PlayHeavyAttack() => PlaySound("HeavyAttack");
    public void PlayDeath() => PlaySound("Death");
    public void PlayHit() => PlaySound("Hit");
}
