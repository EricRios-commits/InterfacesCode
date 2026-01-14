using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Restart")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Restart", message: "Restart behavior", category: "Events", id: "cf2c7758495a286085c9dd5e70e24069")]
public sealed partial class Restart : EventChannel { }

