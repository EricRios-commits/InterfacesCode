namespace Actors
{
    /// <summary>
    /// Interface for health bar view strategies.
    /// Follows Interface Segregation Principle - minimal, focused interface.
    /// Enables Strategy Pattern for different health bar visualizations.
    /// </summary>
    public interface IHealthBarView
    {
        /// <summary>
        /// Update the visual representation with current health values.
        /// </summary>
        void UpdateHealth(float currentHealth, float maxHealth);

        /// <summary>
        /// Handle visual changes when the actor dies.
        /// </summary>
        void OnActorDeath();
    }
}

