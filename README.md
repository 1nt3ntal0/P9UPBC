# P9UPBC
Carpetas contenidas
- Principal (aqui es donde estara la aplicacion MAUI)
  - Indicar archivos de la carpeta Models
  - Indicar archivos de la carpeta Views
- - - - - -
- 8266 (UNICAMENTE LO DEL CONTROLADOR)
- - - - - - 
- API ( UNICAMENTE LA API )
- - - - - - 
- SITIO WEB ( UNICAMENTE LO DEL SITIO WEB se requiere una tabla unicamente para logeo )
  - CSS
  - JS
  - HTML
- - - - - - - 
- RASBPERRY (UNICAMENTE LO DEL RASPBERRY)
  - Base de datos
  - Configuracion para acceso remoto
 
# Descripcion tecnica del proyecto:
Para el proyecto RastroClaro se utilizara :
- Una Raspberry Pi4 la cual estara funcionando como servidor. 
- El Raspberry contara con una API , Sistema Web y Base de datos:
  - Por comodidad se utilizo una API creada en Pyhon con flask ✔️
    - Debe gestionar multiples solicitudes tanto desde el sitio web como desde la aplicacion de Maui ✔️
    - Unicamente contar con los endpoints que se requieran realmente. ( POR REVISAR)
  - Sistema Web el sistema web debe comunicarse con la API para hacer solicitudes a la base de datos ✔️
    - Home ( Mostrara Vision , Mision de la empresa) ** Acceso no restringido (Falta de hacer)
    - Nosotros ( Mostrara Informacion general del proyecto) ** Acceso no restringido ** (Falta de hacer)
    - Login ( Permitira inicar sesion apoyandose en la API) ** Acceso no restirngido ✔️
    - Cliente ( Permitira registrar clientes Apoyandose con la API) ** Acceso Restringido Unicamente personal CON SESION ACTIVA ✔️
    - Paciente ( Permite registrar pacientes y asociarlos a clientes apoyandose con la API) ** Acceso Restringido unicamente personal CON SESION ACTIVA (Falta de hacer)
    - Usuario (Informacio del cliente y permite cambiar contrasena apoyandose en la API) ** Acceso Restringido unicamente al cliente correspondiete (Falta de hacer)
    - Datos Generales ( Permite visualizar y modificar datos del paciente) ** Acceso Restirngido unicamente al paciente ligado con el cliente (Falta de hacer)
      
  - La base de datos constara de multiples tablas (usuarios , coordenadas, cliente , paciente)
    - Debe contar con la tabla Empleados ✔️
    - Debe contar con la tabla Clientes ✔️
    - Debe contar con la tabla Pacientes (con Fk a Clientes) ✔️
    - Debe contar con la tabla Datos Generales ( con Fk al Paciente) ✔️
    - Debe contar con la tabla Coordenadas (con Fk Pacientes) ✔️
      
  - Ademas utilizaremos NGNX para poder tener acceso desde dispositivos fuera de red hacia el sistema y la API.
    - Configurar para que tengan acceso desde el exterior a el sitio o API segun se requeira (Falta de hacer)
      
- Aplicacion Movil desarrollada en MAUI
  - La aplicacion permitira a los usuarios Iniciar sesion (YA HAY COMINICACION A LA API (ㆆ_ㆆ))
    - Mediante la API debe validar las credenciales ✔️
  - Darle seguimiento a los pacientes mediante coordenadas y el uso de OpenStreetMap 
    - Mediante la API debe leer las coordenadas para mostrar la ruta y la ubicacion actual del paciente (Falta de hacer)
  - Consumira la API para validar informacion (login , info de cliente , info de paciente, etc)
    - Obtener los datos del pacinte o cliente segun requiera unicamrnte los permitidos (Falta de hacer)
  - Contara con manual de usuario. ✔️
    - Manual en Pdf disponible para guia rapida para los Usuarios. (Falta el PDF) 

- Placa 8266
  - Contar con soporte para el modulo GPS.
  - Contar con soporte para modulo para SimCard.
  - Enviar los datos a la API. ✔️
  
## Raspberry



## COMENTAR TODOS LOS CAMBIOS CADA CARPETA CON SU RESPECTIVO README POR FAVOR PARA EVITAR DESORDEN SE LES QUIERE 
### Aqui pondremos todo mas general para que pueda entenderse como funciona el proyecto
