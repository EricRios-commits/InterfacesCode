using System;
using Behavior.Enemy;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackGameObject", story: "[Agent] attacks [Target] with [Attack]", category: "Action", id: "cbabc2ea06993cb778f3e8f1e821643b")]
public partial class AttackGameObjectAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<EnemyAttack> Attack;

    protected override Status OnStart()
    {
        if (Attack.Value.Perform(Agent.Value, Target.Value)) return Status.Success;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // // this might need to be removed depending on how attacks are implemented
        if (Attack.Value.Perform(Agent.Value, Target.Value)) return Status.Success;
        return Status.Running;
    }

    protected override void OnEnd()
    {
    }
}
