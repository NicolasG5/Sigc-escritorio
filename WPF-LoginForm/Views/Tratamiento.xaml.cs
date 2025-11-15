using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para Tratamiento.xaml
    /// </summary>
    public partial class Tratamiento : Page
    {
        private readonly TratamientoApiService _apiService = new TratamientoApiService();

        public Tratamiento()
        {
            InitializeComponent();
        }

        public async Task CargarTratamiento(int idTratamiento)
        {
            var tratamiento = await _apiService.GetTratamientoByIdAsync(idTratamiento);
            if (tratamiento == null)
            {
                MessageBox.Show($"No se encontró el tratamiento con ID {idTratamiento}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            tbTipoTratamiento.Text = tratamiento.TipoTratamiento;
            tbDescripcion.Text = tratamiento.Descripcion;
            tbObjetivos.Text = tratamiento.Objetivos;
            tbFechaInicio.Text = tratamiento.FechaInicio;
            tbFechaFinEstimada.Text = tratamiento.FechaFinEstimada;
            tbFechaFinReal.Text = tratamiento.FechaFinReal;
            tbEstado.Text = tratamiento.Estado;
            tbIdTratamiento.Text = tratamiento.IdTratamiento.ToString();
            tbIdPaciente.Text = tratamiento.IdPaciente.ToString();
            tbIdEmpleado.Text = tratamiento.IdEmpleado.ToString();
            tbIdCita.Text = tratamiento.IdCita.ToString();
            tbFechaRegistro.Text = tratamiento.FechaRegistro;
        }

        private void Crear(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new TratamientoG();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Implementar búsqueda con API
        }

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

        private void Guardar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Excel(object sender, RoutedEventArgs e)
        {
            // TODO: Generar reporte Excel
        }

        private void Pdf(object sender, RoutedEventArgs e)
        {
            // TODO: Generar reporte PDF
        }
    }
}
