using Gaze;
using UnityEngine;

namespace Waves
{
    public class AlbertoTrigger : WaveTrigger
    {
        
        public GazeNotifier gazeNotifier;
        public delegate void  message();
        public event message TriggerEnabled;
        [SerializeField] private float triggerDelay = 15f;
        [SerializeField] private bool autoStartTimerOnEnable = true;
        [Tooltip("Si está activado, inicia el timer automáticamente al habilitar el trigger (sin esperar la mirada de Alberto)")]
        
        private WaveManager waveManager;
        private float startedTime;
        private bool isEnabled;
        private bool timerStarted;

        void Start()
        {
            timerStarted = false;
        }

        public override void Initialize(WaveManager manager)
        {
            waveManager = manager;
        }
        
        public override void Enable()
        {
            isEnabled = true;
            gazeNotifier.gazeAlert.AddListener(StartTimer);
            TriggerEnabled?.Invoke();
            if (autoStartTimerOnEnable)
            {
                Debug.Log("[AlbertoTrigger] Trigger habilitado. Iniciando timer automáticamente...");
                StartTimer();
            }
            else
            {
                Debug.Log("[AlbertoTrigger] Trigger habilitado. Esperando mirada de Alberto...");
            }
        }
        
        public override void Disable()
        {
            isEnabled = false;
            gazeNotifier.gazeAlert.AddListener(StartTimer);
        }
        
        public override bool CanTrigger()
        {
            if (!isEnabled)
            {
                return false;
            }
            if (!timerStarted)
            {
                return false;
            }
            float timeElapsed = Time.time - startedTime;
            bool canTrigger = timeElapsed >= triggerDelay;
            return canTrigger;
        }

        public void StartTimer()
        {
            startedTime = Time.time;
            timerStarted = true;
            Debug.Log($"[AlbertoTrigger] Timer iniciado. Siguiente oleada en {triggerDelay}s");
        }
        
        private void Update()
        {
            if (CanTrigger())
            {
                Debug.Log("<color=yellow>⚡ [AlbertoTrigger] Trigger activado - Iniciando siguiente oleada</color>");
                InvokeTriggerActivated();
                timerStarted = false;
                Disable();
            }
        }
    }
}

