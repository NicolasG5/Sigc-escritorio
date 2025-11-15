using System;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Services;
using WPF_LoginForm.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para Medicacion.xaml
    /// </summary>
    public partial class Medicacion : Page
    {
        private readonly PacienteApiService _pacienteService = new PacienteApiService();
        private readonly TratamientoApiService _tratamientoService = new TratamientoApiService();
        private readonly MedicacionApiService _medicacionService = new MedicacionApiService();
        private int idPaciente;
        private int idTratamiento;

        public Medicacion() : this(0) { }
        public Medicacion(int pacienteId)
        {
            InitializeComponent();
            _ = CargarPacientesAsync();
            if (pacienteId > 0)
                idPaciente = pacienteId;
        }

        private async Task CargarPacientesAsync()
        {
            var pacientes = await _pacienteService.GetAllPacientesAsync();
            cbPaciente.ItemsSource = pacientes;
            cbPaciente.DisplayMemberPath = "NombreCompleto";
            cbPaciente.SelectedValuePath = "IdPaciente";
        }

        private async void cbPaciente_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPaciente.SelectedItem is PacienteModel paciente)
            {
                idPaciente = paciente.IdPaciente;
                var tratamiento = await _tratamientoService.GetTratamientoByPacienteIdAsync(idPaciente);
                if (tratamiento != null)
                {
                    idTratamiento = tratamiento.IdTratamiento;
                    tbTratamiento.Text = tratamiento.TipoTratamiento;
                }
                else
                {
                    idTratamiento = 0;
                    tbTratamiento.Text = "";
                }
            }
        }

        private async void Guardar(object sender, RoutedEventArgs e)
        {
            var estado = (cbEstado.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "activo";
            var medicacionRequest = new
            {
                nombre_medicamento = tbNombreMedicamento.Text,
                dosis = tbDosis.Text,
                frecuencia = tbFrecuencia.Text,
                via_administracion = tbViaAdministracion.Text,
                fecha_inicio = dpFechaInicio.SelectedDate?.ToString("yyyy-MM-dd"),
                fecha_fin = dpFechaFin.SelectedDate?.ToString("yyyy-MM-dd"),
                prescrito_por = tbPrescritoPor.Text,
                observaciones = tbObservaciones.Text,
                estado = estado,
                id_paciente = idPaciente,
                id_tratamiento = idTratamiento
            };
            var resultado = await _medicacionService.CrearMedicacionAsync(medicacionRequest);
            if (resultado)
                MessageBox.Show("Medicación creada correctamente.");
            else
                MessageBox.Show("Error al crear la medicación.");
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new MedicacionG();
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

        public async void CargarDatos(MedicacionModel model)
        {
            tbNombreMedicamento.Text = model.NombreMedicamento;
            tbDosis.Text = model.Dosis;
            tbFrecuencia.Text = model.Frecuencia;
            tbViaAdministracion.Text = model.ViaAdministracion;
            if (DateTime.TryParse(model.FechaInicio, out var fechaInicio))
                dpFechaInicio.SelectedDate = fechaInicio;
            if (DateTime.TryParse(model.FechaFin, out var fechaFin))
                dpFechaFin.SelectedDate = fechaFin;
            tbPrescritoPor.Text = model.PrescritoPor;
            tbObservaciones.Text = model.Observaciones;
            // Estado
            foreach (ComboBoxItem item in cbEstado.Items)
            {
                if ((string)item.Tag == model.Estado)
                {
                    cbEstado.SelectedItem = item;
                    break;
                }
            }
            // Esperar a que los pacientes estén cargados
            if (cbPaciente.ItemsSource == null || ((System.Collections.IEnumerable)cbPaciente.ItemsSource).GetEnumerator().MoveNext() == false)
            {
                var pacientes = await _pacienteService.GetAllPacientesAsync();
                cbPaciente.ItemsSource = pacientes;
                cbPaciente.DisplayMemberPath = "NombreCompleto";
                cbPaciente.SelectedValuePath = "IdPaciente";
            }
            cbPaciente.SelectedValue = model.IdPaciente;
            // Cargar tratamiento
            var tratamiento = await _tratamientoService.GetTratamientoByPacienteIdAsync(model.IdPaciente);
            tbTratamiento.Text = tratamiento?.TipoTratamiento ?? "";
        }
    }
}
