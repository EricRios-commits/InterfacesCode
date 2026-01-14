using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace Behavior
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/OnActorStaggered")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "OnActorStaggered", message: "[Actor] was staggered", category: "Events/Actor",
        id: "d7e8f9a0b1c2d3e4f5a6b7c8d9e0f1a2")]
    public sealed partial class OnActorStaggered : EventChannel<GameObject, GameObject>
    {
    }
}

