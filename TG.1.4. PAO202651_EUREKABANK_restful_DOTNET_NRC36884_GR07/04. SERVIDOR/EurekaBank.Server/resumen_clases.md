# Resumen de clases del proyecto

## Creación de Paquetes

El proyecto se organiza en paquetes de contratos, controladores, modelos, servicios y configuración, siguiendo una separación por capas para una API REST. Esta estructura aísla la exposición de endpoints, las reglas de negocio, el acceso a datos y las estructuras de intercambio utilizadas por los clientes.

## Creación de Modelos

Se crean modelos que representan las entidades del dominio bancario y su correspondencia con las tablas de la base de datos, incluyendo roles, sucursales, usuarios, cuentas, titulares, tokens y transacciones. Estas clases encapsulan claves, relaciones, precisiones decimales y restricciones que garantizan la integridad de la información persistida.

## Creación de Vistas

El proyecto no contempla un paquete de vistas, ya que su propósito es exponer una API REST y su documentación interactiva para el consumo de los clientes.

## Creación de Servicios

Se crean servicios que concentran las reglas de negocio y la persistencia, gestionando autenticación, tokens de sesión, consultas de cuentas y perfiles, registro de movimientos, transferencias, reversas y creación de cuentas con validaciones de permisos y saldos. Los paquetes adicionales aportan la configuración de la base de datos, los contratos de entrada y salida, y la documentación de la API.

## Creación de Controladores

Se crean controladores que exponen los endpoints REST versionados, reciben las solicitudes de los clientes, extraen el token de autorización y delegan la ejecución a la capa de servicios. Estos controladores coordinan el flujo general de cada operación y traducen los resultados del dominio en respuestas HTTP adecuadas según el tipo de éxito o error.

## Creación de la clase principal

La clase principal actúa como punto de entrada de la API, donde se configuran el contexto de datos, la inyección de dependencias, los controladores, la documentación interactiva y el enrutamiento. Su rol es ensamblar los componentes del servidor y dejarlo disponible para atender las solicitudes de los clientes.
