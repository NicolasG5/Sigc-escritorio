using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    public class CitaApiService
    {
        private readonly HttpClient _httpClient;

        public CitaApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Obtiene todas las citas
        /// </summary>
        public async Task<IEnumerable<CitaModel>> GetAllCitasAsync()
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return new List<CitaModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/citas/?skip=0&limit=1000");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var citasResponse = JsonConvert.DeserializeObject<CitasResponse>(json);
                return citasResponse.Data ?? new CitaModel[0];
            }
            return new List<CitaModel>();
        }

        /// <summary>
        /// Obtiene todas las citas pendientes (solicitudes desde landing page)
        /// GET /api/v1/citas/pendientes/lista
        /// </summary>
        public async Task<IEnumerable<CitaModel>> GetCitasPendientesAsync(int skip = 0, int limit = 100)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return new List<CitaModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/citas/pendientes/lista?skip={skip}&limit={limit}");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var citasResponse = JsonConvert.DeserializeObject<CitasResponse>(json);
                return citasResponse.Data ?? new CitaModel[0];
            }
            return new List<CitaModel>();
        }

        /// <summary>
        /// Obtiene una cita por ID
        /// </summary>
        public async Task<CitaModel> GetCitaByIdAsync(int idCita)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("Error: Token no disponible");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"Obteniendo cita con ID: {idCita}");
                
                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/citas/{idCita}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"Status Code: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Response JSON: {json}");
                    
                    var cita = JsonConvert.DeserializeObject<CitaModel>(json);
                    System.Diagnostics.Debug.WriteLine($"Cita deserializada: {cita?.IdCita}");
                    
                    return cita;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error Response: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción en GetCitaByIdAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                throw; // Re-lanzar para que el método llamador pueda manejarlo
            }
        }

        /// <summary>
        /// Obtiene una cita por código de confirmación
        /// GET /api/v1/citas/{codigo_confirmacion}
        /// </summary>
        public async Task<CitaModel> GetCitaByCodigoAsync(string codigoConfirmacion)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/citas/{codigoConfirmacion}");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CitaModel>(json);
            }
            return null;
        }

        /// <summary>
        /// Crea una nueva cita (usado internamente desde app escritorio)
        /// </summary>
        public async Task<CitaModel> CreateCitaAsync(CitaModel cita)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/citas/");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var jsonContent = JsonConvert.SerializeObject(cita);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CitaModel>(json);
            }
            return null;
        }

        /// <summary>
        /// Crea una nueva solicitud de cita (desde landing page)
        /// POST /api/v1/citas/
        /// </summary>
        public async Task<CitaModel> CreateSolicitudAsync(SolicitudCreateModel solicitud)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/citas/");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var jsonContent = JsonConvert.SerializeObject(solicitud);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CitaModel>(json);
            }
            return null;
        }

        /// <summary>
        /// Confirma una cita pendiente (cambia estado a "Confirmada")
        /// PUT /api/v1/citas/{id}/confirmar
        /// </summary>
        public async Task<bool> ConfirmarCitaAsync(int idCita)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/citas/{idCita}/confirmar");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Actualiza una cita existente
        /// </summary>
        public async Task<CitaModel> UpdateCitaAsync(int idCita, CitaModel cita)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/citas/{idCita}");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var jsonContent = JsonConvert.SerializeObject(cita);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CitaModel>(json);
            }
            return null;
        }

        /// <summary>
        /// Elimina una cita
        /// </summary>
        public async Task<bool> DeleteCitaAsync(int idCita)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/citas/{idCita}");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}
