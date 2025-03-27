using Microsoft.Maui.Controls;
using RastroClaroPrueba.Models;
using RastroClaroPrueba.Services;
using RastroClaroPrueba.Utils;
using System;
using System.Threading.Tasks;

namespace RastroClaroPrueba.Vista
{
    public partial class MedicalPage : ContentPage
    {
        private readonly ApiService _apiService;
        private usuarios _usuario;
        private pacientes _paciente;

        public MedicalPage()
        {
            InitializeComponent();
            _apiService = new ApiService();

            // Inicializar modelos
            _usuario = new usuarios();
            _paciente = new pacientes();

            // Cargar datos al aparecer
            this.Appearing += async (sender, e) => await CargarDatos();
        }

        private async Task CargarDatos()
        {
            try
            {
                // Cargar datos del usuario y paciente
                _usuario = await _apiService.GetUserAsync(SessionManager.UserId) ?? new usuarios { Id = SessionManager.UserId };
                _paciente = await _apiService.GetPatientAsync(SessionManager.UserId) ?? new pacientes { FkID = SessionManager.UserId };

                // Actualizar bindings
                nombreUsuarioEntry.Text = _usuario.Username;
                contrasenaEntry.Text = _usuario.Password;
                nombrePacienteEntry.Text = _paciente.name;
                telefonoEntry.Text = _paciente.phone;
                tipoSangrePicker.SelectedIndex = _paciente.blood_type;
                religionEntry.Text = _paciente.religion;
                gradoPicker.SelectedIndex = _paciente.grade;
                dificultadPicker.SelectedIndex = _paciente.extra;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al cargar datos: {ex.Message}", "OK");
            }
        }

        private async void BtnUserSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Actualizar modelo con datos de UI
                _usuario.Username = nombreUsuarioEntry.Text;
                _usuario.Password = contrasenaEntry.Text;

                var resultado = await _apiService.UpdateUserAsync(_usuario);

                await DisplayAlert(resultado.success ? "Éxito" : "Error",
                                resultado.message,
                                "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al guardar usuario: {ex.Message}", "OK");
            }
        }

        private async void BtnPacientSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Actualizar modelo con datos de UI
                _paciente.FkID = SessionManager.UserId;
                _paciente.name = nombrePacienteEntry.Text;
                _paciente.phone = telefonoEntry.Text;
                _paciente.blood_type = tipoSangrePicker.SelectedIndex;
                _paciente.religion = religionEntry.Text;
                _paciente.grade = gradoPicker.SelectedIndex;
                _paciente.extra = dificultadPicker.SelectedIndex;

                var resultado = await _apiService.UpdatePatientAsync(_paciente);

                await DisplayAlert(resultado.success ? "Éxito" : "Error",
                                resultado.message,
                                "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al guardar paciente: {ex.Message}", "OK");
            }
        }

        private void OnMapaTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new InicioPage();
        }

        private void OnHistorialTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new HistorialPage();
        }

        private void OnManualTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new ManualPage();
        }

        private void OnPacienteTapped(object sender, TappedEventArgs e)
        {
            Application.Current.MainPage = new MedicalPage();
        }

        private void btncerrar_Clicked(object sender, EventArgs e)
        {
            SessionManager.ClearSession();
            Application.Current.MainPage = new InicioSesionPage();
        }
    }
}