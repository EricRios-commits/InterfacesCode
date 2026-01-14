using System;
using Behavior.Enemy;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackPosition", story: "[Agent] attacks [TargetPosition] with [Attack]", category: "Action", id: "dbabc3fa16aa4dc889f4e9f2e931754c")]
public partial class AttackPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;
    [SerializeReference] public BlackboardVariable<EnemyAttack> Attack;

    protected override Status OnStart()
    {
        if (Attack.Value.Perform(Agent.Value, TargetPosition.Value)) return Status.Success;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // // this might need to be removed depending on how attacks are implemented
        if (Attack.Value.Perform(Agent.Value, TargetPosition.Value)) return Status.Success;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}

