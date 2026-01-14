using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Whisper.Samples;

public class WeaponSwitching : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Referencia al MicrophoneController para escuchar comandos de voz")]
    public MicrophoneController microphoneController;
    
    [Tooltip("GameObject que contiene todas las armas como hijos")]
    public Transform weaponsContainer;
    
    [Tooltip("Transform del controlador derecho (Right Hand XR Controller)")]
    public Transform rightHandTransform;
    
    [Tooltip("Input action para el botón B del controlador derecho")]
    public InputActionReference switchWeaponButton;
    
    [Header("Configuración")]
    [Tooltip("Offset de posición respecto al controlador")]
    public Vector3 positionOffset = Vector3.zero;
    
    [Tooltip("Offset de rotación respecto al controlador")]
    public Vector3 rotationOffset = Vector3.zero;
    
    // Arma actualmente equipada
    private GameObject currentWeapon;
    private string equippedWeaponName = "";
    
    // Control de rotación de armas
    private string[] weaponNames = { "sword", "axe", "spear", "mace" };
    private int currentWeaponIndex = 0;
    private bool wasPressingSwitch = false;
    
    void Start()
    {
        // Suscribirse al evento de comandos de armas
        if (microphoneController != null)
        {
            microphoneController.onWeaponCommand += OnWeaponCommand;
            Debug.Log("[WeaponSwitching] Suscrito a eventos de comandos de armas.");
        }
        else
        {
            Debug.LogError("[WeaponSwitching] ¡MicrophoneController no asignado!");
        }
        
        // Verificar que el contenedor de armas esté asignado
        if (weaponsContainer == null)
        {
            Debug.LogError("[WeaponSwitching] ¡weaponsContainer no asignado!");
        }
        
        // Buscar automáticamente el Right Hand Controller si no está asignado
        if (rightHandTransform == null)
        {
            // Intentar encontrar el XR Origin y el Right Hand Controller
            var xrControllers = FindObjectsByType<XRBaseController>(FindObjectsSortMode.None);
            foreach (var controller in xrControllers)
            {
                // Buscar el controlador derecho por nombre o tag
                if (controller.name.ToLower().Contains("right"))
                {
                    rightHandTransform = controller.transform;
                    Debug.Log($"[WeaponSwitching] Right Hand Controller encontrado automáticamente: {controller.name}");
                    break;
                }
            }
            
            if (rightHandTransform == null)
            {
                Debug.LogError("[WeaponSwitching] ¡No se encontró el Right Hand Controller! Asigna manualmente rightHandTransform.");
            }
        }
        
        // Equipar espada por defecto al iniciar
        Debug.Log("[WeaponSwitching] Equipando espada por defecto...");
        EquipWeapon("mace");
        
        // Habilitar el input action del botón de cambio si está asignado
        if (switchWeaponButton != null && switchWeaponButton.action != null)
        {
            switchWeaponButton.action.Enable();
            Debug.Log("[WeaponSwitching] Botón de cambio de arma habilitado (botón B)");
        }
        else
        {
            Debug.LogWarning("[WeaponSwitching] switchWeaponButton no asignado. No se podrá cambiar de arma con el botón B.");
        }
    }
    
    void Update()
    {
        // Si hay un arma equipada, mantenerla en la posición del controlador derecho
        if (currentWeapon != null)
        {
            UpdateWeaponPosition();
        }
        
        // Detectar presión del botón B para cambiar de arma
        if (switchWeaponButton != null && switchWeaponButton.action != null)
        {
            bool isPressingSwitch = switchWeaponButton.action.ReadValue<float>() > 0.5f;
            
            // Detectar flanco de subida (transición de no presionado a presionado)
            if (isPressingSwitch && !wasPressingSwitch)
            {
                SwitchToNextWeapon();
            }
            
            wasPressingSwitch = isPressingSwitch;
        }
    }
    
    private void SwitchToNextWeapon()
    {
        // Avanzar al siguiente índice
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponNames.Length;
        string nextWeapon = weaponNames[currentWeaponIndex];
        
        Debug.Log($"[WeaponSwitching] 🔄 Cambiando a siguiente arma: {nextWeapon} (índice {currentWeaponIndex}/{weaponNames.Length - 1})");
        EquipWeapon(nextWeapon);
    }
    
    private void OnWeaponCommand(string weaponName)
    {
        Debug.Log($"[WeaponSwitching] ⚡ COMANDO RECIBIDO: {weaponName.ToUpper()}");
        
        if (weaponName == "hand")
        {
            // Desequipar arma actual
            Debug.Log("[WeaponSwitching] 🖐️ Desequipando arma...");
            UnequipWeapon();
        }
        else
        {
            // Equipar el arma solicitada y actualizar el índice
            Debug.Log($"[WeaponSwitching] ⚔️ Equipando {weaponName}...");
            EquipWeapon(weaponName);
            
            // Actualizar el índice para que coincida con el arma equipada
            for (int i = 0; i < weaponNames.Length; i++)
            {
                if (weaponNames[i] == weaponName)
                {
                    currentWeaponIndex = i;
                    break;
                }
            }
        }
    }
    
    private void EquipWeapon(string weaponName)
    {
        if (weaponsContainer == null)
        {
            Debug.LogError("[WeaponSwitching] weaponsContainer no está asignado.");
            return;
        }
        
        // Capitalizar el nombre del arma para buscar el GameObject
        string capitalizedName = char.ToUpper(weaponName[0]) + weaponName.Substring(1);
        
        Debug.Log($"[WeaponSwitching] Buscando arma: '{capitalizedName}' en contenedor '{weaponsContainer.name}'");
        
        // Buscar el arma en el contenedor
        Transform weaponTransform = weaponsContainer.Find(capitalizedName);
        
        if (weaponTransform == null)
        {
            Debug.LogWarning($"[WeaponSwitching] ⚠️ Arma '{capitalizedName}' NO encontrada en el contenedor.");
            Debug.Log($"[WeaponSwitching] Armas disponibles: {ListChildren(weaponsContainer)}");
            return;
        }
        
        Debug.Log($"[WeaponSwitching] ✓ Arma '{capitalizedName}' encontrada");
        
        // Si ya hay un arma equipada, desequiparla primero
        if (currentWeapon != null)
        {
            UnequipWeapon();
        }
        
        // Equipar la nueva arma
        currentWeapon = weaponTransform.gameObject;
        equippedWeaponName = capitalizedName;
        
        Debug.Log($"[WeaponSwitching] Activando arma '{capitalizedName}'...");
        currentWeapon.SetActive(true);
        Debug.Log($"[WeaponSwitching] ⚔️ Arma '{capitalizedName}' activada: {currentWeapon.activeSelf}");
    }
    
    private string ListChildren(Transform parent)
    {
        string result = "";
        for (int i = 0; i < parent.childCount; i++)
        {
            result += parent.GetChild(i).name + ", ";
        }
        return result;
    }
    
    private void UnequipWeapon()
    {
        if (currentWeapon != null)
        {
            // Devolver el arma al contenedor
            currentWeapon.transform.SetParent(weaponsContainer);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
            currentWeapon.SetActive(false);
            
            Debug.Log($"[WeaponSwitching] Arma desequipada.");
            currentWeapon = null;
            equippedWeaponName = "";
        }
    }
    
    private void UpdateWeaponPosition()
    {
        if (rightHandTransform == null)
        {
            Debug.LogWarning("[WeaponSwitching] rightHandTransform no está asignado.");
            return;
        }

        // Obtener la posición y rotación mundial del controlador derecho
        Vector3 controllerPosition = rightHandTransform.position;
        Quaternion controllerRotation = rightHandTransform.rotation;

        // Aplicar offsets específicos por arma
        switch (equippedWeaponName)
        {
            case "Axe":
                controllerRotation *= Quaternion.Euler(0f, 90f, 45f);
                break;
            case "Sword":
                controllerRotation *= Quaternion.Euler(0f, 270f, -45f);
                break;
            case "Spear":
                controllerRotation *= Quaternion.Euler(0f, 90f, 45f);
                controllerPosition += rightHandTransform.TransformDirection(new Vector3(0f, 0.3f, 0.3f));
                break;
            default:
                controllerRotation *= Quaternion.Euler(45f, 0f, 0f);
                break;
        }
        
        // Aplicar offsets adicionales configurables
        currentWeapon.transform.position = controllerPosition + rightHandTransform.TransformDirection(positionOffset);
        currentWeapon.transform.rotation = controllerRotation * Quaternion.Euler(rotationOffset);
    }
    
    private void OnDestroy()
    {
        // Desuscribirse del evento al destruir el objeto
        if (microphoneController != null)
        {
            microphoneController.onWeaponCommand -= OnWeaponCommand;
        }
    }
}
