using UnityEngine;

namespace Behavior.Enemy
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        public virtual float Cooldown => 1f;
        
        /// <summary>
        /// Performs an attack from the agent toward a target GameObject.
        /// </summary>
        public abstract bool Perform(GameObject agent, GameObject target);
        
        /// <summary>
        /// Performs an attack from the agent toward a target position.
        /// Default implementation extracts position from target GameObject if available.
        /// Override this method to provide position-based attack logic.
        /// </summary>
        public virtual bool Perform(GameObject agent, Vector3 targetPosition)
        {
            // Default implementation: create a temporary GameObject at the target position
            // for backwards compatibility, but derived classes should override this
            GameObject tempTarget = new GameObject("TempTarget");
            tempTarget.transform.position = targetPosition;
            bool result = Perform(agent, tempTarget);
            Destroy(tempTarget);
            return result;
        }
    }
}
