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

using WPF_LoginForm.Views;


namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para Customer.xaml
    /// </summary>
    public partial class Customer : UserControl
    {
        public Customer()
        {
            InitializeComponent();
            CargarDatos();
        }

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);

        void CargarDatos()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT s.Id, s.FechaSolicitud, s.Cliente, s.Descripcion, s.FechaInicio, s.HoraInicio, s.Estado, ts.NombreServicio AS TipoServicio FROM SolicitudServicio s INNER JOIN TipoServicio ts ON s.TipoServicio = ts.id_tipoServicio ORDER BY s.Id ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            GridDatos.ItemsSource = dt.DefaultView;
            con.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Obtener el texto actual del TextBox
            string texto = Buscar.Text;

            // Obtener el DataTable actual del DataGrid
            DataTable dt = ((DataView)GridDatos.ItemsSource).Table;

            // Aplicar el filtro en el DataView asociado al DataTable
            dt.DefaultView.RowFilter = $"Cliente LIKE '%{texto}%' OR Descripcion LIKE '%{texto}%' OR Estado LIKE '%{texto}%' OR TipoServicio LIKE '%{texto}%'";
        }

        private void Agregar(object sender, RoutedEventArgs e)
        {
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            FrameCustomer.Content = ventana;
            ventana.BtnCrear.Visibility = Visibility.Visible;
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            ventana.id_solicitud = id;
            ventana.Consultar();
            FrameCustomer.Content = ventana;
            ventana.Titulo.Text = "Consultar Servicio";
            //ventana.tbCliente.IsEnabled = false;
            //ventana.tbDescripcion.IsEnabled = false;
            //ventana.cbTipoServicio.IsEnabled = false;
            //ventana.tbFechaSolicitud.IsEnabled = false;
            //ventana.tbFechaInicio.IsEnabled = false;
            //ventana.tbEstado.IsEnabled = false;
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            ventana.id_solicitud = id;
            ventana.Consultar();
            FrameCustomer.Content = ventana;
            ventana.Titulo.Text = "Actualizar Servicio";
            //ventana.tbCliente.IsEnabled = false;
            //ventana.tbDescripcion.IsEnabled = true;
            //ventana.cbTipoServicio.IsEnabled = false;
            //ventana.cbEquipo.IsEnabled = false;
            //ventana.tbFechaInicio.IsEnabled = false;
            ventana.BtnActualizar.Visibility = Visibility.Visible;
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            ventana.id_solicitud = id;
            ventana.Consultar();
            FrameCustomer.Content = ventana;
            ventana.Titulo.Text = "Eliminar Servicio";
            //ventana.tbCliente.IsEnabled = false;
            //ventana.tbDescripcion.IsEnabled = false;
            //ventana.cbTipoServicio.IsEnabled = false;
            //ventana.tbFechaSolicitud.IsEnabled = false;
            //ventana.tbFechaInicio.IsEnabled = false;
            //ventana.tbEstado.IsEnabled = false;
            ventana.BtnEliminar.Visibility = Visibility.Visible;
        }

    }
}









