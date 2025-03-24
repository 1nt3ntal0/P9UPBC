using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using RastroClaroPrueba.Models; 

namespace RastroClaroPrueba.Vista
{
    public partial class InicioPage : ContentPage
    {
        private ApiService _apiService;
        private int _pacienteId = 1;
        private bool _isRefreshing = false; 

        public InicioPage()
        {
            InitializeComponent();
            _apiService = new ApiService();
            LoadLastLocation(); 
            StartLocationRefresh(); 
        }
        private void UpdateMap(double latitude, double longitude)
        {
            var htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Mapa</title>
                    <link rel='stylesheet' href='https://unpkg.com/leaflet/dist/leaflet.css' />
                    <script src='https://unpkg.com/leaflet/dist/leaflet.js'></script>
                    <style>
                        #map {{ height: 100%; }}
                    </style>
                </head>
                <body>
                    <div id='map'></div>
                    <script>
                        var map = L.map('map').setView([{latitude}, {longitude}], 15);
                        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                            maxZoom: 19,
                        }}).addTo(map);
                        var marker = L.marker([{latitude}, {longitude}]).addTo(map);
                    </script>
                </body>
                </html>";

            webView.Source = new HtmlWebViewSource { Html = htmlContent };
        }

        private async Task LoadLastLocation()
        {
            try
            {
                var ultimaCoordenada = await _apiService.GetCoordenadasAsync(_pacienteId);
                if (ultimaCoordenada != null)
                {
                    UpdateMap(ultimaCoordenada.Latitude, ultimaCoordenada.Longitude);
                }
                else
                {
                    await DisplayAlert("Error", "No se encontraron coordenadas para el paciente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar las coordenadas: {ex.Message}", "OK");
            }
        }

        private void StartLocationRefresh()
        {
            Device.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                if (!_isRefreshing)
                {
                    _isRefreshing = true;
                    Task.Run(async () => await LoadLastLocation()).Wait(); 
                    _isRefreshing = false;
                }
                return true;
            });
        }

        private async void OnHistorialTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushModalAsync(new HistorialPage());
        }
        private async void OnManualTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushModalAsync(new ManualPage());
        }

        // Evento al tocar el ícono de Paciente
        private async void OnPacienteTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushModalAsync(new MedicalPage());
        }

        // Evento al tocar el ícono de Mapa
        private async void OnMapaTapped(object sender, TappedEventArgs e)
        {
            // Puedes agregar lógica adicional aquí si es necesario
        }
    }
}