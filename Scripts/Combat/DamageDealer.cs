using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Component that deals damage to IDamageable objects.
    /// Follows Single Responsibility Principle - only handles damage dealing.
    /// </summary>
    public class DamageDealer : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private DamageType damageType = DamageType.Slash;
        [SerializeField] private bool dealDamageOnCollision = true;
        [SerializeField] protected LayerMask damageableLayers = ~0;

        public float BaseDamage { get => baseDamage; set => baseDamage = value; }
        public DamageType DamageType { get => damageType; set => damageType = value; }

        private WeaponParticleEffect particleEffect;
        private WeaponSoundEffect soundEffect;

        protected virtual void Start()
        {
            particleEffect = GetComponent<WeaponParticleEffect>();
            soundEffect = GetComponent<WeaponSoundEffect>();
        }

        /// <summary>
        /// Deal damage to a specific target.
        /// </summary>
        protected virtual void DealDamage(IDamageable target, Vector3 hitPoint = default, Vector3 hitDirection = default)
        {
            if (target == null || !target.IsAlive)
                return;
            DamageInfo damageInfo = new DamageInfo(
                baseDamage,
                damageType,
                gameObject,
                hitPoint,
                hitDirection
            );
            target.TakeDamage(damageInfo);
            // Trigger particle effect at impact point
            if (particleEffect != null)
            {
                particleEffect.SpawnImpactParticles(hitPoint, hitDirection, baseDamage);
            }
            // Trigger sound effect at impact point
            if (soundEffect != null)
            {
                soundEffect.PlayImpactSoundEffect(hitPoint, baseDamage);
            }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            DealDamageOnCollision(collision);
        }

        protected bool DealDamageOnCollision(Collision collision)
        {
            if (!dealDamageOnCollision)
                return false;
            if (!IsInLayerMask(collision.gameObject.layer, damageableLayers))
                return false;
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector3 hitPoint = collision.contacts.Length > 0
                    ? collision.contacts[0].point
                    : collision.transform.position;
                hitPoint.y += 1.0f;
                Vector3 hitDirection = collision.contacts.Length > 0 ? collision.contacts[0].normal : Vector3.zero;
                DealDamage(damageable, hitPoint, hitDirection);
                return true;
            }
            return false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!dealDamageOnCollision)
                return;
            if (!IsInLayerMask(other.gameObject.layer, damageableLayers))
                return;
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector3 hitDirection = (other.bounds.center - transform.position).normalized;
                Vector3 hitPoint = other.bounds.center - hitDirection * other.bounds.extents.magnitude;
                hitPoint.y += 1.0f;
                DealDamage(damageable, hitPoint, hitDirection);
            }
        }

        private bool IsInLayerMask(int layer, LayerMask layerMask)
        {
            return ((1 << layer) & layerMask) != 0;
        }
    }
}
