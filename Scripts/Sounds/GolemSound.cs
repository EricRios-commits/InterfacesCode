using UnityEngine;

public class GolemSound : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private float volume = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Método llamado desde AnimationEvent del Animator
    public void PlayFootstep()
    {
        if (footstepSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(footstepSound, volume);
        }
    }
}
