using UnityEngine;
using System;
using Utility.Timers;

namespace Behavior
{
    [RequireComponent(typeof(Collider))]
    public class ColliderSensor : Sensor
    {
        [SerializeField] private float timerInterval = 0.5f;
        [SerializeField] private LayerMask layerMask;
        
        [SerializeField] private Collider detectionCollider;

        public event Action OnTargetChanged = delegate { };
        
        private CountdownTimer _timer;
        
        
        private void Awake()
        {
            detectionCollider = GetComponent<Collider>();
            detectionCollider.isTrigger = true;
        }

        private void Start()
        {
            _timer = new CountdownTimer(timerInterval);
            _timer.OnTimerStop += () =>
            {
                SetTarget(_target);
                _timer.Start();
            };
            _timer.Start();
        }
        
        public GameObject GetClosestTarget()
        {
            return base.GetClosestTarget();
        }

        private void UpdateTargetPosition(GameObject newTarget)
        {
            SetTarget(newTarget);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (layerMask == (layerMask | (1 << other.gameObject.layer)))
            {
                UpdateTargetPosition(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (layerMask == (layerMask | (1 << other.gameObject.layer)))
            {
                UpdateTargetPosition(null);
            }
        }
    }
}
