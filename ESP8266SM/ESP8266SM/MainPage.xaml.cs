using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors; // API de geolocalización

namespace ESP8266SM
{
    public partial class MainPage : ContentPage
    {
        private HttpClient client = new HttpClient();
        private string apiUrl = "http://127.0.0.1:5000/coordenadas"; // API en Python

        public MainPage()
        {
            InitializeComponent();
            StartSendingCoordinates(); // Llama a la función para iniciar el envío de coordenadas
        }

        private void StartSendingCoordinates()
        {
            // Intervalo de 5 segundos
            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                // Llama a la función asincrónica sin esperar
                EnviarCoordenadas().ConfigureAwait(false);
                return true; // Retorna 'true' para seguir ejecutando cada 5 segundos
            });
        }

        private async Task EnviarCoordenadas()
        {
            try
            {
                // Intentar obtener la última ubicación conocida
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location == null)
                {
                    // Si no hay una ubicación reciente, obtener una nueva en tiempo real
                    location = await Geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Best, // Mejor precisión
                        Timeout = TimeSpan.FromSeconds(10)
                    });
                }

                if (location == null)
                {
                    Console.WriteLine("No se pudo obtener la ubicación.");
                    return;
                }

                // Crear JSON con las coordenadas obtenidas del celular
                var coordenadas = new
                {
                    latitude = location.Latitude,
                    longitude = location.Longitude,
                    paciente_id = 1233
                };

                var json = JsonSerializer.Serialize(coordenadas);
                Console.WriteLine("JSON a enviar: " + json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Datos enviados correctamente: " + result);
                }
                else
                {
                    Console.WriteLine($"Error al enviar datos: Código {response.StatusCode}\nRespuesta: {result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar coordenadas: " + ex.Message);
            }
        }
    }
}
