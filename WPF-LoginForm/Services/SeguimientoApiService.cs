using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_LoginForm.Repositories;
using System.Collections.Generic;

namespace WPF_LoginForm.Services
{
    public class SeguimientoApiService
    {
        private readonly HttpClient _httpClient;

        public SeguimientoApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        public async Task<bool> CrearSeguimientoAsync(object seguimientoRequest)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                    return false;

                var json = JsonConvert.SerializeObject(seguimientoRequest);
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/seguimiento/");
                httpRequest.Headers.Add("accept", "application/json");
                httpRequest.Headers.Add("Authorization", $"Bearer {token}");
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"[SeguimientoApiService] Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[SeguimientoApiService] Body: {responseContent}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CrearSeguimientoAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<SeguimientoResponse> GetSeguimientoByIdAsync(int id)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                    return null;

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/seguimiento/{id}");
                httpRequest.Headers.Add("accept", "application/json");
                httpRequest.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<SeguimientoResponse>(responseContent);
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetSeguimientoByIdAsync: {ex.Message}");
                return null;
            }
        }

        public class SeguimientoListResponse
        {
            public List<SeguimientoResponse> data { get; set; }
        }

        public async Task<List<SeguimientoResponse>> GetAllSeguimientosAsync()
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                    return null;

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/seguimiento/");
                httpRequest.Headers.Add("accept", "application/json");
                httpRequest.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var listResponse = JsonConvert.DeserializeObject<SeguimientoListResponse>(responseContent);
                    return listResponse?.data ?? new List<SeguimientoResponse>();
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetAllSeguimientosAsync: {ex.Message}");
                return null;
            }
        }
    }

    public class SeguimientoResponse
    {
        public string fecha_seguimiento { get; set; }
        public string tipo_seguimiento { get; set; }
        public string estado_animo { get; set; }
        public int nivel_funcionalidad { get; set; }
        public string adherencia_tratamiento { get; set; }
        public string observaciones { get; set; }
        public string proxima_evaluacion { get; set; }
        public int id_seguimiento { get; set; }
        public int id_paciente { get; set; }
        public int id_empleado { get; set; }
        public int id_tratamiento { get; set; }
        public string fecha_registro { get; set; }
    }
}
