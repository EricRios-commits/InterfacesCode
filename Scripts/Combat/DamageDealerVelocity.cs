using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Component that deals damage to IDamageable objects based on velocity.
    /// Calculates velocity only on horizontal plane (XZ) to ignore gravity effects.
    /// Only deals damage if the velocity exceeds a minimum threshold.
    /// Damage is dealt once per collision until the weapon exits and re-enters the collider.
    /// </summary>
    public class DamageDealerVelocity : DamageDealer
    {
        [Header("Velocity Settings")]
        [SerializeField] private float minimumVelocity = 2f;
        [Tooltip("Velocidad mínima para causar daño (m/s) - solo plano horizontal")]
        
        [Header("Debug")]
        [SerializeField] private bool showVelocityDebug = true;
        
        private Vector3 _previousPosition;
        private float _currentVelocity;
        private HashSet<Collider> _hitColliders = new HashSet<Collider>();
        private bool _isInitialized = false;

        private void Start()
        {
            // Inicializar posición
            _previousPosition = transform.position;
            _currentVelocity = 0f;
            _isInitialized = true;
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
                return;

            // Calcular velocidad solo en el plano horizontal (XZ) - ignorar gravedad (Y)
            Vector3 currentPosition = transform.position;
            Vector3 currentPositionXZ = new Vector3(currentPosition.x, 0f, currentPosition.z);
            Vector3 previousPositionXZ = new Vector3(_previousPosition.x, 0f, _previousPosition.z);
            
            _currentVelocity = (currentPositionXZ - previousPositionXZ).magnitude / Time.fixedDeltaTime;
            
            // Actualizar posición anterior
            _previousPosition = currentPosition;

            if (showVelocityDebug && _currentVelocity > 0.1f)
            {
                Debug.Log($"[DamageDealerVelocity] Velocidad horizontal: {_currentVelocity:F2} m/s");
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            // Verificar si ya golpeó a este collider
            if (_hitColliders.Contains(other))
                return;

            // Verificar velocidad mínima
            if (_currentVelocity < minimumVelocity)
                return;

            // Marcar este collider como golpeado
            _hitColliders.Add(other);
            
            Debug.Log($"[DamageDealerVelocity] {gameObject.name} GOLPEÓ a {other.gameObject.name} a velocidad {_currentVelocity:F1} m/s");

            // Hacer daño sin escalar con velocidad
            base.OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            // Cuando sale del trigger, permitir que pueda golpear de nuevo
            _hitColliders.Remove(other);
        }

        private void OnDisable()
        {
            // Limpiar historial al desactivar
            _hitColliders.Clear();
        }

        /// <summary>
        /// Obtiene la velocidad horizontal actual calculada del arma
        /// </summary>
        public float GetCurrentVelocity()
        {
            return _currentVelocity;
        }
    }
}

