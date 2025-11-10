using Newtonsoft.Json;
using System.Collections.Generic;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo para Ciudad
    /// GET /api/v1/ciudades/
    /// </summary>
    public class CiudadModel
    {
        [JsonProperty("id_ciudad")]
        public int IdCiudad { get; set; }

        [JsonProperty("nombre_ciudad")]
        public string NombreCiudad { get; set; }

        [JsonProperty("id_region")]
        public int IdRegion { get; set; }

        // Propiedad computada para UI
        public string Display => $"{NombreCiudad}";
    }

    /// <summary>
    /// Respuesta de la API para lista de ciudades
    /// </summary>
    public class CiudadesResponse
    {
        [JsonProperty("data")]
        public List<CiudadModel> Data { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
