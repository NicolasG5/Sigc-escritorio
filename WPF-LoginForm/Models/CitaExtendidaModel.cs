using System;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo extendido de Cita que incluye información del paciente, psicólogo y servicio
    /// USADO EN: ControlSolicitudes.xaml (para solicitudes pendientes)
    /// </summary>
    public class CitaExtendidaModel
    {
        // Datos básicos de la cita
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdPsicologo { get; set; }
        public int IdServicio { get; set; }
        
        // Información de fecha y hora
        public string FechaCita { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string HorarioCita => $"{HoraInicio} - {HoraFin}";
        
        // Información de la cita
        public string MotivoConsulta { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Estado { get; set; }
        public string CodigoConfirmacion { get; set; }

        // Información del paciente
        public string NombrePaciente { get; set; }
        public string RutPaciente { get; set; }
        public string EmailPaciente { get; set; }
        public string TelefonoPaciente { get; set; }

        // Información del psicólogo
        public string NombrePsicologo { get; set; }
        public string TituloPsicologo { get; set; }

        // Información del servicio
        public string NombreServicio { get; set; }
        public string PrecioServicio { get; set; }
        public int DuracionServicio { get; set; }
        
        // ? PROPIEDADES CALCULADAS PARA XAML (Compatible con C# 7.3)
        
        /// <summary>
        /// Información completa del servicio para tooltip
        /// </summary>
        public string ServicioInfo
        {
            get
            {
                return $"{NombreServicio}\n" +
                       $"Precio: ${PrecioServicio}\n" +
                       $"Duración: {DuracionServicio} minutos";
            }
        }
        
        /// <summary>
        /// Estado formateado para display
        /// </summary>
        public string EstadoDisplay
        {
            get
            {
                if (Estado == null) return "Desconocido";
                
                switch (Estado)
                {
                    case "Pendiente":
                        return "? Pendiente";
                    case "Confirmada":
                        return "? Confirmada";
                    case "En Curso":
                        return "?? En Curso";
                    case "Completada":
                        return "?? Completada";
                    case "Cancelada":
                        return "? Cancelada";
                    case "No Asistió":
                        return "?? No Asistió";
                    default:
                        return Estado;
                }
            }
        }
        
        /// <summary>
        /// Fecha de solicitud formateada
        /// </summary>
        public string FechaSolicitud
        {
            get
            {
                return FechaCreacion.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
