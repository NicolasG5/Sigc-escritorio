using Newtonsoft.Json;
using System;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo completo de Paciente para GET (incluye todos los campos)
    /// </summary>
    public class PacienteModel
    {
        [JsonProperty("id_paciente")]
        public int IdPaciente { get; set; }

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
        public int? IdCiudad { get; set; }

        [JsonProperty("id_prevision")]
        public int? IdPrevision { get; set; }

        [JsonProperty("fecha_registro")]
        public string FechaRegistro { get; set; }

        // Propiedad calculada para mostrar nombre completo
        public string NombreCompleto => $"{Nombres} {ApellidoPaterno} {ApellidoMaterno}";
    }

    /// <summary>
    /// Respuesta de la API para lista de pacientes
    /// </summary>
    public class PacientesResponse
    {
        [JsonProperty("data")]
        public PacienteModel[] Data { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
