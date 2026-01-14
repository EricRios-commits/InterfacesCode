using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Class to manage the pause menu of the game
/// </summary>
public class MenuManager : MonoBehaviour
{

    [SerializeField]
    private Transform head;      
    public float distance = 1.5f;
    public float heightOffset = -0.1f;

    [SerializeField]
    private GameObject menu;

    private bool lastState = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (leftHand.TryGetFeatureValue(CommonUsages.menuButton, out bool pressed))
        {
            if (pressed && !lastState)
            {
                TogglePauseMenu();
            }

            lastState = pressed;
        }
    }
    /// <summary>
    /// Listener of the Continue button to resume the game
    /// </summary>
    public void ContinuePressed()
    {
        menu.SetActive(false);
    }

    /// <summary>
    /// Listener of the Exit button to quit the game
    /// </summary>
    public void ExitPressed()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }

    /// <summary>
    /// Sets the timeScale to one. 
    /// </summary>
    public void ResumeTime()
    {
        Time.timeScale = 1;
        lastState = false;
    }

    /// <summary>
    /// Activates and sets the position of the pause menu
    /// </summary>
    private void TogglePauseMenu()
    {
        Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;

        transform.position =
            head.position +
            forward * distance +
            Vector3.up * heightOffset;

        transform.rotation = Quaternion.LookRotation(forward);

        Time.timeScale = 0;
        Debug.Log("Botón de menú izquierdo pulsado");
        menu.SetActive(true);
    }
}
