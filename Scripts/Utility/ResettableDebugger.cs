using UnityEngine;

namespace Utility
{
    /// <summary>
    /// Debug utility to manually reset all IResettable components on this GameObject.
    /// Useful for testing object pooling and reset behavior in the editor.
    /// </summary>
    public class ResettableDebugger : MonoBehaviour
    {
        [Header("Debug Info")]
        [SerializeField] private bool showDebugLogs = true;
        
        /// <summary>
        /// Resets all IResettable components on this GameObject.
        /// Can be called from the inspector context menu or via code.
        /// </summary>
        [ContextMenu("Reset All Resettable Components")]
        public void ResetAllResettables()
        {
            var resettables = GetComponents<IResettable>();
            
            if (resettables.Length == 0)
            {
                if (showDebugLogs)
                    Debug.LogWarning($"[ResettableDebugger] No IResettable components found on {gameObject.name}");
                return;
            }
            
            if (showDebugLogs)
                Debug.Log($"[ResettableDebugger] Resetting {resettables.Length} component(s) on {gameObject.name}");
            
            foreach (var resettable in resettables)
            {
                if (showDebugLogs)
                    Debug.Log($"[ResettableDebugger] - Resetting {resettable.GetType().Name}");
                
                resettable.ResetState();
            }
            
            if (showDebugLogs)
                Debug.Log($"[ResettableDebugger] Reset complete for {gameObject.name}");
        }
        
        /// <summary>
        /// Lists all IResettable components in the console.
        /// </summary>
        [ContextMenu("List All Resettable Components")]
        public void ListResettables()
        {
            var resettables = GetComponents<IResettable>();
            
            if (resettables.Length == 0)
            {
                Debug.Log($"[ResettableDebugger] No IResettable components found on {gameObject.name}");
                return;
            }
            
            Debug.Log($"[ResettableDebugger] Found {resettables.Length} IResettable component(s) on {gameObject.name}:");
            for (int i = 0; i < resettables.Length; i++)
            {
                var resettable = resettables[i];
                var component = resettable as Component;
                Debug.Log($"  [{i + 1}] {resettable.GetType().Name} - {(component != null && ((MonoBehaviour)component).enabled ? "Enabled" : "Disabled")}");
            }
        }
        
        /// <summary>
        /// Counts how many IResettable components are on this GameObject.
        /// </summary>
        [ContextMenu("Count Resettable Components")]
        public void CountResettables()
        {
            var resettables = GetComponents<IResettable>();
            Debug.Log($"[ResettableDebugger] {gameObject.name} has {resettables.Length} IResettable component(s)");
        }
        
        #region Editor Buttons
        
#if UNITY_EDITOR
        [Header("Quick Actions")]
        [SerializeField] private bool _resetButton = false;
        
        private void OnValidate()
        {
            if (_resetButton)
            {
                _resetButton = false;
                // Schedule for next frame to avoid issues during OnValidate
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this != null)
                        ResetAllResettables();
                };
            }
        }
#endif
        
        #endregion
    }
}

