using RastroClaroPrueba.Utils;
using RastroClaroPrueba.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RastroClaroPrueba.Models
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://127.0.0.1:5000") // Cambia esto por la URL de tu API
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var data = new { username, password };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("login", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LoginResponse>(json);

                    SessionManager.UserId = result.Id;
                    SessionManager.Token = result.Token;

                    return true;
                }
                else
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error en el login: {errorJson}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la solicitud: {ex.Message}");
            }
        }

        public async Task<usuarios> GetUsuarioAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"usuarios/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<usuarios>(json);
            }
            else
            {
                throw new Exception("Error al obtener los datos del usuario");
            }
        }

        public async Task<bool> UpdateUsuarioAsync(usuarios usuario)
        {
            var content = new StringContent(JsonSerializer.Serialize(usuario), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"usuarios/{usuario.Id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<pacientes> GetPacienteAsync(int pacienteId)
        {
            var response = await _httpClient.GetAsync($"pacientes/{pacienteId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<pacientes>(json);
            }
            else
            {
                throw new Exception("Error al obtener los datos del paciente");
            }
        }

        public async Task<bool> UpdatePacienteAsync(pacientes paciente)
        {
            var content = new StringContent(JsonSerializer.Serialize(paciente), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"pacientes/{paciente.FkID}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<Coordenada> GetCoordenadasAsync(int pacienteId)
        {
            var response = await _httpClient.GetAsync($"coordenadas/{pacienteId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Coordenada>(json);
            }
            else
            {
                throw new Exception("Error al obtener las coordenadas");
            }
        }

        public async Task<List<Coordenada>> GetCoordenadasPorFechaAsync(int pacienteId, string fecha)
        {
            var response = await _httpClient.GetAsync($"coordenadas/por-fecha?paciente_id={pacienteId}&fecha={fecha}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Coordenada>>(json);
            }
            else
            {
                throw new Exception("Error al obtener las coordenadas");
            }
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void Logout()
        {
            SessionManager.ClearSession();
        }

        public class LoginResponse
        {
            public int Id { get; set; }
            public string Token { get; set; }
        }
    }
}