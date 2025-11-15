using Newtonsoft.Json;

namespace WPF_LoginForm.Models
{
    public class MedicacionModel
    {
        [JsonProperty("nombre_medicamento")]
        public string NombreMedicamento { get; set; }
        [JsonProperty("dosis")]
        public string Dosis { get; set; }
        [JsonProperty("frecuencia")]
        public string Frecuencia { get; set; }
        [JsonProperty("via_administracion")]
        public string ViaAdministracion { get; set; }
        [JsonProperty("fecha_inicio")]
        public string FechaInicio { get; set; }
        [JsonProperty("fecha_fin")]
        public string FechaFin { get; set; }
        [JsonProperty("prescrito_por")]
        public string PrescritoPor { get; set; }
        [JsonProperty("observaciones")]
        public string Observaciones { get; set; }
        [JsonProperty("estado")]
        public string Estado { get; set; }
        [JsonProperty("id_medicamento")]
        public int IdMedicamento { get; set; }
        [JsonProperty("id_paciente")]
        public int IdPaciente { get; set; }
        [JsonProperty("id_tratamiento")]
        public int IdTratamiento { get; set; }
        [JsonProperty("fecha_registro")]
        public string FechaRegistro { get; set; }
        // Propiedad extra para mostrar en el DataGrid
        public string NombrePaciente { get; set; }
    }
}
