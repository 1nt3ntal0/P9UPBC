using RastroClaroPrueba.Models;
using System.Text;
using RastroClaroPrueba.Utils;

namespace RastroClaroPrueba.Vista
{
    public partial class HistorialPage : ContentPage
    {
        public HistorialPage()
        {
            InitializeComponent();
        }

        private async void OnBuscarClicked(object sender, EventArgs e)
        {
            string dia = diaEntry.Text;
            string mes = mesEntry.Text;
            string ano = anoEntry.Text;

            if (string.IsNullOrEmpty(dia) || string.IsNullOrEmpty(mes) || string.IsNullOrEmpty(ano))
            {
                await DisplayAlert("Error", "Por favor, ingresa el día, mes y año.", "OK");
                return;
            }

            string fecha = $"{dia}-{mes}-{ano}";
            await CargarHistorial(fecha);
        }

        private async Task CargarHistorial(string fecha)
        {
            try
            {
                int pacienteId = SessionManager.UserId;

                var apiService = new ApiService();

                var coordenadas = await apiService.GetCoordenadasPorFechaAsync(pacienteId, fecha);

                if (coordenadas != null && coordenadas.Count > 0)
                {
                    var htmlContent = GenerarHtmlCoordenadas(coordenadas);
                    webView.Source = new HtmlWebViewSource { Html = htmlContent };
                }
                else
                {
                    await DisplayAlert("Información", "No se encontraron coordenadas para la fecha especificada.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar el historial: {ex.Message}", "OK");
            }
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

        private async void OnMapaTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new InicioPage());
        }

        private async void OnHistorialTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new HistorialPage());
        }

        private async void OnManualTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new ManualPage());
        }

        private async void OnPacienteTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new MedicalPage());
        }
    }
}