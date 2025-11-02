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

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para Seguimiento.xaml
    /// </summary>
    public partial class Seguimiento : UserControl
    {
        public Seguimiento()
        {
            InitializeComponent();
            CargarDatos();
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
            FrameControlSolicitudes.Content = ventana;
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
            ventana.BtnDenegar.Visibility = Visibility.Visible;
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
            FormularioS ventana = new FormularioS();
            //ventana.id_solicitud = id;
            //ventana.Consultar();
            FrameControlSolicitudes.Content = ventana;
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

    }
}
