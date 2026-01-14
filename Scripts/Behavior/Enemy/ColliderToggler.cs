using System;
using UnityEngine;

namespace Behavior.Enemy
{
    [Serializable]
    public struct ColliderData
    {
        public string name;
        public Collider collider;
    }
    
    public class ColliderToggler : MonoBehaviour
    {
        [SerializeField] private ColliderData[] targetColliders;
        
        public void EnableCollider(string colliderName)
        {
            foreach (var collData in targetColliders)
            {
                if (collData.name == colliderName && collData.collider != null)
                {
                    collData.collider.enabled = true;
                }
            }
        }
        
        public void DisableCollider(string colliderName)
        {
            foreach (var collData in targetColliders)
            {
                if (collData.name == colliderName && collData.collider != null)
                {
                    collData.collider.enabled = false;
                }
            }
        }
    }
}