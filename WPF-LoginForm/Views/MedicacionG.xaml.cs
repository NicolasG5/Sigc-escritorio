using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;
using System.Collections.ObjectModel;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para MedicacionG.xaml
    /// </summary>
    public partial class MedicacionG : UserControl
    {
        private readonly MedicacionApiService _medicacionService = new MedicacionApiService();
        private readonly PacienteApiService _pacienteService = new PacienteApiService();
        public ObservableCollection<MedicacionModel> Medicaciones { get; set; } = new ObservableCollection<MedicacionModel>();

        public MedicacionG()
        {
            InitializeComponent();
            _ = CargarMedicacionesAsync();
        }

        private async Task CargarMedicacionesAsync()
        {
            var lista = await _medicacionService.GetAllMedicacionesAsync();
            Medicaciones.Clear();
            if (lista != null)
            {
                foreach (var m in lista)
                {
                    // Obtener nombre del paciente por id
                    if (m.IdPaciente > 0)
                    {
                        try
                        {
                            var paciente = await _pacienteService.GetPacienteByIdAsync(m.IdPaciente);
                            m.NombrePaciente = paciente?.NombreCompleto ?? "";
                        }
                        catch
                        {
                            m.NombrePaciente = "";
                        }
                    }
                    Medicaciones.Add(m);
                }
            }
            GridDatos.ItemsSource = Medicaciones;
        }

        // Evento para búsqueda en el TextBox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = Buscar.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(texto))
            {
                GridDatos.ItemsSource = Medicaciones;
            }
            else
            {
                var filtrados = Medicaciones.Where(m =>
                    (m.NombrePaciente?.ToLower().Contains(texto) ?? false) ||
                    (m.NombreMedicamento?.ToLower().Contains(texto) ?? false) ||
                    (m.Dosis?.ToLower().Contains(texto) ?? false) ||
                    (m.Frecuencia?.ToLower().Contains(texto) ?? false) ||
                    (m.ViaAdministracion?.ToLower().Contains(texto) ?? false) ||
                    (m.PrescritoPor?.ToLower().Contains(texto) ?? false) ||
                    (m.Observaciones?.ToLower().Contains(texto) ?? false)
                ).ToList();
                GridDatos.ItemsSource = filtrados;
            }
        }

        // Evento para botón Agregar
        private void Agregar(object sender, RoutedEventArgs e)
        {
            var medicacionView = new Medicacion();
            FrameControlSolicitudes.Navigate(medicacionView);
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).CommandParameter is MedicacionModel medicacion)
            {
                var medicacionView = new Medicacion();
                medicacionView.CargarDatos(medicacion);
                FrameControlSolicitudes.Navigate(medicacionView);
            }
        }
    }
}
