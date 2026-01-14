using Behavior.Enemy;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForAttack", story: "Wait for [Attack] delay", category: "Action/Delay", id: "d5357cf027913c18202a0343db0bea13")]
public partial class WaitForAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<EnemyAttack> Attack;

    private float m_Timer = 0.0f;
    
    protected override Status OnStart()
    {
        m_Timer = Attack.Value.Cooldown;
        if (m_Timer <= 0.0f)
        {
            return Status.Success;
        }
        return Status.Running;

    }

    protected override Status OnUpdate()
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0)
        {
            return Status.Success;
        }
        return Status.Running;
    }
}
