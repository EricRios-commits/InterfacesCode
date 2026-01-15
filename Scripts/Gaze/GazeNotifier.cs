using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Waves;

namespace Gaze
{
    public class GazeNotifier : MonoBehaviour, IGazeProgressProvider
    {
        public float holdTime;
        public UnityEvent gazeAlert;
        private float timer;
        private bool activatedTimer = false;
        private bool alreadySpoken;

        [SerializeField]
        private AlbertoTrigger triggerNotifier;

        /// <summary>
        /// Returns the current gaze progress as a value between 0 and 1
        /// </summary>
        public float GazeProgress => holdTime > 0 ? Mathf.Clamp01(timer / holdTime) : 0f;

        /// <summary>
        /// Returns true if the gaze timer is currently active
        /// </summary>
        public bool IsGazing => activatedTimer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            timer = 0f;
            alreadySpoken = false;
            triggerNotifier.TriggerEnabled += RestartInteraction;
            activatedTimer = false;
        }

        private void RestartInteraction()
        {
            alreadySpoken = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (activatedTimer)
            {
                timer += Time.deltaTime;
            }
            if (timer >= holdTime && !alreadySpoken)
            {
                gazeAlert.Invoke();
                timer = 0;
                alreadySpoken = true;
            }
        }

        /// <summary>
        /// Listener for the OnHoverEnter callback of the XR Simple Interactable
        /// </summary>
        /// <param name="args"></param>
        public void OnHoverEnter(HoverEnterEventArgs args)
        {
            if (args.interactorObject is XRGazeInteractor)
            {
                Debug.Log("Llamada a función Enter");
                activatedTimer = true;
            }
        }

        /// <summary>
        /// Listener of the OnHoverExit callback of the XR Simple Interactable 
        /// </summary>
        /// <param name="args"></param>
        public void OnHoverExit(HoverExitEventArgs args)
        {
            // if (args.interactorObject is XRGazeInteractor)
            {
                Debug.Log("Llamada a función Exit");
                activatedTimer = false;
                timer = 0f;   
            }
        }

        /// <summary>
        /// Debug function to manually trigger the GazeAlert event from the Inspector
        /// </summary>
        [ContextMenu("Trigger GazeAlert")]
        public void DebugTriggerGazeAlert()
        {
            Debug.Log("Debug: Manually triggering GazeAlert event");
            gazeAlert.Invoke();
        }
    }
}
