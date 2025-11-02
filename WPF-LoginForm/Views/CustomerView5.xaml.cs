using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para CustomerView5.xaml
    /// </summary>
    public partial class CustomerView5 : UserControl
    {
        public CustomerView5()
        {
            InitializeComponent();
            CargarDatos();
        }

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);
        void CargarDatos()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT c.id_cliente, c.Nombre_c, c.A_paterno, c.A_materno, co.tipo_comuna, p.tipo_propiedad, h.tipo_credito FROM cliente c INNER JOIN comuna co ON c.comuna = co.id_comuna INNER JOIN propiedad p ON c.propiedad = p.id_propiedad INNER JOIN hipotecario h ON c.hipotecario = h.Id ORDER BY c.id_cliente ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            GridDatos.ItemsSource = dt.DefaultView;
            con.Close();


        }
        #region buscar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Obtener el texto actual del TextBox
            string texto = Buscar.Text;

            // Obtener el DataTable actual del DataGrid
            DataTable dt = ((DataView)GridDatos.ItemsSource).Table;

            // Aplicar el filtro en el DataView asociado al DataTable
            dt.DefaultView.RowFilter = $"Nombre_c LIKE '%{texto}%' OR A_paterno LIKE '%{texto}%' OR A_materno LIKE '%{texto}%'";
        }
        #endregion
        private void Agregar(object sender, RoutedEventArgs e)
        {
            Paciente ventana = new Paciente();
            FrameCustomerView5.Content = ventana;
            //ventana.BtnCrear.Visibility = Visibility.Visible;
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            Paciente ventana = new Paciente();
            //ventana.id_cliente = id;
            ventana.Consultar();
            FrameCustomerView5.Content = ventana;
            ventana.Titulo.Text = "Consultar Cliente";
            ventana.tbNombre.IsEnabled = false;
            //ventana.tbA_paterno.IsEnabled = false;
            //ventana.tbA_materno.IsEnabled = false;
            //ventana.tbCorreo.IsEnabled = false;
            //ventana.cbComuna.IsEnabled = false;
            //ventana.cbPropiedad.IsEnabled = false;
            //ventana.cbHipotecario.IsEnabled = false;



        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            Paciente ventana = new Paciente();
            //ventana.id_cliente = id;
            ventana.Consultar();
            FrameCustomerView5.Content = ventana;
            ventana.Titulo.Text = "Actualizar Cliente";
            ventana.tbNombre.IsEnabled = true;
            //ventana.tbA_paterno.IsEnabled = true;
            //ventana.tbA_materno.IsEnabled = true;
            //ventana.tbCorreo.IsEnabled = true;
            //ventana.cbComuna.IsEnabled = true;
            //ventana.cbPropiedad.IsEnabled = true;
            //ventana.cbHipotecario.IsEnabled = true;
            //ventana.BtnActualizar.Visibility = Visibility.Visible;
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            Paciente ventana = new Paciente();
            //ventana.id_cliente = id;
            ventana.Consultar();
            FrameCustomerView5.Content = ventana;
            //ventana.Titulo.Text = "Eliminar Cliente";
            //ventana.tbNombre.IsEnabled = false;
            //ventana.tbA_paterno.IsEnabled = false;
            //ventana.tbA_materno.IsEnabled = false;
            //ventana.tbCorreo.IsEnabled = false;
            ////ventana.cbComuna.IsEnabled = false;
            ////ventana.cbPropiedad.IsEnabled = false;
            //ventana.cbHipotecario.IsEnabled = false;
            //ventana.BtnEliminar.Visibility = Visibility.Visible;
        }

    }
}
