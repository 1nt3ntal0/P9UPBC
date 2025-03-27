using RastroClaroPrueba.Models;
using RastroClaroPrueba.Utils; 

namespace RastroClaroPrueba.Vista;

public partial class ManualPage : ContentPage
{
	public ManualPage()
	{
		InitializeComponent();
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
       
    }

    private async void OnPacienteTapped(object sender, TappedEventArgs e)
    {
      
        Application.Current.MainPage = new MedicalPage();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private async void Pdf_Clicked(object sender, EventArgs e)
    {
        try
        {
            // 1. Verificar permisos de almacenamiento (Android)
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permiso requerido", "Se necesita acceso a almacenamiento para descargar el manual", "OK");
                    return;
                }
            }

            string fileId = "1rk8oVwnDeVcUkTGo-sOc74NLaKDn0G6m";
            string downloadUrl = $"https://drive.google.com/uc?export=download&id={fileId}";

            using (var httpClient = new HttpClient())
            {
                var fileBytes = await httpClient.GetByteArrayAsync(downloadUrl);

                string localPath = Path.Combine(FileSystem.CacheDirectory, "Manual.pdf");
                await File.WriteAllBytesAsync(localPath, fileBytes);

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(localPath),
                    Title = "Manual de Usuario"
                });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo descargar el manual: {ex.Message}", "OK");
        }
    }
}