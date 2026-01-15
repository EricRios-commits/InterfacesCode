# InterfacesReborn - Prototipo Interfaces Inteligentes

## Descripción del Proyecto

InterfacesReborn es un prototipo de juego VR desarrollado para Meta Quest 2 que sitúa al jugador en una arena medieval donde debe sobrevivir oleadas progresivas de enemigos utilizando comandos de voz para invocar armas y combatir con movimientos físicos realistas.
![gif](./gifs/gameplay.gif)
## Características Principales

### Mecánicas de Juego
- **Sistema de oleadas (waves)** con dificultad progresiva.
- **Combate basado en física** con detección de velocidad de movimiento.
- **Comandos de voz** para cambio de armas (Sword, Axe, Spear, Mace).
- **Tres tipos de enemigos**: Esqueletos (cuerpo a cuerpo), Magos (ataques a distancia), Golems (tanque pesado).
- **NPC interactivo** que proporciona consejos entre oleadas mediante IA (LLM).
- **Sistema de vida** con visualización en barras de salud para jugador y enemigos.

### Escenas del Juego
1. **Main Menu**: Configuración de movimiento (joystick/teleport) y transición con fade.
2. **Coliseo**: Arena principal con sistema de oleadas y combate:
    - **Pantalla de Muerte**: Habitación oscura con opción de reinicio (Habitación perteneciente a la escena de **Coliseo**).

## Cuestiones Importantes para el Uso

