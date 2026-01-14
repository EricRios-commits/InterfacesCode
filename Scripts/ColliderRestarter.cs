using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

public class ColliderRestarter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        XRBodyTransformer body = GetComponent<XRBodyTransformer>();
        body.enabled = false;
        body.enabled = true;
    }
}
