using System.Collections;
using System.Collections.Generic;
using System.Text;
using Gaze;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace LLMAnswer
{
    public class TextPetitioner : MonoBehaviour
    {
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private TextMeshProUGUI outputText;
        [SerializeField] private TextAsset promptFile;       // optional .txt file assigned in Inspector
        [SerializeField] private PromptSo promptSO;          // optional ScriptableObject containing prompt
        [SerializeField] private bool useContextProviders = true;
        [SerializeField] private List<MonoBehaviour> contextProviderComponents;
        private List<IPromptContextProvider> contextProviders;
        private static string apiUrl = "http://gpu1.esit.ull.es:4000/v1/chat/completions";
        private GazeNotifier gazeNotifier;

        [System.Serializable]
        public class ChatMessage
        {
            public string content;
            public string role;
        }

        [System.Serializable]
        public class ChatChoice
        {
            public int index;
            public string finish_reason;
            public ChatMessage message;
        }

        [System.Serializable]
        public class ChatResponse
        {
            public ChatChoice[] choices;
        }

        void Start() {
            InitializeContextProviders();
        }

        void OnEnable()
        {
            gazeNotifier = FindFirstObjectByType<GazeNotifier>();
            gazeNotifier.gazeAlert.AddListener(RequestToModel);
        }

        void OnDisable()
        {
            if (gazeNotifier != null)
            {
                gazeNotifier.gazeAlert.RemoveListener(RequestToModel);
            }
        }
        
        private void InitializeContextProviders()
        {
            contextProviders = new List<IPromptContextProvider>();
            if (contextProviderComponents != null)
            {
                foreach (var component in contextProviderComponents)
                {
                    if (component is IPromptContextProvider provider)
                    {
                        contextProviders.Add(provider);
                    }
                }
            }
        }

        private void SendMessageFromString(string message)
        {
            StartCoroutine(SendMessageToChatbot(message));
        }

        private void RequestToModel()
        {
            Debug.Log("Requesting to model...");
            string basePrompt = null;
            if (promptSO != null && !string.IsNullOrWhiteSpace(promptSO.prompt))
            {
                basePrompt = promptSO.prompt;
            }
            else if (promptFile != null && !string.IsNullOrEmpty(promptFile.text))
            {
                basePrompt = promptFile.text;
            }
            else
            {
                basePrompt = "You're a dungeon master in a roman collosseum. Taunt the gladiators";
            }
            string finalMessage = BuildPromptWithContext(basePrompt);
            Debug.Log("Final prompt built, sending to model...");
            SendMessageFromString(finalMessage);
        }
        
        /// <summary>
        /// Builds the final prompt by appending context from all registered providers.
        /// </summary>
        private string BuildPromptWithContext(string basePrompt)
        {
            if (!useContextProviders || contextProviders == null || contextProviders.Count == 0)
            {
                return basePrompt;
            }
            var promptBuilder = new StringBuilder(basePrompt);
            foreach (var provider in contextProviders)
            {
                if (provider == null) continue;
                string context = provider.GetContext();
                if (!string.IsNullOrWhiteSpace(context))
                {
                    promptBuilder.Append("\n");
                    promptBuilder.Append(context);
                }
            }
            return promptBuilder.ToString();
        }

        private IEnumerator SendMessageToChatbot(string message)
        {
            Debug.Log("Entering send message function");
            string escapedMessage = message.Replace("\\", "\\\\")
                                           .Replace("\"", "\\\"")
                                           .Replace("\n", "\\n")
                                           .Replace("\r", "\\r")
                                           .Replace("\t", "\\t");
            string jsonPayload = "{"
                                 + "\"model\": \"ollama/llama3.1:8b\"," // Debe coincidir con el modelo cargado en Ollama
                                 + "\"messages\": [{\"role\": \"user\", \"content\": \"" + escapedMessage + "\"}]"
                                 + "}";
            Debug.Log(jsonPayload);
            UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer sk-1234");
            yield return request.SendWebRequest();
            Debug.Log("Petition Sent");
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al conectar con el chatbot: " + request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("Respuesta de la IA: " + jsonResponse);
                var parsed = JsonUtility.FromJson<ChatResponse>(jsonResponse);
                if (parsed != null && parsed.choices != null && parsed.choices.Length > 0 && parsed.choices[0].message != null)
                {
                    Debug.Log(parsed.choices[0].message.content);
                    outputText.text = parsed.choices[0].message.content;
                    ToggleUI(true);
                }
                else
                {
                    outputText.text = jsonResponse;
                    ToggleUI(true);
                }
            }
        }
        
        private void ToggleUI(bool activate)
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(activate);
            }
            if (outputText != null)
            {
                outputText.gameObject.SetActive(activate);
            }
        }
        
        #region Debug Methods
        
        /// <summary>
        /// Debug method to test a petition with a custom message.
        /// Can be called from Unity Editor or during runtime for testing.
        /// </summary>
        [ContextMenu("Debug: Test Petition")]
        public void DebugTestPetition()
        {
            string testMessage = "Tell me a joke about a skeleton warrior.";
            Debug.Log($"[DEBUG] Testing petition with message: {testMessage}");
            SendMessageFromString(testMessage);
        }
        
        /// <summary>
        /// Debug method to test a petition with current prompt and context.
        /// Useful for testing how context providers affect the final prompt.
        /// </summary>
        [ContextMenu("Debug: Test Petition With Context")]
        public void DebugTestPetitionWithContext()
        {
            Debug.Log("[DEBUG] Testing petition with full context...");
            RequestToModel();
        }
        
        /// <summary>
        /// Debug method to test a petition with a custom message (public version).
        /// </summary>
        public void DebugTestPetitionWithMessage(string message)
        {
            Debug.Log($"[DEBUG] Testing petition with custom message: {message}");
            SendMessageFromString(message);
        }
        
        /// <summary>
        /// Debug method to log the final prompt that would be sent (without actually sending it).
        /// Useful for verifying context providers are working correctly.
        /// </summary>
        [ContextMenu("Debug: Log Current Prompt")]
        public void DebugLogCurrentPrompt()
        {
            if (contextProviders == null)
                InitializeContextProviders();
                
            string basePrompt = null;
            if (promptSO != null && !string.IsNullOrWhiteSpace(promptSO.prompt))
            {
                basePrompt = promptSO.prompt;
            }
            else if (promptFile != null && !string.IsNullOrEmpty(promptFile.text))
            {
                basePrompt = promptFile.text;
            }
            else
            {
                basePrompt = "You're a dungeon master in a roman collosseum. Taunt the gladiators";
            }
            
            string finalPrompt = BuildPromptWithContext(basePrompt);
            Debug.Log($"[DEBUG] Base Prompt:\n{basePrompt}");
            Debug.Log($"[DEBUG] Final Prompt with Context:\n{finalPrompt}");
            Debug.Log($"[DEBUG] Context Providers Count: {(contextProviders != null ? contextProviders.Count : 0)}");
        }
        
        #endregion

    }
}