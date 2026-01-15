using Gaze;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace UI
{
    /// <summary>
    /// Provides visual feedback for the gaze interaction by displaying a radial fill indicator
    /// that shows the progress of the gaze timer. This creates a "loading ring" effect
    /// that gives the player visual confirmation that their gaze is being registered.
    /// The UI automatically follows the gaze point in world space.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class GazeFeedbackUI : MonoBehaviour
    {
        [Header("References")] private IGazeProgressProvider progressProvider;

        [SerializeField]
        [Tooltip("The XRGazeInteractor to track for positioning. If null, will attempt to find it automatically.")]
        private XRGazeInteractor gazeInteractor;
        
        [SerializeField]
        private Vector3 offset = Vector3.zero;

        [Header("Positioning Settings")]
        [SerializeField] [Tooltip("How smoothly the UI follows the gaze point")] [Range(0f, 30f)]
        private float followSpeed = 15f;
        
        [SerializeField] [Tooltip("If true, the UI will always face the camera")]
        private bool faceCamera = true;

        [Header("Visual Settings")] [SerializeField] [Tooltip("Color of the fill when actively gazing")]
        private Color activeColor = new Color(1f, 1f, 1f, 0.8f);

        [SerializeField] [Tooltip("Color of the fill when not gazing (usually transparent)")]
        private Color inactiveColor = new Color(1f, 1f, 1f, 0f);

        [SerializeField] [Tooltip("How quickly the color transitions between active and inactive states")]
        private float colorTransitionSpeed = 10f;

        [SerializeField] [Tooltip("Minimum alpha value when the indicator is visible")] [Range(0f, 1f)]
        private float minAlpha = 0.3f;

        [SerializeField] [Tooltip("Maximum alpha value when fully active")] [Range(0f, 1f)]
        private float maxAlpha = 1f;

        [SerializeField] [Tooltip("Optional: Scale animation when gazing starts")]
        private bool useScaleAnimation = true;

        [SerializeField] [Tooltip("Scale multiplier when fully active")]
        private float activeScale = 1.1f;

        [SerializeField] [Tooltip("How quickly the scale changes")]
        private float scaleTransitionSpeed = 8f;

        private Image fillImage;
        private Vector3 initialScale;
        private Color currentColor;
        private float currentScaleMultiplier = 1f;
        private Camera mainCamera;
        private Transform targetTransform;

        private void Awake()
        {
            SetUpComponents();
        }

        private void Start()
        {
            SetUpGazeComponents();
        }

        private void Update()
        {
            UpdateGazeProgress();
        }

        void LateUpdate()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (mainCamera == null || targetTransform == null) return;
            Vector3 targetPosition = targetTransform.position + offset;
            Vector3 desiredPosition = targetPosition;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
            if (faceCamera)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
            }
        }

        private void SetUpGazeComponents()
        {
            if (progressProvider == null)
            {
                progressProvider = FindFirstObjectByType<GazeNotifier>();
                if (progressProvider == null)
                {
                    Debug.LogError(
                        $"[GazeFeedbackUI] GazeController not assigned on {gameObject.name}. Please assign it in the inspector.");
                }
            }
            if (gazeInteractor == null)
            {
                gazeInteractor = FindFirstObjectByType<XRGazeInteractor>();
                if (gazeInteractor == null)
                {
                    Debug.LogWarning(
                        $"[GazeFeedbackUI] XRGazeInteractor not found in scene. UI will not follow gaze point.");
                    return;
                }
                gazeInteractor.hoverEntered.RemoveListener(OnGazingAtTarget);
                gazeInteractor.hoverEntered.AddListener(OnGazingAtTarget);
            }
        }

        private void OnGazingAtTarget(HoverEnterEventArgs args)
        {
            var gazedObject = args.interactableObject.transform.gameObject;
            targetTransform = gazedObject.transform;
            var progressProviderComponent = gazedObject.GetComponent<IGazeProgressProvider>();
            if (progressProviderComponent != null)
            {
                progressProvider = progressProviderComponent;
            }
            else
            {
                Debug.LogWarning(
                    $"[GazeFeedbackUI] Gazed object {gazedObject.name} does not implement IGazeProgressProvider.");
                progressProvider = null;
            }
        }


        private void OnDestroy()
        {
            if (gazeInteractor != null)
                gazeInteractor.hoverEntered.RemoveListener(OnGazingAtTarget);
        }

        private void SetUpComponents()
        {
            fillImage = GetComponent<Image>();
            initialScale = transform.localScale;
            currentColor = inactiveColor;
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogWarning($"[GazeFeedbackUI] No main camera found. UI positioning may not work correctly.");
            }
            if (fillImage.type != Image.Type.Filled)
            {
                Debug.LogWarning(
                    $"[GazeFeedbackUI] Image on {gameObject.name} is not set to Filled type. Setting it now.");
                fillImage.type = Image.Type.Filled;
            }
            if (fillImage.fillMethod != Image.FillMethod.Radial360)
            {
                Debug.LogWarning(
                    $"[GazeFeedbackUI] Image on {gameObject.name} is not using Radial360 fill. Setting it now.");
                fillImage.fillMethod = Image.FillMethod.Radial360;
            }
            fillImage.fillAmount = 0f;
            fillImage.color = inactiveColor;
        }

        private void UpdateGazeProgress()
        {
            if (progressProvider == null)
                return;
            fillImage.fillAmount = progressProvider.GazeProgress;
            Color targetColor = progressProvider.IsGazing ? activeColor : inactiveColor;
            currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorTransitionSpeed);
            if (progressProvider.IsGazing)
            {
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, progressProvider.GazeProgress);
                currentColor.a = alpha;
            }
            fillImage.color = currentColor;
            if (useScaleAnimation)
            {
                float targetScale = progressProvider.IsGazing ? activeScale : 1f;
                currentScaleMultiplier = Mathf.Lerp(currentScaleMultiplier, targetScale,
                    Time.deltaTime * scaleTransitionSpeed);
                transform.localScale = initialScale * currentScaleMultiplier;
            }
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Configure the Image component correctly in the editor
        /// </summary>
        [ContextMenu("Setup Image Component")]
        private void SetupImageComponent()
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                img.type = Image.Type.Filled;
                img.fillMethod = Image.FillMethod.Radial360;
                img.fillAmount = 0f;
                img.color = inactiveColor;
                Debug.Log($"[GazeFeedbackUI] Image component configured on {gameObject.name}");
            }
        }
#endif
    }
}
