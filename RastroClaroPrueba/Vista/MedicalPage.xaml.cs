namespace RastroClaroPrueba.Vista;

public partial class MedicalPage : ContentPage
{
	public MedicalPage()
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

    private async void OnPacienteTapped(object sender, TappedEventArgs e)
    {
       
    }

    private async void OnManualTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new ManualPage());
    }
}