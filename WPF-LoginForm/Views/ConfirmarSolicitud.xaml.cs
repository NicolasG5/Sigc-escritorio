using System;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para ConfirmarSolicitud
    /// </summary>
    public partial class ConfirmarSolicitud : Page
    {
        private readonly CitaApiService _citaService;
        private readonly PacienteApiService _pacienteService;
        private readonly PsicologoApiService _psicologoService;
        private readonly ServicioApiService _servicioService;
        private int _idCita;
        private CitaModel _citaActual;

        /// <summary>
        /// Constructor sin parámetros (modo creación)
        /// </summary>
        public ConfirmarSolicitud()
        {
            InitializeComponent();
            _citaService = new CitaApiService();
            _pacienteService = new PacienteApiService();
            _psicologoService = new PsicologoApiService();
            _servicioService = new ServicioApiService();
        }

        /// <summary>
        /// Constructor con ID de cita (modo confirmación)
        /// </summary>
        public ConfirmarSolicitud(int idCita, CitaModel cita) : this()
        {
            _idCita = idCita;
            _citaActual = cita;
            
            CargarDatosSolicitud();
        }

        private async void CargarDatosSolicitud()
        {
            try
            {
                if (_citaActual == null)
                {
                    MessageBox.Show("⚠️ No hay datos de cita para cargar", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // --- INFORMACIÓN DEL PACIENTE ---
                var paciente = await _pacienteService.GetPacienteByIdAsync(_citaActual.IdPaciente);
                if (paciente != null)
                {
                    tbNombrePaciente.Text = paciente.NombreCompleto ?? "N/A";
                    tbRutPaciente.Text = paciente.Rut ?? "N/A";
                    tbEmailPaciente.Text = paciente.Email ?? "N/A";
                    tbTelefonoPaciente.Text = paciente.Telefono ?? "N/A";
                }
                else
                {
                    tbNombrePaciente.Text = "Paciente no encontrado";
                    tbRutPaciente.Text = "N/A";
                    tbEmailPaciente.Text = "N/A";
                    tbTelefonoPaciente.Text = "N/A";
                }

                // --- INFORMACIÓN DE LA CITA ---
                if (DateTime.TryParse(_citaActual.FechaCita, out DateTime fechaCita))
                {
                    dpFecha.SelectedDate = fechaCita;
                }

                tbHoraInicio.Text = _citaActual.HoraInicio ?? "N/A";
                tbHoraFin.Text = _citaActual.HoraFin ?? "N/A";
                tbCodigoConfirmacion.Text = _citaActual.CodigoConfirmacion ?? "Sin código";
                tbMotivoConsulta.Text = _citaActual.MotivoConsulta ?? "No especificado";
                tbObservaciones.Text = _citaActual.Observaciones ?? "Sin observaciones";

                // --- INFORMACIÓN DEL PSICÓLOGO ---
                var psicologo = await _psicologoService.GetPsicologoByIdAsync(_citaActual.IdPsicologo);
                if (psicologo != null)
                {
                    tbNombrePsicologo.Text = psicologo.NombreCompleto ?? "N/A";
                    tbTituloPsicologo.Text = psicologo.TituloProfesional ?? "N/A";
                    tbEmailPsicologo.Text = psicologo.EmailPersonal ?? "N/A";
                    tbTelefonoPsicologo.Text = psicologo.Telefono ?? "N/A";
                }
                else
                {
                    tbNombrePsicologo.Text = "Psicólogo no encontrado";
                    tbTituloPsicologo.Text = "N/A";
                    tbEmailPsicologo.Text = "N/A";
                    tbTelefonoPsicologo.Text = "N/A";
                }

                // --- INFORMACIÓN DEL SERVICIO ---
                var servicio = await _servicioService.GetServicioByIdAsync(_citaActual.IdServicio);
                if (servicio != null)
                {
                    tbNombreServicio.Text = servicio.NombreServicio ?? "N/A";
                    tbDuracionServicio.Text = $"{servicio.DuracionMinutos} minutos";
                    tbPrecioServicio.Text = $"${servicio.Precio}";
                }
                else
                {
                    tbNombreServicio.Text = "Servicio no encontrado";
                    tbDuracionServicio.Text = "N/A";
                    tbPrecioServicio.Text = "N/A";
                }

                // --- INFORMACIÓN ADICIONAL ---
                tbEstadoCita.Text = DeterminarEstado(_citaActual.IdEstadoCita);
                tbFechaSolicitud.Text = _citaActual.FechaCreacion.ToString("dd/MM/yyyy HH:mm");
                tbSala.Text = $"Sala #{_citaActual.IdSala}";

                System.Diagnostics.Debug.WriteLine($"✅ Datos cargados correctamente para cita ID: {_idCita}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al cargar datos de la solicitud:\n\n{ex.Message}\n\nDetalles: {ex.InnerException?.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Error completo: {ex}");
            }
        }

        /// <summary>
        /// Determina el texto del estado según el ID
        /// </summary>
        private string DeterminarEstado(int idEstadoCita)
        {
            switch (idEstadoCita)
            {
                case 1: return "⏳ Pendiente";
                case 2: return "✅ Confirmada";
                case 3: return "🔄 En Curso";
                case 4: return "✔️ Completada";
                case 5: return "❌ Cancelada";
                case 6: return "⚠️ No Asistió";
                default: return $"Estado {idEstadoCita}";
            }
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new ControlSolicitudes();
        }

        private async void Enviar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_citaActual == null)
                {
                    MessageBox.Show("⚠️ No hay solicitud cargada", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Mostrar confirmación
                var resultado = MessageBox.Show(
                    $"¿Confirmar la siguiente cita?\n\n" +
                    $"👤 Paciente: {tbNombrePaciente.Text}\n" +
                    $"👨‍⚕️ Psicólogo: {tbNombrePsicologo.Text}\n" +
                    $"📅 Fecha: {dpFecha.SelectedDate?.ToString("dd/MM/yyyy")}\n" +
                    $"🕐 Horario: {tbHoraInicio.Text} - {tbHoraFin.Text}\n" +
                    $"💼 Servicio: {tbNombreServicio.Text}\n" +
                    $"💰 Precio: {tbPrecioServicio.Text}\n" +
                    $"🔑 Código: {tbCodigoConfirmacion.Text}\n\n" +
                    $"✉️ Se enviará notificación al paciente y psicólogo\n" +
                    $"📆 Se creará evento en Google Calendar",
                    "Confirmar Solicitud",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.No)
                    return;

                // Confirmar la cita mediante API
                bool confirmada = await _citaService.ConfirmarCitaAsync(_idCita);

                if (confirmada)
                {
                    MessageBox.Show(
                        $"✅ ¡Solicitud confirmada exitosamente!\n\n" +
                        $"📋 Código: {tbCodigoConfirmacion.Text}\n" +
                        $"📅 Fecha: {dpFecha.SelectedDate?.ToString("dd/MM/yyyy")}\n" +
                        $"🕐 Hora: {tbHoraInicio.Text}\n" +
                        $"👤 Paciente: {tbNombrePaciente.Text}\n" +
                        $"👨‍⚕️ Psicólogo: {tbNombrePsicologo.Text}\n\n" +
                        $"✉️ Notificaciones enviadas\n" +
                        $"📆 Evento creado en Google Calendar", 
                        "Confirmación Exitosa", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                    
                    // Volver a la lista de solicitudes
                    Content = new ControlSolicitudes();
                }
                else
                {
                    MessageBox.Show(
                        "❌ No se pudo confirmar la solicitud\n\n" +
                        "Posibles causas:\n" +
                        "• Error de conexión con el servidor\n" +
                        "• La cita ya fue confirmada\n" +
                        "• Datos inválidos\n\n" +
                        "Intente nuevamente.", 
                        "Error al Confirmar", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Error al confirmar solicitud:\n\n" +
                    $"Mensaje: {ex.Message}\n\n" +
                    $"Detalles: {ex.InnerException?.Message}", 
                    "Error Crítico", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Error completo: {ex}");
            }
        }
    }
}
