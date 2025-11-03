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
    public partial class ControlSolicitudes : UserControl
    {
        private readonly CitaApiService _citaService;
        private readonly PacienteApiService _pacienteService;
        private readonly PsicologoApiService _psicologoService;
        private readonly ServicioApiService _servicioService;
        
        private List<CitaExtendidaModel> _citasCompletas;

        public ControlSolicitudes()
        {
            InitializeComponent();
            _citaService = new CitaApiService();
            _pacienteService = new PacienteApiService();
            _psicologoService = new PsicologoApiService();
            _servicioService = new ServicioApiService();
            
            CargarDatos();
        }

        private async void CargarDatos()
        {
            try
            {
                // Mostrar indicador de carga
                GridDatos.ItemsSource = null;
                
                // Obtener solicitudes pendientes desde la API
                var solicitudesPendientes = await _citaService.GetCitasPendientesAsync();
                
                // Cargar datos completos para cada cita
                _citasCompletas = new List<CitaExtendidaModel>();
                
                foreach (var cita in solicitudesPendientes)
                {
                    var citaExtendida = await CargarDatosCompletosCita(cita);
                    if (citaExtendida != null)
                    {
                        _citasCompletas.Add(citaExtendida);
                    }
                }

                // Asignar al DataGrid
                GridDatos.ItemsSource = _citasCompletas;
                
                // Mostrar mensaje si no hay solicitudes
                if (_citasCompletas.Count == 0)
                {
                    MessageBox.Show("No hay solicitudes pendientes en este momento.", 
                        "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar solicitudes:\n\n{ex.Message}\n\nDetalles: {ex.InnerException?.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Carga los datos completos de una cita (paciente, psicólogo, servicio)
        /// </summary>
        private async Task<CitaExtendidaModel> CargarDatosCompletosCita(CitaModel cita)
        {
            try
            {
                // Cargar datos del paciente
                var paciente = await _pacienteService.GetPacienteByIdAsync(cita.IdPaciente);
                
                // Cargar datos del psicólogo
                var psicologo = await _psicologoService.GetPsicologoByIdAsync(cita.IdPsicologo);
                
                // Cargar datos del servicio
                var servicio = await _servicioService.GetServicioByIdAsync(cita.IdServicio);

                // Determinar el estado basado en id_estado_cita
                string estadoTexto = DeterminarEstado(cita.IdEstadoCita);

                return new CitaExtendidaModel
                {
                    // Datos de la cita
                    IdCita = cita.IdCita,
                    FechaCita = cita.FechaCita,
                    HoraInicio = cita.HoraInicio,
                    HoraFin = cita.HoraFin,
                    MotivoConsulta = cita.MotivoConsulta,
                    CodigoConfirmacion = cita.CodigoConfirmacion,
                    FechaCreacion = cita.FechaCreacion,
                    Estado = estadoTexto,
                    
                    // Datos del paciente
                    IdPaciente = cita.IdPaciente,
                    RutPaciente = paciente?.Rut ?? "N/A",
                    NombrePaciente = paciente?.NombreCompleto ?? "Desconocido",
                    TelefonoPaciente = paciente?.Telefono ?? "N/A",
                    EmailPaciente = paciente?.Email ?? "N/A",
                    
                    // Datos del psicólogo
                    IdPsicologo = cita.IdPsicologo,
                    NombrePsicologo = psicologo?.NombreCompleto ?? "Sin asignar",
                    TituloPsicologo = psicologo?.TituloProfesional ?? "N/A",
                    
                    // Datos del servicio
                    IdServicio = cita.IdServicio,
                    NombreServicio = servicio?.NombreServicio ?? "Sin servicio",
                    PrecioServicio = servicio?.Precio ?? "0",
                    DuracionServicio = servicio?.DuracionMinutos ?? 0
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos completos de cita {cita.IdCita}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Determina el texto del estado según el ID
        /// </summary>
        private string DeterminarEstado(int idEstadoCita)
        {
            switch (idEstadoCita)
            {
                case 1: return "Pendiente";
                case 2: return "Confirmada";
                case 3: return "En Curso";
                case 4: return "Completada";
                case 5: return "Cancelada";
                case 6: return "No Asistió";
                default: return $"Estado {idEstadoCita}";
            }
        }

        // Evento para búsqueda en el TextBox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_citasCompletas == null || !_citasCompletas.Any())
                    return;

                string textoBusqueda = Buscar.Text.ToLower().Trim();

                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    // Mostrar todas las citas
                    GridDatos.ItemsSource = _citasCompletas;
                }
                else
                {
                    // Filtrar por múltiples criterios
                    var citasFiltradas = _citasCompletas.Where(c =>
                        c.NombrePaciente.ToLower().Contains(textoBusqueda) ||
                        c.RutPaciente.ToLower().Contains(textoBusqueda) ||
                        c.NombrePsicologo.ToLower().Contains(textoBusqueda) ||
                        c.NombreServicio.ToLower().Contains(textoBusqueda) ||
                        c.CodigoConfirmacion.ToLower().Contains(textoBusqueda) ||
                        c.MotivoConsulta.ToLower().Contains(textoBusqueda)
                    ).ToList();

                    GridDatos.ItemsSource = citasFiltradas;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en búsqueda: {ex.Message}");
            }
        }

        // Evento para botón Agregar (Crear nueva solicitud)
        private void Agregar(object sender, RoutedEventArgs e)
        {
            CrearCita ventana = new CrearCita();
            FrameControlSolicitudes.Content = ventana;
        }

        // Evento para botón Confirmar (Abrir ventana CrearCita para confirmar)
        private async void Confirmar(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar que el CommandParameter no sea nulo
                if (((Button)sender).CommandParameter == null)
                {
                    MessageBox.Show("⚠️ Error: No se pudo obtener el ID de la cita.\n\nIntente nuevamente.", 
                        "Error de Datos", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idCita = (int)((Button)sender).CommandParameter;
                
                System.Diagnostics.Debug.WriteLine($"Confirmando cita con ID: {idCita}");
                
                // OPTIMIZACIÓN: Buscar primero en la lista local antes de hacer llamada a API
                var citaExtendidaLocal = _citasCompletas?.FirstOrDefault(c => c.IdCita == idCita);
                
                if (citaExtendidaLocal != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Cita encontrada en lista local: {citaExtendidaLocal.CodigoConfirmacion}");
                    
                    // Crear CitaModel desde CitaExtendidaModel
                    var citaModel = new CitaModel
                    {
                        IdCita = citaExtendidaLocal.IdCita,
                        FechaCita = citaExtendidaLocal.FechaCita,
                        HoraInicio = citaExtendidaLocal.HoraInicio,
                        HoraFin = citaExtendidaLocal.HoraFin,
                        MotivoConsulta = citaExtendidaLocal.MotivoConsulta,
                        CodigoConfirmacion = citaExtendidaLocal.CodigoConfirmacion,
                        FechaCreacion = citaExtendidaLocal.FechaCreacion,
                        IdPaciente = citaExtendidaLocal.IdPaciente,
                        IdPsicologo = citaExtendidaLocal.IdPsicologo,
                        IdServicio = citaExtendidaLocal.IdServicio,
                        IdEstadoCita = 1, // Pendiente
                        IdSala = 1,
                        RecordatorioEnviado = false
                    };
                    
                    // Navegar a CrearCita con los datos ya cargados
                    CrearCita ventana = new CrearCita(idCita, citaModel);
                    FrameControlSolicitudes.Content = ventana;
                    return;
                }
                
                // Si no está en la lista local, intentar obtener de la API
                System.Diagnostics.Debug.WriteLine($"Cita no encontrada localmente, consultando API...");
                
                var cita = await _citaService.GetCitaByIdAsync(idCita);
                
                if (cita == null)
                {
                    MessageBox.Show($"❌ No se pudo cargar la información de la solicitud\n\n" +
                        $"ID de Cita: {idCita}\n\n" +
                        $"La cita puede haber sido eliminada o no existe en la base de datos.\n\n" +
                        $"Actualizando lista de solicitudes...", 
                        "Error al Cargar Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    // Recargar datos para actualizar la lista
                    CargarDatos();
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"Cita cargada desde API: {cita.CodigoConfirmacion}");
                
                // Navegar a CrearCita pasando el ID de la cita a confirmar
                CrearCita ventana2 = new CrearCita(idCita, cita);
                FrameControlSolicitudes.Content = ventana2;
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show($"⚠️ Error de formato de datos:\n\n{ex.Message}\n\n" +
                    $"El ID de la cita no tiene el formato correcto.", 
                    "Error de Conversión", MessageBoxButton.OK, MessageBoxImage.Warning);
                System.Diagnostics.Debug.WriteLine($"Error de cast: {ex}");
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                MessageBox.Show($"❌ Error de conexión con el servidor:\n\n{ex.Message}\n\n" +
                    $"Verifique su conexión a internet y que el servidor esté disponible.", 
                    "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Error HTTP: {ex}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error inesperado al abrir confirmación:\n\n" +
                    $"Mensaje: {ex.Message}\n\n" +
                    $"Tipo: {ex.GetType().Name}\n\n" +
                    $"Detalles: {ex.InnerException?.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Error completo: {ex}");
            }
        }

        // Evento para botón Denegar (Rechazar solicitud)
        private async void Denegar(object sender, RoutedEventArgs e)
        {
            try
            {
                int idCita = (int)((Button)sender).CommandParameter;

                // Buscar la cita en la lista para mostrar información
                var citaInfo = _citasCompletas?.FirstOrDefault(c => c.IdCita == idCita);
                string infoAdicional = citaInfo != null 
                    ? $"\n\nPaciente: {citaInfo.NombrePaciente}\nFecha: {citaInfo.FechaCita}\nHora: {citaInfo.HorarioCita}"
                    : "";

                var resultado = MessageBox.Show(
                    $"¿Está seguro de denegar esta solicitud?{infoAdicional}\n\nEsta acción eliminará la solicitud permanentemente.",
                    "Confirmar Denegación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    // TODO: Implementar endpoint para denegar/cancelar cita
                    // Por ahora, eliminamos la cita
                    bool eliminada = await _citaService.DeleteCitaAsync(idCita);

                    if (eliminada)
                    {
                        MessageBox.Show($"✅ Solicitud denegada exitosamente\n\nCódigo: {citaInfo?.CodigoConfirmacion ?? idCita.ToString()}", 
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Recargar datos
                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show("❌ No se pudo denegar la solicitud.\n\nIntente nuevamente o contacte al administrador.", 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error al denegar solicitud:\n\n{ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

