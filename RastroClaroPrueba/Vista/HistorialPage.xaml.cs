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