# Resumen de clases del proyecto

## Creación de Paquetes

El proyecto se organiza bajo el patrón MVC de ASP.NET Core en paquetes de controladores, modelos, servicios y vistas, junto con archivos estáticos, configuración y contratos compartidos. Esta estructura separa la presentación web, la coordinación de solicitudes, el acceso a la API REST y las estructuras de datos utilizadas por la aplicación.

## Creación de Modelos

Se crean modelos que representan los contratos de datos del dominio bancario, como sesiones, cuentas, clientes, perfiles, movimientos y solicitudes de operación, además de las estructuras que alimentan las vistas. Estas clases encapsulan la información, los resultados uniformes de las operaciones y las validaciones necesarias para transportar los datos entre la interfaz y los servicios.

## Creación de Vistas

Se crean vistas web que conforman una interfaz con página de inicio de sesión y panel principal, organizado en secciones de resumen, movimientos, transferencias, operaciones, administración de cuentas y perfil. Estas vistas utilizan formularios, componentes reutilizables, estilos y actualizaciones parciales, capturan las acciones del usuario y delegan la ejecución de las operaciones a los controladores.

## Creación de Servicios

Se crean servicios que encapsulan el consumo de la API REST mediante operaciones HTTP, serialización de datos, autenticación por token y manejo de tiempos de espera. Los paquetes adicionales aportan la configuración del cliente HTTP, la gestión de sesiones, los archivos estáticos, los estilos y los contratos compartidos que mantienen una comunicación consistente con el servidor.

## Creación de Controladores

Se crean controladores que reciben las solicitudes de la interfaz, conservan el estado de la sesión activa, validan el rol del usuario y coordinan las operaciones bancarias mediante los servicios. Estos controladores manejan el flujo general de la aplicación, responden tanto a navegaciones completas como a solicitudes parciales y devuelven los resultados para que las vistas actualicen la información mostrada.

## Creación de la clase principal

La clase principal actúa como punto de entrada de la aplicación web, donde se configuran los servicios, la sesión, el enrutamiento y los archivos estáticos antes de iniciar el servidor. Su rol es ensamblar los componentes de la aplicación y dejar disponible la interfaz para los usuarios.
