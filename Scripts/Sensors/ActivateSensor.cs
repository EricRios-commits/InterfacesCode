using UnityEngine;
using UnityEngine.InputSystem;

public class ActivateSensor : MonoBehaviour
{
    public void OnEnable()
    {
        LightSensor lightSensor = InputSystem.GetDevice<LightSensor>();
        if (!lightSensor.enabled)
        {
            InputSystem.EnableDevice(lightSensor);
            Debug.Log("Lightsensor enabled");
        }
    }
}
