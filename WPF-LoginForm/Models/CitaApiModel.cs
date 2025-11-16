using Newtonsoft.Json;

namespace WPF_LoginForm.Models
{
    public class CitaApiModel
    {
        [JsonProperty("fecha_cita")]
        public string FechaCita { get; set; }
        [JsonProperty("hora_inicio")]
        public string HoraInicio { get; set; }
        [JsonProperty("hora_fin")]
        public string HoraFin { get; set; }
        [JsonProperty("motivo_consulta")]
        public string MotivoConsulta { get; set; }
        [JsonProperty("observaciones")]
        public string Observaciones { get; set; }
        [JsonProperty("recordatorio_enviado")]
        public bool RecordatorioEnviado { get; set; }
        [JsonProperty("id_cita")]
        public int IdCita { get; set; }
        [JsonProperty("id_paciente")]
        public int IdPaciente { get; set; }
        [JsonProperty("id_empleado")]
        public int IdEmpleado { get; set; }
        [JsonProperty("id_servicio")]
        public int IdServicio { get; set; }
        [JsonProperty("id_sala")]
        public int IdSala { get; set; }
        [JsonProperty("id_estado_cita")]
        public int IdEstadoCita { get; set; }
        [JsonProperty("codigo_confirmacion")]
        public string CodigoConfirmacion { get; set; }
        [JsonProperty("fecha_creacion")]
        public string FechaCreacion { get; set; }
        [JsonProperty("fecha_modificacion")]
        public string FechaModificacion { get; set; }
    }
}