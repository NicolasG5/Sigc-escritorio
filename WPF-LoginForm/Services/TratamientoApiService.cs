using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.Services
{
    /// <summary>
    /// Servicio para gestionar tratamientos y observaciones iniciales
    /// </summary>
    public class TratamientoApiService
    {
        private readonly HttpClient _httpClient;

        public TratamientoApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://147.182.240.177:8000/");
        }

        /// <summary>
        /// Crea un tratamiento con observación inicial
        /// POST /api/v1/tratamientos/crear-con-observacion
        /// </summary>
        public async Task<TratamientoResponse> CrearTratamientoConObservacionAsync(CrearTratamientoRequest request)
        {
            try
            {
                var token = ApiTokenStore.Instance.Token;
                if (string.IsNullOrEmpty(token))
                {
                    System.Diagnostics.Debug.WriteLine("[TratamientoApiService] Token no disponible");
                    return null;
                }

                System.Diagnostics.Debug.WriteLine($"[TratamientoApiService] Creando tratamiento con observación inicial");
                System.Diagnostics.Debug.WriteLine($"  ID Cita: {request.IdCita}");
                System.Diagnostics.Debug.WriteLine($"  ID Paciente: {request.IdPaciente}");
                System.Diagnostics.Debug.WriteLine($"  ID Empleado: {request.IdEmpleado}");

                var json = JsonConvert.SerializeObject(request);
                System.Diagnostics.Debug.WriteLine($"[TratamientoApiService] JSON enviado: {json}");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/tratamientos/crear-con-observacion");
                httpRequest.Headers.Add("accept", "application/json");
                httpRequest.Headers.Add("Authorization", $"Bearer {token}");
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(httpRequest);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"[TratamientoApiService] Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[TratamientoApiService] Response Body: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var tratamiento = JsonConvert.DeserializeObject<TratamientoResponse>(responseContent);
                    System.Diagnostics.Debug.WriteLine($"? Tratamiento creado exitosamente - ID: {tratamiento.IdTratamiento}");
                    return tratamiento;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"? Error al crear tratamiento: {response.StatusCode} - {responseContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Exception en CrearTratamientoConObservacionAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }

    /// <summary>
    /// Modelo REQUEST para crear tratamiento con observación inicial
    /// POST /api/v1/tratamientos/crear-con-observacion
    /// </summary>
    public class CrearTratamientoRequest
    {
        [JsonProperty("tipo_tratamiento")]
        public string TipoTratamiento { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("objetivos")]
        public string Objetivos { get; set; }

        [JsonProperty("fecha_inicio")]
        public string FechaInicio { get; set; } // Formato: "YYYY-MM-DD"

        [JsonProperty("fecha_fin_estimada")]
        public string FechaFinEstimada { get; set; } // Formato: "YYYY-MM-DD"

        [JsonProperty("fecha_fin_real")]
        public string FechaFinReal { get; set; } // Formato: "YYYY-MM-DD" (opcional)

        [JsonProperty("estado")]
        public string Estado { get; set; } = "activo";

        [JsonProperty("id_paciente")]
        public int IdPaciente { get; set; }

        [JsonProperty("id_empleado")]
        public int IdEmpleado { get; set; }

        [JsonProperty("id_cita")]
        public int IdCita { get; set; }
    }

    /// <summary>
    /// Modelo RESPONSE del tratamiento completo (lo que devuelve la API)
    /// </summary>
    public class TratamientoResponse
    {
        [JsonProperty("tipo_tratamiento")]
        public string TipoTratamiento { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("objetivos")]
        public string Objetivos { get; set; }

        [JsonProperty("fecha_inicio")]
        public string FechaInicio { get; set; }

        [JsonProperty("fecha_fin_estimada")]
        public string FechaFinEstimada { get; set; }

        [JsonProperty("fecha_fin_real")]
        public string FechaFinReal { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [JsonProperty("id_tratamiento")]
        public int IdTratamiento { get; set; }

        [JsonProperty("id_paciente")]
        public int IdPaciente { get; set; }

        [JsonProperty("id_empleado")]
        public int IdEmpleado { get; set; }

        [JsonProperty("id_cita")]
        public int IdCita { get; set; }

        [JsonProperty("fecha_registro")]
        public string FechaRegistro { get; set; }
    }
}
