using RastroClaroPrueba.Models;
using RastroClaroPrueba.Services;
using RastroClaroPrueba.Utils;
using Microsoft.Maui.Controls;

namespace RastroClaroPrueba.Vista;

public partial class InicioPage : ContentPage
{
    private readonly ApiService _apiService;

    public InicioPage()
    {
        InitializeComponent();
        _apiService = new ApiService();

        if (SessionManager.UserId == 0)
        {
            Application.Current.MainPage = new InicioSesionPage();
            return;
        }

        LoadInitialData();
    }

    private async void LoadInitialData()
    {
        try
        {
            var ultimaCoordenada = await _apiService.GetUltimaCoordenadaAsync(SessionManager.UserId);

            if (ultimaCoordenada != null)
            {
                UpdateMap(ultimaCoordenada.Latitude, ultimaCoordenada.Longitude, ultimaCoordenada.Fecha);
            }
            else
            {
                UpdateMap(-12.046374, -77.042793, "No se encontraron coordenadas recientes");
            }
        }
        catch (Exception ex)
        {
            UpdateMap(-12.046374, -77.042793, $"Error: {ex.Message}");
            await DisplayAlert("Error", $"No se pudo cargar la ubicación: {ex.Message}", "OK");
        }
    }

    private void UpdateMap(double latitude, double longitude, string fecha)
    {
        var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Mapa de Rastreo</title>
    <link rel='stylesheet' href='https://unpkg.com/leaflet/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet/dist/leaflet.js'></script>
    <style>
        html, body, #map {{ height: 100%; margin: 0; padding: 0; }}
    </style>
</head>
<body>
    <div id='map' style='height: 100vh; width: 100%;'></div>
    <script>
        var map = L.map('map').setView([{latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}], 15);
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png').addTo(map);
        var marker = L.marker([{latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}]).addTo(map);
        marker.bindPopup('Última ubicación<br>Fecha: {fecha}').openPopup();
    </script>
</body>
</html>";

        webViewMapa.Source = new HtmlWebViewSource { Html = htmlContent };
    }

    private async void OnHistorialTapped(object sender, EventArgs e) => Application.Current.MainPage = new HistorialPage();
    private async void OnManualTapped(object sender, EventArgs e) => Application.Current.MainPage = new ManualPage();
    private async void OnPacienteTapped(object sender, EventArgs e) => Application.Current.MainPage = new MedicalPage();
    private void OnMapaTapped(object sender, EventArgs e) { }
}