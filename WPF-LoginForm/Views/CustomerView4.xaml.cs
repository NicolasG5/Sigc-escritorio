using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para CustomerView4.xaml
    /// </summary>
    public partial class CustomerView4 : UserControl
    {
        private readonly PsicologoApiService _empleadoService;
        private List<PsicologoModel> _todosEmpleados;

        public CustomerView4()
        {
            InitializeComponent();
            _empleadoService = new PsicologoApiService();
            CargarDatos();
        }

        private async void CargarDatos()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== CARGANDO EMPLEADOS ===");
                
                var empleados = await _empleadoService.GetAllPsicologosAsync();
                _todosEmpleados = empleados?.ToList() ?? new List<PsicologoModel>();
                
                System.Diagnostics.Debug.WriteLine($"Empleados cargados: {_todosEmpleados.Count}");
                
                GridDatos.ItemsSource = _todosEmpleados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar empleados: {ex.Message}");
                MessageBox.Show($"Error al cargar empleados:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método público para recargar datos desde CrudEmpleado
        /// </summary>
        public void CargarDatosPublic()
        {
            CargarDatos();
        }

        #region Buscar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_todosEmpleados == null || !_todosEmpleados.Any())
                    return;

                string busqueda = Buscar.Text?.ToLower() ?? "";

                if (string.IsNullOrWhiteSpace(busqueda))
                {
                    GridDatos.ItemsSource = _todosEmpleados;
                    return;
                }

                var filtrados = _todosEmpleados.Where(emp =>
                    (emp.Rut?.ToLower().Contains(busqueda) ?? false) ||
                    (emp.Nombres?.ToLower().Contains(busqueda) ?? false) ||
                    (emp.ApellidoPaterno?.ToLower().Contains(busqueda) ?? false) ||
                    (emp.ApellidoMaterno?.ToLower().Contains(busqueda) ?? false) ||
                    (emp.NombreCompleto?.ToLower().Contains(busqueda) ?? false) ||
                    (emp.EmailPersonal?.ToLower().Contains(busqueda) ?? false) ||
                    (emp.Telefono?.ToLower().Contains(busqueda) ?? false)
                ).ToList();

                GridDatos.ItemsSource = filtrados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en búsqueda: {ex.Message}");
            }
        }
        #endregion

        private void Agregar(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("✅ Navegando a formulario de creación de empleado");
                
                // Navegar al formulario de creación usando el Frame
                var crudEmpleado = new CrudEmpleado(); // Constructor sin parámetros = modo creación
                
                // Suscribirse al evento EmpleadoGuardado para cerrar el formulario
                crudEmpleado.EmpleadoGuardado += (s, args) =>
                {
                    System.Diagnostics.Debug.WriteLine("✅ Evento EmpleadoGuardado recibido - cerrando Frame");
                    FrameCustomerView4.Content = null;
                    CargarDatos(); // Recargar datos
                };
                
                FrameCustomerView4.Content = crudEmpleado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al navegar a creación: {ex.Message}");
                MessageBox.Show($"Error al abrir formulario:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter == null) return;

            int idEmpleado = (int)button.CommandParameter;
            var empleado = _todosEmpleados.FirstOrDefault(emp => emp.IdEmpleado == idEmpleado);

            if (empleado == null)
            {
                MessageBox.Show("Empleado no encontrado", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ Navegando a consulta de empleado: {empleado.NombreCompleto}");
                
                // Navegar al formulario de consulta usando el Frame
                var crudEmpleado = new CrudEmpleado(empleado, soloConsulta: true);
                
                // Suscribirse al evento EmpleadoGuardado para cerrar el formulario
                crudEmpleado.EmpleadoGuardado += (s, args) =>
                {
                    System.Diagnostics.Debug.WriteLine("✅ Evento EmpleadoGuardado recibido - cerrando Frame");
                    FrameCustomerView4.Content = null;
                };
                
                FrameCustomerView4.Content = crudEmpleado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al navegar a consulta: {ex.Message}");
                MessageBox.Show($"Error al abrir formulario:\n\n{ex.Message}\n\nMostrando datos en ventana emergente.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MostrarDatosEmpleado(empleado);
            }
        }

        // Método auxiliar para mostrar datos en MessageBox (fallback)
        private void MostrarDatosEmpleado(PsicologoModel empleado)
        {
            string mensaje = $"INFORMACION DEL EMPLEADO\n" +
                           $"{'=',50}\n\n" +
                           $"DATOS PERSONALES\n" +
                           $"{'-',50}\n" +
                           $"ID: {empleado.IdEmpleado}\n" +
                           $"RUT: {empleado.Rut}\n" +
                           $"Nombre: {empleado.NombreCompleto}\n" +
                           $"Fecha Nacimiento: {empleado.FechaNacimiento}\n\n" +
                           $"CONTACTO\n" +
                           $"{'-',50}\n" +
                           $"Email: {empleado.EmailPersonal}\n" +
                           $"Telefono: {empleado.Telefono}\n" +
                           $"Direccion: {empleado.Direccion}\n\n" +
                           $"INFORMACION PROFESIONAL\n" +
                           $"{'-',50}\n" +
                           $"Titulo: {empleado.TituloProfesional}\n" +
                           $"Universidad: {empleado.Universidad}\n" +
                           $"Registro: {empleado.RegistroProfesional}\n" +
                           $"Experiencia: {empleado.AniosExperiencia} anos\n" +
                           $"Rol: {empleado.RolEmpleado}\n" +
                           $"Estado: {empleado.Estado}\n" +
                           $"Fecha Registro: {empleado.FechaRegistro:dd/MM/yyyy}";

            MessageBox.Show(mensaje, $"Consulta Empleado: {empleado.Nombres}",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Método helper para buscar Frame en el árbol visual
        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter == null) return;

            int idEmpleado = (int)button.CommandParameter;
            var empleado = _todosEmpleados.FirstOrDefault(emp => emp.IdEmpleado == idEmpleado);

            if (empleado == null)
            {
                MessageBox.Show("Empleado no encontrado", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"✅ Navegando a edición de empleado: {empleado.NombreCompleto}");
                
                // Navegar al formulario de edición usando el Frame
                var crudEmpleado = new CrudEmpleado(empleado, soloConsulta: false);
                
                // Suscribirse al evento EmpleadoGuardado para cerrar el formulario y recargar
                crudEmpleado.EmpleadoGuardado += (s, args) =>
                {
                    System.Diagnostics.Debug.WriteLine("✅ Evento EmpleadoGuardado recibido - cerrando Frame");
                    FrameCustomerView4.Content = null;
                    CargarDatos(); // Recargar datos
                };
                
                FrameCustomerView4.Content = crudEmpleado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al navegar a edición: {ex.Message}");
                MessageBox.Show($"Error al abrir formulario:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Eliminar(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter == null) return;

            int idEmpleado = (int)button.CommandParameter;
            var empleado = _todosEmpleados.FirstOrDefault(emp => emp.IdEmpleado == idEmpleado);

            if (empleado == null)
            {
                MessageBox.Show("Empleado no encontrado", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Confirmación de eliminación
            var resultado = MessageBox.Show(
                $"Esta seguro de eliminar al empleado?\n\n" +
                $"Nombre: {empleado.NombreCompleto}\n" +
                $"RUT: {empleado.Rut}\n" +
                $"Rol: {empleado.RolEmpleado}\n\n" +
                $"Esta accion NO se puede deshacer.",
                "Confirmar Eliminacion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    bool eliminado = await _empleadoService.DeleteEmpleadoAsync(idEmpleado);
                    
                    if (eliminado)
                    {
                        MessageBox.Show($"Empleado '{empleado.NombreCompleto}' eliminado exitosamente.",
                            "Exito", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Recargar lista
                        CargarDatos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el empleado.\nVerifique que no tenga citas o atenciones asociadas.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar empleado:\n\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
