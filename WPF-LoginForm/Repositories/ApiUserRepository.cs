using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WPF_LoginForm.Models;

namespace WPF_LoginForm.Repositories
{
    public class ApiUserRepository : IUserRepository
    {
        private readonly HttpClient _httpClient;

        public ApiUserRepository()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("http://147.182.240.177:8000/"); // URL real de tu API
        }

        // Obtener usuario por ID
        public async Task<UserModel> GetByIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"/api/v1/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserModel>(json);
            }
            return null;
        }

        // Crear usuario
        public async Task AddAsync(UserModel userModel)
        {
            var content = new StringContent(JsonConvert.SerializeObject(userModel), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/v1/users/signup", content);
            response.EnsureSuccessStatusCode();
        }

        // Métodos requeridos por la interfaz
        public Task<LoginResponse> AuthenticateUserAsync(string email, string password)
        {
            throw new System.NotImplementedException();
        }
        public Task EditAsync(UserModel userModel)
        {
            throw new System.NotImplementedException();
        }
        public Task RemoveAsync(int id)
        {
            throw new System.NotImplementedException();
        }
        public Task<UserModel> GetByUsernameAsync(string username)
        {
            throw new System.NotImplementedException();
        }
        public Task<IEnumerable<UserModel>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
