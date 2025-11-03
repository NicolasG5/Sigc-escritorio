using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    public class PacienteApiService
    {
        private readonly HttpClient _httpClient;

        public PacienteApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Obtiene todos los pacientes
        /// </summary>
        public async Task<IEnumerable<PacienteModel>> GetAllPacientesAsync()
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return new List<PacienteModel>();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/pacientes/?skip=0&limit=1000");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var pacientes = JsonConvert.DeserializeObject<List<PacienteModel>>(json);
                return pacientes ?? new List<PacienteModel>();
            }
            return new List<PacienteModel>();
        }

        /// <summary>
        /// Obtiene un paciente por ID
        /// </summary>
        public async Task<PacienteModel> GetPacienteByIdAsync(int idPaciente)
        {
            var token = ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return null;

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/pacientes/{idPaciente}");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PacienteModel>(json);
            }
            return null;
        }

        /// <summary>
        /// Obtiene solo pacientes activos (útil para ComboBox en UI)
        /// </summary>
        public async Task<IEnumerable<PacienteModel>> GetPacientesActivosAsync()
        {
            var todosLosPacientes = await GetAllPacientesAsync();
            var pacientesActivos = new List<PacienteModel>();
            
            foreach (var paciente in todosLosPacientes)
            {
                if (paciente.Estado == "activo")
                {
                    pacientesActivos.Add(paciente);
                }
            }
            
            return pacientesActivos;
        }
    }
}
