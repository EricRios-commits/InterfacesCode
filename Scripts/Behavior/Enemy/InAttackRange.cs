using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/InAttackRange")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "InAttackRange", message: "[Agent] has [Target] in attack range", category: "Events", id: "b9a67e9dcc8f8778358147ebf1def38a")]
public sealed partial class InAttackRange : EventChannel<GameObject, GameObject> { }

