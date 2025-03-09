namespace RastroClaroPrueba.Vista;

public partial class InicioSesionPage : ContentPage
{
    private bool isPasswordVisible = false;

    public InicioSesionPage()
	{
		InitializeComponent();
	}
    private void OnTogglePasswordClicked(object sender, EventArgs e)
    {
        isPasswordVisible = !isPasswordVisible;
        ContrasenaEntry.IsPassword = !isPasswordVisible;

        TogglePasswordButton.ImageSource = isPasswordVisible ? "ojo_cerrado.png" : "ojo.png";
    }
    private async void OnIniciarSesionClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPage());

    }

}