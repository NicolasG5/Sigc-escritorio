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
using System.Windows.Threading;
using System.Collections.ObjectModel;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para ReportesG.xaml
    /// </summary>
    public partial class ReportesG : UserControl
    {
        private readonly PacienteApiService _pacienteService = new PacienteApiService();
        private readonly TratamientoApiService _tratamientoService = new TratamientoApiService();
        private readonly SeguimientoApiService _seguimientoService = new SeguimientoApiService();
        private readonly MedicacionApiService _medicacionService = new MedicacionApiService();
        private readonly PsicologoApiService _psicologoService = new PsicologoApiService();

        public ObservableCollection<ReporteModel> Reportes { get; set; } = new ObservableCollection<ReporteModel>();
        private DispatcherTimer _autoRefreshTimer;

        public ReportesG()
        {
            InitializeComponent();
            GridDatos.ItemsSource = Reportes;
            _ = CargarDatosAsync();
            // Configurar timer para refresco automático cada 10 segundos
            _autoRefreshTimer = new DispatcherTimer();
            _autoRefreshTimer.Interval = TimeSpan.FromSeconds(10);
            _autoRefreshTimer.Tick += async (s, e) => await CargarDatosAsync();
            _autoRefreshTimer.Start();
        }

        private async Task CargarDatosAsync()
        {
            var pacientes = await _pacienteService.GetAllPacientesAsync();
            var tratamientos = await _tratamientoService.GetAllTratamientosAsync();
            var seguimientos = await _seguimientoService.GetAllSeguimientosAsync();
            var medicaciones = await _medicacionService.GetAllMedicacionesAsync();
            Reportes.Clear();
            foreach (var paciente in pacientes)
            {
                var tratamiento = tratamientos?.FirstOrDefault(t => t.IdPaciente == paciente.IdPaciente);
                List<SeguimientoResponse> segs = null;
                if (tratamiento != null)
                    segs = seguimientos?.Where(s => s.id_paciente == paciente.IdPaciente && s.id_tratamiento == tratamiento.IdTratamiento).ToList();
                else
                    segs = seguimientos?.Where(s => s.id_paciente == paciente.IdPaciente).ToList();
                var meds = medicaciones?.Where(m => m.IdPaciente == paciente.IdPaciente).ToList();
                PsicologoModel psicologo = null;
                if (tratamiento != null)
                {
                    psicologo = await _psicologoService.GetPsicologoByIdAsync(tratamiento.IdEmpleado);
                }
                Reportes.Add(new ReporteModel
                {
                    Id = paciente.IdPaciente,
                    Paciente = paciente,
                    Tratamiento = tratamiento,
                    Seguimientos = segs,
                    Medicaciones = meds,
                    Psicologo = psicologo,
                    FechaReporte = System.DateTime.Now.ToString("yyyy-MM-dd"),
                    Estado = tratamiento?.Estado
                });
            }
        }

        // Evento para búsqueda en el TextBox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = Buscar.Text?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(texto))
            {
                GridDatos.ItemsSource = Reportes;
            }
            else
            {
                var filtrados = Reportes.Where(r =>
                    (r.Paciente?.NombreCompleto?.ToLower().Contains(texto) ?? false) ||
                    (r.Psicologo?.Nombres?.ToLower().Contains(texto) ?? false) ||
                    (r.Tratamiento?.TipoTratamiento?.ToLower().Contains(texto) ?? false) ||
                    (r.Tratamiento?.Descripcion?.ToLower().Contains(texto) ?? false) ||
                    (r.Tratamiento?.Objetivos?.ToLower().Contains(texto) ?? false) ||
                    (r.FechaReporte?.ToLower().Contains(texto) ?? false)
                ).ToList();
                GridDatos.ItemsSource = filtrados;
            }
        }

        // Evento para botón Agregar
        private void Agregar(object sender, RoutedEventArgs e)
        {
            var ventana = new Reportes();
            ventana.DataContext = null; // No hay datos para agregar
            FrameReportesG.Visibility = Visibility.Visible;
            FrameReportesG.Content = ventana;
        }

        private void Reporte(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            var reporte = Reportes.FirstOrDefault(r => r.Id == id);
            var ventana = new Reportes();
            ventana.DataContext = reporte;
            FrameReportesG.Visibility = Visibility.Visible;
            FrameReportesG.Content = ventana;
        }
    }
}
