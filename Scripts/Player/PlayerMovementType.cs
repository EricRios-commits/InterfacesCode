using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

/// <summary>
///  Handles the movement type for the player in base of the value setted on the PlayerPrefs
/// </summary>
public class PlayerMovementType : MonoBehaviour
{
    [SerializeField]
    private GameObject movementProvider;

    [SerializeField]
    private GameObject teleportationArea;
    private const string movementTypeKey = "MovementType";
    private int movementType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Llamada a START");
        MovementTypeSetter();
        if (teleportationArea == null)
        {
            teleportationArea = FindFirstObjectByType<TeleportationArea>().gameObject;
        }
    }

    /// <summary>
    /// Listener for the dropdown of the menu
    /// </summary>
    /// <param name="newValue"></param>
    public void OnValueChanged(Int32 newValue)
    {
        MovementTypeSetter();
    }

    /// <summary>
    /// Sets the movement type depending on the value of the PlayerPrefs
    /// </summary>
    private void MovementTypeSetter()
    {
        movementType = PlayerPrefs.GetInt(movementTypeKey, 0);
        Debug.Log("El tipo de movimiento guardado es " + movementType);
        if (movementType == 0)
        {
            teleportationArea.SetActive(true);
            movementProvider.SetActive(false);
        } else
        {
            teleportationArea.SetActive(false);
            movementProvider.SetActive(true);
        }
    }
}
