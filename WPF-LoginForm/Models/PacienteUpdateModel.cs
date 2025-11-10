using Newtonsoft.Json;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo para crear o actualizar un paciente (POST y PUT)
    /// </summary>
    public class PacienteUpdateModel
    {
        [JsonProperty("rut")]
        public string Rut { get; set; }

        [JsonProperty("nombres")]
        public string Nombres { get; set; }

        [JsonProperty("apellido_paterno")]
        public string ApellidoPaterno { get; set; }

        [JsonProperty("apellido_materno")]
        public string ApellidoMaterno { get; set; }

        [JsonProperty("fecha_nacimiento")]
        public string FechaNacimiento { get; set; }

        [JsonProperty("genero")]
        public string Genero { get; set; }

        [JsonProperty("telefono")]
        public string Telefono { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("direccion")]
        public string Direccion { get; set; }

        [JsonProperty("estado_civil")]
        public string EstadoCivil { get; set; }

        [JsonProperty("ocupacion")]
        public string Ocupacion { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [JsonProperty("consentimiento_informado")]
        public bool ConsentimientoInformado { get; set; }

        [JsonProperty("id_ciudad")]
        public int IdCiudad { get; set; }

        [JsonProperty("id_prevision")]
        public int IdPrevision { get; set; }
    }
}
