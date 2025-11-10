using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    public partial class Cita : UserControl
    {
        private readonly CitaApiService _citaService;
        private readonly PacienteApiService _pacienteService;
        private List<CitaConfirmadaModel> _todasLasCitas;
        
        public Cita()
        {
            InitializeComponent();
            _citaService = new CitaApiService();
            _pacienteService = new PacienteApiService();
            CargarCitasConfirmadas();
        }
        
        private async void CargarCitasConfirmadas()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== CARGANDO CITAS CONFIRMADAS ===");
                
                GridDatos.ItemsSource = null;
                
                // Obtener solo citas confirmadas
                var citasConfirmadas = await _citaService.GetCitasConfirmadasAsync();
                
                System.Diagnostics.Debug.WriteLine($"Citas confirmadas obtenidas: {citasConfirmadas.Count()}");
                
                // Crear lista extendida con información del paciente
                _todasLasCitas = new List<CitaConfirmadaModel>();
                
                foreach (var cita in citasConfirmadas)
                {
                    System.Diagnostics.Debug.WriteLine($"Procesando cita ID: {cita.IdCita}, Paciente ID: {cita.IdPaciente}");
                    
                    // Obtener información del paciente
                    var paciente = await _pacienteService.GetPacienteByIdAsync(cita.IdPaciente);
                    
                    if (paciente != null)
                    {
                        var citaConfirmada = new CitaConfirmadaModel
                        {
                            IdCita = cita.IdCita,
                            IdPaciente = cita.IdPaciente,
                            IdPsicologo = cita.IdPsicologo,
                            FechaCita = DateTime.TryParse(cita.FechaCita, out DateTime fecha) ? fecha : (DateTime?)null,
                            HoraCita = $"{cita.HoraInicio} - {cita.HoraFin}",
                            EstadoCita = "Confirmada", // Las citas filtradas son confirmadas
                            MotivoCita = cita.MotivoConsulta ?? "Sin motivo especificado",
                            CodigoConfirmacion = cita.CodigoConfirmacion,
                            
                            // Información del paciente
                            NombrePaciente = paciente.NombreCompleto,
                            RutPaciente = paciente.Rut,
                            EmailPaciente = paciente.Email,
                            TelefonoPaciente = paciente.Telefono
                        };
                        
                        _todasLasCitas.Add(citaConfirmada);
                        
                        System.Diagnostics.Debug.WriteLine($"  → Cita agregada: {paciente.NombreCompleto} - {citaConfirmada.FechaCita}");
                    }
                }
                
                GridDatos.ItemsSource = _todasLasCitas;
                
                System.Diagnostics.Debug.WriteLine($"✓ Total citas confirmadas mostradas: {_todasLasCitas.Count}");
                
                if (!_todasLasCitas.Any())
                {
                    MessageBox.Show("No hay citas confirmadas en este momento.",
                        "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al cargar citas: {ex.Message}");
                MessageBox.Show($"Error al cargar citas confirmadas:\n\n{ex.Message}",
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
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
            FrameCustomerView5.Content = ventana;
        }

        private async void Presente(object sender, RoutedEventArgs e)
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
                    $"¿Marcar como PRESENTE al paciente?\n\n" +
                    $"Paciente: {cita.NombrePaciente}\n" +
                    $"RUT: {cita.RutPaciente}\n" +
                    $"Fecha: {cita.FechaCita:dd/MM/yyyy}\n" +
                    $"Hora: {cita.HoraCita}\n" +
                    $"ID Cita: {idCita}\n\n" +
                    $"El estado de la cita cambiará a 'Paciente Presente'.",
                    "Confirmar Asistencia",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Debug.WriteLine($"=== MARCANDO PRESENTE CITA {idCita} ===");
                    System.Diagnostics.Debug.WriteLine($"Paciente: {cita.NombrePaciente}");
                    System.Diagnostics.Debug.WriteLine($"Fecha: {cita.FechaCita:dd/MM/yyyy}");
                    
                    bool success = await _citaService.MarcarPresenteAsync(idCita);
                    
                    if (success)
                    {
                        MessageBox.Show(
                            $"✅ Paciente marcado como PRESENTE\n\n" +
                            $"{cita.NombrePaciente}\n" +
                            $"Cita ID: {idCita}\n" +
                            $"Fecha: {cita.FechaCita:dd/MM/yyyy}\n" +
                            $"Hora: {cita.HoraCita}",
                            "Éxito",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        
                        // Recargar la lista
                        CargarCitasConfirmadas();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("❌ MarcarPresenteAsync retornó false");
                        
                        MessageBox.Show(
                            $"❌ ERROR AL MARCAR PRESENCIA\n\n" +
                            $"Paciente: {cita.NombrePaciente}\n" +
                            $"Cita ID: {idCita}\n\n" +
                            $"POSIBLES CAUSAS:\n\n" +
                            $"1. El endpoint PUT /api/v1/citas/{idCita}/marcar-presente\n" +
                            $"   devolvió un error 500 (Internal Server Error)\n\n" +
                            $"2. El backend no puede actualizar el estado de la cita\n" +
                            $"   - Verifique que el servicio esté disponible\n\n" +
                            $"   - Confirme que el ID de la cita sea correcto\n\n" +
                            $"3. El estado 'paciente_presente' no existe en la BD\n" +
                            $"   - Asegúrese de que los estados de cita estén sincronizados\n\n" +
                            $"   - Consulte al administrador de la base de datos\n\n" +
                            $"4. La cita no está en estado 'Confirmada'\n" +
                            $"   - Revise el estado actual de la cita\n\n" +
                            $"SOLUCIÓN:\n" +
                            $"- Revise los logs del servidor backend\n" +
                            $"- Verifique que el endpoint esté implementado\n" +
                            $"- Confirme que los estados de cita existan en la BD\n\n" +
                            $"Consulte la ventana Output (Debug) para más detalles.",
                            "Error del Servidor (500)",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ EXCEPTION en Presente:");
                System.Diagnostics.Debug.WriteLine($"Mensaje: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                MessageBox.Show(
                    $"❌ ERROR INESPERADO\n\n" +
                    $"Mensaje: {ex.Message}\n\n" +
                    $"Tipo: {ex.GetType().Name}\n\n" +
                    $"StackTrace:\n{ex.StackTrace}",
                    "Error del Sistema",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Funcionalidad de actualizar cita en desarrollo.",
                "En Desarrollo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void Eliminar(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button?.CommandParameter == null) return;

                int idCita = (int)button.CommandParameter;
                var cita = _todasLasCitas.FirstOrDefault(c => c.IdCita == idCita);

                if (cita == null)
                {
                    MessageBox.Show("Cita no encontrada", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar esta cita?\n\n" +
                    $"Paciente: {cita.NombrePaciente}\n" +
                    $"Fecha: {cita.FechaCita:dd/MM/yyyy}\n" +
                    $"Hora: {cita.HoraCita}\n\n" +
                    $"Esta acción NO se puede deshacer.",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    bool eliminado = await _citaService.DeleteCitaAsync(idCita);
                    
                    if (eliminado)
                    {
                        MessageBox.Show("Cita eliminada exitosamente",
                            "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        CargarCitasConfirmadas();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar la cita",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar cita:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
