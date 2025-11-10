using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para Paciente.xaml
    /// </summary>
    public partial class Paciente : Page
    {
        private readonly PacienteApiService _pacienteService;
        private readonly CiudadApiService _ciudadService;
        private readonly PrevisionApiService _previsionService;
        
        private PacienteModel _pacienteEditar;
        private bool _modoEdicion = false;

        // Evento para notificar cuando se guarda un paciente
        public event EventHandler PacienteGuardado;

        /// <summary>
        /// Constructor para modo CREAR
        /// </summary>
        public Paciente()
        {
            InitializeComponent();
            
            _pacienteService = new PacienteApiService();
            _ciudadService = new CiudadApiService();
            _previsionService = new PrevisionApiService();
            
            _modoEdicion = false;
            Titulo.Text = "Crear Nuevo Paciente";
            BtnCrear.Visibility = Visibility.Visible;
            BtnActualizar.Visibility = Visibility.Hidden;
            BtnEliminar.Visibility = Visibility.Hidden;
            
            CargarDatosIniciales();
        }

        /// <summary>
        /// Constructor para modo EDITAR
        /// </summary>
        public Paciente(PacienteModel paciente)
        {
            InitializeComponent();
            
            _pacienteService = new PacienteApiService();
            _ciudadService = new CiudadApiService();
            _previsionService = new PrevisionApiService();
            
            _modoEdicion = true;
            _pacienteEditar = paciente;
            
            Titulo.Text = "Editar Paciente";
            BtnCrear.Visibility = Visibility.Hidden;
            BtnActualizar.Visibility = Visibility.Visible;
            BtnEliminar.Visibility = Visibility.Hidden;
            
            CargarDatosIniciales();
        }

        /// <summary>
        /// Constructor para modo CONSULTAR (solo lectura)
        /// </summary>
        public Paciente(PacienteModel paciente, bool soloConsulta)
        {
            InitializeComponent();
            
            _pacienteService = new PacienteApiService();
            _ciudadService = new CiudadApiService();
            _previsionService = new PrevisionApiService();
            
            _modoEdicion = false; // No es edición, es consulta
            _pacienteEditar = paciente;
            
            Titulo.Text = "Consultar Paciente";
            BtnCrear.Visibility = Visibility.Hidden;
            BtnActualizar.Visibility = Visibility.Hidden;
            BtnEliminar.Visibility = Visibility.Hidden;
            
            // Deshabilitar TODOS los campos en modo consulta
            _soloConsulta = soloConsulta;
            
            CargarDatosIniciales();
        }
        
        private bool _soloConsulta = false;

        private async void CargarDatosIniciales()
        {
            try
            {
                // Mostrar indicador de carga
                LoadingOverlay.Visibility = Visibility.Visible;

                System.Diagnostics.Debug.WriteLine("=== INICIANDO CARGA DE DATOS ===");

                // Cargar ciudades en el ComboBox
                var ciudades = await _ciudadService.GetAllCiudadesAsync();
                if (ciudades != null && ciudades.Any())
                {
                    var listaCiudades = ciudades.OrderBy(c => c.NombreCiudad).ToList();
                    cbCiudad.ItemsSource = listaCiudades;
                    System.Diagnostics.Debug.WriteLine($"✓ Ciudades cargadas: {listaCiudades.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No se cargaron ciudades");
                }
                
                // Cargar previsiones en el ComboBox
                var previsiones = await _previsionService.GetAllPrevisionesAsync();
                if (previsiones != null && previsiones.Any())
                {
                    var listaPrevisiones = previsiones.OrderBy(p => p.NombrePrevision).ToList();
                    cbPrevision.ItemsSource = listaPrevisiones;
                    System.Diagnostics.Debug.WriteLine($"✓ Previsiones cargadas: {listaPrevisiones.Count}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ No se cargaron previsiones");
                }

                // Si hay paciente para editar o consultar, cargar sus datos
                if ((_modoEdicion || _soloConsulta) && _pacienteEditar != null)
                {
                    string modo = _soloConsulta ? "CONSULTA" : "EDICIÓN";
                    System.Diagnostics.Debug.WriteLine($"Modo {modo} - Paciente: {_pacienteEditar.NombreCompleto}");
                    System.Diagnostics.Debug.WriteLine($"  ID: {_pacienteEditar.IdPaciente}");
                    System.Diagnostics.Debug.WriteLine($"  RUT: {_pacienteEditar.Rut}");
                    
                    // Dar tiempo para que los ItemsSource se actualicen completamente
                    await System.Threading.Tasks.Task.Delay(300);
                    
                    // Cargar datos del paciente (ahora es async)
                    await CargarDatosPaciente();
                }
                else if (_pacienteEditar == null && (_modoEdicion || _soloConsulta))
                {
                    System.Diagnostics.Debug.WriteLine("✗ ERROR: Se esperaba un paciente pero _pacienteEditar es NULL");
                    MessageBox.Show("Error: No se pudo cargar la información del paciente.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Modo CREAR - Estableciendo valores por defecto");
                    // Valores por defecto para nuevo paciente
                    if (cbGenero.Items.Count > 0) cbGenero.SelectedIndex = 0;
                    if (cbEstadoCivil.Items.Count > 0) cbEstadoCivil.SelectedIndex = 0;
                    if (cbEstado.Items.Count > 0) cbEstado.SelectedIndex = 0;
                }

                System.Diagnostics.Debug.WriteLine("=== CARGA DE DATOS COMPLETADA ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ ERROR en CargarDatosIniciales: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al cargar datos iniciales:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Ocultar indicador de carga
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private async System.Threading.Tasks.Task CargarDatosPaciente()
        {
            try
            {
                if (_pacienteEditar == null)
                {
                    System.Diagnostics.Debug.WriteLine("✗ ERROR: _pacienteEditar es NULL en CargarDatosPaciente");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($">>> Cargando datos del paciente: {_pacienteEditar.NombreCompleto}");
                System.Diagnostics.Debug.WriteLine($"    ID Paciente: {_pacienteEditar.IdPaciente}");

                // ═══════════════════════════════════════════════════════════
                // CARGAR TODOS LOS DATOS (SIN deshabilitar campos aún)
                // ═══════════════════════════════════════════════════════════

                // Datos personales
                tbRun.Text = _pacienteEditar.Rut ?? "";
                tbNombre.Text = _pacienteEditar.Nombres ?? "";
                tbA_paterno.Text = _pacienteEditar.ApellidoPaterno ?? "";
                tbA_materno.Text = _pacienteEditar.ApellidoMaterno ?? "";
                
                System.Diagnostics.Debug.WriteLine($"  ✓ RUT: '{tbRun.Text}'");
                System.Diagnostics.Debug.WriteLine($"  ✓ Nombre: '{tbNombre.Text} {tbA_paterno.Text} {tbA_materno.Text}'");

                // Fecha nacimiento
                if (!string.IsNullOrEmpty(_pacienteEditar.FechaNacimiento))
                {
                    if (DateTime.TryParse(_pacienteEditar.FechaNacimiento, out DateTime fecha))
                    {
                        dpFechaNacimiento.SelectedDate = fecha;
                        System.Diagnostics.Debug.WriteLine($"  ✓ Fecha Nac: {fecha:dd/MM/yyyy}");
                    }
                }

                // Género
                if (!string.IsNullOrEmpty(_pacienteEditar.Genero))
                {
                    foreach (ComboBoxItem item in cbGenero.Items)
                    {
                        if (item.Tag?.ToString()?.ToLower() == _pacienteEditar.Genero.ToLower())
                        {
                            cbGenero.SelectedItem = item;
                            System.Diagnostics.Debug.WriteLine($"  ✓ Género: {_pacienteEditar.Genero}");
                            break;
                        }
                    }
                }

                // Estado Civil
                if (!string.IsNullOrEmpty(_pacienteEditar.EstadoCivil))
                {
                    foreach (ComboBoxItem item in cbEstadoCivil.Items)
                    {
                        if (item.Tag?.ToString()?.ToLower() == _pacienteEditar.EstadoCivil.ToLower())
                        {
                            cbEstadoCivil.SelectedItem = item;
                            System.Diagnostics.Debug.WriteLine($"  ✓ Estado Civil: {_pacienteEditar.EstadoCivil}");
                            break;
                        }
                    }
                }

                // Ocupación
                tbOcupacion.Text = _pacienteEditar.Ocupacion ?? "";

                // Información de contacto
                tbEmail.Text = _pacienteEditar.Email ?? "";
                tbTelefono.Text = _pacienteEditar.Telefono ?? "";
                tbDireccion.Text = _pacienteEditar.Direccion ?? "";
                
                System.Diagnostics.Debug.WriteLine($"  ✓ Email: '{tbEmail.Text}'");
                System.Diagnostics.Debug.WriteLine($"  ✓ Teléfono: '{tbTelefono.Text}'");

                // Ciudad
                if (_pacienteEditar.IdCiudad.HasValue && cbCiudad.Items.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"  Buscando Ciudad ID: {_pacienteEditar.IdCiudad.Value}");
                    cbCiudad.SelectedValue = _pacienteEditar.IdCiudad.Value;
                    
                    if (cbCiudad.SelectedValue != null)
                    {
                        var ciudadSeleccionada = cbCiudad.SelectedItem as CiudadModel;
                        System.Diagnostics.Debug.WriteLine($"  ✓ Ciudad: {ciudadSeleccionada?.NombreCiudad}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"  ⚠️ Ciudad ID {_pacienteEditar.IdCiudad.Value} no encontrada");
                    }
                }

                // Previsión
                if (_pacienteEditar.IdPrevision.HasValue && cbPrevision.Items.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"  Buscando Previsión ID: {_pacienteEditar.IdPrevision.Value}");
                    cbPrevision.SelectedValue = _pacienteEditar.IdPrevision.Value;
                    
                    if (cbPrevision.SelectedValue != null)
                    {
                        var previsionSeleccionada = cbPrevision.SelectedItem as PrevisionModel;
                        System.Diagnostics.Debug.WriteLine($"  ✓ Previsión: {previsionSeleccionada?.NombrePrevision}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"  ⚠️ Previsión ID {_pacienteEditar.IdPrevision.Value} no encontrada");
                    }
                }

                // Estado
                if (!string.IsNullOrEmpty(_pacienteEditar.Estado))
                {
                    foreach (ComboBoxItem item in cbEstado.Items)
                    {
                        if (item.Tag?.ToString()?.ToLower() == _pacienteEditar.Estado.ToLower())
                        {
                            cbEstado.SelectedItem = item;
                            System.Diagnostics.Debug.WriteLine($"  ✓ Estado: {_pacienteEditar.Estado}");
                            break;
                        }
                    }
                }

                // Consentimiento
                chkConsentimiento.IsChecked = _pacienteEditar.ConsentimientoInformado;

                // ═══════════════════════════════════════════════════════════
                // Forzar actualización del UI
                // ═══════════════════════════════════════════════════════════
                
                this.UpdateLayout();
                await System.Threading.Tasks.Task.Delay(50);
                
                System.Diagnostics.Debug.WriteLine("✅ Datos del paciente cargados exitosamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ ERROR en CargarDatosPaciente: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                MessageBox.Show($"Error al cargar datos del paciente:\n\n{ex.Message}",
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

                // Obtener valores de los ComboBox
                string genero = ((ComboBoxItem)cbGenero.SelectedItem)?.Tag?.ToString() ?? "masculino";
                string estadoCivil = ((ComboBoxItem)cbEstadoCivil.SelectedItem)?.Tag?.ToString() ?? "soltero";
                string estado = ((ComboBoxItem)cbEstado.SelectedItem)?.Tag?.ToString() ?? "activo";
                
                // Formatear fecha de nacimiento
                string fechaNacimiento = dpFechaNacimiento.SelectedDate?.ToString("yyyy-MM-dd") ?? "";

                var nuevoPaciente = new PacienteUpdateModel
                {
                    Rut = tbRun.Text.Trim(),
                    Nombres = tbNombre.Text.Trim(),
                    ApellidoPaterno = tbA_paterno.Text.Trim(),
                    ApellidoMaterno = tbA_materno.Text.Trim(),
                    FechaNacimiento = fechaNacimiento,
                    Genero = genero,
                    Telefono = tbTelefono.Text.Trim(),
                    Email = tbEmail.Text.Trim(),
                    Direccion = tbDireccion.Text.Trim(),
                    EstadoCivil = estadoCivil,
                    Ocupacion = tbOcupacion.Text.Trim(),
                    Estado = estado,
                    ConsentimientoInformado = chkConsentimiento.IsChecked ?? false,
                    IdCiudad = (int)(cbCiudad.SelectedValue ?? 1),
                    IdPrevision = (int)(cbPrevision.SelectedValue ?? 1)
                };

                System.Diagnostics.Debug.WriteLine("Llamando a CreatePacienteAsync...");
                var pacienteCreado = await _pacienteService.CreatePacienteAsync(nuevoPaciente);

                if (pacienteCreado != null)
                {
                    MessageBox.Show(
                        $"Paciente creado exitosamente\n\n" +
                        $"{pacienteCreado.NombreCompleto}\n" +
                        $"RUT: {pacienteCreado.Rut}\n" +
                        $"ID: {pacienteCreado.IdPaciente}",
                        "Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Notificar que se guardó el paciente y cerrar formulario
                    PacienteGuardado?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo crear el paciente.\n\n" +
                        "Intente nuevamente o contacte al administrador.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error al crear paciente: {ex.Message}");
                MessageBox.Show($"Error al crear paciente:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Actualizar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidarCampos())
                    return;

                // Obtener valores de los ComboBox
                string genero = ((ComboBoxItem)cbGenero.SelectedItem)?.Tag?.ToString() ?? "masculino";
                string estadoCivil = ((ComboBoxItem)cbEstadoCivil.SelectedItem)?.Tag?.ToString() ?? "soltero";
                string estado = ((ComboBoxItem)cbEstado.SelectedItem)?.Tag?.ToString() ?? "activo";
                
                // Formatear fecha de nacimiento
                string fechaNacimiento = dpFechaNacimiento.SelectedDate?.ToString("yyyy-MM-dd") ?? "";

                var pacienteUpdate = new PacienteUpdateModel
                {
                    Rut = tbRun.Text.Trim(),
                    Nombres = tbNombre.Text.Trim(),
                    ApellidoPaterno = tbA_paterno.Text.Trim(),
                    ApellidoMaterno = tbA_materno.Text.Trim(),
                    FechaNacimiento = fechaNacimiento,
                    Genero = genero,
                    Telefono = tbTelefono.Text.Trim(),
                    Email = tbEmail.Text.Trim(),
                    Direccion = tbDireccion.Text.Trim(),
                    EstadoCivil = estadoCivil,
                    Ocupacion = tbOcupacion.Text.Trim(),
                    Estado = estado,
                    ConsentimientoInformado = chkConsentimiento.IsChecked ?? false,
                    IdCiudad = (int)(cbCiudad.SelectedValue ?? 1),
                    IdPrevision = (int)(cbPrevision.SelectedValue ?? 1)
                };

                System.Diagnostics.Debug.WriteLine(">>> Actualizando paciente...");
                bool actualizado = await _pacienteService.UpdatePacienteAsync(_pacienteEditar.IdPaciente, pacienteUpdate);

                if (actualizado)
                {
                    MessageBox.Show(
                        $"Paciente actualizado exitosamente\n\n" +
                        $"{pacienteUpdate.Nombres} {pacienteUpdate.ApellidoPaterno}\n" +
                        $"RUT: {pacienteUpdate.Rut}",
                        "Éxito",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    System.Diagnostics.Debug.WriteLine("✓ Paciente actualizado - cerrando formulario");
                    
                    // Notificar que se guardó el paciente para que CustomerView5 cierre el formulario y recargue
                    PacienteGuardado?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo actualizar el paciente.\n\n" +
                        "Intente nuevamente o contacte al administrador.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error al actualizar: {ex.Message}");
                MessageBox.Show($"Error al actualizar paciente:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Eliminar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pacienteEditar == null)
                {
                    MessageBox.Show("No se puede eliminar: paciente no encontrado",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var resultado = MessageBox.Show(
                    $"¿Está seguro de eliminar al paciente?\n\n" +
                    $"{_pacienteEditar.NombreCompleto}\n" +
                    $"RUT: {_pacienteEditar.Rut}\n\n" +
                    $"Esta acción no se puede deshacer.",
                    "Confirmar Eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Debug.WriteLine($"Eliminando paciente ID: {_pacienteEditar.IdPaciente}");
                    
                    bool eliminado = await _pacienteService.DeletePacienteAsync(_pacienteEditar.IdPaciente);

                    if (eliminado)
                    {
                        MessageBox.Show(
                            $"Paciente eliminado exitosamente\n\n" +
                            $"{_pacienteEditar.NombreCompleto}",
                            "Éxito",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        System.Diagnostics.Debug.WriteLine("✓ Paciente eliminado - cerrando formulario");
                        
                        // Notificar que se eliminó el paciente para que CustomerView5 cierre el formulario y recargue
                        PacienteGuardado?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        MessageBox.Show(
                            "No se pudo eliminar el paciente.\n\n" +
                            "Intente nuevamente o contacte al administrador.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"✗ Error al eliminar paciente: {ex.Message}");
                MessageBox.Show($"Error al eliminar paciente:\n\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidarCampos()
        {
            // En modo edición, algunos campos están bloqueados y ya fueron validados al crear el paciente
            // Solo validamos campos que SÍ son editables
            
            if (!_modoEdicion)
            {
                // MODO CREAR: Validar TODO
                
                // Validar RUT
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

                // Validar Nombres
                if (string.IsNullOrWhiteSpace(tbNombre.Text))
                {
                    MessageBox.Show("Los nombres son obligatorios", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    tbNombre.Focus();
                    return false;
                }

                // Validar Apellidos
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

                // Validar Fecha de Nacimiento
                if (!dpFechaNacimiento.SelectedDate.HasValue)
                {
                    MessageBox.Show("La fecha de nacimiento es obligatoria", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpFechaNacimiento.Focus();
                    return false;
                }

                // Validar que la persona tenga al menos 18 años
                if (dpFechaNacimiento.SelectedDate.Value.AddYears(18) > DateTime.Today)
                {
                    MessageBox.Show("El paciente debe ser mayor de 18 años", "Validación", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    dpFechaNacimiento.Focus();
                    return false;
                }
            }
            
            // MODO EDITAR: Solo validar campos EDITABLES
            
            // Validar Email (EDITABLE en ambos modos)
            if (string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                MessageBox.Show("El email es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbEmail.Focus();
                return false;
            }

            if (!ValidarEmail(tbEmail.Text))
            {
                MessageBox.Show("El formato del email no es válido\n\nEjemplo: usuario@ejemplo.com", 
                    "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                tbEmail.Focus();
                return false;
            }

            // Validar Teléfono (EDITABLE)
            if (string.IsNullOrWhiteSpace(tbTelefono.Text))
            {
                MessageBox.Show("El teléfono es obligatorio", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                tbTelefono.Focus();
                return false;
            }

            // Validar Ciudad (EDITABLE)
            if (cbCiudad.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar una ciudad", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbCiudad.Focus();
                return false;
            }

            // Validar Previsión (EDITABLE)
            if (cbPrevision.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar una previsión de salud", "Validación", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cbPrevision.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida un RUT chileno con su dígito verificador
        /// </summary>
        private bool ValidarRutChileno(string rut)
        {
            try
            {
                // Limpiar RUT (quitar puntos y guión)
                rut = rut.Replace(".", "").Replace("-", "").Trim().ToUpper();

                // Validar formato básico
                if (rut.Length < 2)
                    return false;

                // Separar número y dígito verificador
                string rutNumero = rut.Substring(0, rut.Length - 1);
                string dvIngresado = rut.Substring(rut.Length - 1);

                // Validar que el número sea numérico
                if (!int.TryParse(rutNumero, out int numero))
                    return false;

                // Calcular dígito verificador
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

        /// <summary>
        /// Valida formato de email
        /// </summary>
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

        public void Consultar()
        {
            // Método legacy para compatibilidad
            // Ya no se usa porque se pasa el paciente en el constructor
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(">>> Botón Regresar presionado");
            
            // Si está en modo consulta, cerrar directamente sin preguntar
            if (_soloConsulta)
            {
                System.Diagnostics.Debug.WriteLine("✓ Modo consulta - cerrando sin confirmación");
                PacienteGuardado?.Invoke(this, EventArgs.Empty);
                return;
            }
            
            // En modo crear o editar, solo preguntar si hay cambios potenciales
            bool hayCambios = false;
            
            if (_modoEdicion)
            {
                // En modo edición, verificar si se modificó algún campo editable
                if (_pacienteEditar != null)
                {
                    hayCambios = tbEmail.Text != (_pacienteEditar.Email ?? "") ||
                                tbTelefono.Text != (_pacienteEditar.Telefono ?? "") ||
                                tbDireccion.Text != (_pacienteEditar.Direccion ?? "") ||
                                tbOcupacion.Text != (_pacienteEditar.Ocupacion ?? "");
                }
            }
            else
            {
                // Modo crear: verificar si hay datos ingresados
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
                    System.Diagnostics.Debug.WriteLine("Usuario canceló el regreso");
                    return;
                }
            }

            System.Diagnostics.Debug.WriteLine("✓ Cerrando formulario vía evento");
            
            // Invocar evento para que CustomerView5 cierre el formulario
            PacienteGuardado?.Invoke(this, EventArgs.Empty);
        }
    }
}
