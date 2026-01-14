using System;
using UnityEngine;

namespace Behavior
{
    /// <summary>
    /// Raycast-based sensor that periodically casts a ray from an origin in the origin's forward direction
    /// and reports the hit GameObject as the current target.
    /// Implements ISensor.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class RaycastSensor : Sensor
    {
        [Tooltip("How often (seconds) the raycast is performed.")]
        [SerializeField] private float updateInterval = 0.1f;

        [Tooltip("Maximum distance of the raycast.")]
        [SerializeField] private float maxDistance = 10f;

        [Tooltip("Which layers contain valid target objects.")]
        [SerializeField] private LayerMask targetMask = ~0;

        [Tooltip("Which layers will interrupt the raycast (block targets).")]
        [SerializeField] private LayerMask interruptMask = 0;

        [Tooltip("Optional transform to use as ray origin. If null, this GameObject's transform is used.")]
        [SerializeField] private Transform origin;

        [Tooltip("Optional local offset applied to the origin position before casting.")]
        [SerializeField] private Vector3 localOriginOffset = Vector3.zero;

        private Coroutine _loopCoroutine;

        private void Awake()
        {
            if (origin == null)
                origin = transform;
        }

        private void OnEnable()
        {
            _loopCoroutine = StartCoroutine(RaycastLoop());
        }

        private void OnDisable()
        {
            if (_loopCoroutine != null)
                StopCoroutine(_loopCoroutine);
            _loopCoroutine = null;
        }

        public GameObject GetClosestTarget()
        {
            return base.GetClosestTarget();
        }

        private System.Collections.IEnumerator RaycastLoop()
        {
            var wait = new WaitForSeconds(Mathf.Max(0.01f, updateInterval));
            while (true)
            {
                DoRaycast();
                yield return wait;
            }
        }

        private void DoRaycast()
        {
            var originPos = origin.position + origin.TransformDirection(localOriginOffset);
            var dir = origin.forward;
            RaycastHit hit;
            int combinedMask = targetMask.value | interruptMask.value;
            bool gotHit = Physics.Raycast(originPos, dir, out hit, maxDistance, combinedMask);
            GameObject newTarget = null;
            if (gotHit)
            {
                int hitLayerBit = 1 << hit.collider.gameObject.layer;
                if ((interruptMask.value & hitLayerBit) != 0)
                {
                    newTarget = null;
                }
                else if ((targetMask.value & hitLayerBit) != 0)
                {
                    newTarget = hit.collider.gameObject;
                }
            }
            SetTarget(newTarget);
        }
    }
}
