using UnityEngine;

namespace Combat
{
    /// <summary>
    /// Data container for damage information.
    /// Encapsulates all damage-related data in one structure.
    /// </summary>
    public struct DamageInfo
    {
        public float Amount;
        public DamageType Type;
        public GameObject Instigator;
        public Vector3 HitPoint;
        public Vector3 HitDirection;

        public DamageInfo(float amount, DamageType type, GameObject instigator = null, 
                         Vector3 hitPoint = default, Vector3 hitDirection = default)
        {
            Amount = amount;
            Type = type;
            Instigator = instigator;
            HitPoint = hitPoint;
            HitDirection = hitDirection;
        }
    }
}

