using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para FormularioUsuarios.xaml
    /// </summary>
    public partial class FormularioUsuarios : Page
    {
        private readonly UserManagementApiService _userService;
        private UserModel _usuarioEditar;
        private bool _modoEdicion = false;

        // Evento para notificar cuando se crea/actualiza un usuario
        public event EventHandler UsuarioGuardado;

        /// <summary>
        /// Constructor para modo creacion
        /// </summary>
        public FormularioUsuarios()
        {
            InitializeComponent();
            _userService = new UserManagementApiService();
            _modoEdicion = false;
            txtTitulo.Text = "Crear Nuevo Usuario";
            btnGuardar.Content = "Guardar Usuario";
        }

        /// <summary>
        /// Constructor para modo edicion
        /// </summary>
        public FormularioUsuarios(UserModel usuario)
        {
            InitializeComponent();
            _userService = new UserManagementApiService();
            _usuarioEditar = usuario;
            _modoEdicion = true;
            
            txtTitulo.Text = "Editar Usuario";
            btnGuardar.Content = "Actualizar Usuario";
            
            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            if (_usuarioEditar == null) return;

            txtNombreUsuario.Text = _usuarioEditar.Username;
            txtEmail.Text = _usuarioEditar.Email;
            
            // Seleccionar estado
            if (_usuarioEditar.Estado == "activo")
                cboEstado.SelectedIndex = 0;
            else
                cboEstado.SelectedIndex = 1;

            // En modo edicion, la contrasena es opcional
            lblContrasena.Text = "Nueva Contrasena (dejar vacio para mantener actual)";
            lblConfirmarContrasena.Text = "Confirmar Nueva Contrasena";
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar campos
                if (!ValidarCampos())
                    return;

                if (_modoEdicion)
                {
                    await ActualizarUsuario();
                }
                else
                {
                    await CrearNuevoUsuario();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar usuario:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task CrearNuevoUsuario()
        {
            try
            {
                var nuevoUsuario = new CreateUserModel
                {
                    NombreUsuario = txtNombreUsuario.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Contrasena = txtContrasena.Password,
                    Estado = ((ComboBoxItem)cboEstado.SelectedItem).Tag.ToString(),
                    IntentosFallidos = 0
                };

                System.Diagnostics.Debug.WriteLine($"[FormularioUsuarios] Creando usuario via SIGNUP: {nuevoUsuario.NombreUsuario}");
                System.Diagnostics.Debug.WriteLine($"Email: {nuevoUsuario.Email}, Estado: {nuevoUsuario.Estado}");

                // ✅ USAR SIGNUP EN LUGAR DE CREATE
                // SignupUserAsync NO requiere autenticación
                var usuarioCreado = await _userService.SignupUserAsync(nuevoUsuario);

                if (usuarioCreado != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Usuario creado exitosamente: {usuarioCreado.Username} (ID: {usuarioCreado.Id})");
                    
                    MessageBox.Show(
                        $"Usuario creado exitosamente\n\n" +
                        $"Usuario: {usuarioCreado.Username}\n" +
                        $"Email: {usuarioCreado.Email}\n" +
                        $"ID: {usuarioCreado.Id}\n" +
                        $"Estado: {nuevoUsuario.Estado}\n\n" +
                        $"El usuario ya puede iniciar sesion en el sistema.",
                        "Exito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Notificar que se creo el usuario
                    UsuarioGuardado?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ SignupUserAsync retorno NULL");
                    
                    MessageBox.Show(
                        "No se pudo crear el usuario\n\n" +
                        "Posibles causas:\n" +
                        "• El email ya esta registrado en el sistema\n" +
                        "• El nombre de usuario ya existe\n" +
                        "• Error de conexion con el servidor\n" +
                        "• El servidor devolvio un error 500\n\n" +
                        "Intente con datos diferentes o contacte al administrador.",
                        "Error al Crear Usuario",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Exception en CrearNuevoUsuario: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                MessageBox.Show(
                    $"Error inesperado al crear usuario:\n\n{ex.Message}\n\n" +
                    $"Verifique su conexion a Internet y que el servidor este funcionando correctamente.",
                    "Error del Sistema",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task ActualizarUsuario()
        {
            // IMPORTANTE: El endpoint PATCH /api/v1/users/me solo actualiza el usuario actual
            // No puede actualizar usuarios arbitrarios por ID
            
            MessageBox.Show(
                "LIMITACION DE LA API:\n\n" +
                "El endpoint PATCH /api/v1/users/me solo permite actualizar TU PROPIO usuario (el que inicio sesion).\n\n" +
                "No se puede actualizar el usuario seleccionado.\n\n" +
                "Necesitas un endpoint administrativo como PATCH /api/v1/users/{id} en el backend.",
                "Funcionalidad No Disponible",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            
            // Si aún quieres actualizar el usuario actual (el que inició sesión):
            /*
            var usuarioActualizado = new UpdateUserModel
            {
                NombreUsuario = txtNombreUsuario.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Estado = ((ComboBoxItem)cboEstado.SelectedItem).Tag.ToString(),
                IntentosFallidos = 0
            };

            // Solo incluir contrasena si se ingreso una nueva
            if (!string.IsNullOrEmpty(txtContrasena.Password))
            {
                usuarioActualizado.Contrasena = txtContrasena.Password;
            }

            System.Diagnostics.Debug.WriteLine($"[FormularioUsuarios] Actualizando usuario actual");

            bool actualizado = await _userService.UpdateCurrentUserAsync(usuarioActualizado);

            if (actualizado)
            {
                MessageBox.Show(
                    $"Usuario actualizado exitosamente\n\n" +
                    $"Usuario: {usuarioActualizado.NombreUsuario}\n" +
                    $"Email: {usuarioActualizado.Email}\n" +
                    $"Estado: {usuarioActualizado.Estado}",
                    "Exito",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Notificar que se actualizo el usuario
                UsuarioGuardado?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show(
                    "No se pudo actualizar el usuario\n\n" +
                    "Intente nuevamente o contacte al administrador.",
                    "Error al Actualizar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            */
        }

        private bool ValidarCampos()
        {
            // Validar nombre de usuario
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
            {
                MessageBox.Show("El nombre de usuario es obligatorio",
                    "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombreUsuario.Focus();
                return false;
            }

            if (txtNombreUsuario.Text.Contains(" "))
            {
                MessageBox.Show("El nombre de usuario no puede contener espacios",
                    "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNombreUsuario.Focus();
                return false;
            }

            // Validar email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("El email es obligatorio",
                    "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            if (!ValidarEmail(txtEmail.Text))
            {
                MessageBox.Show("El formato del email no es valido\n\n" +
                    "Ejemplo valido: usuario@ejemplo.com",
                    "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validar contrasena (solo en modo creacion o si se ingreso en modo edicion)
            if (!_modoEdicion || !string.IsNullOrEmpty(txtContrasena.Password))
            {
                if (string.IsNullOrEmpty(txtContrasena.Password))
                {
                    MessageBox.Show("La contrasena es obligatoria",
                        "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtContrasena.Focus();
                    return false;
                }

                if (txtContrasena.Password.Length < 8)
                {
                    MessageBox.Show("La contrasena debe tener al menos 8 caracteres",
                        "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtContrasena.Focus();
                    return false;
                }

                if (txtContrasena.Password != txtConfirmarContrasena.Password)
                {
                    MessageBox.Show("Las contrasenas no coinciden\n\n" +
                        "Verifique que ambos campos sean identicos.",
                        "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtConfirmarContrasena.Focus();
                    return false;
                }
            }

            // Validar estado
            if (cboEstado.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un estado",
                    "Validacion", MessageBoxButton.OK, MessageBoxImage.Warning);
                cboEstado.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarEmail(string email)
        {
            try
            {
                // Expresion regular para validar email
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            bool hayCambios = !string.IsNullOrWhiteSpace(txtNombreUsuario.Text) ||
                             !string.IsNullOrWhiteSpace(txtEmail.Text) ||
                             !string.IsNullOrEmpty(txtContrasena.Password);

            if (hayCambios)
            {
                var resultado = MessageBox.Show(
                    "Esta seguro de salir sin guardar?\n\nSe perderan los cambios no guardados.",
                    "Confirmar Salida",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            // Notificar que se cerró el formulario
            UsuarioGuardado?.Invoke(this, EventArgs.Empty);
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            BtnVolver_Click(sender, e);
        }
    }
}
