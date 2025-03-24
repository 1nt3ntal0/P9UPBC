# Crear tablas con los siguients campos
- Tabla Empleado ** Metodos para la API: Post Delete **
  - id (int 11, primary , autoincrementable) 
  - Username (varchar 20)
  - Correo (varchar 20)
  - Password(varchar 120 (ENCRIPTADO))

- Tabla Clientes **  Metodos para la API: Post Get Update Delete **
  - id (int 11)
  - Nombre (varchar 100)
  - Password(varchar 120 (ENCRIPTADO))

- Tabla Datos Generales ** Metodos para la API: Post Get Update Delete **
  - Id (int 11)
  - Nombre (varchar 100)
  - FechaDeNacimiento (data time ( MM:DD:YY)
  - Telefono (int 11)
  - Correo (varchar 100)
  - Sangre (int 2)
    - A+ , A- , B+ , B- , O+ , O- , AB+ , AB-
  - Religion (varchar 100)
  - Grado (int 2)
    - Inicial , Intermedio , Avnazado
  - Comunicacion (int 2)
    - Ninguna , Persona muda , Persona sorda , persona sorda con dificultades en el habla , Afasia , Trastorno del lenguaje , Mutismo selectivo
  - Comentarios (varchar 200)

- Coordenadas **  Metodos para la API: Post Get **
  - latitude (float 6)
  - longitud (float 6)
  - id de paciente  (int 11)
