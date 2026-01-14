using System;
using UnityEngine;

namespace Behavior
{
    /// <summary>
    /// Abstract base sensor providing common functionality for concrete sensors.
    /// Use protected SetTarget(...) to update the current target and raise OnTargetChanged
    /// when the target reference or its position changes.
    /// </summary>
    public abstract class Sensor : MonoBehaviour
    {
        /// <summary>Raised when the sensor's detected target or its position changes.</summary>
        public event Action OnTargetChanged = delegate { };

        protected GameObject _target;
        protected Vector3 _lastKnownPosition;

        /// <summary>World position of the current target, or Vector3.zero when none.</summary>
        public virtual Vector3 TargetPosition => _target ? _target.transform.position : Vector3.zero;

        /// <summary>Quick check whether there's a valid target in range.</summary>
        public virtual bool IsTargetInRange => _target != null;

        /// <summary>Return the current closest target GameObject, or null if none.</summary>
        public virtual GameObject GetClosestTarget() => IsTargetInRange ? _target : null;

        /// <summary>
        /// Helper to update the sensor's target. Derived classes should call this
        /// whenever they detect a new target or want to refresh the current one.
        /// This method updates internal state and invokes OnTargetChanged if the
        /// reference changed or the target moved since last known position.
        /// </summary>
        /// <param name="newTarget">The newly-detected target, or null.</param>
        protected virtual void SetTarget(GameObject newTarget)
        {
            if (newTarget != _target || (newTarget != null && _lastKnownPosition != newTarget.transform.position))
            {
                _target = newTarget;
                _lastKnownPosition = _target ? _target.transform.position : Vector3.zero;
                OnTargetChanged?.Invoke();
            }
            else
            {
                _target = newTarget;
            }
        }
    }
}
