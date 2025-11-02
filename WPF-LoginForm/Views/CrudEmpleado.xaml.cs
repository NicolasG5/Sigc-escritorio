using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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
    
    public partial class CrudEmpleado : Page
    {
        public CrudEmpleado()
        {
            InitializeComponent();
            CargarCB();
            
        }
        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new CustomerView4();

        }
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);
        void CargarCB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select tipo_comuna from comuna", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //cbComuna.Items.Add(dr["tipo_comuna"].ToString());
            }
            con.Close();
        }

        #region CRUD (create, read, update, delete)
        public int Id;

        #region Crear
        private void Crear(object sender, RoutedEventArgs e)
        {
            if (tbNombre.Text == "" || tbA_paterno.Text == "" || tbA_materno.Text == "" || tbFecha.Text == "" || /*cbComuna.Text == ""*/ tbUsuario.Text == "" || tbContrasenia.Text == "")
            {
                MessageBox.Show("Los campos no pueden quedar vacíos");
            }
            else
            {
                con.Open();

                string patron = "WPFLoginForm";
                using (SqlCommand cmd = new SqlCommand("INSERT INTO empleados (Run, Nombre, A_paterno, A_materno, Fecha_nacimiento, usuario, contrasenia, comuna) VALUES (@Run, @Nombre, @A_paterno, @A_materno, @Fecha_nacimiento, @usuario, (EncryptByPassPhrase('" + patron + "','" + tbContrasenia.Text + "')), @comuna)", con))
                {
                    cmd.Parameters.AddWithValue("@Run", tbRun.Text);
                    cmd.Parameters.AddWithValue("@Nombre", tbNombre.Text);
                    cmd.Parameters.AddWithValue("@A_paterno", tbA_paterno.Text);
                    cmd.Parameters.AddWithValue("@A_materno", tbA_materno.Text);
                    cmd.Parameters.AddWithValue("@Fecha_nacimiento", tbFecha.Text);
                    cmd.Parameters.AddWithValue("@usuario", tbUsuario.Text);

                    SqlCommand cmdComuna = new SqlCommand("SELECT id_comuna FROM comuna WHERE tipo_comuna = @tipoComuna", con);
                    //cmdComuna.Parameters.AddWithValue("@tipoComuna", cbComuna.Text);
                    int comuna = (int)cmdComuna.ExecuteScalar();
                    //cmd.Parameters.AddWithValue("@comuna", comuna);


                    cmd.ExecuteNonQuery();
                }

                con.Close();

                Content = new CustomerView4();
            }
        }
        #endregion

        #region Consultar
        public void Consultar()
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT e.Run, e.Nombre, e.A_paterno, e.A_materno, e.Fecha_nacimiento, c.tipo_comuna, e.usuario FROM empleados e INNER JOIN comuna c ON e.comuna = c.id_comuna WHERE e.Id = @Id", con))
            {
                cmd.Parameters.AddWithValue("@Id", Id);

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (rdr.Read())
                    {
                        tbRun.Text = rdr["Run"].ToString();
                        tbNombre.Text = rdr["Nombre"].ToString();
                        tbA_paterno.Text = rdr["A_paterno"].ToString();
                        tbA_materno.Text = rdr["A_materno"].ToString();
                        tbFecha.Text = rdr["Fecha_nacimiento"].ToString();
                        //cbComuna.SelectedItem = rdr["tipo_comuna"].ToString();
                        tbUsuario.Text = rdr["usuario"].ToString();
                    }
                }
            }

            con.Close();
        }
        #endregion
        #region Eliminar
        private void Eliminar(object sender, RoutedEventArgs e)
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("EliminarEmpleado", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);

                cmd.ExecuteNonQuery();
            }

            con.Close();
            Content = new CustomerView4();
        }
        #endregion
        #region Actualizar
        string patron = "WPFLoginForm";
        private void Actualizar(object sender, RoutedEventArgs e)
        {
            if (tbNombre.Text == "" || tbA_paterno.Text == "" || tbA_materno.Text == "" || tbFecha.Text == "" || /*cbComuna.Text == ""*/  tbUsuario.Text == "")
            {
                MessageBox.Show("Los campos no pueden quedar vacíos");
            }
            else
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("UPDATE empleados SET Nombre = @Nombre, A_paterno = @A_paterno, A_materno = @A_materno, Fecha_nacimiento = @Fecha_nacimiento, usuario = @usuario, comuna = @comuna WHERE Id = @Id", con))
                {
                    cmd.Parameters.AddWithValue("@Nombre", tbNombre.Text);
                    cmd.Parameters.AddWithValue("@A_paterno", tbA_paterno.Text);
                    cmd.Parameters.AddWithValue("@A_materno", tbA_materno.Text);
                    cmd.Parameters.AddWithValue("@Fecha_nacimiento", tbFecha.Text);
                    cmd.Parameters.AddWithValue("@usuario", tbUsuario.Text);

                    if (tbContrasenia.Text != "")
                    {
                        SqlCommand com = new SqlCommand("Update empleados set contrasenia=(EncryptByPassPhrase('" + patron + "','" + tbContrasenia.Text + "')) where Id='" + Id + "'", con);
                        com.ExecuteNonQuery();

                    }

                    SqlCommand cmdComuna = new SqlCommand("SELECT id_comuna FROM comuna WHERE tipo_comuna = @tipoComuna", con);
                    //cmdComuna.Parameters.AddWithValue("@tipoComuna", cbComuna.Text);
                    int comuna = (int)cmdComuna.ExecuteScalar();
                    cmd.Parameters.AddWithValue("@comuna", comuna);


                    cmd.Parameters.AddWithValue("@Id", Id);

                    cmd.ExecuteNonQuery();
                }

                con.Close();

                Content = new CustomerView4();
            }
        }
        #endregion
        #endregion

    }
}
