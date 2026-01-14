namespace Combat
{
    /// <summary>
    /// Strategy interface for modifying damage based on type.
    /// Implements Strategy Pattern for damage calculation.
    /// </summary>
    public interface IDamageModifier
    {
        float ModifyDamage(DamageInfo damageInfo);
    }
}

