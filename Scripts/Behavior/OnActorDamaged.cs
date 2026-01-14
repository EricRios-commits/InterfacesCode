using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace Behavior
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/OnActorDamaged")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "OnActorDamaged", message: "[Actor] took [DamageData]", category: "Events/Actor",
        id: "f2e3d4c5b6a7890123456789cafebabe")]
    public sealed partial class OnActorDamaged : EventChannel<GameObject, GameObject>
    {
    }
}