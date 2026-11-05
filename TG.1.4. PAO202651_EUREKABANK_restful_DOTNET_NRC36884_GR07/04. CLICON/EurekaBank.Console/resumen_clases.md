# Resumen de clases del proyecto

## Creación de Paquetes

El proyecto se organiza bajo el patrón MVC en paquetes diferenciados de controladores, modelos, servicios y vistas, junto con carpetas de configuración y contratos compartidos. Esta distribución separa la presentación en consola, la coordinación de acciones, el acceso a la API REST y las estructuras de datos que se intercambian con el servidor.

## Creación de Modelos

Se crean modelos que representan los contratos de datos del dominio bancario, como sesiones, cuentas, clientes, perfiles, movimientos y solicitudes de operación. Estas clases encapsulan la estructura de la información, los resultados uniformes de las operaciones y las validaciones básicas necesarias para transportar datos entre la vista y los servicios.

## Creación de Vistas

Se crean vistas de consola que conforman una interfaz interactiva con pantalla de inicio de sesión, menú navegable mediante teclado, tablas de datos y mensajes de estado. Estas vistas capturan las acciones del usuario, presentan la información de forma legible y delegan la ejecución de las operaciones a los controladores sin gestionar directamente la comunicación HTTP.

## Creación de Servicios

Se crean servicios que encapsulan el consumo de la API REST mediante operaciones HTTP, serialización de datos, autenticación por token y manejo de tiempos de espera. Los paquetes adicionales aportan la configuración del endpoint, las propiedades de ejecución y los contratos compartidos que permiten mantener una comunicación consistente con el servidor.

## Creación de Controladores

Se crean controladores que reciben las acciones de la vista, conservan el estado de la sesión activa e invocan los servicios correspondientes. Estos controladores coordinan el flujo general de la aplicación: interpretan la solicitud, ejecutan la operación bancaria y devuelven el resultado para que la vista lo presente al usuario.

## Creación de la clase principal

La clase principal actúa como punto de entrada de la aplicación de consola, donde se configura la interfaz, se inicializa el controlador y se inicia el ciclo de autenticación y navegación. Su rol es ensamblar los componentes necesarios y mantener la aplicación en ejecución hasta que el usuario finalice la sesión.
