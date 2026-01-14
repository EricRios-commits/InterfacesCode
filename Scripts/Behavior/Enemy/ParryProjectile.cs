using Combat;
using UnityEngine;

namespace Behavior.Enemy
{
    public class ParryProjectile : MonoBehaviour, IDamageable
    {
        [SerializeField] private Projectile projectile;
        [SerializeField] private LayerMask damageableLayersAfterParry;
        
        public void TakeDamage(DamageInfo damageInfo)
        {
            projectile.Reflect(damageableLayersAfterParry);
        }

        public bool IsAlive => true; 
        public float CurrentHealth => 1f;
        public float MaxHealth => 1f;
    }
}