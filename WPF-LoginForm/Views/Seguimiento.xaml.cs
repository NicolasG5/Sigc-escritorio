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
using WPF_LoginForm.Services;
using System.Collections.ObjectModel;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para Seguimiento.xaml
    /// </summary>
    public partial class Seguimiento : UserControl
    {
        private readonly SeguimientoApiService _seguimientoService = new SeguimientoApiService();
        private readonly PacienteApiService _pacienteService = new PacienteApiService();

        public ObservableCollection<SeguimientoGridItem> Seguimientos { get; set; } = new ObservableCollection<SeguimientoGridItem>();

        public Seguimiento()
        {
            InitializeComponent();
            _ = CargarSeguimientosAsync();
        }

        public class SeguimientoGridItem
        {
            public int IdSeguimiento { get; set; }
            public string NombrePaciente { get; set; }
            public string FechaSeguimiento { get; set; }
            public string EstadoAnimo { get; set; }
            public string AdherenciaTratamiento { get; set; }
            public string Observaciones { get; set; }
            public string ProximaEvaluacion { get; set; }
        }

        private async Task CargarSeguimientosAsync()
        {
            var lista = await _seguimientoService.GetAllSeguimientosAsync();
            Seguimientos.Clear();
            if (lista != null)
            {
                foreach (var s in lista)
                {
                    string nombrePaciente = "";
                    var paciente = await _pacienteService.GetPacienteByIdAsync(s.id_paciente);
                    if (paciente != null)
                        nombrePaciente = paciente.NombreCompleto;
                    Seguimientos.Add(new SeguimientoGridItem
                    {
                        IdSeguimiento = s.id_seguimiento,
                        NombrePaciente = nombrePaciente,
                        FechaSeguimiento = s.fecha_seguimiento,
                        EstadoAnimo = s.estado_animo,
                        AdherenciaTratamiento = s.adherencia_tratamiento,
                        Observaciones = s.observaciones,
                        ProximaEvaluacion = s.proxima_evaluacion,
                    });
                }
            }
            GridDatos.ItemsSource = Seguimientos;
        }

        // Nuevo método para consultar y mostrar seguimiento
        public async Task MostrarSeguimiento(int idSeguimiento)
        {
            var seguimiento = await _seguimientoService.GetSeguimientoByIdAsync(idSeguimiento);
            if (seguimiento == null)
            {
                MessageBox.Show("No se encontró seguimiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var paciente = await _pacienteService.GetPacienteByIdAsync(seguimiento.id_paciente);
            string nombrePaciente = paciente?.NombreCompleto ?? "";
            tbPaciente.Text = nombrePaciente;
            tbFechaSeguimiento.Text = seguimiento.fecha_seguimiento;
            tbEstadoAnimo.Text = seguimiento.estado_animo;
            tbAdherencia.Text = seguimiento.adherencia_tratamiento;
            tbObservaciones.Text = seguimiento.observaciones;
            tbProximaEvaluacion.Text = seguimiento.proxima_evaluacion;
        }

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);

        void CargarDatos()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT s.Id, s.FechaSolicitud, s.Cliente, s.Descripcion,s.FechaInicio, s.Estado, ts.NombreServicio AS TipoServicio FROM SolicitudServicio s INNER JOIN TipoServicio ts ON s.TipoServicio = ts.id_tipoServicio ORDER BY s.Id ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            GridDatos.ItemsSource = dt.DefaultView;
            con.Close();
        }

        // Evento para búsqueda en el TextBox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //string texto = Buscar.Text;
            //DataTable dt = ((DataView)GridDatos.ItemsSource).Table;
            //dt.DefaultView.RowFilter = $"Cliente LIKE '%{texto}%' OR Descripcion LIKE '%{texto}%' OR Estado LIKE '%{texto}%' OR TipoServicio LIKE '%{texto}%'";
        }

        // Evento para botón Agregar
        private void Agregar(object sender, RoutedEventArgs e)
        {
            CrearCita ventana = new CrearCita();
            // FrameControlSolicitudes.Content = ventana;
            //ventana.BtnCrear.Visibility = Visibility.Visible;
        }

        // Evento para botón Denegar
        private void Denegar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            //ventana.id_solicitud = id;
            //ventana.Consultar();
            //FrameControlSolicitudes.Content = ventana;
            //ventana.Titulo.Text = "Consultar Servicio";
            //ventana.tbCliente.IsEnabled = false;
            //ventana.tbDescripcion.IsEnabled = false;
            //ventana.cbTipoServicio.IsEnabled = false;
            //ventana.tbFechaSolicitud.IsEnabled = false;
            //ventana.tbFechaInicio.IsEnabled = false;
            //ventana.tbEstado.IsEnabled = false;
            // ventana.BtnDenegar.Visibility = Visibility.Visible;
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE SolicitudServicio SET Estado = 'Denegado' WHERE Id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        // Evento para botón Confirmar
        private void Confirmar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            FormularioS ventana = new FormularioS(null, null);
            //ventana.id_solicitud = id;
            //ventana.Consultar();
            //FrameControlSolicitudes.Content = ventana;
            //ventana.Titulo.Text = "Actualizar Servicio";
            //ventana.tbCliente.IsEnabled = false;
            //ventana.tbDescripcion.IsEnabled = false;
            //ventana.cbTipoServicio.IsEnabled = false;
            //ventana.cbEquipo.IsEnabled = false;
            //ventana.tbFechaInicio.IsEnabled = false;
            //ventana.BtnEnviar.Visibility = Visibility.Visible;
            //con.Open();
            //using (SqlCommand cmd = new SqlCommand("UPDATE SolicitudServicio SET Estado = 'Confirmado' WHERE Id = @id", con))
            //{
            //    cmd.Parameters.AddWithValue("@id", id);
            //    cmd.ExecuteNonQuery();
            //}
            //con.Close();
        }

        private async void BtnConsultarSeguimiento_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Buscar.Text, out int idSeguimiento))
            {
                await MostrarSeguimiento(idSeguimiento);
            }
            else
            {
                MessageBox.Show("Ingrese un ID de seguimiento válido.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void ConsultarSeguimientoGrid_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).CommandParameter is int idSeguimiento)
            {
                var seguimiento = await _seguimientoService.GetSeguimientoByIdAsync(idSeguimiento);
                if (seguimiento == null)
                {
                    MessageBox.Show("No se encontró seguimiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var paciente = await _pacienteService.GetPacienteByIdAsync(seguimiento.id_paciente);
                var formularioS = new WPF_LoginForm.Views.FormularioS(null, paciente);
                formularioS.tbNombre.Text = paciente?.NombreCompleto ?? "";
                formularioS.tbFecha.Text = seguimiento.fecha_seguimiento;
                formularioS.tbFecha_Copiar1.Text = seguimiento.estado_animo;
                formularioS.tbFecha_Copiar.Text = seguimiento.adherencia_tratamiento;
                formularioS.tbFecha_Copiar4.Text = seguimiento.observaciones;
                formularioS.tbFecha_Copiar5.Text = seguimiento.proxima_evaluacion;
                FrameFormularioS.Navigate(formularioS);
            }
        }

    }
}
