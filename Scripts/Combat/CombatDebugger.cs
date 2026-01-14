// using UnityEngine;
// using Combat;

// /// <summary>
// /// Script de diagnóstico para identificar problemas en el sistema de combate.
// /// Añádelo al jugador o a un GameObject vacío en la escena.
// /// </summary>
// public class CombatDebugger : MonoBehaviour
// {
//     [Header("Referencias a verificar")]
//     [SerializeField] private GameObject testDummy;
//     [SerializeField] private GameObject[] weapons;
    
//     void Start()
//     {
//         Debug.Log("========== COMBAT DEBUGGER ==========");
//         CheckTestDummy();
//         CheckWeapons();
//         Debug.Log("====================================");
//     }
    
//     void Update()
//     {
//         // Detectar si se presiona el trigger
//         if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
//         {
//             Debug.Log("[CombatDebugger] ⚡ TRIGGER DERECHO PRESIONADO");
//             CheckActiveWeapon();
//         }
//     }
    
//     private void CheckTestDummy()
//     {
//         if (testDummy == null)
//         {
//             Debug.LogError("[CombatDebugger] ❌ TestDummy no asignado!");
//             return;
//         }
        
//         Debug.Log($"[CombatDebugger] TestDummy encontrado: {testDummy.name}");
//         Debug.Log($"  - Layer: {LayerMask.LayerToName(testDummy.layer)} (ID: {testDummy.layer})");
//         Debug.Log($"  - Activo: {testDummy.activeInHierarchy}");
        
//         var healthComp = testDummy.GetComponent<HealthComponent>();
//         if (healthComp != null)
//         {
//             Debug.Log($"  - ✅ HealthComponent: HP {healthComp.CurrentHealth}/{healthComp.MaxHealth}");
//         }
//         else
//         {
//             Debug.LogError("  - ❌ HealthComponent NO encontrado!");
//         }
        
//         var testDummyScript = testDummy.GetComponent<TestDummy>();
//         if (testDummyScript != null)
//         {
//             Debug.Log("  - ✅ TestDummy script encontrado");
//         }
//         else
//         {
//             Debug.LogError("  - ❌ TestDummy script NO encontrado!");
//         }
        
//         var collider = testDummy.GetComponent<Collider>();
//         if (collider != null)
//         {
//             Debug.Log($"  - ✅ Collider: {collider.GetType().Name}, IsTrigger: {collider.isTrigger}");
//         }
//         else
//         {
//             Debug.LogError("  - ❌ Collider NO encontrado!");
//         }
//     }
    
//     private void CheckWeapons()
//     {
//         if (weapons == null || weapons.Length == 0)
//         {
//             Debug.LogWarning("[CombatDebugger] ⚠️ No hay armas asignadas para verificar");
//             return;
//         }
        
//         foreach (var weapon in weapons)
//         {
//             if (weapon == null) continue;
            
//             Debug.Log($"\n[CombatDebugger] Verificando arma: {weapon.name}");
//             Debug.Log($"  - Activa: {weapon.activeInHierarchy}");
            
//             // var weaponController = weapon.GetComponent<WeaponController>();
//             // if (weaponController != null)
//             {
//                 Debug.Log("  - ✅ WeaponController encontrado");
//             }
//             else
//             {
//                 Debug.LogError("  - ❌ WeaponController NO encontrado!");
//             }
            
//             // var hitboxes = weapon.GetComponentsInChildren<WeaponHitbox>(true);
//             Debug.Log($"  - Hitboxes encontradas: {hitboxes.Length}");
            
//             foreach (var hitbox in hitboxes)
//             {
//                 Debug.Log($"    - Hitbox: {hitbox.gameObject.name}");
//                 var collider = hitbox.GetComponent<Collider>();
//                 if (collider != null)
//                 {
//                     Debug.Log($"      - Collider: {collider.GetType().Name}, IsTrigger: {collider.isTrigger}");
//                 }
//                 else
//                 {
//                     Debug.LogError("      - ❌ Collider NO encontrado en hitbox!");
//                 }
//             }
//         }
//     }
    
//     private void CheckActiveWeapon()
//     {
//         if (weapons == null) return;
        
//         foreach (var weapon in weapons)
//         {
//             if (weapon != null && weapon.activeInHierarchy)
//             {
//                 Debug.Log($"[CombatDebugger] Arma activa detectada: {weapon.name}");
                
//                 var weaponController = weapon.GetComponent<WeaponController>();
//                 if (weaponController != null)
//                 {
//                     Debug.Log($"  - WeaponController presente, IsAttacking: {weaponController.IsAttacking}");
//                 }
                
//                 return;
//             }
//         }
        
//         Debug.LogWarning("[CombatDebugger] ⚠️ No hay armas activas en este momento");
//     }
// }
