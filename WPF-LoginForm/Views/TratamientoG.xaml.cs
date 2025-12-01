using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para TratamientoG.xaml
    /// </summary>
    public partial class TratamientoG : UserControl
    {
        private readonly TratamientoApiService _apiService = new TratamientoApiService();
        private readonly PacienteApiService _pacienteService = new PacienteApiService();

        public TratamientoG()
        {
            InitializeComponent();
            _ = CargarTratamientosAsync();
        }

        public class TratamientoGridItem
        {
            public int IdTratamiento { get; set; }
            public string NombrePaciente { get; set; }
            public string FechaInicio { get; set; }
            public string Descripcion { get; set; }
            // Puedes agregar más campos si lo necesitas
        }

        private List<TratamientoGridItem> _tratamientosOriginales = new List<TratamientoGridItem>();

        private async Task CargarTratamientosAsync()
        {
            var lista = await _apiService.GetAllTratamientosAsync();
            if (lista == null)
            {
                MessageBox.Show("No se pudieron obtener los tratamientos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var items = new List<TratamientoGridItem>();
            foreach (var t in lista)
            {
                string nombrePaciente = "";
                try
                {
                    var paciente = await _pacienteService.GetPacienteByIdAsync(t.IdPaciente);
                    nombrePaciente = paciente?.NombreCompleto ?? "";
                }
                catch
                {
                    nombrePaciente = "";
                }
                items.Add(new TratamientoGridItem
                {
                    IdTratamiento = t.IdTratamiento,
                    NombrePaciente = nombrePaciente,
                    FechaInicio = t.FechaInicio,
                    Descripcion = t.Descripcion
                });
            }
            _tratamientosOriginales = items;
            GridDatos.ItemsSource = items;
        }

        // Evento para búsqueda en el TextBox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = Buscar.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(texto))
            {
                GridDatos.ItemsSource = _tratamientosOriginales;
            }
            else
            {
                var filtrados = _tratamientosOriginales.Where(t =>
                    (t.NombrePaciente?.ToLower().Contains(texto) ?? false) ||
                    (t.FechaInicio?.ToLower().Contains(texto) ?? false) ||
                    (t.Descripcion?.ToLower().Contains(texto) ?? false)
                ).ToList();
                GridDatos.ItemsSource = filtrados;
            }
        }

        // Evento para botón Agregar
        private void Agregar(object sender, RoutedEventArgs e)
        {
            Tratamiento ventana = new Tratamiento();
            FrameTratamientoG.Content = ventana;
        }

        private async void VerDetalle(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).CommandParameter is int id)
            {
                var ventana = new Tratamiento();
                await ventana.CargarTratamiento(id);
                FrameTratamientoG.Content = ventana;
            }
        }

        private async void Atender(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).CommandParameter is int idTratamiento)
            {
                // Obtener el tratamiento y el paciente
                var tratamiento = await _apiService.GetTratamientoByIdAsync(idTratamiento);
                if (tratamiento == null)
                {
                    MessageBox.Show("No se pudo obtener el tratamiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var paciente = await _pacienteService.GetPacienteByIdAsync(tratamiento.IdPaciente);
                if (paciente == null)
                {
                    MessageBox.Show("No se pudo obtener el paciente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Crear y mostrar el formulario de seguimiento
                var formularioS = new WPF_LoginForm.Views.FormularioS(tratamiento, paciente);
                FrameTratamientoG.Content = formularioS;
            }
        }

        private async void ConsultarSeguimiento(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).CommandParameter is int idTratamiento)
            {
                var seguimientoService = new WPF_LoginForm.Services.SeguimientoApiService();
                var seguimiento = await seguimientoService.GetSeguimientoByIdAsync(idTratamiento);
                if (seguimiento == null)
                {
                    MessageBox.Show("No se encontró seguimiento para este tratamiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Buscar el nombre real del paciente
                var pacienteService = new WPF_LoginForm.Services.PacienteApiService();
                var paciente = await pacienteService.GetPacienteByIdAsync(seguimiento.id_paciente);
                string nombrePaciente = paciente?.NombreCompleto ?? "";
                // Mostrar los datos en FormularioS
                var formularioS = new WPF_LoginForm.Views.FormularioS(null, paciente);
                formularioS.tbNombre.Text = nombrePaciente;
                formularioS.tbFecha.Text = seguimiento.fecha_seguimiento;
                formularioS.tbFecha_Copiar1.Text = seguimiento.estado_animo;
                formularioS.tbFecha_Copiar.Text = seguimiento.adherencia_tratamiento;
                formularioS.tbFecha_Copiar4.Text = seguimiento.observaciones;
                formularioS.tbFecha_Copiar5.Text = seguimiento.proxima_evaluacion;
                // Puedes mapear más campos según tu UI
                FrameTratamientoG.Content = formularioS;
            }
        }
    }
}
