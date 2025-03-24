using System;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
namespace RastroClaroPrueba.Vista;

public partial class HistorialPage : ContentPage
{
	public HistorialPage()
	{
		InitializeComponent();
        CargarHistorial();
    }
    private async void CargarHistorial()
    {
        string fileName = "historial_rutas.html";
        string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        // Copiar el archivo desde los recursos si no existe
        if (!File.Exists(filePath))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            string content = await reader.ReadToEndAsync();
            File.WriteAllText(filePath, content);
        }

        // Imprime la ruta del archivo para depuración
        Console.WriteLine($"Ruta del archivo en Android: {filePath}");

        // Cargar en WebView usando "file://"
        webView.Source = new UrlWebViewSource { Url = $"file://{filePath}" };
    }

    private async void OnMapaTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new InicioPage());
    }
    private async Task ObtenerCoordenadasPorFecha(string fecha)
    {
        try
        {
            var response = await _httpClient.GetAsync($"coordenadas/por-fecha?fecha={fecha}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var coordenadas = JsonSerializer.Deserialize<List<string>>(json);

                foreach (var coord in coordenadas)
                {
                    Console.WriteLine(coord); // Ejemplo: "19.4326077:-99.133208"
                }
            }
            else
            {
                await DisplayAlert("Error", "No se encontraron coordenadas para la fecha especificada.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al obtener las coordenadas: {ex.Message}", "OK");
        }
    }

    private async void OnManualTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new ManualPage());
    }

    private async void OnPacienteTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MedicalPage());
    }
    private async void OnHistorialTapped(object sender, TappedEventArgs e)
    {
        
    }

}