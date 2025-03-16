namespace RastroClaroPrueba.Vista;

public partial class ManualPage : ContentPage
{
	public ManualPage()
	{
		InitializeComponent();
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
       
    }

    private async void OnPacienteTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new MedicalPage());
    }
}