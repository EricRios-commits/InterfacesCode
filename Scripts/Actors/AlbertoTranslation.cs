using UnityEngine;
using Waves;

namespace Actors
{
    public class AlbertoTranslation : MonoBehaviour
    {
        [SerializeField]
        private AlbertoTrigger notifier;

        [SerializeField] private string idleAnimationTrigger = "TrIdle";
        [SerializeField] private string sitAnimationTrigger = "TrSit";
        [SerializeField]
        private Transform targetPosition;
        [SerializeField]
        private Transform playerTransform;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        [SerializeField]
        private Animator animator;

        [SerializeField] private GameObject textBox;

        void Start()
        {
            animator.SetTrigger(sitAnimationTrigger);
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            notifier.TriggerEnabled += TranslateAlbertoToArena;
            notifier.OnTriggerActivated += TranslateAlbertoToThrone;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void TranslateAlbertoToArena()
        {
            transform.position = targetPosition.position;
            transform.LookAt(playerTransform);
            animator.SetTrigger(idleAnimationTrigger);
            ToggleTextBox(true);
        }

        private void TranslateAlbertoToThrone()
        {
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            animator.SetTrigger(sitAnimationTrigger);
            ToggleTextBox(false);
        }
    
        private void ToggleTextBox(bool state)
        {
            if (textBox != null)
                textBox.SetActive(state);
        }
    }
}
