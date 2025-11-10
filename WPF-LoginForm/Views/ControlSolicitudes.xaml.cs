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
                System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] Iniciando carga para cita ID: {cita.IdCita}");
                System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] IdPaciente: {cita.IdPaciente}, IdPsicologo: {cita.IdPsicologo}, IdServicio: {cita.IdServicio}");
                
                // Cargar datos del paciente
                System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] Cargando paciente ID: {cita.IdPaciente}");
                var paciente = await _pacienteService.GetPacienteByIdAsync(cita.IdPaciente);
                System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] Paciente cargado: {paciente?.NombreCompleto ?? "NULL"}");
                
                // Cargar datos del psicólogo (OPCIONAL para solicitudes pendientes)
                System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] Verificando psicólogo ID: {cita.IdPsicologo}");
                
                PsicologoModel psicologo = null;
                string nombrePsicologo = "Sin asignar";
                string tituloPsicologo = "Pendiente de asignación";
                
                if (cita.IdPsicologo > 0)
                {
                    try
                    {
                        psicologo = await _psicologoService.GetPsicologoByIdAsync(cita.IdPsicologo);
                        
                        if (psicologo != null)
                        {
                            nombrePsicologo = psicologo.NombreCompleto;
                            tituloPsicologo = psicologo.TituloProfesional ?? "N/A";
                            System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] ✅ Psicólogo: {nombrePsicologo}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] ⚠️ Psicólogo NULL para ID: {cita.IdPsicologo}");
                            nombrePsicologo = $"⚠️ Error (ID: {cita.IdPsicologo} no encontrado)";
                        }
                    }
                    catch (Exception exPsi)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] ❌ Error al cargar psicólogo: {exPsi.Message}");
                        nombrePsicologo = "Error al cargar";
                    }
                }
                else
                {
                    // NORMAL: Las solicitudes pendientes no tienen psicólogo asignado todavía
                    System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] ℹ️ Solicitud pendiente sin psicólogo asignado (normal)");
                }
                
                // Cargar datos del servicio
                System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] Cargando servicio ID: {cita.IdServicio}");
                var servicio = await _servicioService.GetServicioByIdAsync(cita.IdServicio);
                System.Diagnostics.Debug.WriteLine($"[CargarDatosCompletosCita] Servicio cargado: {servicio?.NombreServicio ?? "NULL"}");
                
                if (servicio == null)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ [CargarDatosCompletosCita] ADVERTENCIA: Servicio es NULL para IdServicio={cita.IdServicio}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ [CargarDatosCompletosCita] Servicio OK: {servicio.NombreServicio}, Precio: {servicio.Precio}, Duracion: {servicio.DuracionMinutos}");
                }

                // Determinar el estado basado en id_estado_cita
                string estadoTexto = DeterminarEstado(cita.IdEstadoCita);

                var citaExtendida = new CitaExtendidaModel
                {
                    // Datos de la cita (con valores por defecto seguros)
                    IdCita = cita.IdCita,
                    FechaCita = cita.FechaCita ?? "",
                    HoraInicio = cita.HoraInicio ?? "",
                    HoraFin = cita.HoraFin ?? "",
                    MotivoConsulta = cita.MotivoConsulta ?? "Sin motivo especificado",
                    CodigoConfirmacion = cita.CodigoConfirmacion ?? "",
                    FechaCreacion = cita.FechaCreacion,
                    Estado = estadoTexto ?? "Desconocido",
                    
                    // Datos del paciente (con valores por defecto)
                    IdPaciente = cita.IdPaciente,
                    RutPaciente = paciente?.Rut ?? "N/A",
                    NombrePaciente = paciente?.NombreCompleto ?? "Desconocido",
                    TelefonoPaciente = paciente?.Telefono ?? "N/A",
                    EmailPaciente = paciente?.Email ?? "N/A",
                    
                    // Datos del psicólogo (puede ser "Sin asignar" para solicitudes pendientes)
                    IdPsicologo = cita.IdPsicologo,
                    NombrePsicologo = nombrePsicologo,
                    TituloPsicologo = tituloPsicologo,
                    
                    // Datos del servicio (con valores por defecto)
                    IdServicio = cita.IdServicio,
                    NombreServicio = servicio?.NombreServicio ?? "Sin servicio",
                    PrecioServicio = servicio?.Precio ?? "0",
                    DuracionServicio = servicio?.DuracionMinutos ?? 0
                };
                
                System.Diagnostics.Debug.WriteLine($"✅ [CargarDatosCompletosCita] CitaExtendida creada - Servicio: {citaExtendida.NombreServicio}");
                
                return citaExtendida;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ [CargarDatosCompletosCita] Error al cargar datos completos de cita {cita.IdCita}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ [CargarDatosCompletosCita] StackTrace: {ex.StackTrace}");
                
                // ✅ CORRECCIÓN: En caso de error, devolver objeto con valores por defecto en lugar de null
                return new CitaExtendidaModel
                {
                    IdCita = cita.IdCita,
                    FechaCita = cita.FechaCita ?? "",
                    HoraInicio = cita.HoraInicio ?? "",
                    HoraFin = cita.HoraFin ?? "",
                    MotivoConsulta = cita.MotivoConsulta ?? "Error al cargar datos",
                    CodigoConfirmacion = cita.CodigoConfirmacion ?? "",
                    FechaCreacion = cita.FechaCreacion,
                    Estado = "Error",
                    IdPaciente = cita.IdPaciente,
                    RutPaciente = "Error",
                    NombrePaciente = "Error al cargar",
                    TelefonoPaciente = "N/A",
                    EmailPaciente = "N/A",
                    IdPsicologo = cita.IdPsicologo,
                    NombrePsicologo = "Error al cargar",
                    TituloPsicologo = "N/A",
                    IdServicio = cita.IdServicio,
                    NombreServicio = "Error al cargar",
                    PrecioServicio = "0",
                    DuracionServicio = 0
                };
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
                    // ✅ CORRECCIÓN: Filtrar con manejo seguro de valores nulos
                    var citasFiltradas = _citasCompletas.Where(c =>
                        (!string.IsNullOrEmpty(c.NombrePaciente) && c.NombrePaciente.ToLower().Contains(textoBusqueda)) ||
                        (!string.IsNullOrEmpty(c.RutPaciente) && c.RutPaciente.ToLower().Contains(textoBusqueda)) ||
                        (!string.IsNullOrEmpty(c.NombrePsicologo) && c.NombrePsicologo.ToLower().Contains(textoBusqueda)) ||
                        (!string.IsNullOrEmpty(c.NombreServicio) && c.NombreServicio.ToLower().Contains(textoBusqueda)) ||
                        (!string.IsNullOrEmpty(c.CodigoConfirmacion) && c.CodigoConfirmacion.ToLower().Contains(textoBusqueda)) ||
                        (!string.IsNullOrEmpty(c.MotivoConsulta) && c.MotivoConsulta.ToLower().Contains(textoBusqueda)) ||
                        (!string.IsNullOrEmpty(c.Estado) && c.Estado.ToLower().Contains(textoBusqueda))
                    ).ToList();

                    GridDatos.ItemsSource = citasFiltradas;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en búsqueda: {ex.Message}");
                // En caso de error, mostrar todas las citas
                GridDatos.ItemsSource = _citasCompletas;
            }
        }

        // Evento para botón Agregar (Crear nueva solicitud)
        private void Agregar(object sender, RoutedEventArgs e)
        {
            CrearCita ventana = new CrearCita();
            FrameControlSolicitudes.Content = ventana;
        }

        // Evento para botón Confirmar (Abrir ventana ConfirmarSolicitud para confirmar)
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
                        Observaciones = "",
                        CodigoConfirmacion = citaExtendidaLocal.CodigoConfirmacion,
                        FechaCreacion = citaExtendidaLocal.FechaCreacion,
                        IdPaciente = citaExtendidaLocal.IdPaciente,
                        IdPsicologo = citaExtendidaLocal.IdPsicologo,
                        IdServicio = citaExtendidaLocal.IdServicio,
                        IdEstadoCita = 1, // Pendiente
                        IdSala = 1,
                        RecordatorioEnviado = false
                    };
                    
                    // ✅ CORRECCIÓN: Navegar a ConfirmarSolicitud en lugar de CrearCita
                    ConfirmarSolicitud ventana = new ConfirmarSolicitud(idCita, citaModel);
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
                
                // ✅ CORRECCIÓN: Navegar a ConfirmarSolicitud pasando el ID de la cita a confirmar
                ConfirmarSolicitud ventana2 = new ConfirmarSolicitud(idCita, cita);
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

