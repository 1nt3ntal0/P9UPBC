El objetivo es simular el envío de coordenadas geográficas (latitud y longitud) desde el dispositivo móvil a una API Python a través de una conexión HTTP, emulando el comportamiento del ESP8266 al enviar datos.

Componentes Clave de la Simulación:
Obtención de Coordenadas Geográficas

Se utiliza la API de Geolocalización de .NET MAUI para obtener las coordenadas del dispositivo móvil (latitud y longitud) en tiempo real.

Si la ubicación está disponible en el dispositivo, se obtiene la última ubicación conocida usando Geolocation.GetLastKnownLocationAsync().

Si no está disponible, la aplicación solicita una ubicación actualizada utilizando Geolocation.GetLocationAsync(), lo que permite obtener la ubicación con la precisión configurada.

Formato de Datos

Una vez obtenidas las coordenadas, estos datos se empaquetan en un objeto JSON para enviarlos de manera estructurada a la API. El objeto JSON contiene:

latitude: la latitud del dispositivo.

longitude: la longitud del dispositivo.

paciente_id: identificador de la persona asociada con la coordenada (puede ser un identificador de base de datos o un número de paciente).

Envío de Datos a la API

Los datos serializados en JSON se envían a la API utilizando un POST HTTP.

Para ello, se utiliza la clase HttpClient de .NET, que permite realizar solicitudes HTTP. La URL de la API está configurada en el proyecto.

En este caso, la API se simula como una API en Python (Flask, por ejemplo) que recibe las coordenadas y realiza algún procesamiento con ellas (almacenarlas, enviarlas a una base de datos, etc.).

Manejo de Respuestas

Una vez que se realiza la solicitud, se maneja la respuesta de la API:

Si la solicitud es exitosa (código HTTP 200), se muestra un mensaje de éxito con los datos enviados.

Si ocurre un error (como problemas de conexión o respuesta incorrecta), se muestra un mensaje de error al usuario.