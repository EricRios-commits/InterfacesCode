using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace Behavior.Enemy
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "DetectTargetInRange", story: "[Agent] detects [Target] with [Sensor]", category: "Action", id: "a12755bdda304162e987b421ab7978b9")]
    public partial class DetectTargetInRangeAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<Sensor> Sensor;

        private NavMeshAgent agent;
    
        protected override Status OnStart()
        {
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            var target = Sensor.Value.GetClosestTarget();
            if (target == null) return Status.Running;
            Target.Value = target.gameObject;
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}

