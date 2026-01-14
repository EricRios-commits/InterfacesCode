using System;
using UnityEngine;
using UnityEngine.UI;

public class MovementTypeManager : MonoBehaviour
{
    [SerializeField]
    private Dropdown dropdown;
    private const string movementTypeKey = "MovementType";
    private int movementType;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementType = PlayerPrefs.GetInt(movementTypeKey, 0);
        dropdown.value = movementType;
    }

    /// <summary>
    /// Listener of the dropdown of the main menu to set the movement type on the PlayerPrefs 
    /// </summary>
    /// <param name="newValue"></param>
    public void OnValueChanged(Int32 newValue)
    {
        movementType = newValue;
        Debug.Log("El tipo de movimiento ha cambiado a " + movementType);
        PlayerPrefs.SetInt(movementTypeKey, movementType);
        PlayerPrefs.Save();
    }
}
