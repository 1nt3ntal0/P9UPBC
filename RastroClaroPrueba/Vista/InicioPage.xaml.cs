using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using RastroClaroPrueba.Models;
using RastroClaroPrueba.Utils;

namespace RastroClaroPrueba.Vista
{
    public partial class InicioPage : ContentPage
    {
        private readonly ApiService _apiService;
        private const int PacienteId = 1;
        private bool _isRefreshing;

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
                        html, body, #map {{ height: 100vh; margin: 0; padding: 0; }}
                    </style>
                </head>
                <body>
                    <div id='map'></div>
                    <script>
                        var map = L.map('map').setView([{latitude}, {longitude}], 15);
                        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                            maxZoom: 19,
                        }}).addTo(map);
                        L.marker([{latitude}, {longitude}]).addTo(map);
                    </script>
                </body>
                </html>";

            webViewMapa.Source = new HtmlWebViewSource { Html = htmlContent };
        }

        private async Task LoadLastLocation()
        {
            try
            {
                // Obtener el ID del paciente desde SessionManager
                int pacienteId = SessionManager.UserId;

                // Obtener la última coordenada del paciente
                var ultimaCoordenada = await _apiService.GetCoordenadasAsync(pacienteId);

                if (ultimaCoordenada != null)
                {
                    // Actualizar el mapa con la última coordenada
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
                if (_isRefreshing) return true;

                _isRefreshing = true;
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        await LoadLastLocation();
                    }
                    finally
                    {
                        _isRefreshing = false;
                    }
                });

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

        private async void OnPacienteTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushModalAsync(new MedicalPage());
        }

        private void OnMapaTapped(object sender, TappedEventArgs e)
        {
        }
    }
}
