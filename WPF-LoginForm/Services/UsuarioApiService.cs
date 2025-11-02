using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WPF_LoginForm.Services
{
    public class UsuarioApiService : ApiServiceBase
    {
        // Método para autenticar usuario (login) - ajustado al backend real
        public async Task<TokenResponse> AutenticarAsync(string email, string password)
        {
            try
            {
                var loginData = new { username = email, password };  // API usa 'username' para email
                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/login/access-token", content);  // Endpoint correcto
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    return null;

                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TokenResponse>(responseJson);
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "Autenticar");
                return null;
            }
        }

        // Método para obtener usuario actual con token (test-token)
        public async Task<Usuario> GetUsuarioActualAsync(string token)
        {
            try
            {
                // Crear nuevo HttpClient con el token del login
                var client = new HttpClient();
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync("/login/test-token");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Usuario>(json);
            }
            catch (Exception ex)
            {
                HandleApiError(ex, "GetUsuarioActual");
                return null;
            }
        }

        // Mantener otros métodos si los necesitas
    }

    // Modelo TokenResponse para respuesta de login
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }
    }

    // Modelo Usuario ajustado al backend real
    public class Usuario
    {
        [JsonProperty("id_usuario")]
        public int IdUsuario { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("contrasena")]
        public string Contrasena { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }  // 'activo' o 'inactivo'

        [JsonProperty("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        // Agregar otros campos si existen en la API
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("apellido")]
        public string Apellido { get; set; }
    }
}