namespace RastroClaroPrueba.Vista;

public partial class MedicalPage : ContentPage
{
	public MedicalPage()
	{
		InitializeComponent();
	}

    private async void OnMapaTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new InicioPage());
    }

    private async void OnHistorialTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new HistorialPage());
    }

    private async void OnPacienteTapped(object sender, TappedEventArgs e)
    {
       
    }

    private async void OnManualTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new ManualPage());
    }

    private void BtnUserSave_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnPacientSave_Clicked(object sender, EventArgs e)
    {
       
    }
}