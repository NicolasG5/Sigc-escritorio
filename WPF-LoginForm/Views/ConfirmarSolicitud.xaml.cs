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
using System.Security.Cryptography;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para CrudClientes.xaml
    /// </summary>
    public partial class ConfirmarSolicitud : Page
    {
        public ConfirmarSolicitud()
        {
            InitializeComponent();
            CargarCB();
            CargarCB2();
            CargarCB3();


        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new ControlSolicitudes();

        }
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);
        void CargarCB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT tipo_comuna FROM comuna", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
               
            }
            con.Close();
        }



        void CargarCB2()
        {
            con.Open();
            SqlCommand cmd2 = new SqlCommand("SELECT tipo_propiedad FROM propiedad", con);
            SqlDataReader dr = cmd2.ExecuteReader();
            while (dr.Read())
            {
                //cbPropiedad.Items.Add(dr["tipo_propiedad"].ToString());
            }
            con.Close();

        }

        void CargarCB3()
        {
            con.Open();
            SqlCommand cmd3 = new SqlCommand("SELECT tipo_credito FROM hipotecario", con);
            SqlDataReader dr = cmd3.ExecuteReader();
            while (dr.Read())
            {
                //cbHipotecario.Items.Add(dr["tipo_credito"].ToString());
            }
            con.Close();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        #region CRUD (create, read, update, delete)
        public int id_cliente;
        #region Crear
        private void Crear(object sender, RoutedEventArgs e)
        {
            if (tbNombre.Text == "" )
            {
                MessageBox.Show("Los campos no pueden quedar vacíos");
            }
            else
            {
                con.Open();

                //string patron = "WPFLoginForm";
                //using (SqlCommand cmd = new SqlCommand("INSERT INTO cliente (Nombre_c, A_paterno, A_materno, Correo, usuario, contrasenia, comuna, propiedad,hipotecario) VALUES (@Nombre_c, @A_paterno, @A_materno, @Correo, @usuario, (EncryptByPassPhrase('" + patron + "', '" + tbContrasenia.Text + "')), @comuna, @propiedad,@hipotecario); SELECT SCOPE_IDENTITY()", con))
                //{
                //    //cmd.Parameters.AddWithValue("@Nombre_c", tbNombre.Text);
                //    //cmd.Parameters.AddWithValue("@A_paterno", tbA_paterno.Text);
                //    //cmd.Parameters.AddWithValue("@A_materno", tbA_materno.Text);
                //    //cmd.Parameters.AddWithValue("@Correo", tbCorreo.Text);
                //    //cmd.Parameters.AddWithValue("@usuario", tbUsuario.Text);

                //    // Fetch comuna ID
                //    SqlCommand cmdComuna = new SqlCommand("SELECT id_comuna FROM comuna WHERE tipo_comuna = @tipoComuna", con);
                //    //cmdComuna.Parameters.AddWithValue("@tipoComuna", cbComuna.Text);
                //    int comuna = (int)cmdComuna.ExecuteScalar();
                //    cmd.Parameters.AddWithValue("@comuna", comuna);

                //    // Fetch propiedad ID
                //    SqlCommand cmdPropiedad = new SqlCommand("SELECT id_propiedad FROM propiedad WHERE tipo_propiedad = @tipoPropiedad", con);
                //    //cmdPropiedad.Parameters.AddWithValue("@tipoPropiedad", cbPropiedad.Text);
                //    int propiedad = (int)cmdPropiedad.ExecuteScalar();
                //    cmd.Parameters.AddWithValue("@propiedad", propiedad);


                //    SqlCommand cmdHipotecario = new SqlCommand("SELECT Id FROM hipotecario WHERE tipo_credito = @tipo_credito", con);
                //    //cmdHipotecario.Parameters.AddWithValue("@tipo_credito", cbHipotecario.Text);
                //    int hipotecario = (int)cmdHipotecario.ExecuteScalar();
                //    cmd.Parameters.AddWithValue("@hipotecario", hipotecario);

                //    cmd.ExecuteNonQuery(); // Ejecutar la consulta para insertar los datos


                //}

                //con.Close();

                Content = new CustomerView5();
            }
        }
        #endregion

        #region Consultar
        public void Consultar()
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT c.Nombre_c, c.A_paterno, c.A_materno, c.Correo, c.usuario, co.tipo_comuna, p.tipo_propiedad, h.tipo_credito FROM cliente c JOIN comuna co ON c.comuna = co.id_comuna JOIN propiedad p ON c.propiedad = p.id_propiedad JOIN hipotecario h ON c.hipotecario = h.Id WHERE c.id_cliente = @id_cliente", con))
            {
                cmd.Parameters.AddWithValue("@id_cliente", id_cliente);

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (rdr.Read())
                    {
                        tbNombre.Text = rdr["Nombre_c"].ToString();
                        
                        //tbUsuario.Text = rdr["usuario"].ToString();
                        //cbComuna.SelectedItem = rdr["tipo_comuna"].ToString();
                        //cbPropiedad.SelectedItem = rdr["tipo_propiedad"].ToString();
                        //cbHipotecario.SelectedItem = rdr["tipo_credito"].ToString();

                        /*tbContrasenia.Clear();*/ // Limpiar el TextBox de la contraseña
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

            using (SqlCommand cmd = new SqlCommand("EliminarCliente", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", id_cliente);
                cmd.ExecuteNonQuery();
            }

            con.Close();

            Content = new CustomerView5();
        }
        #endregion
        #region Actualizar
        string patron = "WPFLoginForm";
        private void Actualizar(object sender, RoutedEventArgs e)
        {
            if (tbNombre.Text == "" )
            {
                MessageBox.Show("Los campos no pueden quedar vacíos");
            }
            else
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("UPDATE cliente SET Nombre_c = @Nombre_c, A_paterno = @A_paterno, A_materno = @A_materno, Correo = @Correo, usuario = @usuario, comuna = @comuna, propiedad = @propiedad, hipotecario = @hipotecario WHERE id_cliente = @id_cliente", con))
                {
                    cmd.Parameters.AddWithValue("@Nombre_c", tbNombre.Text);
                    
                    //cmd.Parameters.AddWithValue("@usuario", tbUsuario.Text);

                    //if (tbContrasenia.Text != "")
                    //{
                    //    SqlCommand com = new SqlCommand("Update cliente set contrasenia=(EncryptByPassPhrase('" + patron + "','" + tbContrasenia.Text + "')) where id_Cliente='" + id_cliente + "'", con);
                    //    com.ExecuteNonQuery();
                    //}

                    // Fetch comuna ID
                    SqlCommand cmdComuna = new SqlCommand("SELECT id_comuna FROM comuna WHERE tipo_comuna = @tipoComuna", con);
                    //cmdComuna.Parameters.AddWithValue("@tipoComuna", cbComuna.Text);
                    int comuna = (int)cmdComuna.ExecuteScalar();
                    cmd.Parameters.AddWithValue("@comuna", comuna);

                    // Fetch propiedad ID
                    SqlCommand cmdPropiedad = new SqlCommand("SELECT id_propiedad FROM propiedad WHERE tipo_propiedad = @tipoPropiedad", con);
                    //cmdPropiedad.Parameters.AddWithValue("@tipoPropiedad", cbPropiedad.Text);
                    int propiedad = (int)cmdPropiedad.ExecuteScalar();
                    cmd.Parameters.AddWithValue("@propiedad", propiedad);

                    // Fetch hipotecario ID
                    SqlCommand cmdHipotecario = new SqlCommand("SELECT Id FROM hipotecario WHERE tipo_credito = @tipoCredito", con);
                    //cmdHipotecario.Parameters.AddWithValue("@tipoCredito", cbHipotecario.Text);
                    int hipotecario = (int)cmdHipotecario.ExecuteScalar();
                    cmd.Parameters.AddWithValue("@hipotecario", hipotecario);

                    cmd.Parameters.AddWithValue("@id_cliente", id_cliente);

                    cmd.ExecuteNonQuery();
                }

                con.Close();

                Content = new CustomerView5();
            }
        }
        #endregion
        public void Enviar(object sender, RoutedEventArgs e)
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT c.Nombre_c, c.A_paterno, c.A_materno, c.Correo, c.usuario, co.tipo_comuna, p.tipo_propiedad, h.tipo_credito FROM cliente c JOIN comuna co ON c.comuna = co.id_comuna JOIN propiedad p ON c.propiedad = p.id_propiedad JOIN hipotecario h ON c.hipotecario = h.Id WHERE c.id_cliente = @id_cliente", con))
            {
                cmd.Parameters.AddWithValue("@id_cliente", id_cliente);

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (rdr.Read())
                    {
                        tbNombre.Text = rdr["Nombre_c"].ToString();

                        //tbUsuario.Text = rdr["usuario"].ToString();
                        //cbComuna.SelectedItem = rdr["tipo_comuna"].ToString();
                        //cbPropiedad.SelectedItem = rdr["tipo_propiedad"].ToString();
                        //cbHipotecario.SelectedItem = rdr["tipo_credito"].ToString();

                        /*tbContrasenia.Clear();*/ // Limpiar el TextBox de la contraseña
                    }
                }
            }

            con.Close();
        }

        

        #endregion
    }
}
