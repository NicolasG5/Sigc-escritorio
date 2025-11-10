using System;

namespace WPF_LoginForm.Models
{
    /// <summary>
    /// Modelo para citas con paciente presente (usado en Atencion.xaml)
    /// Incluye información del paciente para mostrar en atención
    /// </summary>
    public class CitaPacientePresenteModel
    {
        // Datos de la cita
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdPsicologo { get; set; }
        public DateTime? FechaCita { get; set; }
        public string HoraCita { get; set; } // Formato: "09:00 - 10:00"
        public string EstadoCita { get; set; }
        public string MotivoCita { get; set; }
        public string CodigoConfirmacion { get; set; }

        // Datos del paciente (extendidos)
        public string NombrePaciente { get; set; }
        public string RutPaciente { get; set; }
        public string EmailPaciente { get; set; }
        public string TelefonoPaciente { get; set; }

        // Propiedad calculada para mostrar fecha formateada
        public string FechaCitaFormateada => FechaCita?.ToString("dd/MM/yyyy") ?? "";
    }
}
