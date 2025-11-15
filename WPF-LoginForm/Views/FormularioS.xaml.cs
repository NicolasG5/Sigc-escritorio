using System;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para FormularioS.xaml
    /// </summary>
    public partial class FormularioS : Page
    {
        private readonly SeguimientoApiService _seguimientoService = new SeguimientoApiService();
        private readonly PacienteModel _paciente;
        private readonly TratamientoResponse _tratamiento;
        private readonly int _idPsicologo;

        public FormularioS(TratamientoResponse tratamiento, PacienteModel paciente, int idPsicologo = 0)
        {
            InitializeComponent();
            _tratamiento = tratamiento;
            _paciente = paciente;
            _idPsicologo = idPsicologo;
            tbNombre.Text = paciente.NombreCompleto;
        }

        private async void Guardar(object sender, RoutedEventArgs e)
        {
            try
            {
                var seguimiento = new SeguimientoRequest
                {
                    fecha_seguimiento = DateTime.Now.ToString("yyyy-MM-dd"),
                    tipo_seguimiento = "Seguimiento",
                    estado_animo = tbFecha_Copiar1.Text,
                    nivel_funcionalidad = 0,
                    adherencia_tratamiento = tbFecha_Copiar.Text,
                    observaciones = tbFecha_Copiar4.Text,
                    proxima_evaluacion = DateTime.TryParse(tbFecha.Text, out var fechaEval) ? fechaEval.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd"),
                    id_seguimiento = 0,
                    id_paciente = _paciente.IdPaciente,
                    id_empleado = _tratamiento.IdEmpleado,
                    id_tratamiento = _tratamiento.IdTratamiento,
                    fecha_registro = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };
                var result = await _seguimientoService.CrearSeguimientoAsync(seguimiento);
                if (result)
                {
                    MessageBox.Show("Seguimiento guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    Content = new Seguimiento();
                }
                else
                {
                    MessageBox.Show("Error al guardar el seguimiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new Seguimiento();
        }

        #region buscar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Implementar búsqueda con API
        }
        #endregion

        private void Agregar(object sender, RoutedEventArgs e)
        {
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }
    }

    public class SeguimientoRequest
    {
        public string fecha_seguimiento { get; set; }
        public string tipo_seguimiento { get; set; }
        public string estado_animo { get; set; }
        public int nivel_funcionalidad { get; set; }
        public string adherencia_tratamiento { get; set; }
        public string observaciones { get; set; }
        public string proxima_evaluacion { get; set; }
        public int id_seguimiento { get; set; } = 0;
        public int id_paciente { get; set; }
        public int id_empleado { get; set; }
        public int id_tratamiento { get; set; }
        public string fecha_registro { get; set; }
    }
}
