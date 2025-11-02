using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_LoginForm.Models;

namespace WPF_LoginForm.Repositories
{
    public class ApiService
    {
        private readonly string _baseUrl = "http://147.182.240.177:8000";

        public async Task<List<UserModel>> GetUsersAsync(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{_baseUrl}/users/");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuariosResponse = JsonConvert.DeserializeObject<UsuariosPublicResponse>(json);
                    return usuariosResponse.data;
                }
                else
                {
                    // Maneja error
                    return null;
                }
            }
        }

        // Primero intentar desde API
        public async Task<UserModel> GetUsuarioByUsernameAsync(string username)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{_baseUrl}/users?username={username}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuariosResponse = JsonConvert.DeserializeObject<UsuariosPublicResponse>(json);
                    return usuariosResponse.data.FirstOrDefault();
                }
                else
                {
                    // Maneja error
                    return null;
                }
            }
        }
    }
}
