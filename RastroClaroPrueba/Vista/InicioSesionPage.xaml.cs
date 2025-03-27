using RastroClaroPrueba.Services;
using RastroClaroPrueba.Utils;

namespace RastroClaroPrueba.Vista
{
    public partial class InicioSesionPage : ContentPage
    {
        private bool _isPasswordVisible = false;
        private readonly ApiService _apiService = new ApiService();

        public InicioSesionPage()
        {
            InitializeComponent();
            LoadUser();
        }

        private async void LoadUser()
        {
            var filepath = Path.Combine(FileSystem.AppDataDirectory, "Contrasena.txt");
            if (File.Exists(filepath))
            {
                var login = await File.ReadAllTextAsync(filepath);
                var parts = login.Split(',');
                UserEntry.Text = parts[0];
                PassEntry.Text = parts[1];
            }
        }

        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            PassEntry.IsPassword = !_isPasswordVisible;
            TogglePasswordButton.ImageSource = _isPasswordVisible ? "ojo_cerrado.png" : "ojo.png";
        }

        private async void OnIniciarSesionClicked(object sender, EventArgs e)
        {
            var usuario = UserEntry.Text;
            var password = PassEntry.Text;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Usuario y contraseña son requeridos", "OK");
                return;
            }

            var (success, _) = await _apiService.LoginAsync(usuario, password);

            if (success)
            {
                if(Checkrecuerdame.IsChecked)
                {
                    var filepath = Path.Combine(FileSystem.AppDataDirectory, "Contrasena.txt");
                    var login = $"{usuario},{password}";
                    await File.WriteAllTextAsync(filepath, login);

                }
                Application.Current.MainPage = new InicioPage();
            }
            else
            {
                await DisplayAlert("Error", "Credenciales incorrectas", "OK");
            }
        }

        private async void BtnProducto_Clicked(object sender, EventArgs e)
        {
            try
            {
                string url = "https://www.tupaginaweb.com"; 
                await Launcher.Default.OpenAsync(new Uri(url));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo abrir la página: {ex.Message}", "OK");
            }
        }
    }
}