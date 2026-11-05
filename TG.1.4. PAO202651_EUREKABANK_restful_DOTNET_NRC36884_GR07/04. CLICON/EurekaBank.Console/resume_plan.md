# Plan para la creación del resumen de clases del proyecto

> **Destinatario:** agente de implementación.
> **Objetivo:** analizar el código fuente de un proyecto organizado bajo el patrón MVC y generar un documento de resumen (`resumen_clases.md`) que describa, por paquete, los roles que cumplen las clases.

---

## 1. Contexto y alcance

El agente recibe un proyecto con arquitectura MVC (Modelo, Vista, Controlador), acompañado de paquetes de servicios y una clase principal. La tarea consiste en **leer el código, comprender el rol de cada paquete y redactar un resumen** siguiendo la estructura y el estilo definidos en este plan.

**Entregable único:** un archivo `resumen_clases.md` en la raíz del proyecto (o en la carpeta que indique el usuario).

**Fuera de alcance:**
- No modificar el código fuente del proyecto.
- No generar documentación de métodos, atributos ni firmas.
- No documentar clases individuales por nombre.

---

## 2. Reglas de estructura

El documento final contiene **seis secciones**, en este orden exacto:

| # | Sección | Qué debe describir |
|---|---------|--------------------|
| 1 | Creación de Paquetes | Breve descripción del paquete MVC y cómo se organiza el proyecto. |
| 2 | Creación de Modelos | Funcionalidades que se manejan dentro de las clases de modelos. |
| 3 | Creación de Vistas | Estructura de UI que manejan los archivos del paquete de vistas (pantallas principales, componentes reutilizables, etc.). |
| 4 | Creación de Servicios | Funcionalidades del paquete de servicios, junto con los paquetes adicionales (utilidades, configuración, persistencia, etc.). |
| 5 | Creación de Controladores | Descripción de los controladores, sus roles y funciones. |
| 6 | Creación de la clase principal | Rol de la clase principal (punto de entrada). |

---

## 3. Reglas de estilo de escritura

1. **Tiempo verbal presente**, con construcciones como: *"Se crean…"*, *"que se conectan…"*, *"que manejan…"*.
2. **Enfoque en roles:** cada sección explica qué responsabilidad cumplen las clases del paquete en conjunto.
3. **Un solo párrafo por sección:** corto, breve y conciso (máximo 3–5 líneas).
4. **No se nombran clases específicas** (ni en texto, ni en listas, ni entre paréntesis).
5. **Se permiten flujos generales** (por ejemplo: "la vista captura la acción, el controlador la procesa y el modelo actualiza los datos"), sin detallar pasos internos.
6. Sin viñetas dentro de los párrafos de resumen; solo prosa.
7. Tono técnico, neutro y en español.

---

## 4. Pasos de implementación

### Paso 1 — Reconocer la estructura del proyecto
- Listar el árbol de directorios del proyecto (`Glob` con patrón `**/*`).
- Identificar el lenguaje y la convención de paquetes (por ejemplo `src/main/java/...`, `src/`, `app/`).
- Localizar los paquetes equivalentes a: **modelos**, **vistas**, **controladores**, **servicios** y cualquier **paquete adicional**.
- Localizar la **clase principal** (punto de entrada: `main`, `App`, `Main`, `Program`, etc.).

> Si algún paquete no existe, anotarlo y omitir su sección indicando brevemente que el proyecto no lo contempla (ver Paso 8).

### Paso 2 — Analizar el paquete de modelos
- Leer los archivos del paquete para entender: qué datos representan, qué reglas o validaciones encapsulan, cómo se relacionan entre sí.
- Extraer **funcionalidades generales** (representar entidades, encapsular lógica de negocio, mantener estado), no nombres de clases.

### Paso 3 — Analizar el paquete de vistas
- Identificar la estructura de UI: pantallas principales, ventanas o formularios, componentes reutilizables, navegación entre pantallas.
- Determinar cómo la vista se comunica con los controladores (eventos, listeners, enlaces de datos).

### Paso 4 — Analizar el paquete de servicios y paquetes adicionales
- Leer el paquete de servicios para identificar sus funcionalidades (acceso a datos, lógica de apoyo, integraciones, validaciones, etc.).
- Revisar los paquetes adicionales (utilidades, configuración, persistencia, constantes, excepciones) y resumir su aporte en conjunto.

### Paso 5 — Analizar el paquete de controladores
- Identificar cómo los controladores reciben las acciones de la vista, invocan servicios y actualizan modelos.
- Extraer el **flujo general** de coordinación entre vista, servicio y modelo.

### Paso 6 — Analizar la clase principal
- Determinar su rol: inicialización de la aplicación, ensamblaje de componentes, arranque de la primera vista.

### Paso 7 — Redactar el documento
Crear `resumen_clases.md` con la siguiente plantilla (reemplazar cada marcador por un párrafo propio, siguiendo las reglas de estilo):

```markdown
# Resumen de clases del proyecto

## Creación de Paquetes
[Párrafo: organización MVC del proyecto y cómo se distribuyen los paquetes.]

## Creación de Modelos
[Párrafo: funcionalidades que manejan las clases de modelos.]

## Creación de Vistas
[Párrafo: estructura de UI, pantallas principales y componentes reutilizables.]

## Creación de Servicios
[Párrafo: funcionalidades del paquete de servicios y de los paquetes adicionales.]

## Creación de Controladores
[Párrafo: roles y funciones de los controladores, con el flujo general que coordinan.]

## Creación de la clase principal
[Párrafo: rol de la clase principal como punto de entrada.]
```

### Paso 8 — Casos especiales
- **Paquete ausente:** conservar el encabezado y escribir una sola frase indicando que el proyecto no incluye ese paquete.
- **Proyecto sin MVC estricto:** adaptar la descripción al esquema equivalente que exista, manteniendo los seis encabezados.
- **Paquetes adicionales múltiples:** resumirlos juntos dentro de la sección de Servicios.

### Paso 9 — Verificación final
Antes de entregar, validar cada punto de la lista:

- [ ] Existen exactamente las seis secciones, en el orden indicado.
- [ ] Cada sección contiene un único párrafo breve.
- [ ] Todos los verbos están en presente.
- [ ] No aparece ningún nombre de clase específica.
- [ ] Los flujos, si se mencionan, son generales.
- [ ] El texto está en español y sin viñetas en los resúmenes.
- [ ] El archivo no modifica ni duplica código del proyecto.

---

## 5. Ejemplo de estilo esperado (ilustrativo)

> **Creación de Controladores**
> Se crean los controladores que reciben las acciones de la interfaz, las interpretan y coordinan la comunicación con los servicios y los modelos. Estos controladores manejan el flujo de la aplicación: capturan el evento, solicitan la operación correspondiente y actualizan la vista con el resultado.

Este ejemplo solo ilustra tono, longitud y enfoque; el contenido real debe derivarse del análisis del proyecto.

---

## 6. Criterios de aceptación

- El archivo `resumen_clases.md` existe y sigue la plantilla del Paso 7.
- El contenido refleja fielmente el código analizado.
- Se respetan todas las reglas de estructura y estilo de las secciones 2 y 3.
- El resumen es legible en menos de dos minutos.