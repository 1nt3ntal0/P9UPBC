using System.Text.Json;
using RastroClaroPrueba.Models;

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
        PassEntry.IsPassword = !isPasswordVisible;

        TogglePasswordButton.ImageSource = isPasswordVisible ? "ojo_cerrado.png" : "ojo.png";
    }

    private async void OnIniciarSesionClicked(object sender, EventArgs e)
    {
        string user = UserEntry.Text;
        string pass = PassEntry.Text;

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            await DisplayAlert("Alerta", "Debes ingresar un usuario y contraseña", "Aceptar");
            return;
        }

        ApiService apiService = new ApiService(); // Instancia del servicio
        string jsonResponse = await apiService.LoginAsync(user, pass);

        if (!string.IsNullOrEmpty(jsonResponse))
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("access_token", out JsonElement tokenElement))
                {
                    string token = tokenElement.GetString();
                    Preferences.Set("AuthToken", token); // Guardamos el token para futuras solicitudes

                    await Navigation.PushAsync(new InicioPage()); // Navegar a la pantalla principal
                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al procesar la respuesta: {ex.Message}", "Aceptar");
            }
        }

        await DisplayAlert("Alerta", "Usuario o contraseña incorrectos", "Aceptar");
    }


    private async void BtnProducto_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Alerta", "No tienes permisos para acceder a esta funcionalidad", "Aceptar");
    }
}