# Resumen de clases del proyecto

## Creación de Paquetes

El proyecto se organiza bajo el patrón MVC en paquetes de controladores, modelos, servicios y vistas, junto con recursos, configuración y código específico de plataforma. Esta estructura separa la interfaz móvil, la coordinación de operaciones, el acceso a la API REST y las estructuras de datos que se intercambian con el servidor.

## Creación de Modelos

Se crean modelos que representan los contratos de datos del dominio bancario, como sesiones, cuentas, clientes, perfiles, movimientos y solicitudes de operación. Estas clases encapsulan la estructura de la información, los resultados uniformes de las operaciones y las validaciones necesarias para transportar los datos entre la vista y los servicios.

## Creación de Vistas

Se crean vistas móviles que conforman una interfaz con pantalla de inicio de sesión, panel de resumen, secciones de movimientos, transferencias, operaciones, administración de cuentas, perfil y configuración del servidor. Estas vistas utilizan componentes reutilizables, capturan los eventos del usuario y delegan la ejecución de las operaciones a los controladores.

## Creación de Servicios

Se crean servicios que encapsulan el consumo de la API REST mediante operaciones HTTP, serialización de datos, autenticación por token y manejo de tiempos de espera. Los paquetes adicionales aportan la configuración y persistencia local del endpoint, los recursos visuales y el código de inicialización propio de la plataforma móvil.

## Creación de Controladores

Se crean controladores que reciben las acciones de la interfaz, conservan el estado de la sesión activa, validan el rol del usuario e invocan los servicios correspondientes. Estos controladores coordinan el flujo general de la aplicación: interpretan la solicitud, ejecutan la operación bancaria, permiten ajustar la conexión y devuelven el resultado para que la vista actualice la información.

## Creación de la clase principal

La clase principal actúa como punto de entrada de la aplicación móvil, donde se construye el host de la interfaz, se configura la navegación y se carga la página inicial. Su rol es ensamblar los componentes de la aplicación y presentar la vista de inicio de sesión al usuario.
