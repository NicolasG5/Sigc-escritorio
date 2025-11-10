using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Módulo de Atención - Gestiona citas con paciente presente
    /// </summary>
    public partial class Atencion : UserControl
    {
        private readonly CitaApiService _citaService;
        private readonly PacienteApiService _pacienteService;
        private List<CitaPacientePresenteModel> _todasLasCitas;
        
        public Atencion()
        {
            InitializeComponent();
            _citaService = new CitaApiService();
            _pacienteService = new PacienteApiService();
            CargarCitasPacientePresente();
        }
        
        private async void CargarCitasPacientePresente()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== CARGANDO CITAS CON PACIENTE PRESENTE ===");
                
                GridDatos.ItemsSource = null;
                
                // Obtener citas con paciente presente (IdEstadoCita = 9)
                var citasPacientePresente = await _citaService.GetCitasPacientePresenteAsync();
                
                System.Diagnostics.Debug.WriteLine($"Citas con paciente presente obtenidas: {citasPacientePresente.Count()}");
                
                // Crear lista extendida con información del paciente
                _todasLasCitas = new List<CitaPacientePresenteModel>();
                
                foreach (var cita in citasPacientePresente)
                {
                    System.Diagnostics.Debug.WriteLine($"Procesando cita ID: {cita.IdCita}, Paciente ID: {cita.IdPaciente}");
                    
                    // Obtener información del paciente
                    var paciente = await _pacienteService.GetPacienteByIdAsync(cita.IdPaciente);
                    
                    if (paciente != null)
                    {
                        var citaPacientePresente = new CitaPacientePresenteModel
                        {
                            IdCita = cita.IdCita,
                            IdPaciente = cita.IdPaciente,
                            IdPsicologo = cita.IdPsicologo,
                            FechaCita = DateTime.TryParse(cita.FechaCita, out DateTime fecha) ? fecha : (DateTime?)null,
                            HoraCita = $"{cita.HoraInicio} - {cita.HoraFin}",
                            EstadoCita = "Paciente Presente",
                            MotivoCita = cita.MotivoConsulta ?? "Sin motivo especificado",
                            CodigoConfirmacion = cita.CodigoConfirmacion,
                            
                            // Información del paciente
                            NombrePaciente = paciente.NombreCompleto,
                            RutPaciente = paciente.Rut,
                            EmailPaciente = paciente.Email,
                            TelefonoPaciente = paciente.Telefono
                        };
                        
                        _todasLasCitas.Add(citaPacientePresente);
                        
                        System.Diagnostics.Debug.WriteLine($"  → Cita agregada: {paciente.NombreCompleto} - {citaPacientePresente.FechaCita}");
                    }
                }
                
                GridDatos.ItemsSource = _todasLasCitas;
                
                System.Diagnostics.Debug.WriteLine($"✓ Total citas con paciente presente: {_todasLasCitas.Count}");
                
                if (!_todasLasCitas.Any())
                {
                    MessageBox.Show("No hay citas con paciente presente en este momento.\n\n" +
                        "Las citas deben estar marcadas como 'Paciente Presente' en el módulo de Citas.",
                        "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al cargar citas: {ex.Message}");
                MessageBox.Show($"Error al cargar citas con paciente presente:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        #region buscar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_todasLasCitas == null || !_todasLasCitas.Any())
                    return;

                string textoBusqueda = Buscar.Text.ToLower().Trim();

                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    GridDatos.ItemsSource = _todasLasCitas;
                }
                else
                {
                    var citasFiltradas = _todasLasCitas.Where(c =>
                        (c.NombrePaciente != null && c.NombrePaciente.ToLower().Contains(textoBusqueda)) ||
                        (c.RutPaciente != null && c.RutPaciente.ToLower().Contains(textoBusqueda)) ||
                        (c.MotivoCita != null && c.MotivoCita.ToLower().Contains(textoBusqueda)) ||
                        (c.FechaCita != null && c.FechaCita.ToString().Contains(textoBusqueda))
                    ).ToList();

                    GridDatos.ItemsSource = citasFiltradas;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en búsqueda: {ex.Message}");
            }
        }
        #endregion
        
        private void Agregar(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Para crear una nueva cita, vaya al módulo 'Gestión de Citas'.",
                "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Inicia la atención de un paciente
        /// 1. Cambia estado de cita a "En Curso"
        /// 2. Abre formulario de atención
        /// </summary>
        private async void Consultar(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.CommandParameter == null)
                {
                    MessageBox.Show("No se pudo identificar la cita", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Obtener el IdCita desde el CommandParameter
                int idCita = (int)button.CommandParameter;
                
                var cita = _todasLasCitas.FirstOrDefault(c => c.IdCita == idCita);
                
                if (cita == null)
                {
                    MessageBox.Show("Cita no encontrada", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var resultado = MessageBox.Show(
                    $"¿Iniciar atención del paciente?\n\n" +
                    $"Paciente: {cita.NombrePaciente}\n" +
                    $"RUT: {cita.RutPaciente}\n" +
                    $"Fecha: {cita.FechaCita:dd/MM/yyyy}\n" +
                    $"Hora: {cita.HoraCita}\n" +
                    $"Motivo: {cita.MotivoCita}\n\n" +
                    $"El estado de la cita cambiará a 'En Curso'.",
                    "Iniciar Atención",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Debug.WriteLine($"=== INICIANDO ATENCIÓN CITA {idCita} ===");
                    
                    // Cambiar estado a "En Curso"
                    bool success = await _citaService.IniciarAtencionAsync(idCita);
                    
                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Estado cambiado a 'En Curso', abriendo formulario de atención...");
                        
                        // Abrir formulario de atención
                        FormularioA formularioAtencion = new FormularioA(idCita, cita);

                        // Suscribirse al evento de guardado para recargar la lista y limpiar el frame
                        formularioAtencion.AtencionGuardada += (s, args) =>
                        {
                            System.Diagnostics.Debug.WriteLine("Atención guardada, recargando lista desde Atencion...");
                            FrameAtencion.Content = null;
                            FrameAtencion.Visibility = Visibility.Collapsed; // ocultar frame cuando se cierre el formulario
                            CargarCitasPacientePresente();
                        };

                        // Mostrar el frame y cargar el formulario
                        FrameAtencion.Visibility = Visibility.Visible;
                        FrameAtencion.Content = formularioAtencion;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ IniciarAtencionAsync retornó false");
                        
                        MessageBox.Show(
                            $"❌ ERROR AL INICIAR ATENCIÓN\n\n" +
                            $"No se pudo cambiar el estado de la cita a 'En Curso'.\n\n" +
                            $"POSIBLES CAUSAS:\n\n" +
                            $"1. El endpoint PUT /api/v1/citas/{idCita}/iniciar-atencion no existe\n" +
                            $"2. El estado de la cita no es válido\n" +
                            $"3. Error en el servidor backend\n\n" +
                            $"Consulte la ventana Output (Debug) para más detalles.",
                            "Error del Servidor",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR en Consultar: {ex.Message}");
                MessageBox.Show($"Error al iniciar atención:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            // No implementado - no se pueden actualizar citas en atención
            MessageBox.Show("No se pueden modificar citas en atención.\n\n" +
                "Use el módulo 'Gestión de Citas' para editar citas.",
                "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            // No implementado - no se pueden eliminar citas en atención
            MessageBox.Show("No se pueden eliminar citas en atención.\n\n" +
                "Use el módulo 'Gestión de Citas' para cancelar citas.",
                "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Recarga la lista de citas (llamado desde FormularioA después de guardar)
        /// </summary>
        public void RecargarLista()
        {
            FrameAtencion.Content = null;
            CargarCitasPacientePresente();
        }
    }
}
