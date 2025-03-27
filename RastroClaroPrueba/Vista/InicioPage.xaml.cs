using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;  // Para manejar el JSON
using Microsoft.Maui.Controls;

namespace RastroClaroPrueba.Vista
{
    public partial class InicioPage : ContentPage
    {
        private readonly HttpClient client = new HttpClient();
        private int pacienteId = 1;  // Cambiar según sea necesario o de manera dinámica

        public InicioPage()
        {
            InitializeComponent();

            // Llamar al método que obtiene las coordenadas al cargar la página
            ObtenerCoordenadas();
        }

        private async void ObtenerCoordenadas()
        {
            try
            {
                // Cambia la URL a la de tu API
                string apiUrl = $"http://192.168.1.100:5000/coordenadas/{pacienteId}";  // Cambia por la IP de tu servidor
                string response = await client.GetStringAsync(apiUrl);

                // Parsear el JSON que devuelve la API
                var json = JArray.Parse(response);

                if (json.Count > 0)
                {
                    // Obtener las coordenadas
                    double latitud = (double)json[0]["latitude"];
                    double longitud = (double)json[0]["longitude"];

                    // Mostrar en el mapa usando OpenStreetMap
                    string url = $"https://www.openstreetmap.org/#map=14/{latitud}/{longitud}";
                    webView.Source = url;
                }
                else
                {
                    // Si no se encontraron coordenadas, establecer ubicación predeterminada
                    string url = $"https://www.openstreetmap.org/#map=14/32.58304614574744/-115.36246831218202"; // Mexicali por defecto
                    webView.Source = url;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener coordenadas: {ex.Message}");
            }
        }

        private async void OnHistorialTapped(object sender, TappedEventArgs e)
        {
            // Navegar a la página HistorialPage
            await Navigation.PushModalAsync(new HistorialPage());
        }

        private async void OnManualTapped(object sender, TappedEventArgs e)
        {
            // Navegar a la página ManualPage
            await Navigation.PushModalAsync(new ManualPage());
        }

        private async void OnPacienteTapped(object sender, TappedEventArgs e)
        {
            // Navegar a la página MedicalPage
            await Navigation.PushModalAsync(new MedicalPage());
        }

        private async void OnMapaTapped(object sender, TappedEventArgs e)
        {
            // Llamar a la función para obtener las coordenadas al hacer clic en el mapa
            ObtenerCoordenadas();
        }
    }
}
