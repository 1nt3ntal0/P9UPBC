using RastroClaroPrueba.Utils;
using System;
using Microsoft.Maui.Controls;
using RastroClaroPrueba.Models;

namespace RastroClaroPrueba.Vista
{
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

            ApiService apiService = new ApiService();
            bool loginExitoso = await apiService.LoginAsync(user, pass);

            if (loginExitoso && !string.IsNullOrEmpty(SessionManager.Token) && SessionManager.UserId > 0)
            {
                Preferences.Set("AuthToken", SessionManager.Token);
                await DisplayAlert("Éxito", "Inicio de sesión correcto", "OK");
                Application.Current.MainPage = new InicioPage();
            }
            else
            {
                await DisplayAlert("Error", "Usuario o contraseña incorrectos", "Aceptar");
            }
        }

        private async void BtnProducto_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new InicioPage();
        }
    }
}
