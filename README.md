# Práctica XR Interaction Toolkit – Interacción con objetos en VR

Escena de realidad virtual en Unity con **XR Interaction Toolkit** y **OpenXR**. Se prueba en el editor con el simulador del XR Interaction Toolkit y se puede compilar para **Meta Quest** (Android).

## Requisitos

- Unity **6000.6.4f1** (plantilla Universal 3D)
- Paquetes:
  - **XR Plugin Management**
  - **OpenXR Plugin**
  - **XR Interaction Toolkit**, con los samples **Starter Assets** y **XR Device Simulator / XR Interaction Simulator**
  - **TextMeshPro**, para el texto de la UI

## Configuración del proyecto

**XR Plug-in Management**
- **Windows (Standalone)**: OpenXR activado. Es la plataforma que usa el simulador en el editor.
- **Android**: OpenXR activado con *Meta Quest feature group / Meta Quest Support*, para el build a Quest.

**Perfiles de interacción de OpenXR** (en las dos plataformas)
- Oculus Touch Controller Profile (mandos de Quest 2)
- Meta Quest Touch Plus Controller Profile (mandos de Quest 3 / 3S)
- Meta Quest Touch Pro Controller Profile (mandos de Quest Pro)

Los avisos de *Project Validation* se resolvieron con **Fix All**.

## Escena

| Elemento | Descripción |
|---|---|
| **XR Origin (XR Rig)** | Prefab de *Starter Assets*. Sustituye a la Main Camera, que se eliminó para no tener dos cámaras. Contiene la cámara (cabeza), los dos mandos con su *Near-Far Interactor* (rayo a distancia y agarre directo cerca) y el sistema de locomoción (*Locomotion Mediator*, *Teleportation Provider*, movimiento y giro). |
| **XR Interaction Simulator** | Simula el casco y los mandos con teclado y ratón. Se desactiva para el build a las gafas. |
| **Plane** | Suelo de la escena. Solo lleva collider, sin Rigidbody. |
| **Cubo** | Lejos del jugador. Se selecciona con el **rayo** y cambia de color. |
| **Esfera** | Más lejos todavía. Se selecciona con el **rayo** y cambia de color. |
| **Objeto de la Asset Store** | Al alcance de la mano. Se coge con **interacción directa** y actualiza la UI. |
| **Canvas (World Space)** | Panel con un texto TextMeshPro que muestra el estado del objeto de la Asset Store. |

Cada objeto interactuable tiene tres componentes:

| Componente | Qué aporta | Si falta |
|---|---|---|
| **Collider** | Permite que los interactores lo detecten | Los interactores no lo ven |
| **Rigidbody** | Física: cae, rebota, se puede lanzar | No responde a la física al soltarlo |
| **XR Grab Interactable** | Lo marca como agarrable y define cómo se comporta al cogerlo | No se puede agarrar |

Los objetos están a **distintas distancias**. El cubo y la esfera solo se alcanzan con el rayo, y el objeto de la Asset Store, que está cerca, se coge directamente con la mano.

## Scripts

Los dos scripts están en `Assets/Scripts/`. Funcionan igual: se suscriben a los **eventos del interactable** (`selectEntered` / `selectExited`) en lugar de comprobar la entrada en cada frame. Así no dependen del tipo de interactor (rayo, directo, simulador o mando real). Se ejecutan cuando el XR Interaction Toolkit notifica que el objeto ha sido seleccionado o soltado.

### `ColorOnSelect.cs`

**Dónde va:** en el cubo y en la esfera.

**Qué hace:** cada vez que el objeto se selecciona con el rayo, cambia su material a un color aleatorio.

**Cómo funciona:**
- `[RequireComponent(typeof(XRBaseInteractable))]` garantiza que el objeto tenga un interactable (en este caso, el *XR Grab Interactable*).
- En `Awake()` guarda las referencias al `Renderer` (para cambiar el color) y al `XRBaseInteractable` (para escuchar sus eventos).
- En `OnEnable()` se suscribe al evento `selectEntered`, y en `OnDisable()` se da de baja. Así no quedan suscripciones colgando si el objeto se desactiva o se destruye.
- `OnSelect()` asigna a `rend.material.color` un color aleatorio con `Random.ColorHSV`, limitando saturación y brillo para que los colores sean vivos. En URP, `material.color` corresponde a la propiedad *Base Color* del shader Lit.

### `UIOnGrab.cs`

**Dónde va:** en el objeto de la Asset Store.

**Qué hace:** actualiza un texto de la UI al coger el objeto y al soltarlo, y lleva la cuenta de cuántas veces se ha agarrado.

**Cómo funciona:**
- Tiene un campo público `label` (`TMP_Text`) donde se arrastra desde el inspector el texto del Canvas.
- Se suscribe a dos eventos del interactable:
  - `selectEntered` → `OnGrab()`: incrementa el contador `grabs` y muestra `Agarrado: <nombre>` con el número de veces.
  - `selectExited` → `OnRelease()`: muestra `Soltado: <nombre>` con el mismo contador.
- Igual que el script anterior, se suscribe en `OnEnable()` y se da de baja en `OnDisable()`.

## Cómo probarlo sin gafas

1. Plataforma activa **Windows** (*File → Build Profiles*).
2. El objeto **XR Interaction Simulator** activo en la escena.
3. Pulsar **Play** y hacer clic en la ventana *Game*.

Controles principales del simulador (la lista completa aparece en el panel del simulador en pantalla):

| Acción | Control |
|---|---|
| Moverse | WASD (Q/E para bajar y subir) |
| Mirar alrededor | Ratón |
| Cambiar entre modo FPS y modo dispositivo | Tab |
| Activar mando izquierdo / derecho / ambos | `[` / `]` / `[]` |
| Agarrar / seleccionar | Grip (tecla indicada en el panel del simulador) |

**Pruebas realizadas**
- Rayo sobre el cubo o la esfera + grip → el objeto cambia de color.
- Al soltar → el objeto cae por la física.
- Acercarse al objeto de la Asset Store y cogerlo con la mano → el texto pasa a "Agarrado" y aumenta el contador.
- Al soltarlo → el texto pasa a "Soltado".

## Build para Meta Quest

1. *File → Build Profiles → Android → Switch Platform*.
2. OpenXR en la pestaña Android con *Initialize XR on Startup* y *Meta Quest Support* activados.
3. Desactivar el simulador en la escena.
4. **Build** → se genera el `.apk`, que solo funciona en gafas Meta Quest.
