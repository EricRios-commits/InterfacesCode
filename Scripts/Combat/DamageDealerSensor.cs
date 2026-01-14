using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Combat
{
    /// <summary>
    /// Component that deals damage to IDamageable objects based on controller sensor velocity.
    /// Uses XR InputDevice velocity sensors to determine impact force.
    /// Calculates velocity only on horizontal plane (XZ) to ignore gravity effects.
    /// Only deals damage if the velocity exceeds a minimum threshold.
    /// Damage is dealt once per collision until the weapon exits and re-enters the collider.
    /// </summary>
    public class DamageDealerSensor : DamageDealer
    {
        [Header("Velocity Settings")]
        [SerializeField] private float minimumVelocity = 0.5f;
        [Tooltip("Velocidad mínima para causar daño (m/s) - solo plano horizontal")]
        
        [Header("Controller Reference")]
        [SerializeField] private XRNode controllerNode = XRNode.RightHand;
        [Tooltip("Nodo del controlador XR (RightHand o LeftHand)")]
        
        [SerializeField] private bool useManualCalculation = true;
        [Tooltip("Si está activado, calcula la velocidad manualmente como respaldo cuando no hay sensores")]
        
        [Header("Debug")]
        [SerializeField] private bool showVelocityDebug = true;
        
        private float _currentVelocity;
        private HashSet<Collider> _hitColliders = new HashSet<Collider>();
        private InputDevice _controllerDevice;
        private bool _deviceFound = false;
        private Vector3 _previousPosition;
        private bool _isInitialized = false;

        private void Start()
        {
            base.Start();
            _currentVelocity = 0f;
            _previousPosition = transform.position;
            _isInitialized = true;
            FindController();
        }

        private void FindController()
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(controllerNode, devices);
            
            if (devices.Count > 0)
            {
                _controllerDevice = devices[0];
                _deviceFound = true;
                // Debug.Log($"[DamageDealerSensor] InputDevice encontrado: {_controllerDevice.name} en {controllerNode}");
            }
            else
            {
                // Debug.LogWarning($"[DamageDealerSensor] No se encontró InputDevice en {controllerNode}. Intentando nuevamente...");
            }
        }

        private void FixedUpdate()
        {
            if (!_isInitialized)
                return;
            if (!_deviceFound)
            {
                FindController();
                if (!_deviceFound)
                {
                    // Usar cálculo manual como respaldo
                    if (useManualCalculation)
                    {
                        CalculateVelocityManually();
                    }
                    return;
                }
            }
            Vector3 deviceVelocity;
            bool velocityObtained = false;
            if (_controllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out deviceVelocity))
            {
                // Calcular velocidad solo en el plano horizontal (XZ) - ignorar gravedad (Y)
                Vector3 velocityXZ = new Vector3(deviceVelocity.x, 0f, deviceVelocity.z);
                _currentVelocity = velocityXZ.magnitude;
                velocityObtained = true;

                if (showVelocityDebug && _currentVelocity > 0.1f)
                {
                    // Debug.Log($"[DamageDealerSensor] Velocidad del sensor XR: {_currentVelocity:F2} m/s (Vector completo: {deviceVelocity})");
                }
            }
            else
            {
                if (showVelocityDebug)
                {
                    Debug.LogWarning("[DamageDealerSensor] No se pudo obtener velocidad del InputDevice");
                }
            }
            if (!velocityObtained && useManualCalculation)
            {
                CalculateVelocityManually();
            }
            else if (!velocityObtained)
            {
                _currentVelocity = 0f;
            }
        }

        private void CalculateVelocityManually()
        {
            Vector3 currentPosition = transform.position;
            Vector3 currentPositionXZ = new Vector3(currentPosition.x, 0f, currentPosition.z);
            Vector3 previousPositionXZ = new Vector3(_previousPosition.x, 0f, _previousPosition.z);
            float distance = (currentPositionXZ - previousPositionXZ).magnitude;
            _currentVelocity = distance / Time.fixedDeltaTime;
            _previousPosition = currentPosition;
            if (showVelocityDebug && _currentVelocity > 0.1f)
            {
                // Debug.Log($"[DamageDealerSensor] Velocidad calculada manualmente: {_currentVelocity:F2} m/s (distancia: {distance:F4}m)");
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"[DamageDealerSensor] OnTriggerEnter detectado con: {other.gameObject.name}");
            if (_hitColliders.Contains(other))
            {
                // Debug.Log($"[DamageDealerSensor] Ya golpeó a {other.gameObject.name} anteriormente - IGNORADO");
                return;
            }
            // Debug.Log($"[DamageDealerSensor] Velocidad actual: {_currentVelocity:F2} m/s | Mínima requerida: {minimumVelocity:F2} m/s");
            if (_currentVelocity < minimumVelocity)
            {
                // Debug.Log($"[DamageDealerSensor] Velocidad insuficiente - IGNORADO");
                return;
            }
            _hitColliders.Add(other);
            // Debug.Log($"[DamageDealerSensor] {gameObject.name} GOLPEÓ a {other.gameObject.name} a velocidad {_currentVelocity:F1} m/s - Llamando a base.OnTriggerEnter");
            base.OnTriggerEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            _hitColliders.Remove(other);
        }

        private void OnDisable()
        {
            _hitColliders.Clear();
        }

        /// <summary>
        /// Obtiene la velocidad horizontal actual del sensor del controlador
        /// Cambia el nodo del controlador a usar (RightHand o LeftHand)
        /// </summary>
        public void SetControllerNode(XRNode node)
        {
            if (controllerNode != node)
            {
                controllerNode = node;
                _deviceFound = false;
                FindController();
            }
        }
    }
}
