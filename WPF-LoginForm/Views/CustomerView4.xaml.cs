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
    /// Lógica de interacción para CustomerView4.xaml
    /// </summary>
    public partial class CustomerView4 : UserControl
    {
        public CustomerView4()
        {
            InitializeComponent();
            CargarDatos();
        }
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);
        void CargarDatos()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select Id,Run, Nombre, A_paterno,A_materno, tipo_comuna from Empleados inner join comuna on Empleados.comuna=comuna.id_comuna order by Id ASC", con);
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
            dt.DefaultView.RowFilter = $"Nombre LIKE '%{texto}%' OR A_paterno LIKE '%{texto}%' OR A_materno LIKE '%{texto}%'";
        }
        #endregion
        private void Agregar(object sender, RoutedEventArgs e)
        {
            CrudEmpleado ventana = new CrudEmpleado();
            FrameCustomerView4.Content = ventana;
            ventana.BtnCrear.Visibility = Visibility.Visible;
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudEmpleado ventana = new CrudEmpleado();
            ventana.Id = id;
            ventana.Consultar();
            FrameCustomerView4.Content = ventana;
            ventana.Titulo.Text = "Consultar Empleado";
            ventana.tbRun.IsEnabled = false;
            ventana.tbNombre.IsEnabled = false;
            ventana.tbA_paterno.IsEnabled = false;
            ventana.tbA_materno.IsEnabled = false;

            ventana.tbFecha.IsEnabled = false;

            //ventana.cbComuna.IsEnabled = false;



        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudEmpleado ventana = new CrudEmpleado();
            ventana.Id = id;
            ventana.Consultar();
            FrameCustomerView4.Content = ventana;
            ventana.Titulo.Text = "Actualizar Empleado";
            ventana.tbRun.IsEnabled = true;
            ventana.tbNombre.IsEnabled = true;
            ventana.tbA_paterno.IsEnabled = true;
            ventana.tbA_materno.IsEnabled = true;


            ventana.tbFecha.IsEnabled = true;

            //ventana.cbComuna.IsEnabled = true;



            ventana.BtnActualizar.Visibility = Visibility.Visible;
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudEmpleado ventana = new CrudEmpleado();
            ventana.Id = id;
            ventana.Consultar();
            FrameCustomerView4.Content = ventana;
            ventana.Titulo.Text = "Eliminar Empleado";
            ventana.tbRun.IsEnabled = false;
            ventana.tbNombre.IsEnabled = false;
            ventana.tbA_paterno.IsEnabled = false;
            ventana.tbA_materno.IsEnabled = false;

            ventana.tbFecha.IsEnabled = false;

            //ventana.cbComuna.IsEnabled = false;


            ventana.BtnEliminar.Visibility = Visibility.Visible;
        }
    }
}
