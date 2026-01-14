using UnityEngine;

namespace Player
{
    public class PlayerCollisionSetter : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private CharacterController characterController;
        
        private void Awake()
        {
            if (capsuleCollider == null)
            {
                capsuleCollider = GetComponent<CapsuleCollider>();
            }
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
            SyncColliders();
        }
        
        private void SyncColliders()
        {
            if (capsuleCollider != null && characterController != null)
            {
                capsuleCollider.height = characterController.height;
                capsuleCollider.radius = characterController.radius;
                capsuleCollider.center = characterController.center;
            }
            else
            {
                Debug.LogWarning("PlayerCollisionSetter: Missing CapsuleCollider or CharacterController reference.");
            }
        }
    }
}