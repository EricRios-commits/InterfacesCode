using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;


[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitForAnimation", story: "Wait for [Animator] to finish current animation on layer [Layer]", category: "Action/Delay", id: "f3a1b8e9c2d4a5f6b7c8d9e0f1a2b3c4")]
public partial class WaitForAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<Animator> Animator;
    [SerializeReference] public BlackboardVariable<int> Layer = new BlackboardVariable<int>(0);

    private bool waitingForTransition = false;

    protected override Status OnStart()
    {
        if (Animator.Value == null || Animator.Value.runtimeAnimatorController == null)
        {
            return Status.Success;
        }
        waitingForTransition = Animator.Value.IsInTransition(Layer.Value);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Animator.Value == null)
        {
            return Status.Failure;
        }
        if (waitingForTransition)
        {
            if (Animator.Value.IsInTransition(Layer.Value))
            {
                return Status.Running;
            }
            waitingForTransition = false;
            return Status.Running;
        }
        if (Animator.Value.IsInTransition(Layer.Value))
        {
            return Status.Success;
        }
        AnimatorStateInfo stateInfo = Animator.Value.GetCurrentAnimatorStateInfo(Layer.Value);
        if (stateInfo.normalizedTime >= 1.0f)
        {
            return Status.Success;
        }
        return Status.Running;
    }

    protected override void OnEnd()
    {
        waitingForTransition = false;
    }
}