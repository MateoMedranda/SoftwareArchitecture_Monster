# Arquitectura de Software

Este repositorio reúne los proyectos de la asignatura **Arquitectura de Software**. Cada proyecto se administrará de forma independiente mediante su propia rama.

## Política de ramas

- La rama principal (`main` o `master`) **no se utilizará para el desarrollo** ni para subir entregables de los proyectos.
- Cada carpeta o proyecto tendrá una rama dedicada. Los cambios de un proyecto deben realizarse y confirmarse únicamente en su rama correspondiente.
- No se deben mezclar archivos, cambios ni commits de proyectos distintos en una misma rama.
- Antes de empezar a trabajar, verifica la rama activa con `git branch` y cámbiate a la rama del proyecto con `git switch <nombre-rama>`.

## Ramas iniciales

Por el momento se manejarán las siguientes cuatro ramas:

| Rama | Carpeta / proyecto asociado |
| --- | --- |
| `soap-java` | `TI.1.1. PAO202651_SOAP_JAVA_SINBDD_NRC36884_GR07` |
| `soap-dotnet` | `TI.1.2. PAO202651_SOAP_DOTNET_SINBDD_NRC36884_GR07` |
| `restful-java` | `TI.1.3. PAO202651_RESTFUL_JAVA_SINBDD_NRC36884_GR07` |
| `restful-dotnet` | `TI.1.4. PAO202651_RESTFUL_DOTNET_SINBDD_NRC36884_GR07` |

## Flujo de trabajo

1. Selecciona la rama del proyecto que vas a modificar:

   ```bash
   git switch soap-java
   ```

2. Realiza los cambios solamente dentro de la carpeta asociada a esa rama.
3. Revisa y confirma los cambios:

   ```bash
   git status
   git add <archivos-del-proyecto>
   git commit -m "Descripción breve del cambio"
   ```

4. Sube los cambios a la misma rama:

   ```bash
   git push origin soap-java
   ```

Si se incorpora un proyecto nuevo, primero se debe crear su carpeta y una rama exclusiva con un nombre claro y consistente con este esquema.
