using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    [ExecuteInEditMode] 
    public sealed class WorldSpaceOverlayUI : MonoBehaviour
    {
        private const string ShaderTestMode = "unity_GUIZTestMode"; 
        [SerializeField] UnityEngine.Rendering.CompareFunction desiredUIComparison = UnityEngine.Rendering.CompareFunction.Always; //If you want to try out other effects
        [Tooltip("Set to blank to automatically populate from the child UI elements")]
        [SerializeField] Graphic[] uiElementsToApplyTo;

        private readonly Dictionary<Material, Material> _materialMappings = new Dictionary<Material, Material>();
        private static readonly int UnityGuIzTestMode = Shader.PropertyToID(ShaderTestMode);

        private void Start()
        {
            if (uiElementsToApplyTo.Length == 0)
            {
                uiElementsToApplyTo = gameObject.GetComponentsInChildren<Graphic>();
            }
            foreach (var graphic in uiElementsToApplyTo)
            {
                Material material = graphic.materialForRendering;
                if (material == null)
                {
                    Debug.LogError($"{nameof(WorldSpaceOverlayUI)}: skipping target without material {graphic.name}.{graphic.GetType().Name}");
                    continue;
                }

                if (!_materialMappings.TryGetValue(material, out Material materialCopy))
                {
                    materialCopy = new Material(material);
                    _materialMappings.Add(material, materialCopy);
                }

                materialCopy.SetInt(UnityGuIzTestMode, (int)desiredUIComparison);
                graphic.material = materialCopy;
            }
        }
    }
}