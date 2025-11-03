using Newtonsoft.Json;

namespace WPF_LoginForm.Models
{
    public class ServicioModel
    {
        [JsonProperty("id_servicio")]
        public int IdServicio { get; set; }

        [JsonProperty("nombre_servicio")]
        public string NombreServicio { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("duracion_minutos")]
        public int DuracionMinutos { get; set; }

        [JsonProperty("precio")]
        public string Precio { get; set; }

        [JsonProperty("tipo_servicio")]
        public string TipoServicio { get; set; }

        [JsonProperty("estado")]
        public string Estado { get; set; }

        // Propiedad calculada para mostrar en UI
        public string DisplayName => $"{NombreServicio} - ${Precio} ({DuracionMinutos} min)";
    }
}
