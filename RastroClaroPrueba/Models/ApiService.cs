using System;
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
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://127.0.0.1:5000"); // Cambia esto por la URL de tu API
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Método para iniciar sesión y obtener el token JWT
        public async Task<string> LoginAsync(string username, string password)
        {
            var data = new { username, password };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("login", content);
            return await response.Content.ReadAsStringAsync();
        }

        // Método para actualizar un paciente
        public async Task<string> UpdatePacienteAsync(int fkId, string nombre, int? edad = null, string religion = null, string grado = null, string extra = null, string telefono = null)
        {
            var data = new { FkID = fkId, nombre, edad, religion, Grado = grado, Extra = extra, Telefono = telefono };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("update_paciente", content);
            return await response.Content.ReadAsStringAsync();
        }

       // Método para obtener coordenadas de un paciente
        public async Task<string> GetCoordenadasAsync(int pacienteId)
        {
            var response = await _httpClient.GetAsync($"get_coordenadas?paciente_id={pacienteId}");
            return await response.Content.ReadAsStringAsync();
        }

        // Método para establecer el token JWT en las cabeceras de las solicitudes
        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}