using Combat;
using UnityEngine;

namespace Behavior.Enemy
{
    public class AnimationProjectileAttack : AnimationAttack
    {
        [SerializeField] private ProjectileAttack projectileAttack;

        private GameObject currentAgent;
        private GameObject currentTarget;
        private Vector3? currentTargetPosition;
        
        public override bool Perform(GameObject agent, GameObject target)
        {
            currentAgent = agent;
            currentTarget = target;
            currentTargetPosition = null;
            return base.Perform(agent, target);
        }
        
        public override bool Perform(GameObject agent, Vector3 targetPosition)
        {
            currentAgent = agent;
            currentTarget = null;
            currentTargetPosition = targetPosition;
            return base.Perform(agent, targetPosition);
        }
        
        public void ShootProjectile()
        {
            if (projectileAttack != null && currentAgent != null)
            {
                // Prioritize GameObject target if available
                if (currentTarget != null)
                {
                    projectileAttack.Perform(currentAgent, currentTarget);
                }
                else if (currentTargetPosition.HasValue)
                {
                    projectileAttack.Perform(currentAgent, currentTargetPosition.Value);
                }
            }
        }
    }
}