using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using RastroClaroPrueba.Utils;
using RastroClaroPrueba.Models;

namespace RastroClaroPrueba.Vista
{
    public partial class HistorialPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public HistorialPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("http://127.0.0.1:5000") };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            CargarHistorial();
        }

        private async void CargarHistorial()
        {
            try
            {
                // Obtener el ID del paciente desde SessionManager
                int pacienteId = SessionManager.UserId;

                // Obtener la fecha seleccionada (puedes usar un DatePicker o campos de entrada)
                string fecha = $"{diaEntry.Text}-{mesEntry.Text}-{anoEntry.Text}";

                // Obtener las coordenadas por fecha
                var coordenadas = await _apiService.GetCoordenadasPorFechaAsync(pacienteId, fecha);

                if (coordenadas != null && coordenadas.Any())
                {
                    // Generar el contenido HTML para mostrar las coordenadas en el WebView
                    var htmlContent = GenerarHtmlCoordenadas(coordenadas);
                    webView.Source = new HtmlWebViewSource { Html = htmlContent };
                }
                else
                {
                    await DisplayAlert("Información", "No se encontraron coordenadas para la fecha seleccionada.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar el historial: {ex.Message}", "OK");
            }
        }
        private async void OnBuscarClicked(object sender, EventArgs e)
        {
            // Validar que los campos de fecha no estén vacíos
            if (string.IsNullOrEmpty(diaEntry.Text) || string.IsNullOrEmpty(mesEntry.Text) || string.IsNullOrEmpty(anoEntry.Text))
            {
                await DisplayAlert("Error", "Por favor, ingresa el día, mes y año.", "OK");
                return;
            }

            // Cargar el historial con la fecha seleccionada
            CargarHistorial();
        }

        private string GenerarHtmlCoordenadas(List<Coordenada> coordenadas)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head><title>Historial de Coordenadas</title></head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Historial de Coordenadas</h1>");
            sb.AppendLine("<ul>");

            foreach (var coord in coordenadas)
            {
                sb.AppendLine($"<li>Latitud: {coord.Latitude}, Longitud: {coord.Longitude}</li>");
            }

            sb.AppendLine("</ul>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
        private async void OnHistorialTapped(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Historial", "Estás en la página del historial.", "OK");
        }

        private async void OnMapaTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new InicioPage();
        }

        private async void OnManualTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new ManualPage();
        }

        private async void OnPacienteTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new MedicalPage();
        }
    }
}
