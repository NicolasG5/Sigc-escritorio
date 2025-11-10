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
    /// Lógica de interacción para GestionUsurarios.xaml
    /// </summary>
    public partial class GestionUsurarios : UserControl
    {
        private readonly UserManagementApiService _userService;
        private List<UserModel> _todosLosUsuarios;

        public GestionUsurarios()
        {
            InitializeComponent();
            _userService = new UserManagementApiService();
            CargarUsuarios();
        }

        private async void CargarUsuarios()
        {
            try
            {
                dgUsuarios.ItemsSource = null;
                
                var usuarios = await _userService.GetAllUsersAsync();
                _todosLosUsuarios = usuarios.ToList();
                
                dgUsuarios.ItemsSource = _todosLosUsuarios;
                
                if (!_todosLosUsuarios.Any())
                {
                    MessageBox.Show("No hay usuarios registrados en el sistema.",
                        "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[GestionUsuarios] Botón Agregar Usuario presionado");
                
                // Crear instancia del formulario de usuarios en modo creación
                var formularioUsuarios = new FormularioUsuarios();
                
                // Suscribirse al evento para recargar la lista cuando se guarde
                formularioUsuarios.UsuarioGuardado += (s, args) =>
                {
                    System.Diagnostics.Debug.WriteLine("[GestionUsuarios] Usuario guardado, volviendo a lista...");
                    
                    // Ocultar el Frame y mostrar el Grid principal
                    FrameGestionUsuarios.Visibility = Visibility.Collapsed;
                    GridCliente.Visibility = Visibility.Visible;
                    
                    // Recargar la lista de usuarios
                    CargarUsuarios();
                };
                
                // Ocultar el Grid principal y mostrar el Frame
                GridCliente.Visibility = Visibility.Collapsed;
                FrameGestionUsuarios.Visibility = Visibility.Visible;
                
                // Navegar al formulario
                FrameGestionUsuarios.Content = formularioUsuarios;
                
                System.Diagnostics.Debug.WriteLine("[GestionUsuarios] ✓ Formulario de usuario cargado en Frame");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error en BtnNuevoUsuario_Click: {ex.Message}");
                MessageBox.Show($"Error al abrir formulario de usuario:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter == null) return;

            // CommandParameter viene como string desde el Binding
            string idUsuarioStr = button.CommandParameter.ToString();
            var usuario = _todosLosUsuarios.FirstOrDefault(u => u.Id == idUsuarioStr);

            if (usuario == null)
            {
                MessageBox.Show("Usuario no encontrado", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ADVERTENCIA: El endpoint /api/v1/users/me solo permite editar el usuario actual
            MessageBox.Show(
                $"LIMITACION DE LA API:\n\n" +
                $"El endpoint /api/v1/users/me solo permite editar el usuario actual (tu propio usuario).\n\n" +
                $"No se puede editar el usuario: {usuario.Username}\n\n" +
                $"Necesitas un endpoint como PUT /api/v1/users/{usuario.Id} para editar otros usuarios.\n\n" +
                $"Contacta al desarrollador del backend para habilitar esta funcionalidad.",
                "Funcionalidad No Disponible", 
                MessageBoxButton.OK, 
                MessageBoxImage.Warning);
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.CommandParameter == null) return;

            // CommandParameter viene como string desde el Binding
            string idUsuarioStr = button.CommandParameter.ToString();
            var usuario = _todosLosUsuarios.FirstOrDefault(u => u.Id == idUsuarioStr);

            if (usuario == null)
            {
                MessageBox.Show("Usuario no encontrado", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // ADVERTENCIA: El endpoint /api/v1/users/me solo permite eliminar el usuario actual
            var resultado = MessageBox.Show(
                $"ADVERTENCIA IMPORTANTE:\n\n" +
                $"El endpoint DELETE /api/v1/users/me solo permite eliminar el usuario actual.\n\n" +
                $"Si eliminas al usuario '{usuario.Username}', se eliminara TU PROPIA CUENTA " +
                $"(el usuario con el que iniciaste sesion).\n\n" +
                $"Esta es una LIMITACION de la API actual.\n\n" +
                $"¿Deseas continuar y eliminar TU PROPIA CUENTA?",
                "ADVERTENCIA: Eliminar Usuario Actual",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    // Usar el método DeleteCurrentUserAsync ya que DELETE /api/v1/users/me elimina al usuario actual
                    bool eliminado = await _userService.DeleteCurrentUserAsync();

                    if (eliminado)
                    {
                        MessageBox.Show(
                            $"Tu cuenta ha sido eliminada exitosamente.\n\n" +
                            $"Seras redirigido a la pantalla de login.",
                            "Cuenta Eliminada", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Information);
                        
                        // Cerrar sesión y volver al login
                        var mainWindow = Window.GetWindow(this) as MainView;
                        if (mainWindow != null)
                        {
                            var loginView = new LoginView();
                            loginView.Show();
                            mainWindow.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "No se pudo eliminar el usuario.\n\n" +
                            "Verifica los logs para más detalles.",
                            "Error", 
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar usuario:\n\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            CargarUsuarios();
        }

        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_todosLosUsuarios == null || !_todosLosUsuarios.Any())
                    return;

                string textoBusqueda = txtBuscar.Text.ToLower().Trim();

                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    dgUsuarios.ItemsSource = _todosLosUsuarios;
                }
                else
                {
                    var usuariosFiltrados = _todosLosUsuarios.Where(u =>
                        (!string.IsNullOrEmpty(u.Username) && u.Username.ToLower().Contains(textoBusqueda)) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(textoBusqueda)) ||
                        (!string.IsNullOrEmpty(u.Estado) && u.Estado.ToLower().Contains(textoBusqueda))
                    ).ToList();

                    dgUsuarios.ItemsSource = usuariosFiltrados;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en busqueda: {ex.Message}");
            }
        }
    }
}
