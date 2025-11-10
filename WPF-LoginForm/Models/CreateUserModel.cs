using Newtonsoft.Json;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo para crear un nuevo usuario mediante API
    /// POST /api/v1/users/
    /// </summary>
    public class CreateUserModel
    {
        [JsonProperty("nombre_usuario")]
        public string NombreUsuario { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; } = "activo";

        [JsonProperty("intentos_fallidos")]
        public int IntentosFallidos { get; set; } = 0;

        [JsonProperty("contrasena")]
        public string Contrasena { get; set; }
    }

    /// <summary>
    /// Modelo para actualizar un usuario existente
    /// PUT /api/v1/users/{id}
    /// </summary>
    public class UpdateUserModel
    {
        [JsonProperty("nombre_usuario")]
        public string NombreUsuario { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        [JsonProperty("intentos_fallidos")]
        public int IntentosFallidos { get; set; }

        // Nota: La contraseña solo se incluye si se desea cambiar
        [JsonProperty("contrasena")]
        public string Contrasena { get; set; }
    }
}
