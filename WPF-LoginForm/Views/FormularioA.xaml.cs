using System;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Formulario de Atención - Registra observación inicial del tratamiento
    /// </summary>
    public partial class FormularioA : Page
    {
        private readonly TratamientoApiService _tratamientoService;
        private readonly CitaApiService _citaService;
        private readonly PsicologoApiService _psicologoService;
        private readonly int _idCita;
        private readonly CitaPacientePresenteModel _cita;
        
        public event EventHandler AtencionGuardada;
        
        /// <summary>
        /// Constructor para nueva atención
        /// </summary>
        public FormularioA(int idCita, CitaPacientePresenteModel cita)
        {
            InitializeComponent();
            _tratamientoService = new TratamientoApiService();
            _citaService = new CitaApiService();
            _psicologoService = new PsicologoApiService();
            _idCita = idCita;
            _cita = cita;
            
            CargarDatosCita();
        }
        
        private void CargarDatosCita()
        {
            try
            {
                // Cargar información de la cita en el formulario
                if (_cita != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Formulario de atención cargado para cita {_idCita}");
                    System.Diagnostics.Debug.WriteLine($"Paciente: {_cita.NombrePaciente}");
                    System.Diagnostics.Debug.WriteLine($"RUT: {_cita.RutPaciente}");
                    System.Diagnostics.Debug.WriteLine($"Fecha Cita: {_cita.FechaCita:dd/MM/yyyy}");
                    System.Diagnostics.Debug.WriteLine($"Hora: {_cita.HoraCita}");
                    System.Diagnostics.Debug.WriteLine($"Motivo: {_cita.MotivoCita}");
                    
                    // Establecer fechas por defecto
                    dpFechaInicio.SelectedDate = DateTime.Now;
                    dpFechaFinEstimada.SelectedDate = DateTime.Now.AddMonths(3); // 3 meses desde hoy
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos de cita: {ex.Message}");
            }
        }
        
        private void Crear(object sender, RoutedEventArgs e)
        {
            // No implementado - este formulario solo guarda
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                "¿Está seguro de salir sin guardar?\n\n" +
                "Se perderán los cambios no guardados y el estado de la cita seguirá en 'En Curso'.",
                "Confirmar Salida",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                VolverAAtencion();
            }
        }
        
        #region buscar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // No implementado
        }
        #endregion
        
        private void Agregar(object sender, RoutedEventArgs e)
        {
            // No implementado
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            // No implementado
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            // No implementado
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            // No implementado
        }

        /// <summary>
        /// Guarda la observación inicial y completa la cita
        /// </summary>
        private async void Guardar(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== GUARDANDO ATENCIÓN CITA {_idCita} ===");
                
                // Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(txtTipoTratamiento.Text))
                {
                    MessageBox.Show("El campo 'Tipo de Tratamiento' es obligatorio.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtTipoTratamiento.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
                {
                    MessageBox.Show("El campo 'Descripción del Tratamiento' es obligatorio.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtDescripcion.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtObjetivos.Text))
                {
                    MessageBox.Show("El campo 'Objetivos del Tratamiento' es obligatorio.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtObjetivos.Focus();
                    return;
                }

                if (!dpFechaInicio.SelectedDate.HasValue)
                {
                    MessageBox.Show("Debe seleccionar una 'Fecha de Inicio'.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpFechaInicio.Focus();
                    return;
                }

                if (!dpFechaFinEstimada.SelectedDate.HasValue)
                {
                    MessageBox.Show("Debe seleccionar una 'Fecha Fin Estimada'.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpFechaFinEstimada.Focus();
                    return;
                }

                // Validar que fecha fin sea posterior a fecha inicio
                if (dpFechaFinEstimada.SelectedDate < dpFechaInicio.SelectedDate)
                {
                    MessageBox.Show("La 'Fecha Fin Estimada' debe ser posterior a la 'Fecha de Inicio'.",
                        "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpFechaFinEstimada.Focus();
                    return;
                }

                // VALIDACIÓN CRÍTICA: Verificar que el psicólogo esté asignado
                if (_cita.IdPsicologo == 0)
                {
                    MessageBox.Show(
                        "⚠️ ERROR: LA CITA NO TIENE PSICÓLOGO ASIGNADO\n\n" +
                        "No se puede crear el tratamiento sin un psicólogo asignado.\n\n" +
                        "SOLUCIÓN:\n" +
                        "1. Asigne un psicólogo a esta cita antes de atenderla\n" +
                        "2. Contacte al administrador del sistema",
                        "Psicólogo No Asignado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"Obteniendo información del psicólogo/empleado ID: {_cita.IdPsicologo}");
                
                // Obtener información del psicólogo para extraer el IdEmpleado
                var psicologo = await _psicologoService.GetPsicologoByIdAsync(_cita.IdPsicologo);
                
                if (psicologo == null)
                {
                    MessageBox.Show(
                        $"⚠️ ERROR: NO SE PUDO OBTENER INFORMACIÓN DEL PSICÓLOGO\n\n" +
                        $"ID Psicólogo/Empleado: {_cita.IdPsicologo}\n\n" +
                        $"POSIBLES CAUSAS:\n" +
                        $"1. El psicólogo/empleado no existe en el sistema\n" +
                        $"2. Error de conexión con el servidor\n" +
                        $"3. Token de autenticación inválido\n\n" +
                        $"Consulte la ventana Output (Debug) para más detalles.",
                        "Error al Obtener Psicólogo",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Psicólogo obtenido: {psicologo.NombreCompleto}");
                System.Diagnostics.Debug.WriteLine($"   ID Empleado: {psicologo.IdEmpleado}");
                
                var confirmacion = MessageBox.Show(
                    $"¿Guardar atención del paciente?\n\n" +
                    $"Paciente: {_cita?.NombrePaciente}\n" +
                    $"Psicólogo: {psicologo.NombreCompleto}\n" +
                    $"ID Empleado: {psicologo.IdEmpleado}\n" +
                    $"Cita ID: {_idCita}\n" +
                    $"Tipo Tratamiento: {txtTipoTratamiento.Text}\n" +
                    $"Fecha Inicio: {dpFechaInicio.SelectedDate:dd/MM/yyyy}\n" +
                    $"Fecha Fin Estimada: {dpFechaFinEstimada.SelectedDate:dd/MM/yyyy}\n\n" +
                    $"Al guardar, el estado de la cita cambiará a 'Completada'.",
                    "Confirmar Guardar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmacion != MessageBoxResult.Yes)
                    return;

                // Preparar observación inicial con datos del formulario
                var estadoSeleccionado = ((ComboBoxItem)cboEstado.SelectedItem).Tag.ToString();
                
                // Crear request con todos los datos del formulario
                var crearTratamientoRequest = new CrearTratamientoRequest
                {
                    TipoTratamiento = txtTipoTratamiento.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim(), // La observación inicial va en descripcion
                    Objetivos = txtObjetivos.Text.Trim(),
                    FechaInicio = dpFechaInicio.SelectedDate.Value.ToString("yyyy-MM-dd"),
                    FechaFinEstimada = dpFechaFinEstimada.SelectedDate.Value.ToString("yyyy-MM-dd"),
                    FechaFinReal = null, // No finalizado aún
                    Estado = estadoSeleccionado,
                    IdPaciente = _cita.IdPaciente,
                    IdEmpleado = psicologo.IdEmpleado, // ✅ CORRECCIÓN: Usar IdEmpleado del psicólogo obtenido
                    IdCita = _idCita
                };
                
                System.Diagnostics.Debug.WriteLine($"Datos de observación:");
                System.Diagnostics.Debug.WriteLine($"  TipoTratamiento: {txtTipoTratamiento.Text.Trim()}");
                System.Diagnostics.Debug.WriteLine($"  Descripcion: {txtDescripcion.Text.Trim().Substring(0, Math.Min(50, txtDescripcion.Text.Trim().Length))}...");
                System.Diagnostics.Debug.WriteLine($"  Objetivos: {txtObjetivos.Text.Trim().Substring(0, Math.Min(50, txtObjetivos.Text.Trim().Length))}...");
                System.Diagnostics.Debug.WriteLine($"  IdPaciente: {_cita.IdPaciente}");
                System.Diagnostics.Debug.WriteLine($"  IdEmpleado (Psicologo): {_cita.IdPsicologo}");
                System.Diagnostics.Debug.WriteLine($"  IdCita: {_idCita}");
                System.Diagnostics.Debug.WriteLine($"  FechaInicio: {dpFechaInicio.SelectedDate.Value:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"  FechaFinEstimada: {dpFechaFinEstimada.SelectedDate.Value:yyyy-MM-dd}");
                System.Diagnostics.Debug.WriteLine($"  Estado: {estadoSeleccionado}");
                System.Diagnostics.Debug.WriteLine($"Guardando observación inicial...");
                
                System.Diagnostics.Debug.WriteLine($"=== DATOS DEL TRATAMIENTO A ENVIAR ===");
                System.Diagnostics.Debug.WriteLine($"  TipoTratamiento: {crearTratamientoRequest.TipoTratamiento}");
                System.Diagnostics.Debug.WriteLine($"  Descripcion: {crearTratamientoRequest.Descripcion.Substring(0, Math.Min(50, crearTratamientoRequest.Descripcion.Length))}...");
                System.Diagnostics.Debug.WriteLine($"  Objetivos: {crearTratamientoRequest.Objetivos.Substring(0, Math.Min(50, crearTratamientoRequest.Objetivos.Length))}...");
                System.Diagnostics.Debug.WriteLine($"  IdPaciente: {crearTratamientoRequest.IdPaciente}");
                System.Diagnostics.Debug.WriteLine($"  IdEmpleado (Psicologo): {crearTratamientoRequest.IdEmpleado}");
                System.Diagnostics.Debug.WriteLine($"  IdCita: {crearTratamientoRequest.IdCita}");
                System.Diagnostics.Debug.WriteLine($"  FechaInicio: {crearTratamientoRequest.FechaInicio}");
                System.Diagnostics.Debug.WriteLine($"  FechaFinEstimada: {crearTratamientoRequest.FechaFinEstimada}");
                System.Diagnostics.Debug.WriteLine($"  Estado: {crearTratamientoRequest.Estado}");
                System.Diagnostics.Debug.WriteLine($"Creando tratamiento con observación inicial...");
                
                // 1. Crear tratamiento con observación inicial
                var tratamientoResponse = await _tratamientoService.CrearTratamientoConObservacionAsync(crearTratamientoRequest);
                
                if (tratamientoResponse == null)
                {
                    MessageBox.Show(
                        $"❌ ERROR AL CREAR TRATAMIENTO\n\n" +
                        $"No se pudo crear el tratamiento con la observación inicial.\n\n" +
                        $"DATOS ENVIADOS:\n" +
                        $"• ID Paciente: {crearTratamientoRequest.IdPaciente}\n" +
                        $"• ID Empleado (Psicólogo): {crearTratamientoRequest.IdEmpleado}\n" +
                        $"• ID Cita: {crearTratamientoRequest.IdCita}\n\n" +
                        $"POSIBLES CAUSAS:\n" +
                        $"1. El endpoint POST /api/v1/tratamientos/crear-con-observacion no existe\n" +
                        $"2. El ID del empleado ({crearTratamientoRequest.IdEmpleado}) no existe o no es válido\n" +
                        $"3. El ID del paciente ({crearTratamientoRequest.IdPaciente}) no existe\n" +
                        $"4. La cita ya tiene un tratamiento asociado\n" +
                        $"5. Error en el servidor backend\n\n" +
                        $"Consulte la ventana Output (Debug) para más detalles.\n\n" +
                        $"⚠️ LA CITA NO SE COMPLETARÁ HASTA QUE SE GUARDE EL TRATAMIENTO.",
                        "Error al Crear Tratamiento",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return; // ← IMPORTANTE: Detener aquí, NO completar la cita
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Tratamiento creado exitosamente - ID: {tratamientoResponse.IdTratamiento}");
                System.Diagnostics.Debug.WriteLine($"Completando cita...");
                
                // 2. Cambiar estado de cita a "Completada"
                bool citaCompletada = await _citaService.CompletarCitaAsync(_idCita);
                
                if (!citaCompletada)
                {
                    MessageBox.Show(
                        $"⚠️ ADVERTENCIA\n\n" +
                        $"La observación se guardó correctamente, pero no se pudo cambiar el estado de la cita a 'Completada'.\n\n" +
                        $"Deberá actualizar el estado manualmente.",
                        "Advertencia",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Cita completada exitosamente");
                }
                
                MessageBox.Show(
                    $"✅ ATENCIÓN GUARDADA EXITOSAMENTE\n\n" +
                    $"Paciente: {_cita?.NombrePaciente}\n" +
                    $"Cita ID: {_idCita}\n" +
                    $"Estado: Completada\n\n" +
                    $"Tratamiento Iniciado:\n" +
                    $"• ID Tratamiento: {tratamientoResponse.IdTratamiento}\n" +
                    $"• Tipo: {tratamientoResponse.TipoTratamiento}\n" +
                    $"• Fecha Inicio: {tratamientoResponse.FechaInicio}\n" +
                    $"• Estado: {tratamientoResponse.Estado}",
                    "Éxito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                
                // 3. Notificar que se guardó y volver a Atencion
                System.Diagnostics.Debug.WriteLine(">>> GUARDADO EXITOSO - VOLVIENDO A ATENCION");
                AtencionGuardada?.Invoke(this, EventArgs.Empty);

                // Intentar limpiar el Frame padre (mismo comportamiento que ControlSolicitudes/ConfirmarSolicitud flow)
                VolverAAtencion();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR en Guardar: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                MessageBox.Show(
                    $"❌ ERROR INESPERADO\n\n" +
                    $"Mensaje: {ex.Message}\n\n" +
                    $"Tipo: {ex.GetType().Name}",
                    "Error del Sistema",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // VolverAAtencion: busca Frame padre y limpia su Content, ocultando si existe
        private void VolverAAtencion()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(">>> VolverAAtencion() - buscando Frame padre...");
                var parent = this.Parent;
                while (parent != null && !(parent is Frame))
                {
                    parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
                }

                if (parent is Frame frame)
                {
                    System.Diagnostics.Debug.WriteLine(">>> Frame padre encontrado - limpiando Content y ocultando Frame si aplica");
                    frame.Content = null;

                    // Si el Frame está dentro de la vista Atencion y tiene nombre FrameAtencion, intentar ocultarlo
                    try
                    {
                        var possibleAtencion = System.Windows.Media.VisualTreeHelper.GetParent(frame);
                        // No reliable cast here; Atencion already maneja ocultar su Frame vía evento AtencionGuardada
                    }
                    catch { }

                    return;
                }

                // Fallback: reemplazar Content por nueva instancia (menos preferible)
                System.Diagnostics.Debug.WriteLine(">>> No se encontró Frame padre, usando fallback Content = new Atencion()");
                Content = new Atencion();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en VolverAAtencion: " + ex.Message);
            }
        }
    }
}
