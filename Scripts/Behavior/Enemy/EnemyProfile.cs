using UnityEngine;

namespace Behavior.Enemy
{
    /// <summary>
    /// Defines the type of behavior for an enemy.
    /// Used to determine which initial state and behavior pattern to use.
    /// </summary>
    public enum EnemyBehaviorType
    {
        GroundChaser,    // Ground enemy that chases and attacks (requires NavMeshAgent)
        Turret           // Stationary enemy that rotates to track and attack
    }
    
    /// <summary>
    /// ScriptableObject that defines enemy configuration.
    /// Data-driven approach: create different enemy types by creating different profiles.
    /// No code changes needed to create new enemy variants.
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy Profile", menuName = "Actors/Enemy Profile")]
    public class EnemyProfile : ScriptableObject
    {
        [Header("Behavior Type")]
        [SerializeField] private EnemyBehaviorType behaviorType = EnemyBehaviorType.GroundChaser;
        
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float chaseSpeed = 5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float stoppingDistance = 2f;
        
        [Header("Detection")]
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float loseTargetRange = 15f;
        [SerializeField] private float fieldOfViewAngle = 120f;
        [SerializeField] private LayerMask detectionLayers;
        
        [Header("Combat")]
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCooldown = 1.5f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private Combat.DamageType damageType = Combat.DamageType.Slash;
        
        [Header("Visual")]
        [SerializeField] private Color gizmoColor = Color.red;

        // Properties
        public EnemyBehaviorType BehaviorType => behaviorType;
        public float MoveSpeed => moveSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float RotationSpeed => rotationSpeed;
        public float StoppingDistance => stoppingDistance;
        public float DetectionRange => detectionRange;
        public float LoseTargetRange => loseTargetRange;
        public float FieldOfViewAngle => fieldOfViewAngle;
        public LayerMask DetectionLayers => detectionLayers;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public float AttackDamage => attackDamage;
        public Combat.DamageType DamageType => damageType;
        public Color GizmoColor => gizmoColor;
    }
}
