using Newtonsoft.Json;
using System;

namespace WPF_LoginForm.Models
{
    public class PsicologoModel
    {
        [JsonProperty("id_psicologo")]
        public int IdPsicologo { get; set; }

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

        [JsonProperty("telefono")]
        public string Telefono { get; set; }

        [JsonProperty("email_personal")]
        public string EmailPersonal { get; set; }

        [JsonProperty("direccion")]
        public string Direccion { get; set; }

        [JsonProperty("registro_profesional")]
        public string RegistroProfesional { get; set; }

        [JsonProperty("titulo_profesional")]
        public string TituloProfesional { get; set; }

        [JsonProperty("universidad")]
        public string Universidad { get; set; }

        [JsonProperty("anios_experiencia")]
        public int AniosExperiencia { get; set; }

        [JsonProperty("foto_perfil")]
        public string FotoPerfil { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [JsonProperty("id_usuario")]
        public int IdUsuario { get; set; }

        [JsonProperty("fecha_registro")]
        public DateTime? FechaRegistro { get; set; }

        // Propiedad calculada para mostrar en UI
        public string NombreCompleto => $"{Nombres} {ApellidoPaterno} {ApellidoMaterno}";
        
        public string DisplayName => $"{NombreCompleto} - {TituloProfesional}";
    }
}
