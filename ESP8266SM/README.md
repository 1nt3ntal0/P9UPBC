# Proyecto API - Envío de Coordenadas

Este proyecto proporciona una API para gestionar usuarios, pacientes y sus coordenadas geográficas, diseñada para ser utilizada por una aplicación móvil que envía datos de ubicación de pacientes con Alzheimer a través de un servidor.

## Requisitos

Asegúrate de tener las siguientes librerías instaladas:

```bash
pip install pymysql
pip install flask
pip install flask-sqlalchemy
pip install werkzeug
pip install pyjwt
pip install flask-migrate
pip install flask-cors

Enviar coordenadas a la API en Python
La aplicación en .NET MAUI enviará coordenadas a la API en Python, que las recibirá y procesará según las especificaciones.

Descripción de las funciones del código
StartSendingCoordinates()
Inicia el envío de coordenadas a la API de forma periódica (cada 5 segundos).

EnviarCoordenadas()
Obtiene las coordenadas GPS del dispositivo móvil utilizando la API de geolocalización de .NET MAUI.

Si la ubicación no está disponible, obtiene una nueva ubicación en tiempo real.

Envía las coordenadas como JSON a la API en Python utilizando una solicitud HTTP POST.

Si la solicitud es exitosa (código HTTP 200), se muestra un mensaje de éxito con los datos enviados.

Si ocurre un error (como problemas de conexión o respuesta incorrecta), se muestra un mensaje de error al usuario.
