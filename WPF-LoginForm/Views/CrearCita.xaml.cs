using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para CrearCita.xaml
    /// </summary>
    public partial class CrearCita : Page
    {
        private readonly CitaApiService _citaService;
        private readonly ServicioApiService _servicioService;
        private readonly PsicologoApiService _psicologoService;
        private readonly PacienteApiService _pacienteService;
        
        // Variables para modo confirmación
        private int? _idCitaConfirmar = null;
        private CitaModel _citaActual = null;
        private bool _modoConfirmacion = false;

        /// <summary>
        /// Constructor por defecto (modo creación de nueva cita)
        /// </summary>
        public CrearCita()
        {
            InitializeComponent();
            _citaService = new CitaApiService();
            _servicioService = new ServicioApiService();
            _psicologoService = new PsicologoApiService();
            _pacienteService = new PacienteApiService();
            
            _modoConfirmacion = false;
            Titulo.Text = "Crear Nueva Cita";
            BtnEnviar.Content = "Crear Cita";
            
            CargarDatosIniciales();
        }

        /// <summary>
        /// Constructor con parámetros (modo confirmación de solicitud)
        /// </summary>
        public CrearCita(int idCita, CitaModel cita) : this()
        {
            _idCitaConfirmar = idCita;
            _citaActual = cita;
            _modoConfirmacion = true;
            
            Titulo.Text = "Confirmar Solicitud";
            BtnEnviar.Content = "Confirmar Cita";
            
            CargarDatosSolicitud();
        }

        private async void CargarDatosIniciales()
        {
            try
            {
                // Cargar pacientes
                var pacientes = await _pacienteService.GetPacientesActivosAsync();
                // Nota: tbNombre debería ser un ComboBox para seleccionar pacientes
                // Por ahora, se puede usar como texto libre
                
                // Cargar servicios en cbComuna (temporal, debería ser cbServicio)
                var servicios = await _servicioService.GetServiciosActivosAsync();
                cbComuna.ItemsSource = servicios;
                cbComuna.DisplayMemberPath = "DisplayName";
                cbComuna.SelectedValuePath = "IdServicio";

                // Cargar psicólogos en cbHora (temporal, debería ser cbPsicologo)
                var psicologos = await _psicologoService.GetPsicologosActivosAsync();
                cbHora.ItemsSource = psicologos;
                cbHora.DisplayMemberPath = "NombreCompleto";
                cbHora.SelectedValuePath = "IdPsicologo";

                // Inicializar fecha con hoy
                dpFecha.SelectedDate = DateTime.Today;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CargarDatosSolicitud()
        {
            try
            {
                if (_citaActual == null) return;

                // Primero cargar los datos de servicios y psicólogos
                var servicios = await _servicioService.GetServiciosActivosAsync();
                cbComuna.ItemsSource = servicios;
                cbComuna.DisplayMemberPath = "DisplayName";
                cbComuna.SelectedValuePath = "IdServicio";

                var psicologos = await _psicologoService.GetPsicologosActivosAsync();
                cbHora.ItemsSource = psicologos;
                cbHora.DisplayMemberPath = "NombreCompleto";
                cbHora.SelectedValuePath = "IdPsicologo";

                // Cargar información del paciente
                var paciente = await _pacienteService.GetPacienteByIdAsync(_citaActual.IdPaciente);
                if (paciente != null)
                {
                    tbNombre.Text = paciente.NombreCompleto;
                    tbNombre.IsReadOnly = true; // Bloquear edición en modo confirmación
                }

                // Seleccionar psicólogo
                cbHora.SelectedValue = _citaActual.IdPsicologo;

                // Seleccionar servicio
                cbComuna.SelectedValue = _citaActual.IdServicio;

                // Mostrar fecha y hora
                if (DateTime.TryParse(_citaActual.FechaCita, out DateTime fechaCita))
                {
                    dpFecha.SelectedDate = fechaCita;
                }

                // Mostrar motivo de consulta
                tbNombre_Copiar1.Text = _citaActual.MotivoConsulta;

                // Mostrar observaciones en dirección (temporal)
                txtDireccion.Text = _citaActual.Observaciones;

                // Mostrar código de confirmación como referencia
                if (!string.IsNullOrEmpty(_citaActual.CodigoConfirmacion))
                {
                    tbNombre_Copiar.Text = $"Código: {_citaActual.CodigoConfirmacion}";
                    tbNombre_Copiar.IsReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos de la solicitud: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                // Si estamos en modo confirmación
                if (_modoConfirmacion && _idCitaConfirmar.HasValue)
                {
                    await ConfirmarSolicitud();
                }
                else
                {
                    await CrearNuevaCita();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Confirma una solicitud de cita pendiente
        /// </summary>
        private async Task ConfirmarSolicitud()
        {
            try
            {
                // Validar campos obligatorios
                if (!ValidarCampos())
                    return;

                // Mostrar mensaje de confirmación
                var resultado = MessageBox.Show(
                    $"¿Confirmar la siguiente cita?\n\n" +
                    $"📋 Paciente: {tbNombre.Text}\n" +
                    $"👨‍⚕️ Psicólogo: {(cbHora.SelectedItem as PsicologoModel)?.NombreCompleto}\n" +
                    $"📅 Fecha: {dpFecha.SelectedDate?.ToString("dd/MM/yyyy")}\n" +
                    $"🔧 Servicio: {(cbComuna.SelectedItem as ServicioModel)?.NombreServicio}\n" +
                    $"💬 Motivo: {tbNombre_Copiar1.Text}\n\n" +
                    $"Se enviará notificación al paciente y psicólogo,\n" +
                    $"y se creará el evento en Google Calendar.",
                    "Confirmar Solicitud",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.No)
                    return;

                // Llamar al endpoint de confirmación
                bool confirmada = await _citaService.ConfirmarCitaAsync(_idCitaConfirmar.Value);

                if (confirmada)
                {
                    MessageBox.Show(
                        $"✅ ¡Solicitud confirmada exitosamente!\n\n" +
                        $"📋 Código: {_citaActual.CodigoConfirmacion}\n" +
                        $"📅 Fecha: {dpFecha.SelectedDate?.ToString("dd/MM/yyyy")}\n" +
                        $"🕐 Hora: {_citaActual.HoraInicio}\n\n" +
                        $"✉️ Se han enviado notificaciones\n" +
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
                        "❌ No se pudo confirmar la solicitud.\n\n" +
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
                    $"{ex.Message}\n\n" +
                    $"Detalles técnicos:\n{ex.InnerException?.Message}", 
                    "Error Crítico", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Crea una nueva cita desde cero
        /// </summary>
        private async Task CrearNuevaCita()
        {
            try
            {
                // Validar campos obligatorios
                if (!ValidarCampos())
                    return;

                // Obtener servicio seleccionado para calcular duración
                var servicioSeleccionado = cbComuna.SelectedItem as ServicioModel;
                var horaInicio = "09:00:00"; // Hora por defecto (puedes implementar un selector)
                var horaFin = CalcularHoraFin(horaInicio, servicioSeleccionado?.DuracionMinutos ?? 50);

                // Crear objeto de cita
                var nuevaCita = new CitaModel
                {
                    FechaCita = dpFecha.SelectedDate.Value.ToString("yyyy-MM-dd"),
                    HoraInicio = horaInicio,
                    HoraFin = horaFin,
                    MotivoConsulta = tbNombre_Copiar1.Text ?? "Consulta general",
                    Observaciones = txtDireccion.Text,
                    IdPaciente = 1, // TODO: Obtener del ComboBox de pacientes cuando lo implementes
                    IdPsicologo = (int)cbHora.SelectedValue,
                    IdServicio = (int)cbComuna.SelectedValue,
                    IdSala = 1, // TODO: Implementar selector de sala
                    IdEstadoCita = 1, // Estado inicial (pendiente)
                    RecordatorioEnviado = false
                };

                // Llamar al servicio para crear la cita
                var citaCreada = await _citaService.CreateCitaAsync(nuevaCita);

                if (citaCreada != null)
                {
                    MessageBox.Show(
                        $"✅ Cita creada exitosamente\n\n" +
                        $"📅 Fecha: {nuevaCita.FechaCita}\n" +
                        $"🕐 Hora: {nuevaCita.HoraInicio}\n" +
                        $"👨‍⚕️ Psicólogo: {(cbHora.SelectedItem as PsicologoModel)?.NombreCompleto}\n" +
                        $"🔧 Servicio: {servicioSeleccionado?.NombreServicio}", 
                        "Éxito", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Information);
                    
                    Content = new ControlSolicitudes();
                }
                else
                {
                    MessageBox.Show(
                        "❌ No se pudo crear la cita.\n" +
                        "Verifique los datos e intente nuevamente.", 
                        "Error", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear la cita: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Valida que todos los campos obligatorios estén completos
        /// </summary>
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(tbNombre.Text))
            {
                MessageBox.Show("⚠️ El campo Paciente no puede estar vacío", 
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dpFecha.SelectedDate == null)
            {
                MessageBox.Show("⚠️ Debe seleccionar una fecha para la cita", 
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbComuna.SelectedValue == null)
            {
                MessageBox.Show("⚠️ Debe seleccionar un servicio", 
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (cbHora.SelectedValue == null)
            {
                MessageBox.Show("⚠️ Debe seleccionar un psicólogo", 
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calcula la hora de fin basándose en la hora de inicio y la duración en minutos
        /// </summary>
        private string CalcularHoraFin(string horaInicio, int duracionMinutos)
        {
            if (TimeSpan.TryParse(horaInicio, out TimeSpan inicio))
            {
                var fin = inicio.Add(TimeSpan.FromMinutes(duracionMinutos));
                return fin.ToString(@"hh\:mm\:ss");
            }
            return "10:00:00"; // Valor por defecto
        }
    }
}
