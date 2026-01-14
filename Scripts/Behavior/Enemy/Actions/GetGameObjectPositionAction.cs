using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.Serialization;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "GetGameObjectPosition", story: "Store [GameObject] position in [Vector3]", category: "Action/GameObject", id: "c440f03920a799292be1131ca59a80a9")]
public partial class GetGameObjectPositionAction : Action
{
    [FormerlySerializedAs("GameObject")] [SerializeReference] public BlackboardVariable<GameObject> gameObject;
    [SerializeReference] public BlackboardVariable<Vector3> Vector3;

    protected override Status OnStart()
    {
        if (gameObject.Value == null)
            return Status.Failure;
        Vector3.Value = gameObject.Value.transform.position;
        return Status.Success;
    }
}

