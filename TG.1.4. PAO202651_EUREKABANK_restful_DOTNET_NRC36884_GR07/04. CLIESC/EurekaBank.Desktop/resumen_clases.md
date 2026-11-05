# Resumen de clases del proyecto

## Creación de Paquetes

El proyecto se organiza bajo el patrón MVC en paquetes de controladores, modelos, servicios y vistas, acompañados por recursos gráficos, configuración y contratos compartidos. Esta estructura separa la interfaz de escritorio, la coordinación de operaciones, el acceso a la API REST y las estructuras de datos utilizadas por la aplicación.

## Creación de Modelos

Se crean modelos que representan los contratos de datos del dominio bancario, como sesiones, cuentas, clientes, perfiles, movimientos y solicitudes de operación. Estas clases encapsulan la estructura de la información, los resultados uniformes de las operaciones y las reglas de validación necesarias para transportar los datos entre la vista y los servicios.

## Creación de Vistas

Se crean vistas de escritorio que conforman una interfaz gráfica con pantalla de inicio de sesión, panel principal, secciones de resumen, movimientos, transferencias, operaciones, administración de cuentas y perfil. Estas vistas definen estilos, componentes reutilizables y navegación, capturan los eventos del usuario y delegan la ejecución de las operaciones a los controladores.

## Creación de Servicios

Se crean servicios que encapsulan el consumo de la API REST mediante operaciones HTTP, serialización de datos, autenticación por token y manejo de tiempos de espera. Los recursos y paquetes adicionales aportan la configuración del endpoint, los elementos visuales de la aplicación y los contratos compartidos que mantienen una comunicación consistente con el servidor.

## Creación de Controladores

Se crean controladores que reciben las acciones de la interfaz, conservan el estado de la sesión activa e invocan los servicios correspondientes. Estos controladores coordinan el flujo general de la aplicación: interpretan la solicitud, ejecutan la operación bancaria y devuelven el resultado para que la vista actualice la información mostrada al usuario.

## Creación de la clase principal

La clase principal actúa como punto de entrada de la aplicación de escritorio, donde se configura el arranque y se carga la ventana inicial de la interfaz. Su rol es ensamblar los componentes de la aplicación y presentar la vista de inicio de sesión al usuario.
