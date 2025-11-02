using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WPF_LoginForm.Models
{
    public class UserModel
    {
        [JsonProperty("id_usuario")]
        public string Id { get; set; }
        [JsonProperty("nombre_usuario")]
        public string Username { get; set; }
        [JsonProperty("contrasena")]
        public string Password { get; set; }
        [JsonProperty("nombres")]
        public string Name { get; set; }
        [JsonProperty("apellidos")]
        public string LastName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        public string Token { get; set; }
        [JsonProperty("estado")]
        public string Estado { get; set; } // Cambiado de bool a string
        public DateTime FechaCreacion { get; set; }
        public int RolId { get; set; }

        public bool Autenticar(string password)
        {
            return this.Password == password;
        }
        // Propiedad calculada para saber si está activo
        public bool IsActive => Estado == "activo";
    }
}
