using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Marca un paciente como presente en una cita
        /// PUT /api/v1/citas/{id}/marcar-presente
        /// </summary>
        public async Task<bool> MarcarPresenteAsync(int idCita)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[CitaApiService] Token no disponible");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Marcando presente cita ID: {idCita}");
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] URL: {_httpClient.BaseAddress}api/v1/citas/{idCita}/marcar-presente");

                var request = new HttpRequestMessage(HttpMethod.Put, $"api/v1/citas/{idCita}/marcar-presente");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response Body: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"? Paciente marcado como presente en cita {idCita}");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"? Error al marcar presente: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"? Detalle del error: {responseContent}");
                    
                    // Intentar parsear el detalle del error
                    try
                    {
                        var errorObj = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        string errorDetail = errorObj?.detail ?? responseContent;
                        System.Diagnostics.Debug.WriteLine($"? Error específico: {errorDetail}");
                    }
                    catch { }
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Exception en MarcarPresenteAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene citas confirmadas (estado = "confirmada")
        /// GET /api/v1/citas/ con filtro por estado
        /// </summary>
        public async Task<IEnumerable<CitaModel>> GetCitasConfirmadasAsync(int skip = 0, int limit = 1000)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[CitaApiService] Token no disponible");
                    return new List<CitaModel>();
                }

                System.Diagnostics.Debug.WriteLine("[CitaApiService] Obteniendo citas confirmadas...");

                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/citas/?skip={skip}&limit={limit}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response JSON: {json.Substring(0, Math.Min(200, json.Length))}...");
                    
                    var citasResponse = JsonConvert.DeserializeObject<CitasResponse>(json);
                    var todasLasCitas = citasResponse?.Data != null 
                        ? citasResponse.Data.ToList() 
                        : new List<CitaModel>();
                    
                    // Filtrar solo citas confirmadas
                    var citasConfirmadas = todasLasCitas
                        .Where(c => c.IdEstadoCita == 2) // 2 = Confirmada según la base de datos
                        .ToList();
                    
                    System.Diagnostics.Debug.WriteLine($"[CitaApiService] Total citas: {todasLasCitas.Count}, Confirmadas: {citasConfirmadas.Count}");
                    
                    return citasConfirmadas;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? Error: {response.StatusCode} - {errorContent}");
                }
                
                return new List<CitaModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Exception en GetCitasConfirmadasAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return new List<CitaModel>();
            }
        }

        /// <summary>
        /// Obtiene citas con paciente presente (estado = 9 "paciente_presente")
        /// GET /api/v1/citas/ con filtro por IdEstadoCita == 9
        /// </summary>
        public async Task<IEnumerable<CitaModel>> GetCitasPacientePresenteAsync(int skip = 0, int limit = 1000)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[CitaApiService] Token no disponible");
                    return new List<CitaModel>();
                }

                System.Diagnostics.Debug.WriteLine("[CitaApiService] Obteniendo citas con paciente presente...");

                var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/citas/?skip={skip}&limit={limit}");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response JSON: {json.Substring(0, Math.Min(200, json.Length))}...");
                    
                    var citasResponse = JsonConvert.DeserializeObject<CitasResponse>(json);
                    var todasLasCitas = citasResponse?.Data != null 
                        ? citasResponse.Data.ToList() 
                        : new List<CitaModel>();
                    
                    // Filtrar solo citas con paciente presente (IdEstadoCita = 9)
                    var citasPacientePresente = todasLasCitas
                        .Where(c => c.IdEstadoCita == 9) // 9 = Paciente Presente
                        .ToList();
                    
                    System.Diagnostics.Debug.WriteLine($"[CitaApiService] Total citas: {todasLasCitas.Count}, Paciente Presente: {citasPacientePresente.Count}");
                    
                    return citasPacientePresente;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"? Error: {response.StatusCode} - {errorContent}");
                }
                
                return new List<CitaModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Exception en GetCitasPacientePresenteAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return new List<CitaModel>();
            }
        }

        /// <summary>
        /// Cambia el estado de una cita a "En Curso" (IdEstadoCita = 3)
        /// PUT /api/v1/citas/{id}/iniciar-sesion
        /// </summary>
        public async Task<bool> IniciarAtencionAsync(int idCita)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[CitaApiService] Token no disponible");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Iniciando sesión (atención) para cita ID: {idCita}");

                var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/citas/{idCita}/iniciar-sesion");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response Body: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"? Sesión iniciada para cita {idCita} - Estado cambiado a 'En Curso'");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"? Error al iniciar sesión: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Exception en IniciarAtencionAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cambia el estado de una cita a "Completada" (IdEstadoCita = 4)
        /// PUT /api/v1/citas/{id}/completar
        /// </summary>
        public async Task<bool> CompletarCitaAsync(int idCita)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[CitaApiService] Token no disponible");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Completando cita ID: {idCita}");

                var request = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/citas/{idCita}/completar");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                var response = await _httpClient.SendAsync(request);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[CitaApiService] Response Body: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"? Cita {idCita} completada - Estado cambiado a 'Completada'");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"? Error al completar cita: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Exception en CompletarCitaAsync: {ex.Message}");
                return false;
            }
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
