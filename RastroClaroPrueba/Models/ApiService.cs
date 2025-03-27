using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RastroClaroPrueba.Utils;
using RastroClaroPrueba.Models;
using System.Diagnostics;
using Microsoft.Maui.Graphics;

namespace RastroClaroPrueba.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = "http://127.0.0.1:5000";

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseApiUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<(bool success, string message)> LoginAsync(string username, string password)
        {
            try
            {
                var loginData = new { username, password };
                var content = new StringContent(
                    JsonSerializer.Serialize(loginData),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("login", content);

                var jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                    {
                        var root = doc.RootElement;

                        if (root.TryGetProperty("id", out var idElement) &&
                            root.TryGetProperty("access_token", out var tokenElement))
                        {
                            SessionManager.UserId = idElement.GetInt32();
                            SessionManager.Token = tokenElement.GetString();
                            SetAuthToken(SessionManager.Token);
                            return (true, "Login exitoso");
                        }
                    }
                    return (false, "Respuesta inválida del servidor");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    return (false, "Credenciales incorrectas");
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    return (false, "Usuario y contraseña son requeridos");

                return (false, $"Error: {jsonResponse}");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<usuarios> GetUserAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"users/{userId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<usuarios>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener usuario: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool success, string message)> UpdateUserAsync(usuarios user)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(user),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync("update_user", content);

                if (response.IsSuccessStatusCode)
                    return (true, "Usuario actualizado correctamente");

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, $"Error al actualizar usuario: {errorMessage}");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<pacientes> GetPatientAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"patients/{userId}");

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<pacientes>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener paciente: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool success, string message)> UpdatePatientAsync(pacientes patient)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(patient),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PutAsync("update_patient", content);

                if (response.IsSuccessStatusCode)
                    return (true, "Paciente actualizado correctamente");

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, $"Error al actualizar paciente: {errorMessage}");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        public async Task<coordenadas> GetUltimaCoordenadaAsync(int pacienteId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"latest_coordinates/{pacienteId}");

                if (!response.IsSuccessStatusCode)
                    return CreateDefaultCoordenada(pacienteId);

                var json = await response.Content.ReadAsStringAsync();

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    return new coordenadas
                    {
                        Latitude = root.GetProperty("latitude").GetDouble(),
                        Longitude = root.GetProperty("longitude").GetDouble(),
                        PacienteId = pacienteId,
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al obtener coordenadas: {ex.Message}");
                return CreateDefaultCoordenada(pacienteId);
            }
        }

        public async Task<(bool success, string message)> AddCoordinatesAsync(int patientId, double latitude, double longitude)
        {
            try
            {
                var coordinateData = new
                {
                    patient_id = patientId,
                    latitude = latitude.ToString(CultureInfo.InvariantCulture),
                    longitude = longitude.ToString(CultureInfo.InvariantCulture)
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(coordinateData),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync("add_coordinates", content);

                if (response.IsSuccessStatusCode)
                    return (true, "Coordenadas agregadas correctamente");

                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, $"Error al agregar coordenadas: {errorMessage}");
            }
            catch (Exception ex)
            {
                return (false, $"Error de conexión: {ex.Message}");
            }
        }

        private coordenadas CreateDefaultCoordenada(int pacienteId)
        {
            return new coordenadas
            {
                Id = 0,
                Latitude = 0,
                Longitude = 0,
                PacienteId = pacienteId,
                Fecha = "No hay datos disponibles"
            };
        }

        private void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token) ? null :
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
