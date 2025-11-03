using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    public class PsicologoApiService
    {
        private readonly HttpClient _httpClient;

        public PsicologoApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Obtiene todos los psicólogos
        /// </summary>
        public async Task<IEnumerable<PsicologoModel>> GetAllPsicologosAsync()
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return new List<PsicologoModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/psicologos/?skip=0&limit=1000");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var psicologos = JsonConvert.DeserializeObject<List<PsicologoModel>>(json);
                return psicologos ?? new List<PsicologoModel>();
            }
            return new List<PsicologoModel>();
        }

        /// <summary>
        /// Obtiene un psicólogo por ID
        /// </summary>
        public async Task<PsicologoModel> GetPsicologoByIdAsync(int idPsicologo)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/psicologos/{idPsicologo}");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PsicologoModel>(json);
            }
            return null;
        }

        /// <summary>
        /// Obtiene solo psicólogos activos (útil para ComboBox en UI)
        /// </summary>
        public async Task<IEnumerable<PsicologoModel>> GetPsicologosActivosAsync()
        {
            var todosLosPsicologos = await GetAllPsicologosAsync();
            var psicologosActivos = new List<PsicologoModel>();
            
            foreach (var psicologo in todosLosPsicologos)
            {
                if (psicologo.Estado == "activo")
                {
                    psicologosActivos.Add(psicologo);
                }
            }
            
            return psicologosActivos;
        }
    }
}
