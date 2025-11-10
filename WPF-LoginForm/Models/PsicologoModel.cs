using Newtonsoft.Json;
using System;

namespace WPF_LoginForm.Models
{
    public class PsicologoModel
    {
        // ? CORRECCIÓN: La API devuelve "id_empleado" no "id_psicologo"
        [JsonProperty("id_empleado")]
        public int IdEmpleado { get; set; }
        
        // Alias para compatibilidad con código existente
        [JsonIgnore]
        public int IdPsicologo 
        { 
            get => IdEmpleado; 
            set => IdEmpleado = value; 
        }

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

        // ? ACTUALIZACIÓN: La API ahora devuelve "id_rol" en lugar de "rol_empleado"
        [JsonProperty("id_rol")]
        public int IdRol { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [JsonProperty("id_usuario")]
        public int IdUsuario { get; set; }

        [JsonProperty("fecha_registro")]
        public DateTime? FechaRegistro { get; set; }

        // Propiedad calculada para obtener el nombre del rol
        [JsonIgnore]
        public string RolEmpleado
        {
            get
            {
                switch (IdRol)
                {
                    case 1: return "psicologo";
                    case 2: return "administrativo";
                    case 3: return "recepcionista";
                    default: return "desconocido";
                }
            }
        }

        // Propiedad calculada para mostrar en UI
        public string NombreCompleto => $"{Nombres} {ApellidoPaterno} {ApellidoMaterno}";
        
        public string DisplayName => $"{NombreCompleto} - {TituloProfesional}";
    }
}
