using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Simple projectile component that moves forward and deactivates after lifetime.
    /// Attach this to your projectile prefabs.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : DamageDealer
    {
        private float _lifetime;
        private float _spawnTime;
        private Rigidbody _rb;
        [SerializeField] private LayerMask blockingLayers;
        private LayerMask originalDamageableLayers;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            originalDamageableLayers = damageableLayers;
        }

        /// <summary>
        /// Initialize the projectile with its settings.
        /// Called when the projectile is retrieved from the pool.
        /// </summary>
        public void Initialize(Vector3 position, Vector3 direction, float speed, float lifetime)
        {
            _lifetime = lifetime;
            _spawnTime = Time.time;
            transform.position = position;
            transform.forward = direction;
            damageableLayers = originalDamageableLayers;
            _rb.linearVelocity = direction.normalized * speed;
        }

        private void Update()
        {
            if (Time.time - _spawnTime >= _lifetime)
            {
                ReturnToPool();
            }
        }
        
        public void Reflect(LayerMask newDamageableLayers)
        {
            damageableLayers = newDamageableLayers;
            _rb.linearVelocity = -_rb.linearVelocity;
            transform.forward = _rb.linearVelocity.normalized;
        }

        protected override void OnCollisionEnter(Collision collision)
        {
            if (DealDamageOnCollision(collision))
            {
                ReturnToPool();
            }
            if (((1 << collision.gameObject.layer) & blockingLayers) != 0)
            {
                ReturnToPool();
            }
        }

        private void ReturnToPool()
        {
            _rb.linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}

