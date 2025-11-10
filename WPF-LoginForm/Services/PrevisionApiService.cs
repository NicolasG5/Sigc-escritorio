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
    public class PrevisionApiService
    {
        private readonly HttpClient _httpClient;

        public PrevisionApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Obtiene todas las previsiones
        /// GET /api/v1/previsiones/
        /// </summary>
        public async Task<IEnumerable<PrevisionModel>> GetAllPrevisionesAsync()
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[PrevisionApiService] Token no disponible");
                    return new List<PrevisionModel>();
                }

                var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/previsiones/?skip=0&limit=1000");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[PrevisionApiService] GET {_httpClient.BaseAddress}api/v1/previsiones/");

                var response = await _httpClient.SendAsync(request);

                System.Diagnostics.Debug.WriteLine($"[PrevisionApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<PrevisionesResponse>(json);
                    System.Diagnostics.Debug.WriteLine($"[PrevisionApiService] Previsiones cargadas: {apiResponse?.Data?.Count ?? 0}");

                    return apiResponse?.Data ?? new List<PrevisionModel>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[PrevisionApiService] Error: {response.StatusCode} - {errorContent}");
                }

                return new List<PrevisionModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PrevisionApiService] Exception: {ex.Message}");
                return new List<PrevisionModel>();
            }
        }

        /// <summary>
        /// Obtiene una previsión por ID
        /// GET /api/v1/previsiones/{prevision_id}
        /// </summary>
        public async Task<PrevisionModel> GetPrevisionByIdAsync(int previsionId)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[PrevisionApiService] Token no disponible");
                    return null;
                }

                // ? CORRECCIÓN: Usar prevision_id en lugar de id
                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/previsiones/{previsionId}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[PrevisionApiService] GET {_httpClient.BaseAddress}api/v1/previsiones/{previsionId}");

                var response = await _httpClient.SendAsync(request);

                System.Diagnostics.Debug.WriteLine($"[PrevisionApiService] Response Status para prevision_id {previsionId}: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var prevision = JsonConvert.DeserializeObject<PrevisionModel>(json);

                    if (prevision != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"? [PrevisionApiService] Previsión encontrada: {prevision.NombrePrevision} ({prevision.Tipo})");
                    }

                    return prevision;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? [PrevisionApiService] Error para prevision_id {previsionId}: {response.StatusCode} - {errorContent}");
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? [PrevisionApiService] Exception en GetPrevisionByIdAsync({previsionId}): {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PrevisionApiService] StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
