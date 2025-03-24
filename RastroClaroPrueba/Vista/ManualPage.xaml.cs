using System;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace RastroClaroPrueba.Vista
{
    public partial class ManualPage : ContentPage
    {
        public ManualPage()
        {
            InitializeComponent();
        }

        private async void Pdf_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Ruta del archivo PDF en la carpeta Utils
                var filePath = Path.Combine(FileSystem.AppDataDirectory, "Utils", "manual.pdf");

                // Verificar si el archivo existe
                if (!File.Exists(filePath))
                {
                    await DisplayAlert("Error", "El archivo PDF no se encontró.", "OK");
                    return;
                }

                // Abrir el archivo PDF con la aplicación predeterminada
                await Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir el archivo PDF: {ex.Message}", "OK");
            }
        }

        private async void OnMapaTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new InicioPage();
        }

        private async void OnHistorialTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new HistorialPage();
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