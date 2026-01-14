using Combat;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsDead", story: "[Health] is empty", category: "Conditions", id: "2987064877d95ad9bd880ea16a308f95")]
public partial class IsDeadCondition : Condition
{
    [SerializeReference] public BlackboardVariable<HealthComponent> Health;

    public override bool IsTrue()
    {
        return Health.Value.IsAlive == false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
