using UnityEngine;

namespace LLMAnswer
{
    [CreateAssetMenu(fileName = "PromptSO", menuName = "Prompts/PromptSO")]
    public class PromptSo : ScriptableObject
    {
        [TextArea(5, 20)]
        public string prompt;
    }
}