using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    public partial class CrudEmpleado : Page
    {
        private readonly PsicologoApiService _empleadoService;
        private readonly UserManagementApiService _userService;
        private PsicologoModel _empleado;
        private bool _soloConsulta;
        private bool _modoEdicion = false;
        private List<UserModel> _usuariosDisponibles;

        // Evento para notificar cuando se cierra el formulario
        public event EventHandler EmpleadoGuardado;

        public CrudEmpleado()
        {
            InitializeComponent();
            _empleadoService = new PsicologoApiService();
            _userService = new UserManagementApiService();
            _modoEdicion = false;
            _soloConsulta = false;
            ConfigurarModoCreacion();
            CargarUsuariosDisponibles();
        }

        public CrudEmpleado(PsicologoModel empleado, bool soloConsulta)
        {
            InitializeComponent();
            _empleadoService = new PsicologoApiService();
            _userService = new UserManagementApiService();
            _empleado = empleado;
            _soloConsulta = soloConsulta;
            _modoEdicion = !soloConsulta;
            
            if (soloConsulta)
            {
                ConfigurarModoConsulta();
            }
            else
            {
                ConfigurarModoEdicion();
            }
            
            CargarUsuariosDisponibles();
            CargarDatosEmpleado();
        }

        private async void CargarUsuariosDisponibles()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(">>> Cargando usuarios disponibles...");
                txtInfoUsuario.Text = "⏳ Cargando usuarios disponibles...";
                txtInfoUsuario.Foreground = new SolidColorBrush(Colors.Gray);
                
                // Obtener TODOS los usuarios del sistema
                var todosUsuarios = await _userService.GetAllUsersAsync();
                System.Diagnostics.Debug.WriteLine($"Total usuarios en sistema: {todosUsuarios.Count()}");
                
                // Obtener TODOS los empleados actuales
                var todosEmpleados = await _empleadoService.GetAllPsicologosAsync();
                System.Diagnostics.Debug.WriteLine($"Total empleados en sistema: {todosEmpleados.Count()}");
                
                // IDs de usuarios que YA están asignados a empleados
                var usuariosAsignados = todosEmpleados
                    .Where(e => e.IdUsuario > 0)
                    .Select(e => e.IdUsuario)
                    .ToHashSet();
                
                System.Diagnostics.Debug.WriteLine($"Usuarios ya asignados: {string.Join(", ", usuariosAsignados)}");
                
                // Filtrar solo usuarios NO asignados
                _usuariosDisponibles = todosUsuarios
                    .Where(u => {
                        int userId;
                        if (int.TryParse(u.Id, out userId))
                        {
                            bool estaAsignado = usuariosAsignados.Contains(userId);
                            System.Diagnostics.Debug.WriteLine($"Usuario {u.Username} (ID: {userId}) - Asignado: {estaAsignado}");
                            return !estaAsignado;
                        }
                        return false;
                    })
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine($"Usuarios disponibles encontrados: {_usuariosDisponibles.Count}");
                
                // Crear lista para el ComboBox con información completa
                var usuariosParaComboBox = _usuariosDisponibles.Select(u => new
                {
                    Id = u.Id,
                    DisplayUsuario = $"{u.Username} ({u.Email}) - {u.Name} {u.LastName}",
                    Username = u.Username,
                    Email = u.Email,
                    NombreCompleto = $"{u.Name} {u.LastName}"
                }).ToList();
                
                cbUsuario.ItemsSource = usuariosParaComboBox;
                cbUsuario.DisplayMemberPath = "DisplayUsuario";
                cbUsuario.SelectedValuePath = "Id";
                
                if (_usuariosDisponibles.Count == 0)
                {
                    txtInfoUsuario.Text = "⚠️ No hay usuarios disponibles. Debe crear usuarios en 'Gestión de Usuarios' primero.";
                    txtInfoUsuario.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    cbUsuario.IsEnabled = false;
                    
                    System.Diagnostics.Debug.WriteLine("⚠️ No hay usuarios disponibles para asignar");
                }
                else
                {
                    txtInfoUsuario.Text = $"✓ {_usuariosDisponibles.Count} usuario(s) disponible(s) para asignar";
                    txtInfoUsuario.Foreground = new SolidColorBrush(Colors.Green);
                    cbUsuario.IsEnabled = true;
                    
                    // Si estamos en modo edición y el empleado tiene un usuario asignado
                    if (_modoEdicion && _empleado != null && _empleado.IdUsuario > 0)
                    {
                        // Buscar el usuario en la lista (puede no estar disponible porque ya está asignado)
                        var usuarioEmpleado = todosUsuarios.FirstOrDefault(u => u.Id == _empleado.IdUsuario.ToString());
                        
                        if (usuarioEmpleado != null)
                        {
                            // Agregar el usuario actual a la lista aunque esté asignado (para mostrarlo)
                            var usuarioActual = new
                            {
                                Id = usuarioEmpleado.Id,
                                DisplayUsuario = $"{usuarioEmpleado.Username} ({usuarioEmpleado.Email}) - ACTUAL",
                                Username = usuarioEmpleado.Username,
                                Email = usuarioEmpleado.Email,
                                NombreCompleto = $"{usuarioEmpleado.Name} {usuarioEmpleado.LastName}"
                            };
                            
                            // Agregar a la lista del ComboBox
                            var listaConActual = usuariosParaComboBox.ToList();
                            listaConActual.Insert(0, usuarioActual);
                            cbUsuario.ItemsSource = listaConActual;
                            
                            // Seleccionar el usuario actual
                            cbUsuario.SelectedValue = _empleado.IdUsuario.ToString();
                            txtInfoUsuario.Text = $"✓ Usuario asignado actualmente: {usuarioEmpleado.Username}";
                            

                            System.Diagnostics.Debug.WriteLine($"Usuario del empleado cargado: {usuarioEmpleado.Username} (ID: {_empleado.IdUsuario})");
                        }
                        
                        // En modo edición, no permitir cambiar el usuario
                        cbUsuario.IsEnabled = false;
                        txtInfoUsuario.Text += " (No modificable en edición)";
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"✓ Usuarios cargados en ComboBox: {cbUsuario.Items.Count}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al cargar usuarios: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                txtInfoUsuario.Text = "❌ Error al cargar usuarios. Verifique su conexión.";
                txtInfoUsuario.Foreground = new SolidColorBrush(Colors.Red);
                cbUsuario.IsEnabled = false;
                
                MessageBox.Show(
                    $"Error al cargar usuarios disponibles:\n\n{ex.Message}\n\n" +
                    $"Verifique que el servicio de usuarios esté funcionando correctamente.",
                    "Error de Carga",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void cbUsuario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbUsuario.SelectedItem != null)
            {
                dynamic usuarioSeleccionado = cbUsuario.SelectedItem;
                string nombreCompleto = usuarioSeleccionado.NombreCompleto;
                string email = usuarioSeleccionado.Email;
                
                txtInfoUsuario.Text = $"✓ Seleccionado: {nombreCompleto} - {email}";
                txtInfoUsuario.Foreground = new SolidColorBrush(Colors.DarkGreen);
                
                System.Diagnostics.Debug.WriteLine($"Usuario seleccionado: ID={usuarioSeleccionado.Id}, Email={email}");
            }
        }

        private void ConfigurarModoCreacion()
        {
            Titulo.Text = "Crear Nuevo Empleado";
            BtnCrear.Visibility = Visibility.Visible;
            BtnActualizar.Visibility = Visibility.Collapsed;
            BtnEliminar.Visibility = Visibility.Collapsed;
            
            if (cbRol.Items.Count > 0) cbRol.SelectedIndex = 0;
            if (cbEstado.Items.Count > 0) cbEstado.SelectedIndex = 0;
            
            System.Diagnostics.Debug.WriteLine("✓ Modo CREACIÓN configurado");
        }

        private void ConfigurarModoConsulta()
        {
            Titulo.Text = $"Consultar Empleado: {_empleado?.Nombres}";
            
            tbRun.IsEnabled = false;
            dpFechaNacimiento.IsEnabled = false;
            tbNombre.IsEnabled = false;
            tbA_paterno.IsEnabled = false;
            tbA_materno.IsEnabled = false;
            tbEmailPersonal.IsEnabled = false;
            tbTelefono.IsEnabled = false;
            tbDireccion.IsEnabled = false;
            tbTituloProfesional.IsEnabled = false;
            tbUniversidad.IsEnabled = false;
            tbRegistroProfesional.IsEnabled = false;
            tbAniosExperiencia.IsEnabled = false;
            cbRol.IsEnabled = false;
            cbEstado.IsEnabled = false;
            cbUsuario.IsEnabled = false;
            
            BtnCrear.Visibility = Visibility.Collapsed;
            BtnActualizar.Visibility = Visibility.Collapsed;
            BtnEliminar.Visibility = Visibility.Collapsed;
            
            System.Diagnostics.Debug.WriteLine("✓ Modo CONSULTA configurado");
        }

        private void ConfigurarModoEdicion()
        {
            Titulo.Text = $"Editar Empleado: {_empleado?.Nombres}";
            
            tbRun.IsEnabled = true;
            dpFechaNacimiento.IsEnabled = true;
            tbNombre.IsEnabled = true;
            tbA_paterno.IsEnabled = true;
            tbA_materno.IsEnabled = true;
            tbEmailPersonal.IsEnabled = true;
            tbTelefono.IsEnabled = true;
            tbDireccion.IsEnabled = true;
            tbTituloProfesional.IsEnabled = true;
            tbUniversidad.IsEnabled = true;
            tbRegistroProfesional.IsEnabled = true;
            tbAniosExperiencia.IsEnabled = true;
            cbRol.IsEnabled = true;
            cbEstado.IsEnabled = true;
            cbUsuario.IsEnabled = false; // No permitir cambiar usuario en edición
            
            BtnCrear.Visibility = Visibility.Collapsed;
            BtnActualizar.Visibility = Visibility.Visible;
            BtnEliminar.Visibility = Visibility.Visible;
            
            System.Diagnostics.Debug.WriteLine("✓ Modo EDICIÓN configurado");
        }

        private void CargarDatosEmpleado()
        {
            try
            {
                if (_empleado == null)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ _empleado es NULL");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($">>> Cargando datos del empleado: {_empleado.NombreCompleto}");

                tbRun.Text = _empleado.Rut ?? "";
                tbNombre.Text = _empleado.Nombres ?? "";
                tbA_paterno.Text = _empleado.ApellidoPaterno ?? "";
                tbA_materno.Text = _empleado.ApellidoMaterno ?? "";
                
                if (!string.IsNullOrEmpty(_empleado.FechaNacimiento))
                {
                    if (DateTime.TryParse(_empleado.FechaNacimiento, out DateTime fecha))
                    {
                        dpFechaNacimiento.SelectedDate = fecha;
                    }
                }

                tbEmailPersonal.Text = _empleado.EmailPersonal ?? "";
                tbTelefono.Text = _empleado.Telefono ?? "";
                tbDireccion.Text = _empleado.Direccion ?? "";
                tbTituloProfesional.Text = _empleado.TituloProfesional ?? "";
                tbUniversidad.Text = _empleado.Universidad ?? "";
                tbRegistroProfesional.Text = _empleado.RegistroProfesional ?? "";
                tbAniosExperiencia.Text = _empleado.AniosExperiencia.ToString();

                if (_empleado.IdRol > 0)
                {
                    foreach (ComboBoxItem item in cbRol.Items)
                    {
                        if (item.Tag?.ToString() == _empleado.IdRol.ToString())
                        {
                            cbRol.SelectedItem = item;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(_empleado.Estado))
                {
                    foreach (ComboBoxItem item in cbEstado.Items)
                    {
                        if (item.Tag?.ToString()?.ToLower() == _empleado.Estado.ToLower())
                        {
                            cbEstado.SelectedItem = item;
                            break;
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine("✅ Datos del empleado cargados");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ ERROR: {ex.Message}");
                MessageBox.Show($"Error al cargar datos:\n\n{ex.Message}",
                    "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Crear(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(">>> Botón Crear presionado");

                if (!ValidarCampos())
                    return;

                LoadingOverlay.Visibility = Visibility.Visible;

                int idRol = int.Parse(((ComboBoxItem)cbRol.SelectedItem)?.Tag?.ToString() ?? "1");
                string estado = ((ComboBoxItem)cbEstado.SelectedItem)?.Tag?.ToString() ?? "activo";
                string fechaNacimiento = dpFechaNacimiento.SelectedDate?.ToString("yyyy-MM-dd") ?? "";
                
                // Obtener el ID del usuario seleccionado
                int idUsuario = int.Parse(cbUsuario.SelectedValue?.ToString() ?? "0");

                var nuevoEmpleado = new PsicologoApiService.CreateEmpleadoModel
                {
                    Rut = tbRun.Text.Trim(),
                    Nombres = tbNombre.Text.Trim(),
                    ApellidoPaterno = tbA_paterno.Text.Trim(),
                    ApellidoMaterno = tbA_materno.Text.Trim(),
                    FechaNacimiento = fechaNacimiento,
                    Telefono = tbTelefono.Text.Trim(),
                    EmailPersonal = tbEmailPersonal.Text.Trim(),
                    Direccion = tbDireccion.Text.Trim(),
                    RegistroProfesional = tbRegistroProfesional.Text.Trim(),
                    TituloProfesional = tbTituloProfesional.Text.Trim(),
                    Universidad = tbUniversidad.Text.Trim(),
                    AniosExperiencia = int.TryParse(tbAniosExperiencia.Text, out int anios) ? anios : 0,
                    FotoPerfil = "",
                    IdRol = idRol,
                    Estado = estado,
                    IdUsuario = idUsuario // ✅ ASIGNAR EL ID DEL USUARIO SELECCIONADO
                };

                System.Diagnostics.Debug.WriteLine($"Creando empleado con Usuario ID: {idUsuario}");
                var empleadoCreado = await _empleadoService.CreateEmpleadoAsync(nuevoEmpleado);

                LoadingOverlay.Visibility = Visibility.Collapsed;

                if (empleadoCreado != null)
                {
                    MessageBox.Show(
                        $"Empleado creado exitosamente\n\n" +
                        $"{empleadoCreado.NombreCompleto}\n" +
                        $"RUT: {empleadoCreado.Rut}\n" +
                        $"ID: {empleadoCreado.IdEmpleado}\n" +
                        $"Usuario ID: {empleadoCreado.IdUsuario}",
                        "Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    EmpleadoGuardado?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo crear el empleado.\n\n" +
                        "Intente nuevamente o contacte al administrador.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
                System.Diagnostics.Debug.WriteLine($"✗ Error al crear empleado: {ex.Message}");
                MessageBox.Show($"Error al crear empleado:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Actualizar(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(">>> Botón Actualizar presionado");

                if (_empleado == null)
                {
                    MessageBox.Show("No se puede actualizar: empleado no encontrado",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!ValidarCampos())
                    return;

                LoadingOverlay.Visibility = Visibility.Visible;

                int idRol = int.Parse(((ComboBoxItem)cbRol.SelectedItem)?.Tag?.ToString() ?? "1");
                string estado = ((ComboBoxItem)cbEstado.SelectedItem)?.Tag?.ToString() ?? "activo";
                string fechaNacimiento = dpFechaNacimiento.SelectedDate?.ToString("yyyy-MM-dd") ?? "";

                // Crear modelo de actualización
                var empleadoActualizado = new PsicologoApiService.CreateEmpleadoModel
                {
                    Rut = tbRun.Text.Trim(),
                    Nombres = tbNombre.Text.Trim(),
                    ApellidoPaterno = tbA_paterno.Text.Trim(),
                    ApellidoMaterno = tbA_materno.Text.Trim(),
                    FechaNacimiento = fechaNacimiento,
                    Telefono = tbTelefono.Text.Trim(),
                    EmailPersonal = tbEmailPersonal.Text.Trim(),
                    Direccion = tbDireccion.Text.Trim(),
                    RegistroProfesional = tbRegistroProfesional.Text.Trim(),
                    TituloProfesional = tbTituloProfesional.Text.Trim(),
                    Universidad = tbUniversidad.Text.Trim(),
                    AniosExperiencia = int.TryParse(tbAniosExperiencia.Text, out int anios) ? anios : 0,
                    FotoPerfil = _empleado.FotoPerfil ?? "",
                    IdRol = idRol,
                    Estado = estado,
                    IdUsuario = _empleado.IdUsuario // Mantener el usuario actual (no se puede cambiar)
                };

                System.Diagnostics.Debug.WriteLine($"Actualizando empleado ID: {_empleado.IdEmpleado}");
                System.Diagnostics.Debug.WriteLine($"  Nombre: {empleadoActualizado.Nombres} {empleadoActualizado.ApellidoPaterno}");
                System.Diagnostics.Debug.WriteLine($"  Estado: {empleadoActualizado.Estado}");
                System.Diagnostics.Debug.WriteLine($"  Rol ID: {empleadoActualizado.IdRol}");
                System.Diagnostics.Debug.WriteLine($"  Usuario ID: {empleadoActualizado.IdUsuario}");

                // Llamar al servicio de actualización
                var empleadoResult = await _empleadoService.UpdateEmpleadoAsync(_empleado.IdEmpleado, empleadoActualizado);

                LoadingOverlay.Visibility = Visibility.Collapsed;

                if (empleadoResult != null)
                {
                    MessageBox.Show(
                        $"✅ Empleado actualizado exitosamente\n\n" +
                        $"{empleadoResult.NombreCompleto}\n" +
                        $"RUT: {empleadoResult.Rut}\n" +
                        $"ID: {empleadoResult.IdEmpleado}\n" +
                        $"Estado: {empleadoResult.Estado}\n" +
                        $"Rol: {empleadoResult.RolEmpleado}",
                        "Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Notificar que se actualizó
                    EmpleadoGuardado?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show(
                        "❌ No se pudo actualizar el empleado.\n\n" +
                        "Posibles causas:\n" +
                        "• Error de conexión con el servidor\n" +
                        "• Datos inválidos\n" +
                        "• El empleado no existe\n\n" +
                        "Consulte la ventana Output (Debug) para más detalles.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
                System.Diagnostics.Debug.WriteLine($"✗ Error al actualizar empleado: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                MessageBox.Show(
                    $"❌ Error al actualizar empleado:\n\n{ex.Message}",
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private async void Eliminar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_empleado == null)
                {
                    MessageBox.Show("No se puede eliminar: empleado no encontrado",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar al empleado?\n\n" +
                    $"{_empleado.NombreCompleto}\n" +
                    $"RUT: {_empleado.Rut}\n" +
                    $"Rol: {_empleado.RolEmpleado}\n\n" +
                    $"Esta acción NO se puede deshacer.",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    LoadingOverlay.Visibility = Visibility.Visible;
                    
                    bool eliminado = await _empleadoService.DeleteEmpleadoAsync(_empleado.IdEmpleado);

                    LoadingOverlay.Visibility = Visibility.Collapsed;

                    if (eliminado)
                    {
                        MessageBox.Show(
                            $"Empleado eliminado exitosamente\n\n{_empleado.NombreCompleto}",
                            "Éxito",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        EmpleadoGuardado?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        MessageBox.Show(
                            "No se pudo eliminar el empleado.\n\nVerifique que no tenga citas o atenciones asociadas.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
                System.Diagnostics.Debug.WriteLine($"✗ Error al eliminar: {ex.Message}");
                MessageBox.Show($"Error al eliminar empleado:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(tbRun.Text))
            {
                MessageBox.Show("El RUT es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbRun.Focus();
                return false;
            }

            if (!ValidarRutChileno(tbRun.Text))
            {
                MessageBox.Show("El RUT ingresado no es válido\n\nFormato esperado: 12345678-9", 
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbRun.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbNombre.Text))
            {
                MessageBox.Show("Los nombres son obligatorios", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbA_paterno.Text))
            {
                MessageBox.Show("El apellido paterno es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbA_paterno.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbA_materno.Text))
            {
                MessageBox.Show("El apellido materno es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbA_materno.Focus();
                return false;
            }

            if (!dpFechaNacimiento.SelectedDate.HasValue)
            {
                MessageBox.Show("La fecha de nacimiento es obligatoria", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dpFechaNacimiento.Focus();
                return false;
            }

            if (dpFechaNacimiento.SelectedDate.Value.AddYears(18) > DateTime.Today)
            {
                MessageBox.Show("El empleado debe ser mayor de 18 años", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                dpFechaNacimiento.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbEmailPersonal.Text))
            {
                MessageBox.Show("El email es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbEmailPersonal.Focus();
                return false;
            }

            if (!ValidarEmail(tbEmailPersonal.Text))
            {
                MessageBox.Show("El formato del email no es válido\n\nEjemplo: usuario@ejemplo.com", 
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbEmailPersonal.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbTelefono.Text))
            {
                MessageBox.Show("El teléfono es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbTelefono.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(tbTituloProfesional.Text))
            {
                MessageBox.Show("El título profesional es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbTituloProfesional.Focus();
                return false;
            }

            if (cbRol.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un rol", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbRol.Focus();
                return false;
            }

            // ✅ VALIDAR QUE SE HAYA SELECCIONADO UN USUARIO
            if (cbUsuario.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un usuario del sistema", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbUsuario.Focus();
                return false;
            }

            return true;
        }

        private bool ValidarRutChileno(string rut)
        {
            try
            {
                rut = rut.Replace(".", "").Replace("-", "").Trim().ToUpper();

                if (rut.Length < 2)
                    return false;

                string rutNumero = rut.Substring(0, rut.Length - 1);
                string dvIngresado = rut.Substring(rut.Length - 1);

                if (!int.TryParse(rutNumero, out int numero))
                    return false;

                int suma = 0;
                int multiplicador = 2;

                for (int i = rutNumero.Length - 1; i >= 0; i--)
                {
                    suma += int.Parse(rutNumero[i].ToString()) * multiplicador;
                    multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
                }

                int resto = suma % 11;
                int dvCalculado = 11 - resto;

                string dvEsperado;
                if (dvCalculado == 11)
                    dvEsperado = "0";
                else if (dvCalculado == 10)
                    dvEsperado = "K";
                else
                    dvEsperado = dvCalculado.ToString();

                return dvIngresado == dvEsperado;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidarEmail(string email)
        {
            try
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(">>> Botón Regresar presionado");
            
            if (_soloConsulta)
            {
                EmpleadoGuardado?.Invoke(this, EventArgs.Empty);
                return;
            }
            
            bool hayCambios = false;
            
            if (_empleado != null)
            {
                hayCambios = tbNombre.Text != (_empleado.Nombres ?? "") ||
                            tbA_paterno.Text != (_empleado.ApellidoPaterno ?? "") ||
                            tbA_materno.Text != (_empleado.ApellidoMaterno ?? "");
            }
            else
            {
                hayCambios = !string.IsNullOrWhiteSpace(tbRun.Text) || 
                            !string.IsNullOrWhiteSpace(tbNombre.Text) ||
                            !string.IsNullOrWhiteSpace(tbA_paterno.Text);
            }

            if (hayCambios)
            {
                var resultado = MessageBox.Show(
                    "¿Está seguro de salir sin guardar?\n\nSe perderán los cambios no guardados.",
                    "Confirmar Salida",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (resultado != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            EmpleadoGuardado?.Invoke(this, EventArgs.Empty);
        }
    }
}
