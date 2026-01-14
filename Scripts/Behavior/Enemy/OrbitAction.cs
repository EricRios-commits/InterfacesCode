using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;
using Unity.Properties;

namespace Behavior.Enemy
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "OrbitAction", story: "[Agent] orbits around [Target]", category: "Action/Navigation", id: "ada7464116926f0ae0de7e1a73e786a2")]
    public partial class OrbitAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;

        [SerializeReference] public BlackboardVariable<float> OrbitDistance = new BlackboardVariable<float>(2.0f);
        [SerializeReference] public BlackboardVariable<float> OrbitSpeed = new BlackboardVariable<float>(90.0f); // degrees per second
        [SerializeReference] public BlackboardVariable<float> HeightOffset = new BlackboardVariable<float>(0.0f);
        [SerializeReference] public BlackboardVariable<bool> Clockwise = new BlackboardVariable<bool>(false);
        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedMagnitude");

        private NavMeshAgent m_NavMeshAgent;
        private Animator m_Animator;
        private float m_CurrentSpeed;
        private float m_AngleDegrees;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_ColliderAdjustedTargetPosition;
        [CreateProperty] private float m_OriginalStoppingDistance = -1f;
        [CreateProperty] private float m_OriginalSpeed = -1f;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;
            return Initialize();
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;

            // If target moved, recalculate center and adjusted collider position
            bool boolUpdateTargetPosition = !Mathf.Approximately(m_LastTargetPosition.x, Target.Value.transform.position.x)
                || !Mathf.Approximately(m_LastTargetPosition.y, Target.Value.transform.position.y)
                || !Mathf.Approximately(m_LastTargetPosition.z, Target.Value.transform.position.z);

            if (boolUpdateTargetPosition)
            {
                m_LastTargetPosition = Target.Value.transform.position;
                m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();
            }

            float delta = Time.deltaTime;
            float dir = Clockwise.Value ? -1f : 1f;
            m_AngleDegrees += dir * OrbitSpeed.Value * delta;

            // compute desired orbit position around the (possibly collider-adjusted) target
            Vector3 center = m_ColliderAdjustedTargetPosition;
            float rad = m_AngleDegrees * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * OrbitDistance.Value;
            Vector3 desiredPosition = center + offset;
            desiredPosition.y = center.y + HeightOffset.Value;
            if (m_NavMeshAgent == null)
            {
                float distance = Vector3.Distance(Agent.Value.transform.position, desiredPosition);
                m_CurrentSpeed = SimpleMoveTowardsLocation(Agent.Value.transform, desiredPosition,
                    OrbitSpeed.Value, distance, 0.1f);
            }
            else
            {
                if (m_NavMeshAgent.isOnNavMesh)
                {
                    m_NavMeshAgent.SetDestination(desiredPosition);
                    m_CurrentSpeed = m_NavMeshAgent.velocity.magnitude;
                }
            }

            UpdateAnimatorSpeed();

            return Status.Running;
        }

        protected override void OnEnd()
        {
            UpdateAnimatorSpeed(0f);

            if (m_NavMeshAgent != null)
            {
                if (m_NavMeshAgent.isOnNavMesh)
                {
                    m_NavMeshAgent.ResetPath();
                }
                m_NavMeshAgent.speed = m_OriginalSpeed;
                m_NavMeshAgent.stoppingDistance = m_OriginalStoppingDistance;
            }

            m_NavMeshAgent = null;
            m_Animator = null;
        }

        protected override void OnDeserialize()
        {
            // Recreate navmesh agent reference and restore values if available
            if (Agent?.Value != null)
            {
                m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
                if (m_NavMeshAgent != null)
                {
                    if (m_OriginalSpeed >= 0f)
                        m_NavMeshAgent.speed = m_OriginalSpeed;
                    if (m_OriginalStoppingDistance >= 0f)
                        m_NavMeshAgent.stoppingDistance = m_OriginalStoppingDistance;

                    m_NavMeshAgent.Warp(Agent.Value.transform.position);
                }
            }

            Initialize();
        }

        private Status Initialize()
        {
            if (Agent.Value == null || Target.Value == null)
                return Status.Failure;

            m_LastTargetPosition = Target.Value.transform.position;
            m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();

            // set initial angle based on current agent position relative to target
            Vector3 dir = Agent.Value.transform.position - m_ColliderAdjustedTargetPosition;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                m_AngleDegrees = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            }
            else
            {
                m_AngleDegrees = 0f;
            }

            // NavMeshAgent setup
            m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
            if (m_NavMeshAgent != null)
            {
                if (m_NavMeshAgent.isOnNavMesh)
                {
                    m_NavMeshAgent.ResetPath();
                }

                m_OriginalSpeed = m_NavMeshAgent.speed;
                // Convert angular orbit speed (deg/s) to linear tangential speed (units/s): v = omega * r
                float angularRadPerSec = OrbitSpeed.Value * Mathf.Deg2Rad;
                float linearSpeed = angularRadPerSec * OrbitDistance.Value;
                m_NavMeshAgent.speed = linearSpeed;
                m_OriginalStoppingDistance = m_NavMeshAgent.stoppingDistance;
                m_NavMeshAgent.stoppingDistance = 0f; // orbit target point precisely

                Vector3 initialPos = m_ColliderAdjustedTargetPosition + new Vector3(Mathf.Cos(m_AngleDegrees * Mathf.Deg2Rad), 0f, Mathf.Sin(m_AngleDegrees * Mathf.Deg2Rad)) * OrbitDistance.Value;
                initialPos.y = m_ColliderAdjustedTargetPosition.y + HeightOffset.Value;
                m_NavMeshAgent.Warp(Agent.Value.transform.position);
                m_NavMeshAgent.SetDestination(initialPos);
            }

            m_Animator = Agent.Value.GetComponentInChildren<Animator>();
            UpdateAnimatorSpeed(0f);

            return Status.Running;
        }

        private Vector3 GetPositionColliderAdjusted()
        {
            // Try to use the closest point on any collider of the target
            Collider anyCollider = Target.Value.GetComponentInChildren<Collider>(includeInactive: false);
            if (anyCollider != null && anyCollider.enabled)
                return anyCollider.ClosestPoint(Agent.Value.transform.position);

            // Fallback to target transform position
            return Target.Value.transform.position;
        }

        // Local replacement for the internal NavigationUtility.SimpleMoveTowardsLocation
        private float SimpleMoveTowardsLocation(Transform agentTransform, Vector3 desiredPosition, float orbitSpeedDegreesPerSec, float distanceToTarget, float stoppingDistance)
        {
            // Convert angular orbit speed (deg/s) to linear tangential speed (units/s): v = omega * r, omega in rad/s
            float angularRadPerSec = orbitSpeedDegreesPerSec * Mathf.Deg2Rad;
            float linearSpeed = angularRadPerSec * OrbitDistance.Value;

            if (distanceToTarget <= stoppingDistance)
                return 0f;

            float step = linearSpeed * Time.deltaTime;
            Vector3 prevPos = agentTransform.position;
            agentTransform.position = Vector3.MoveTowards(prevPos, desiredPosition, step);

            // Face movement direction on horizontal plane
            Vector3 moveDir = agentTransform.position - prevPos;
            Vector3 flatMoveDir = new Vector3(moveDir.x, 0f, moveDir.z);
            if (flatMoveDir.sqrMagnitude > 0.000001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(flatMoveDir);
                agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, targetRot, 10f * Time.deltaTime);
            }

            return linearSpeed;
        }

        // Local replacement for NavigationUtility.UpdateAnimatorSpeed
        private void UpdateAnimatorSpeed(float explicitSpeed = -1)
        {
            if (m_Animator == null)
                return;

            string paramName = AnimatorSpeedParam?.Value;
            if (string.IsNullOrEmpty(paramName))
                return;

            float speedToSet = explicitSpeed >= 0f ? explicitSpeed : (m_NavMeshAgent != null ? m_NavMeshAgent.velocity.magnitude : m_CurrentSpeed);
            m_Animator.SetFloat(paramName, speedToSet);
        }
    }
} // namespace Behavior.Enemy
