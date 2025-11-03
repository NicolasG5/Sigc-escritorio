using Newtonsoft.Json;
using System;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo extendido de cita con información completa para mostrar en DataGrid
    /// Incluye datos de Paciente, Psicólogo y Servicio expandidos
    /// </summary>
    public class CitaExtendidaModel
    {
        // Datos de la Cita
        public int IdCita { get; set; }
        public string FechaCita { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string MotivoConsulta { get; set; }
        public string CodigoConfirmacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; }

        // Datos del Paciente (expandidos)
        public int IdPaciente { get; set; }
        public string RutPaciente { get; set; }
        public string NombrePaciente { get; set; }
        public string TelefonoPaciente { get; set; }
        public string EmailPaciente { get; set; }

        // Datos del Psicólogo (expandidos)
        public int IdPsicologo { get; set; }
        public string NombrePsicologo { get; set; }
        public string TituloPsicologo { get; set; }

        // Datos del Servicio (expandidos)
        public int IdServicio { get; set; }
        public string NombreServicio { get; set; }
        public string PrecioServicio { get; set; }
        public int DuracionServicio { get; set; }

        // Propiedades calculadas para mostrar en DataGrid
        public string FechaSolicitud => FechaCreacion.ToString("dd/MM/yyyy HH:mm");
        public string HorarioCita => $"{HoraInicio} - {HoraFin}";
        public string PacienteInfo => $"{NombrePaciente} ({RutPaciente})";
        public string PsicologoInfo => $"{NombrePsicologo} - {TituloPsicologo}";
        public string ServicioInfo => $"{NombreServicio} - ${PrecioServicio}";
        public string EstadoDisplay
        {
            get
            {
                switch (Estado?.ToLower())
                {
                    case "pendiente": return "? Pendiente";
                    case "confirmada": return "? Confirmada";
                    case "cancelada": return "? Cancelada";
                    case "completada": return "?? Completada";
                    default: return Estado;
                }
            }
        }
    }
}
