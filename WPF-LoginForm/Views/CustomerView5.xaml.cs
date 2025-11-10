using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    public partial class CustomerView5 : UserControl
    {
        private readonly PacienteApiService _pacienteService;
        private List<PacienteModel> _todosPacientes;

        public CustomerView5()
        {
            InitializeComponent();
            _pacienteService = new PacienteApiService();
            CargarDatos();
        }

        private async void CargarDatos()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== CARGANDO PACIENTES ===");
                
                var pacientes = await _pacienteService.GetAllPacientesAsync();
                _todosPacientes = pacientes?.ToList() ?? new List<PacienteModel>();
                
                System.Diagnostics.Debug.WriteLine($"Pacientes cargados: {_todosPacientes.Count}");
                
                GridDatos.ItemsSource = _todosPacientes;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar pacientes: {ex.Message}");
                MessageBox.Show($"Error al cargar pacientes:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_todosPacientes == null || !_todosPacientes.Any())
                    return;

                string busqueda = Buscar.Text?.ToLower() ?? "";

                if (string.IsNullOrWhiteSpace(busqueda))
                {
                    GridDatos.ItemsSource = _todosPacientes;
                    return;
                }

                var filtrados = _todosPacientes.Where(p =>
                    (p.Rut?.ToLower().Contains(busqueda) ?? false) ||
                    (p.NombreCompleto?.ToLower().Contains(busqueda) ?? false) ||
                    (p.Email?.ToLower().Contains(busqueda) ?? false) ||
                    (p.Telefono?.ToLower().Contains(busqueda) ?? false)
                ).ToList();

                GridDatos.ItemsSource = filtrados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en búsqueda: {ex.Message}");
            }
        }

        private void Agregar(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(">>> Abriendo formulario CREAR paciente");
            
            // Crear formulario en modo crear
            var formulario = new Paciente();
            
            // Suscribirse al evento para cerrar y recargar
            formulario.PacienteGuardado += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine("? Evento PacienteGuardado recibido - cerrando formulario");
                FrameCustomerView5.Content = null;
                CargarDatos();
            };
            
            // Mostrar formulario en el Frame
            FrameCustomerView5.Content = formulario;
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter == null) return;

            int idPaciente = (int)button.CommandParameter;
            var paciente = _todosPacientes.FirstOrDefault(p => p.IdPaciente == idPaciente);

            if (paciente == null)
            {
                MessageBox.Show("Paciente no encontrado", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            System.Diagnostics.Debug.WriteLine($">>> Abriendo formulario EDITAR (consulta) paciente: {paciente.NombreCompleto}");
            
            // Crear formulario en modo editar (consulta es como editar pero sin guardar)
            var formulario = new Paciente(paciente);
            
            // Suscribirse al evento para cerrar
            formulario.PacienteGuardado += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine("? Cerrando consulta de paciente");
                FrameCustomerView5.Content = null;
            };
            
            // Mostrar formulario en el Frame
            FrameCustomerView5.Content = formulario;
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter == null) return;

            int idPaciente = (int)button.CommandParameter;
            var paciente = _todosPacientes.FirstOrDefault(p => p.IdPaciente == idPaciente);

            if (paciente == null)
            {
                MessageBox.Show("Paciente no encontrado", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            System.Diagnostics.Debug.WriteLine($">>> Abriendo formulario EDITAR paciente: {paciente.NombreCompleto}");
            
            // Crear formulario en modo editar
            var formulario = new Paciente(paciente);
            
            // Suscribirse al evento para cerrar y recargar
            formulario.PacienteGuardado += (s, args) =>
            {
                System.Diagnostics.Debug.WriteLine("? Evento PacienteGuardado recibido - cerrando formulario y recargando");
                FrameCustomerView5.Content = null;
                CargarDatos();
            };
            
            // Mostrar formulario en el Frame
            FrameCustomerView5.Content = formulario;
        }

        private async void Eliminar(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter == null) return;

            int idPaciente = (int)button.CommandParameter;
            var paciente = _todosPacientes.FirstOrDefault(p => p.IdPaciente == idPaciente);

            if (paciente == null)
            {
                MessageBox.Show("Paciente no encontrado", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var resultado = MessageBox.Show(
                $"¿Está seguro de eliminar al paciente?\n\n" +
                $"{paciente.NombreCompleto}\n" +
                $"RUT: {paciente.Rut}\n\n" +
                $"Esta acción no se puede deshacer.",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine($"Eliminando paciente ID: {idPaciente}");
                    
                    bool eliminado = await _pacienteService.DeletePacienteAsync(idPaciente);

                    if (eliminado)
                    {
                        MessageBox.Show(
                            $"Paciente eliminado exitosamente\n\n{paciente.NombreCompleto}",
                            "Éxito",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Recargar lista de pacientes
                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show(
                            "No se pudo eliminar el paciente.\n\nIntente nuevamente.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error al eliminar paciente:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }
}
