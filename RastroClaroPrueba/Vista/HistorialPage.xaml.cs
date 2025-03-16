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
        var filePath = Path.Combine(FileSystem.AppDataDirectory, "historial_rutas.html");

        if (!File.Exists(filePath))
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("historial_rutas.html");
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            File.WriteAllText(filePath, content);
        }

        webView.Source = new UrlWebViewSource { Url = $"file://{filePath}" };
    }

    private async void OnMapaTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new InicioPage());
    }

    private async void OnManualTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ManualPage());
    }

    private async void OnPacienteTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MedicalPage());
    }
    private async void OnHistorialTapped(object sender, TappedEventArgs e)
    {
        
    }

}