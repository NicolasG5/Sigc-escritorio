using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    public class MedicacionApiService
    {
        private readonly HttpClient _httpClient;

        public MedicacionApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        public async Task<List<MedicacionModel>> GetAllMedicacionesAsync()
        {
            var token = ApiTokenStore.Instance.Token;
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/medicacion/");
            httpRequest.Headers.Add("accept", "application/json");
            httpRequest.Headers.Add("Authorization", $"Bearer {token}");
            var response = await _httpClient.SendAsync(httpRequest);
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) return null;
            var result = JsonConvert.DeserializeObject<MedicacionResponse>(json);
            return result?.Data ?? new List<MedicacionModel>();
        }

        public async Task<bool> CrearMedicacionAsync(object medicacionRequest)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                    return false;

                var json = JsonConvert.SerializeObject(medicacionRequest);
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/medicacion/");
                httpRequest.Headers.Add("accept", "application/json");
                httpRequest.Headers.Add("Authorization", $"Bearer {token}");
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[MedicacionApiService] Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[MedicacionApiService] Body: {responseContent}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CrearMedicacionAsync: {ex.Message}");
                return false;
            }
        }
    }
}
