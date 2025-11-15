using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Services;
using WPF_LoginForm.Models;

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
            GridDatos.ItemsSource = items;
        }

        // Evento para búsqueda en el TextBox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Implementar búsqueda con API
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
    }
}