### Requisitos del Sistema
- Meta Quest 2
- Unity 6000.0.58f2 o superior
- XR Interaction Toolkit
- API Key de Groq (reconocimiento de voz - gratuita en https://console.groq.com)
- Conexión a Internet (para transcripción de audio)

### Controles
- **Botón A (controlador derecho)**: Mantener presionado para grabar comando de voz.
- **Botón B (controlador derecho)**: Cambiar arma (rotación cíclica).
- **Botón Menu (controlador izquierdo)**: Abrir menú de pausa.
- **Movimiento físico**: Balancear el arma para atacar (requiere velocidad mínima).

### Comandos de Voz Reconocidos
- "Sword" / "Espada"
- "Axe" / "Hacha"
- "Spear" / "Lanza"
- "Mace" / "Maza"
- "Hand" / "Mano" (desequipar arma)

### Configuración Inicial

#### 1. Configurar API Key de Groq (Reconocimiento de Voz)

La API key NO está incluida en el repositorio por seguridad. Cada desarrollador debe configurarla localmente:

**Paso 1: Obtener API Key**
- Ve a https://console.groq.com/keys
- Regístrate gratis (límite: 14,400 requests/día)
- Crea una nueva API Key y cópiala

**Paso 2: Configurar localmente**
- Navega a `Assets/Resources/`
- Abre el archivo `groq_config.json` (si no existe, copia `groq_config.json.example` y renómbralo)
- Reemplaza `PEGA_TU_API_KEY_AQUI` con tu API key:
```json
{
  "apiKey": "gsk_tu_key_aqui",
  "serverUrl": "https://api.groq.com/openai/v1/audio/transcriptions",
  "modelName": "whisper-large-v3"
}
```
- **NO subas este archivo a Git** (ya está en .gitignore)

**Paso 3: Verificar**
- El script `WhisperServerClient` cargará automáticamente la configuración al iniciar
- Verás en la consola: `✅ API Key cargada desde groq_config.json`

#### 2. Configuración en Quest 2
2. Asegurar que el Quest 2 tenga conexión a Internet.
3. Seleccionar tipo de locomoción en el Main Menu.
4. Probar reconocimiento de voz manteniendo presionado el botón A.

## Hitos de Programación Logrados

### 1. **Patrones de Diseño y Arquitectura (Relacionado con POO y diseño de software)**
- **Observer Pattern**: Sistema de salud (`IHealthObserver`) para notificar cambios de HP a múltiples componentes.
- **Strategy Pattern**: Modificadores de daño (`IDamageModifier`) para diferentes tipos de resistencias.
- **Dependency Injection**: Componentes desacoplados que reciben dependencias explícitamente.
- **Single Responsibility Principle**: Clases con una única responsabilidad (ej: `HealthComponent` solo gestiona salud).

```csharp
// Ejemplo de Observer Pattern
public interface IHealthObserver
{
    void OnHealthChanged(float currentHealth, float maxHealth, float delta);
    void OnDamageTaken(DamageInfo damageInfo, float currentHealth, float maxHealth);
    void OnDeath(GameObject dead, DamageInfo finalDamage);
}
```

### 2. **Sistema de Combate Completo (Relacionado con física y colisiones)**
- Detección de velocidad mediante sensores XR (`DamageDealerSensor.cs`).
- Sistema de daño con tipos (`DamageType`) y modificadores.
- Hitboxes configurables por arma.
- Pool de objetos para proyectiles (`PoolManager.cs`, `ObjectPool.cs`).

```csharp
// Detección de velocidad en plano horizontal (ignora gravedad)
Vector3 velocityXZ = new Vector3(deviceVelocity.x, 0f, deviceVelocity.z);
_currentVelocity = velocityXZ.magnitude;
```

### 3. **Reconocimiento de Voz con IA (Relacionado con interfaces multimodales)**
- Integración con API de Groq (Whisper Large V3) para speech-to-text en la nube.
- Procesamiento remoto para mejor velocidad (2-3s vs 15s local).
- Amplificación de audio con normalización automática.
- Sistema de similitud de Levenshtein para tolerancia a errores.
- Comandos de voz con detección fuzzy matching.

```csharp
// Amplificación de audio para mejorar reconocimiento
private float[] AmplifyAudio(float[] audioData, float gain)
{
    float[] amplified = new float[audioData.Length];
    // Amplifica y normaliza para evitar clipping
}
```

### 4. **Sistema de Oleadas Procedural (Relacionado con algoritmos y estructuras de datos)**
- Generador de oleadas con curvas de dificultad (`StandardWaveGenerator.cs`).
- Sistema de composición de enemigos por pesos.
- Oleadas especiales (Elite, Boss) cada N rondas.
- Escalado de dificultad basado en `AnimationCurve`.

```csharp
// Cálculo de enemigos usando curva de crecimiento
float curveValue = profile.enemyCountGrowthCurve.Evaluate(waveNumber / 100f) * 10f;
int enemyCount = Mathf.RoundToInt(profile.baseEnemyCount + curveValue);
```

### 5. **IA de Comportamiento de Enemigos (Relacionado con IA y grafos de comportamiento)**
- Sistema de behavior trees con Unity Behavior.
- Estados de enemigos (patrullar, perseguir, atacar).
- Sensores para detección de jugador (`ColliderSensor.cs`, `RaycastSensor.cs`).
- Acciones de ataque polimórficas (`EnemyAttack.cs`).

### 6. **Interfaz Multimodal (Relacionado con HCI y usabilidad)**
- Menús 3D adaptados para VR (`WorldSpaceOverlayUI.cs`).
- Feedback visual de daño con color y escala.
- Barras de salud contextuales (sobre enemigos y en muñeca del jugador).
- Menú de pausa con congelación de tiempo.

### 7. **Gestión de Estado y Persistencia (Relacionado con gestión de datos)**
- `PlayerPrefs` para configuración de movimiento.
- Sistema de reset para pooling (`IResettable`).
- Transiciones de escena con fade (`ScreenFader.cs`).
- Sistema de muerte y respawn del jugador.

### 8. **Optimización y Rendimiento (Relacionado con rendimiento en tiempo real)**
- Object pooling para enemigos y proyectiles.
- Custom PlayerLoop para timers (`TimerBootstrapper.cs`).
- Lazy initialization de componentes.
- Reducción de llamadas con `FixedUpdate` para física.

## Aspectos Destacables de la Aplicación

### 1. **Sistema de Reconocimiento de Voz Robusto**
- Procesamiento en la nube mediante API de Groq (Whisper Large V3).
- Transcripción rápida (2-3 segundos) sin sobrecarga del Quest 2.
- Amplificación adaptativa de audio con normalización automática.
- Algoritmo de similitud de Levenshtein para tolerar errores de pronunciación.
- Múltiples patrones por palabra para mejorar precisión.
- Umbral de similitud ajustable (50% por defecto).
- 14,400 requests gratuitos por día (suficiente para uso extensivo).

### 2. **Arquitectura Extensible y Mantenible**
- Uso extensivo de interfaces para desacoplar componentes.
- ScriptableObjects para configuración sin código (`WaveGenerationProfile`, `EntityStatsProfile`).
- Sistema de eventos para comunicación entre sistemas.
- Patrones de diseño bien implementados (Observer, Strategy, Factory).

### 3. **Sistema de Oleadas Dinámico**
- Generación procedural de oleadas con balance automático.
- Curvas de dificultad configurables visualmente.
- Sistema de pesos para composición de enemigos.
- Tipos de oleadas especiales (Normal, Elite, Boss).

### 4. **Combate Basado en Física Real**
- Detección de velocidad solo en plano horizontal (ignora gravedad).
- Velocidad mínima configurable para validar ataques.
- Sistema anti-spam (un golpe por entrada de collider).
- Feedback visual y auditivo inmediato.

### 5. **NPC con IA Generativa**
- Integración con modelo LLM (Llama 3.1:8b) para diálogos dinámicos.
- Prompt engineering para mantener contexto del juego.
- Tres categorías de comentarios (consejos, trivia, easter eggs).
- Respuestas limitadas para mantener inmersión.

### 6. **Sistema de Salud Modular**
- Modificadores de daño apilables (`ArmorModifier`, `CriticalHitModifier`).
- Resistencias por tipo de daño.
- Observer pattern para múltiples visualizaciones.
- Invulnerabilidad temporal y revivir.

## Sensores de Interfaces Multimodales Incluidos

### 1. **Sensor de Movimiento (XR Device Velocity)**
- **Ubicación**: `DamageDealerSensor.cs`
- **Función**: Detecta la velocidad del controlador VR para validar ataques.
- **Configuración**: 
  - Velocidad mínima: 0.5 m/s
  - Ganancia de amplificación: 4.0x
  - Solo plano horizontal (ignora eje Y)

```csharp
if (_controllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out deviceVelocity))
{
    Vector3 velocityXZ = new Vector3(deviceVelocity.x, 0f, deviceVelocity.z);
    _currentVelocity = velocityXZ.magnitude;
}
```

### 2. **Sensor de Voz (Groq Whisper Large V3 API)**
- **Ubicación**: `MicrophoneController.cs` + `WhisperServerClient.cs`
- **Función**: Reconocimiento de comandos de voz para cambio de armas mediante API en la nube.
- **Tecnología**: Groq API con modelo Whisper Large V3 (https://console.groq.com)
- **Ventajas**:
  - Procesamiento remoto ultra-rápido (2-3 segundos)
  - Sin carga de procesamiento en Quest 2
  - Modelo de última generación (mejor precisión)
  - Gratuito con límite de 14,400 requests/día
- **Configuración**:
  - Endpoint: `https://api.groq.com/openai/v1/audio/transcriptions`
  - Modelo: `whisper-large-v3`
  - Tiempo mínimo de grabación: 0.5s
  - Amplificación de audio: 4.0x
  - Formato de audio: WAV (16kHz, mono)

### 3. **Sensor de Luz Ambiental (Light Sensor)**
- **Ubicación**: `LightController.cs`
- **Función**: Ajusta la iluminación del mundo virtual según luz ambiental real.
- **Método**: Análisis de passthrough de cámara con mipmaps.
- **Actualización**: Cada 0.5s

```csharp
float brightness = GetBrightness(); // De passthrough
smoothBrightness = Mathf.Lerp(smoothBrightness, brightness, 0.1f);
worldLight.intensity = smoothBrightness;
```

### 4. **Sensor de Posición (XR Tracking)**
- **Ubicación**: Implícito en `XROrigin`
- **Función**: Tracking 6DOF (6 grados de libertad) para cámara y controladores.
- **Uso**: Posicionamiento de armas en mano del jugador.

### 5. **Sensor Háptico (Controller Vibration)**
- **Ubicación**: Sistema XR nativo.
- **Función**: Feedback táctil en colisiones y eventos.
- **Implementación**: Integrado con XR Interaction Toolkit.

### 6. **Sensor Gaze/Eye Tracking (Gaze Interactor)**
- **Ubicación**: `GazeController.cs`
- **Función**: Detección de mirada para interacción con NPC Alberto.
- **Configuración**: 
  - Tiempo de mantenimiento: Configurable.
  - Evento activado tras tiempo fijo mirando al objetivo.

```csharp
public void OnHoverEnter(HoverEnterEventArgs args)
{
    if (args.interactorObject is XRGazeInteractor)
    {
        activatedTimer = true;
    }
}
```

## Acta de Acuerdos del Grupo

### Metodología de Trabajo
- Desarrollo ágil con reuniones semanales.
- Convenciones de código basadas en C# Coding Conventions.
- Uso de branches feature para nuevas funcionalidades.

### Reparto de Tareas

#### Tareas Individuales

**Fabián González Lence:**
- Reconocimiento de voz.
- Sistema de armas y hitboxes de las mismas.
- Implementación de sensores XR.

**Eric Ríos Hamilton:**
- Sistema de combate y oleadas.
- Sistema de salud.
- IA de enemigos con Behavior Trees.

**Enmanuel Vegas Acosta:**
- Integración con LLM.
- Transiciones de escenas.
- Efectos visuales y audio.

**Diego Hernández Chico:**
- Integración de modelado 3D y animaciones.
- Configuración de hitboxes de los entornos de escenas.
- UI/UX en VR.

> [!IMPORTANT]
> En gran medida, todos los miembros han trabajado en la mayoría de tareas de forma colaborativa y no se debería atribuir el mérito de un miembro a únicamente los puntos mencionados, sino a parte de las demás tareas asignadas a otros miembros como colaborador.

#### Tareas Desarrolladas en Grupo
- Diseño de arquitectura general del proyecto
- Definición de interfaces y contratos entre sistemas
- Pruebas de jugabilidad y balance
- Creación de assets 3D y configuración de escenas
- Debugging de integración entre sistemas

### Herramientas y Flujo de Trabajo
- **Control de versiones**: Git + GitHub
- **Gestión de tareas**: GitHub Projects
- **Comunicación**: Discord / WhatsApp
- **Documentación**: Markdown + comentarios en código

## Check-list de Recomendaciones de Diseño VR

### Comodidad y Prevención de Motion Sickness

| Recomendación | Estado | Detalles |
|--------------|--------|----------|
| Framerate estable (90 FPS mínimo en Quest 2) | Se contempla | Optimización con pooling |
| Dos opciones de locomoción (teleport y smooth) | Se contempla | Configurable en Main Menu |
| Vignette al movimiento artificial | No se contempla | Podría agregarse para reducir motion sickness |
| Evitar aceleraciones bruscas de cámara | Se contempla | Movimiento controlado por jugador |
| Punto de referencia estático (horizonte) | Se contempla | Arena con paredes visibles |
| Opciones de snap turning | Se contempla | Solo rotación con snap turning |

### Interacción Natural

| Recomendación | Estado | Detalles |
|--------------|--------|----------|
| Retroalimentación háptica en interacciones | Se contempla | Vibraciones en colisiones |
| Escala 1:1 de objetos | Se contempla | Armas en tamaño real |
| Feedback visual inmediato | Se contempla | Cambios de color, partículas, sonidos |
| Evitar UI fija en cámara | Se contempla | UI 3D en mundo (World Space Canvas) |
| Reach ergonómico para interactivos | Se contempla | Botones y objetos alcanzables |

### Usabilidad

| Recomendación | Estado | Detalles |
|--------------|--------|----------|
| Tutorial o zona de práctica | No se contempla | Podría añadirse para onboarding |
| Indicadores claros de objetivos | Se contempla | UI de oleadas, contador de enemigos |
| Opciones de accesibilidad | No se contempla | Solo configuración de movimiento |
| Menú de pausa accesible | Se contempla | Botón Menu del controlador izquierdo |
| Sistema de guardado de progreso | No se contempla | Las oleadas no persisten entre sesiones |

### Audio

| Recomendación | Estado | Detalles |
|--------------|--------|----------|
| Audio espacializado 3D | Se contempla | AudioSource 3D en enemigos y efectos |
| Volumen ajustable | No se contempla | Sin configuración de audio |
| Feedback auditivo de acciones | Se contempla | Sonidos de golpes, daño, muerte |
| Audio no invasivo en menús | Se contempla | Sondidos ambientales suaves |

### Rendimiento

| Recomendación | Estado | Detalles |
|--------------|--------|----------|
| Occlusion culling | Se contempla | Configurado en escenas |
| LOD en modelos 3D | No se contempla | Depende de assets importados |
| Object pooling | Se contempla | Sistema completo implementado |
| Baked lighting | No se contempla | Una única fuente de luz |

### Seguridad y Confort

| Recomendación | Estado | Detalles |
|--------------|--------|----------|
| Advertencia de espacio físico | No aplica | Responsabilidad del Guardian de Quest |
| Recordatorios de descanso | No se contempla | Sesiones cortas recomendadas |
| Opción de altura de jugador | No se contempla | Usa calibración de Quest |
| Brillo ajustable | No se contempla | Configuración de sistema Quest |

### Inmersión

| Recomendación | Estado | Detalles |
|--------------|--------|----------|
| Manos/controladores virtuales visibles | Se contempla | Armas en mano derecha |
| Sonido ambiente coherente | Se contempla | Atmósfera de coliseo |
| Interacciones físicas realistas | Se contempla | Combate basado en física real |
| Escala consistente del mundo | Se contempla | Proporciones realistas |

## Estructura del Proyecto

```
InterfacesReborn/
├── Assets/
│   ├── Scenes/
│   │   ├── MainMenu.unity
│   │   └── Coliseo.unity
│   ├── Scripts/
│   │   ├── Actors/                 # Barras de salud de actores
│   │   ├── Behavior/               # IA y comportamiento de enemigos
│   │   │   ├── Enemy/
│   │   │   │   ├── EnemyAI.cs
│   │   │   │   ├── EnemyAttack.cs
│   │   │   │   └── EnemySpawner.cs
│   │   │   └── Conditions/
│   │   ├── Combat/                 # Sistema de combate
│   │   │   ├── HealthComponent.cs
│   │   │   ├── DamageDealer.cs
│   │   │   ├── DamageDealerSensor.cs
│   │   │   ├── Projectile.cs
│   │   │   └── Interfaces/
│   │   ├── Player/                 # Sistemas del jugador
│   │   │   ├── PlayerDeathHandler.cs
│   │   │   └── PlayerMovementType.cs
│   │   ├── Waves/                  # Sistema de oleadas
│   │   │   ├── WaveManager.cs
│   │   │   ├── WaveGenerator.cs
│   │   │   └── WaveTrigger.cs
│   │   ├── VoiceController/        # Reconocimiento de voz con Groq API
│   │   │   ├── MicrophoneController.cs
│   │   │   ├── WhisperServerClient.cs
│   │   │   └── WeaponSwitching.cs
│   │   ├── LLMAnswer/              # Integración con LLM
│   │   ├── UI/                     # Interfaces de usuario
│   │   ├── Utility/                # Utilidades generales
│   │   │   ├── PoolManager.cs
│   │   │   ├── ObjectPool.cs
│   │   │   └── Timers/
│   │   └── Sensors/                # Sensores ambientales
│   ├── Prefabs/
│   │   ├── Enemies/
│   │   ├── Weapons/
│   │   └── UI/
│   └── Materials/
├── Packages/
└── ProjectSettings/
```

## Tecnologías Utilizadas

- **Unity 6000.0.5.8f2** - Motor de juego
- **XR Interaction Toolkit** - Framework de interacción VR
- **Unity Behavior** - Sistema de behavior trees
- **Groq API (Whisper Large V3)** - Reconocimiento de voz en la nube
- **Meta XR SDK** - Integración específica de Quest
- **TextMeshPro** - Renderizado de texto

## Referencias y Recursos

- [Unity XR Interaction Toolkit Documentation](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest)
- [Meta Quest Developer Documentation](https://developer.oculus.com/documentation/unity/)
- [Groq API Documentation](https://console.groq.com/docs)
- [Groq Console (API Keys)](https://console.groq.com/keys)
- [Whisper AI (OpenAI)](https://github.com/openai/whisper)
- [Unity Behavior Documentation](https://docs.unity.com/behavior/)

## Contribuciones

Este proyecto es parte de una asignatura académica. Las contribuciones externas no están abiertas actualmente.

## Licencia

MIT

## Miembros del Equipo

- Fabián González Lence - alu0101549491@ull.edu.es
- Eric Ríos Hamilton - alu0101549835@ull.edu.es
- Diego Hernández Chico - alu0101572062@ull.edu.es
- Enmanuel Vegas Acosta - alu0101281698@ull.edu.es

---
