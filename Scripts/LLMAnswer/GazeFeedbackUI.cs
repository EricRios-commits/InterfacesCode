using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Provides visual feedback for the gaze interaction by displaying a radial fill indicator
/// that shows the progress of the gaze timer. This creates a "loading ring" effect
/// that gives the player visual confirmation that their gaze is being registered.
/// The UI automatically follows the gaze point in world space.
/// </summary>
[RequireComponent(typeof(Image))]
public class GazeFeedbackUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The GazeController to read progress from")]
    private GazeController gazeController;

    [SerializeField]
    [Tooltip("The XRGazeInteractor to track for positioning. If null, will attempt to find it automatically.")]
    private XRGazeInteractor gazeInteractor;

    [Header("Positioning Settings")]
    [SerializeField]
    [Tooltip("Distance from the camera/gaze origin where the UI appears")]
    private float displayDistance = 2f;

    [SerializeField]
    [Tooltip("How smoothly the UI follows the gaze point")]
    [Range(0f, 30f)]
    private float followSpeed = 15f;

    [SerializeField]
    [Tooltip("Offset from the exact gaze point (useful for fine-tuning position)")]
    private Vector3 positionOffset = Vector3.zero;

    [SerializeField]
    [Tooltip("If true, the UI will always face the camera")]
    private bool faceCamera = true;

    [Header("Visual Settings")]
    [SerializeField]
    [Tooltip("Color of the fill when actively gazing")]
    private Color activeColor = new Color(1f, 1f, 1f, 0.8f);

    [SerializeField]
    [Tooltip("Color of the fill when not gazing (usually transparent)")]
    private Color inactiveColor = new Color(1f, 1f, 1f, 0f);

    [SerializeField]
    [Tooltip("How quickly the color transitions between active and inactive states")]
    private float colorTransitionSpeed = 10f;

    [SerializeField]
    [Tooltip("Minimum alpha value when the indicator is visible")]
    [Range(0f, 1f)]
    private float minAlpha = 0.3f;

    [SerializeField]
    [Tooltip("Maximum alpha value when fully active")]
    [Range(0f, 1f)]
    private float maxAlpha = 1f;

    [SerializeField]
    [Tooltip("Optional: Scale animation when gazing starts")]
    private bool useScaleAnimation = true;

    [SerializeField]
    [Tooltip("Scale multiplier when fully active")]
    private float activeScale = 1.1f;

    [SerializeField]
    [Tooltip("How quickly the scale changes")]
    private float scaleTransitionSpeed = 8f;

    private Image fillImage;
    private Vector3 initialScale;
    private Color currentColor;
    private float currentScaleMultiplier = 1f;
    private Camera mainCamera;
    private Vector3 targetPosition;

    void Awake()
    {
        fillImage = GetComponent<Image>();
        initialScale = transform.localScale;
        currentColor = inactiveColor;

        // Find main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning($"[GazeFeedbackUI] No main camera found. UI positioning may not work correctly.");
        }

        // Ensure the Image component is configured correctly for radial fill
        if (fillImage.type != Image.Type.Filled)
        {
            Debug.LogWarning($"[GazeFeedbackUI] Image on {gameObject.name} is not set to Filled type. Setting it now.");
            fillImage.type = Image.Type.Filled;
        }

        if (fillImage.fillMethod != Image.FillMethod.Radial360)
        {
            Debug.LogWarning($"[GazeFeedbackUI] Image on {gameObject.name} is not using Radial360 fill. Setting it now.");
            fillImage.fillMethod = Image.FillMethod.Radial360;
        }

        // Start with no fill
        fillImage.fillAmount = 0f;
        fillImage.color = inactiveColor;
    }

    void Start()
    {
        // Validate the reference
        if (gazeController == null)
        {
            Debug.LogError($"[GazeFeedbackUI] GazeController not assigned on {gameObject.name}. Please assign it in the inspector.");
        }

        // Attempt to find the XRGazeInteractor if not assigned
        if (gazeInteractor == null)
        {
            gazeInteractor = FindFirstObjectByType<XRGazeInteractor>();
            if (gazeInteractor == null)
            {
                Debug.LogWarning($"[GazeFeedbackUI] XRGazeInteractor not found in scene. UI will not follow gaze point.");
            }
            else
            {
                Debug.Log($"[GazeFeedbackUI] XRGazeInteractor automatically found and assigned.");
            }
        }
    }

    void Update()
    {
        if (gazeController == null)
            return;

        // Update fill amount based on gaze progress
        fillImage.fillAmount = gazeController.GazeProgress;

        // Determine target color based on gazing state
        Color targetColor = gazeController.IsGazing ? activeColor : inactiveColor;
        
        // Smoothly transition color
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorTransitionSpeed);
        
        // Apply alpha modulation based on fill amount if actively gazing
        if (gazeController.IsGazing)
        {
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, gazeController.GazeProgress);
            currentColor.a = alpha;
        }

        fillImage.color = currentColor;

        // Optional scale animation
        if (useScaleAnimation)
        {
            float targetScale = gazeController.IsGazing ? activeScale : 1f;
            currentScaleMultiplier = Mathf.Lerp(currentScaleMultiplier, targetScale, Time.deltaTime * scaleTransitionSpeed);
            transform.localScale = initialScale * currentScaleMultiplier;
        }
    }

    void LateUpdate()
    {
        // Position the UI to follow the gaze point
        UpdatePosition();
    }

    /// <summary>
    /// Updates the position of the UI to follow the gaze interactor
    /// </summary>
    private void UpdatePosition()
    {
        if (gazeInteractor == null || mainCamera == null)
            return;

        // Get the gaze direction from the interactor's attach transform
        Transform gazeTransform = gazeInteractor.attachTransform;
        if (gazeTransform == null)
        {
            // Fallback to using the interactor's transform itself
            gazeTransform = gazeInteractor.transform;
        }

        // Calculate target position along the gaze direction
        Vector3 gazeDirection = gazeTransform.forward;
        Vector3 gazeOrigin = gazeTransform.position;
        targetPosition = gazeOrigin + gazeDirection * displayDistance + positionOffset;

        // Smoothly move to target position
        if (followSpeed > 0f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
        }
        else
        {
            transform.position = targetPosition;
        }

        // Make the UI face the camera if enabled
        if (faceCamera)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0); // Flip to face the camera correctly
        }
    }

    /// <summary>
    /// Manually set the GazeController reference. Useful for runtime setup.
    /// </summary>
    /// <param name="controller">The GazeController to track</param>
    public void SetGazeController(GazeController controller)
    {
        gazeController = controller;
    }

    /// <summary>
    /// Update the active color at runtime
    /// </summary>
    public void SetActiveColor(Color color)
    {
        activeColor = color;
    }

    /// <summary>
    /// Update the inactive color at runtime
    /// </summary>
    public void SetInactiveColor(Color color)
    {
        inactiveColor = color;
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

