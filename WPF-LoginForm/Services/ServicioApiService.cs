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
    public class ServicioApiService
    {
        private readonly HttpClient _httpClient;
        private static List<ServicioModel> _serviciosCache = null;
        private static DateTime _lastCacheUpdate = DateTime.MinValue;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public ServicioApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Obtiene todos los servicios disponibles
        /// </summary>
        public async Task<IEnumerable<ServicioModel>> GetAllServiciosAsync()
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[ServicioApiService] Token no disponible");
                    return new List<ServicioModel>();
                }

                // Usar caché si está disponible y no ha expirado
                if (_serviciosCache != null && (DateTime.Now - _lastCacheUpdate) < CacheDuration)
                {
                    System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Usando caché de servicios ({_serviciosCache.Count} servicios)");
                    return _serviciosCache;
                }

                // ? Endpoint corregido
                var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/servicios/?skip=0&limit=1000");
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");

                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] GET {_httpClient.BaseAddress}api/v1/servicios/");

                var response = await _httpClient.SendAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Response JSON: {json.Substring(0, Math.Min(200, json.Length))}...");
                    
                    var servicios = JsonConvert.DeserializeObject<List<ServicioModel>>(json);
                    System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Servicios deserializados: {servicios?.Count ?? 0}");
                    
                    // Actualizar caché
                    _serviciosCache = servicios ?? new List<ServicioModel>();
                    _lastCacheUpdate = DateTime.Now;
                    
                    return _serviciosCache;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Error: {response.StatusCode} - {errorContent}");
                }
                
                return new List<ServicioModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Exception en GetAllServiciosAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] StackTrace: {ex.StackTrace}");
                return new List<ServicioModel>();
            }
        }

        /// <summary>
        /// Obtiene un servicio por ID (busca en caché local)
        /// </summary>
        public async Task<ServicioModel> GetServicioByIdAsync(int idServicio)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Buscando servicio ID: {idServicio}");
                
                // Primero obtener todos los servicios (usa caché si está disponible)
                var todosLosServicios = await GetAllServiciosAsync();
                
                if (todosLosServicios == null || !todosLosServicios.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"[ServicioApiService] No hay servicios disponibles en la lista");
                    return null;
                }
                
                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Buscando en {todosLosServicios.Count()} servicios disponibles");
                
                // Buscar el servicio por ID en la lista
                var servicio = todosLosServicios.FirstOrDefault(s => s.IdServicio == idServicio);
                
                if (servicio != null)
                {
                    System.Diagnostics.Debug.WriteLine($"? [ServicioApiService] Servicio encontrado: {servicio.NombreServicio} (Precio: {servicio.Precio}, Duración: {servicio.DuracionMinutos} min)");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"?? [ServicioApiService] Servicio con ID {idServicio} no encontrado en la lista");
                    
                    // Mostrar IDs disponibles para debug
                    var idsDisponibles = string.Join(", ", todosLosServicios.Select(s => s.IdServicio));
                    System.Diagnostics.Debug.WriteLine($"[ServicioApiService] IDs disponibles: {idsDisponibles}");
                }
                
                return servicio;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? [ServicioApiService] Exception en GetServicioByIdAsync({idServicio}): {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene solo servicios activos (útil para ComboBox en UI)
        /// </summary>
        public async Task<IEnumerable<ServicioModel>> GetServiciosActivosAsync()
        {
            try
            {
                var todosLosServicios = await GetAllServiciosAsync();
                var serviciosActivos = todosLosServicios.Where(s => s.Estado == "activo").ToList();
                
                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Servicios activos encontrados: {serviciosActivos.Count}");
                
                return serviciosActivos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ServicioApiService] Exception en GetServiciosActivosAsync: {ex.Message}");
                return new List<ServicioModel>();
            }
        }
        
        /// <summary>
        /// Limpia el caché de servicios (útil para forzar actualización)
        /// </summary>
        public static void ClearCache()
        {
            _serviciosCache = null;
            _lastCacheUpdate = DateTime.MinValue;
            System.Diagnostics.Debug.WriteLine("[ServicioApiService] Caché limpiado");
        }
    }
}
