using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    public class CiudadApiService
    {
        private readonly HttpClient _httpClient;

        public CiudadApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Obtiene todas las ciudades
        /// GET /api/v1/ciudades/
        /// </summary>
        public async Task<IEnumerable<CiudadModel>> GetAllCiudadesAsync()
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[CiudadApiService] Token no disponible");
                    return new List<CiudadModel>();
                }

                var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/ciudades/?skip=0&limit=1000");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[CiudadApiService] GET {_httpClient.BaseAddress}api/v1/ciudades/");

                var response = await _httpClient.SendAsync(request);

                System.Diagnostics.Debug.WriteLine($"[CiudadApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<CiudadesResponse>(json);
                    System.Diagnostics.Debug.WriteLine($"[CiudadApiService] Ciudades cargadas: {apiResponse?.Data?.Count ?? 0}");

                    return apiResponse?.Data ?? new List<CiudadModel>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[CiudadApiService] Error: {response.StatusCode} - {errorContent}");
                }

                return new List<CiudadModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CiudadApiService] Exception: {ex.Message}");
                return new List<CiudadModel>();
            }
        }

        /// <summary>
        /// Obtiene una ciudad por ID
        /// GET /api/v1/ciudades/{id}
        /// </summary>
        public async Task<CiudadModel> GetCiudadByIdAsync(int idCiudad)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                    return null;

                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/ciudades/{idCiudad}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CiudadModel>(json);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CiudadApiService] Exception en GetCiudadByIdAsync: {ex.Message}");
                return null;
            }
        }
    }
}
