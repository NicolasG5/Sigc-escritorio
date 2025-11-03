using Newtonsoft.Json;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo para crear una solicitud de cita desde la landing page
    /// POST /api/v1/citas/
    /// </summary>
    public class SolicitudCreateModel
    {
        [JsonProperty("rut")]
        public string Rut { get; set; }

        [JsonProperty("nombres")]
        public string Nombres { get; set; }

        [JsonProperty("apellido_paterno")]
        public string ApellidoPaterno { get; set; }

        [JsonProperty("apellido_materno")]
        public string ApellidoMaterno { get; set; }

        [JsonProperty("telefono")]
        public string Telefono { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("fecha_nacimiento")]
        public string FechaNacimiento { get; set; }

        [JsonProperty("id_servicio")]
        public int IdServicio { get; set; }

        [JsonProperty("id_psicologo")]
        public int IdPsicologo { get; set; }

        [JsonProperty("fecha_cita")]
        public string FechaCita { get; set; }

        [JsonProperty("hora_inicio")]
        public string HoraInicio { get; set; }

        [JsonProperty("hora_fin")]
        public string HoraFin { get; set; }

        [JsonProperty("motivo_consulta")]
        public string MotivoConsulta { get; set; }
    }
}
