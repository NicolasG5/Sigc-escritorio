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
   
    public partial class CustomerView3 : UserControl
    {
        public CustomerView3()
        {
            InitializeComponent();
            CargarDatos();
        }

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB"].ConnectionString);
        void CargarDatos()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select IdCliente, Nombre, Apellido, Telefono, Correo, NombrePrivilegio from Cliente inner join Privilegios on Cliente.Privilegio=Privilegios.IdPrivilegio order by IdCliente ASC", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            GridDatos.ItemsSource = dt.DefaultView;
            con.Close();

                
        }

        private void Agregar(object sender, RoutedEventArgs e)
        {
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
            FrameCustomerView3.Content = ventana;
            //ventana.BtnCrear.Visibility = Visibility.Visible;
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
        //    ventana.IdCliente = id;
        //    ventana.Consultar();
        //    FrameCustomerView3.Content = ventana;
        //    ventana.Titulo.Text = "Consultar Cliente";
        //    ventana.tbNombre.IsEnabled = false;
        //    ventana.tbApellido.IsEnabled = false;
        //    ventana.tbDUI.IsEnabled = false;
        //    ventana.tbNIT.IsEnabled = false;
        //    ventana.tbFecha.IsEnabled = false;
        //    ventana.tbTelefono.IsEnabled = false;
        //    ventana.tbCorreo.IsEnabled = false;
        //    ventana.cbPrivilegio.IsEnabled = false;
        //    ventana.tbUsuario.IsEnabled = false;
        //    ventana.tbContrasenia.IsEnabled = false;
        //    ventana.BtnSubir.IsEnabled = false;
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
            //ventana.IdCliente = id;
            //ventana.Consultar();
            //FrameCustomerView3.Content = ventana;
            ////ventana.Titulo.Text = "Actualizar Cliente";
            ////ventana.tbNombre.IsEnabled = true;
            ////ventana.tbApellido.IsEnabled = true;
            ////ventana.tbDUI.IsEnabled = true;
            ////ventana.tbNIT.IsEnabled = true;
            ////ventana.tbFecha.IsEnabled = true;
            ////ventana.tbTelefono.IsEnabled = true;
            ////ventana.tbCorreo.IsEnabled = true;
            ////ventana.cbPrivilegio.IsEnabled = true;
            ////ventana.tbUsuario.IsEnabled = true;
            ////ventana.tbContrasenia.IsEnabled = true;
            ////ventana.BtnSubir.IsEnabled = true;
            //ventana.BtnActualizar.Visibility = Visibility.Visible;
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
            //ventana.IdCliente = id;
            //ventana.Consultar();
            //FrameCustomerView3.Content = ventana;
            //ventana.Titulo.Text = "Eliminar Cliente";
            //ventana.tbNombre.IsEnabled = false;
            //ventana.tbApellido.IsEnabled = false;
            //ventana.tbDUI.IsEnabled = false;
            //ventana.tbNIT.IsEnabled = false;
            //ventana.tbFecha.IsEnabled = false;
            //ventana.tbTelefono.IsEnabled = false;
            //ventana.tbCorreo.IsEnabled = false;
            //ventana.cbPrivilegio.IsEnabled = false;
            //ventana.tbUsuario.IsEnabled = false;
            //ventana.tbContrasenia.IsEnabled = false;
            //ventana.BtnSubir.IsEnabled = false;
            //ventana.BtnEliminar.Visibility = Visibility.Visible;
        }
    }
}
