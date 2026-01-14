using System;
using Combat;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace Behavior
{
#if UNITY_EDITOR
    [CreateAssetMenu(menuName = "Behavior/Event Channels/OnActorDeath", fileName = "OnActorDeath")]
#endif
    [Serializable, GeneratePropertyBag]
    [EventChannelDescription(name: "OnActorDeath", message: "[Actor] died", category: "Events/Actor", id: "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6")]
    public sealed partial class OnActorDeath : EventChannel<GameObject, DamageInfo> { }
}