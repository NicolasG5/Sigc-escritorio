using Newtonsoft.Json;
using System.Collections.Generic;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo para Previsión de Salud
    /// GET /api/v1/previsiones/
    /// </summary>
    public class PrevisionModel
    {
        [JsonProperty("id_prevision")]
        public int IdPrevision { get; set; }

        [JsonProperty("nombre_prevision")]
        public string NombrePrevision { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        // Propiedad computada para UI
        public string Display => $"{NombrePrevision} ({Tipo})";
    }

    /// <summary>
    /// Respuesta de la API para lista de previsiones
    /// </summary>
    public class PrevisionesResponse
    {
        [JsonProperty("data")]
        public List<PrevisionModel> Data { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
