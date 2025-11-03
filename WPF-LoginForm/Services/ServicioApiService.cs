using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    public class ServicioApiService
    {
        private readonly HttpClient _httpClient;

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
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return new List<ServicioModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/servicios/?skip=0&limit=1000");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var servicios = JsonConvert.DeserializeObject<List<ServicioModel>>(json);
                return servicios ?? new List<ServicioModel>();
            }
            return new List<ServicioModel>();
        }

        /// <summary>
        /// Obtiene un servicio por ID
        /// </summary>
        public async Task<ServicioModel> GetServicioByIdAsync(int idServicio)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/servicios/{idServicio}");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ServicioModel>(json);
            }
            return null;
        }

        /// <summary>
        /// Obtiene solo servicios activos (útil para ComboBox en UI)
        /// </summary>
        public async Task<IEnumerable<ServicioModel>> GetServiciosActivosAsync()
        {
            var todosLosServicios = await GetAllServiciosAsync();
            var serviciosActivos = new List<ServicioModel>();
            
            foreach (var servicio in todosLosServicios)
            {
                if (servicio.Estado == "activo")
                {
                    serviciosActivos.Add(servicio);
                }
            }
            
            return serviciosActivos;
        }
    }
}
