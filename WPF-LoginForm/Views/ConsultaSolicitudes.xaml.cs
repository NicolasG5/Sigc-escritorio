using Python.Runtime;
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
    public partial class ConsultaSolicitudes : Page
    {
        public ConsultaSolicitudes()
        {
            InitializeComponent();
            CargarCBTipoServicio();
            AutocompletarFecha();
        }

        // Evento para botón Regresar
        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new Customer();
        }

        // Evento para botón Crear
        private void Crear(object sender, RoutedEventArgs e)
        {
            // Implementación original (puedes dejarlo vacío si no lo usas)
            if (tbCliente.Text == "" || tbDescripcion.Text == "" || tbFechaInicio.Text == "" || cbTipoServicio.SelectedItem == null)
            {
                MessageBox.Show("Los campos no pueden quedar vacíos");
            }
            else
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("INSERT INTO SolicitudServicio (Cliente, Descripcion, FechaInicio, TipoServicio) VALUES (@Cliente, @Descripcion, @FechaInicio, @TipoServicio)", con))
                {
                    cmd.Parameters.AddWithValue("@Cliente", tbCliente.Text);
                    cmd.Parameters.AddWithValue("@Descripcion", tbDescripcion.Text);
                    cmd.Parameters.AddWithValue("@FechaInicio", tbFechaInicio.Text);
                    cmd.Parameters.AddWithValue("@TipoServicio", ((KeyValuePair<int, string>)cbTipoServicio.SelectedItem).Key);

                    cmd.ExecuteNonQuery();
                }

                con.Close();

                Content = new Customer();
            }
        }

        // Evento para botón Eliminar
        private void Eliminar(object sender, RoutedEventArgs e)
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("DELETE FROM SolicitudServicio WHERE Id = @id_solicitud", con))
            {
                cmd.Parameters.AddWithValue("@id_solicitud", id_solicitud);
                cmd.ExecuteNonQuery();
            }

            con.Close();

            Content = new Customer();
        }

        // Evento para botón Actualizar
        private void Actualizar(object sender, RoutedEventArgs e)
        {
            if (tbCliente.Text == "" || tbDescripcion.Text == "" || tbFechaInicio.Text == "" || cbTipoServicio.SelectedItem == null)
            {
                MessageBox.Show("Los campos no pueden quedar vacíos");
            }
            else
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("UPDATE SolicitudServicio SET Cliente = @Cliente, Descripcion = @Descripcion, FechaInicio = @FechaInicio, TipoServicio = @TipoServicio WHERE Id = @id_solicitud", con))
                {
                    cmd.Parameters.AddWithValue("@id_solicitud", id_solicitud);
                    cmd.Parameters.AddWithValue("@Cliente", tbCliente.Text);
                    cmd.Parameters.AddWithValue("@Descripcion", tbDescripcion.Text);
                    cmd.Parameters.AddWithValue("@FechaInicio", tbFechaInicio.Text);
                    cmd.Parameters.AddWithValue("@TipoServicio", ((KeyValuePair<int, string>)cbTipoServicio.SelectedItem).Key);

                    cmd.ExecuteNonQuery();
                }

                con.Close();

                Content = new Customer();
            }
        }

        // Evento para ComboBox Propiedad
        private void cbPropiedad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Método vacío para evitar error CS1061.
        }

        // Métodos auxiliares y originales
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);

        private void AutocompletarFecha()
        {
            tbFechaSolicitud.Text = DateTime.Now.ToString(); // Establecer la fecha actual en el TextBox
        }

        void CargarCBTipoServicio()
        {
            try
            {
                using (Py.GIL())
                {
                    dynamic pyro = Py.Import("Pyro4");
                    dynamic proxy = pyro.Proxy("PYRO:obj_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX@localhost:50001");

                    List<dynamic> choices = new List<dynamic>(proxy.get_choices());

                    foreach (dynamic choice in choices)
                    {
                        cbTipoServicio.Items.Add(new KeyValuePair<int, string>((int)choice[0], choice[1].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las opciones de servicio: " + ex.Message);
            }
        }

        // CRUD completo
        public int id_solicitud;

        public void Consultar()
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT s.FechaSolicitud, s.Cliente, s.Descripcion, s.FechaInicio, s.Estado, t.NombreServicio AS TipoServicio FROM SolicitudServicio s INNER JOIN TipoServicio t ON s.TipoServicio = t.id_tipoServicio WHERE s.Id = @id_solicitud", con))
            {
                cmd.Parameters.AddWithValue("@id_solicitud", id_solicitud);

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (rdr.Read())
                    {
                        DateTime fechaSolicitud = rdr.GetDateTime(rdr.GetOrdinal("FechaSolicitud"));
                        tbFechaSolicitud.Text = fechaSolicitud.ToString(); // Asigna el valor convertido a string

                        if (!rdr.IsDBNull(rdr.GetOrdinal("FechaInicio")))
                        {
                            DateTime fechaInicio = rdr.GetDateTime(rdr.GetOrdinal("FechaInicio"));
                            tbFechaInicio.Text = fechaInicio.ToString(); // Asigna el valor convertido a string
                        }

                        tbCliente.Text = rdr.GetString(rdr.GetOrdinal("Cliente"));
                        tbDescripcion.Text = rdr.GetString(rdr.GetOrdinal("Descripcion"));
                        tbEstado.Text = rdr.GetString(rdr.GetOrdinal("Estado"));

                        string tipoServicio = rdr.GetString(rdr.GetOrdinal("TipoServicio"));
                        KeyValuePair<int, string> tipoServicioItem = cbTipoServicio.Items.Cast<KeyValuePair<int, string>>().FirstOrDefault(item => item.Value == tipoServicio);
                        if (tipoServicioItem.Key != 0)
                        {
                            cbTipoServicio.SelectedItem = tipoServicioItem;
                        }
                    }
                }
            }
        }
    }
}
