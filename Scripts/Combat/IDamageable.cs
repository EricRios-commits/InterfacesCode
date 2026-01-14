namespace Combat
{
    /// <summary>
    /// Interface for objects that can receive damage.
    /// Following Interface Segregation Principle.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(DamageInfo damageInfo);
        bool IsAlive { get; }
        float CurrentHealth { get; }
        float MaxHealth { get; }
    }
}
