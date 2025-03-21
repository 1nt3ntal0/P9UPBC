namespace RastroClaroPrueba.Vista;

public partial class InicioPage : ContentPage
{
	public InicioPage()
	{
		InitializeComponent();
        
            // Coordenadas de Mexicali, Baja California, México
            double latitud = 32.58304614574744;
            double longitud = -115.36246831218202;
            int zoom = 140000; // Nivel de zoom

            // URL de OpenStreetMap centrada en Mexicali
            string url = $"https://www.openstreetmap.org/#map={zoom}/{latitud}/{longitud}";

            // Cargar la URL en el WebView
            webView.Source = url;
    }
    private async void OnHistorialTapped(object sender, TappedEventArgs e)
    {
        // Navegar a la página HistorialPage
        await Navigation.PushModalAsync(new HistorialPage());
    }

    private async void OnManualTapped(object sender, TappedEventArgs e)
    {
        // Navegar a la página HistorialPage
        await Navigation.PushModalAsync(new ManualPage());
    }

    private async void OnPacienteTapped(object sender, TappedEventArgs e)
    {
        // Navegar a la página HistorialPage
        await Navigation.PushModalAsync(new MedicalPage());
    }
    private async void OnMapaTapped(object sender, TappedEventArgs e)
    {
        
    }
}