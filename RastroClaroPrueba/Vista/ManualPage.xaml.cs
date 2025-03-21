//using RastroClaroPrueba.Utils; /// sin validad la lectura del archivo

namespace RastroClaroPrueba.Vista;

public partial class ManualPage : ContentPage
{
	public ManualPage()
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

    private async void OnManualTapped(object sender, TappedEventArgs e)
    {
       
    }

    private async void OnPacienteTapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushModalAsync(new MedicalPage());
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private void Pdf_Clicked(object sender, EventArgs e)
    {

    }
}