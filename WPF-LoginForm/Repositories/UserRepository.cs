using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WPF_LoginForm.Models;

namespace WPF_LoginForm.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;

        public UserRepository()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("http://147.182.240.177:8000/");
        }

        // Autenticación usando la API, adaptando TokenResponse a LoginResponse
        public async Task<LoginResponse> AuthenticateUserAsync(string email, string password)
        {
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", email),
                new KeyValuePair<string, string>("password", password)
            };
            var content = new FormUrlEncodedContent(keyValues);
            var response = await _httpClient.PostAsync("/api/v1/login/access-token", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<LoginResponse>(json);
                tokenResponse.Success = !string.IsNullOrEmpty(tokenResponse.Token);
                tokenResponse.Username = email;
                return tokenResponse;
            }
            return new LoginResponse { Success = false, ErrorMessage = "Credenciales inválidas o error de conexión." };
        }

        public Task AddAsync(UserModel userModel)
        {
            throw new NotImplementedException();
        }
        public Task EditAsync(UserModel userModel)
        {
            throw new NotImplementedException();
        }
        public Task RemoveAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<UserModel> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task<UserModel> GetByUsernameAsync(string username)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] Called with username: {username}");
                
                // Obtener el token desde el almacén global (ajusta si usas otro método)
                var token = ApiTokenStore.Instance.Token;
                
                System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] Token: {(string.IsNullOrEmpty(token) ? "NULL/EMPTY" : token.Substring(0, 20) + "...")}");
                
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[GetByUsernameAsync] Token is null or empty, returning null");
                    return null;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/users/me");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] Sending request to: {_httpClient.BaseAddress}/api/v1/users/me");
                
                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] Response JSON: {json}");
                    
                    var user = JsonConvert.DeserializeObject<UserModel>(json);
                    
                    if (user != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] User deserialized: {user.Name} {user.LastName} (Username: {user.Username})");
                    }
                    
                    return user;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] Error response: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[GetByUsernameAsync] StackTrace: {ex.StackTrace}");
            }
            
            return null;
        }
        // Método para obtener todos los usuarios usando el token
        public async Task<IEnumerable<UserModel>> GetAllAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/users/?skip=0&limit=100");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<UsuariosPublicResponse>(json);
                return apiResponse.data;
            }
            return new List<UserModel>();
        }

        public async Task<IEnumerable<UserModel>> GetAllAsync()
        {
            // Obtener el token desde el almacén global (ajusta si usas otro método)
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return new List<UserModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/users/?skip=0&limit=100");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<UsuariosPublicResponse>(json);
                return apiResponse.data;
            }
            return new List<UserModel>();
        }
    }
}
