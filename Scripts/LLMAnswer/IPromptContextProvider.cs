namespace LLMAnswer
{
    /// <summary>
    /// Interface for components that provide additional context to be appended to AI prompts.
    /// Follows the Open/Closed Principle - new context providers can be added without modifying existing code.
    /// </summary>
    public interface IPromptContextProvider
    {
        /// <summary>
        /// Gets the current context string to be appended to the prompt.
        /// Returns null or empty string if no context is available.
        /// </summary>
        string GetContext();
    }
}

