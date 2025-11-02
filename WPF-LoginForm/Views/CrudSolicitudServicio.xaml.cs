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

namespace WPF_LoginForm.Views
{
   
    public partial class CrudSolicitudServicio : Page
    {
        public CrudSolicitudServicio()
        {
            InitializeComponent();
            CargarCBTipoServicio();
            CargarCBEquipo();
            AutocompletarFecha();
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new Customer();
        }

        readonly SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conexionDB2"].ConnectionString);

        private void AutocompletarFecha()
        {
            //tbFechaSolicitud.Text = DateTime.Now.ToString(); // Establecer la fecha actual en el TextBox
        }

        void CargarCBTipoServicio()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT id_tipoServicio, NombreServicio FROM TipoServicio", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //cbTipoServicio.Items.Add(new KeyValuePair<int, string>((int)dr["id_tipoServicio"], dr["NombreServicio"].ToString()));
            }
            con.Close();
        }

        void CargarCBEquipo()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT Id, Nombre FROM EquipoTrabajo", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                //cbEquipo.Items.Add(new KeyValuePair<int, string>((int)dr["Id"], dr["Nombre"].ToString()));
            }
            con.Close();
        }

        #region CRUD (create, read, update, delete)
        public int id_solicitud;

        #region Crear
        private void Crear(object sender, RoutedEventArgs e)
        {
            //if (tbCliente.Text == "" || tbDescripcion.Text == "" || tbFechaInicio.Text == "" || tbHoraInicio.Text == "" || cbTipoServicio.SelectedItem == null || cbEquipo.SelectedItem == null)
            //{
            //    MessageBox.Show("Los campos no pueden quedar vacíos");
            //}
            //else
            //{
            //    con.Open();

            //    using (SqlCommand cmd = new SqlCommand("INSERT INTO SolicitudServicio (Cliente, Descripcion, FechaInicio, HoraInicio, TipoServicio, EquipoId) VALUES (@Cliente, @Descripcion, @FechaInicio, @HoraInicio, @TipoServicio, @EquipoId)", con))
            //    {
            //        cmd.Parameters.AddWithValue("@Cliente", tbCliente.Text);
            //        cmd.Parameters.AddWithValue("@Descripcion", tbDescripcion.Text);

            //        DateTime fechaInicio;
            //        if (DateTime.TryParse(tbFechaInicio.Text, out fechaInicio))
            //        {
            //            cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
            //        }
            //        else
            //        {
            //            MessageBox.Show("La fecha de inicio no es válida");
            //            return;
            //        }

            //        TimeSpan horaInicio;
            //        if (TimeSpan.TryParse(tbHoraInicio.Text, out horaInicio))
            //        {
            //            cmd.Parameters.AddWithValue("@HoraInicio", horaInicio.ToString(@"hh\:mm\:ss"));
            //        }
            //        else
            //        {
            //            MessageBox.Show("La hora de inicio no es válida");
            //            return;
            //        }

            //        cmd.Parameters.AddWithValue("@TipoServicio", ((KeyValuePair<int, string>)cbTipoServicio.SelectedItem).Key);
            //        cmd.Parameters.AddWithValue("@EquipoId", ((KeyValuePair<int, string>)cbEquipo.SelectedItem).Key);

            //        cmd.ExecuteNonQuery();
            //    }

            //    con.Close();

            //    var equipoSeleccionado = (KeyValuePair<int, string>)cbEquipo.SelectedItem;
            //    equipoSeleccionado = new KeyValuePair<int, string>(equipoSeleccionado.Key, equipoSeleccionado.Value + " (No disponible)");
            //    cbEquipo.Items[cbEquipo.SelectedIndex] = equipoSeleccionado;

            //    cbEquipo.SelectedItem = null;

            //    Content = new Customer();
            //}
        }
        #endregion

        #region Consultar
        public void Consultar()
        {
            con.Open();

            //using (SqlCommand cmd = new SqlCommand("SELECT s.FechaSolicitud, s.Cliente, s.Descripcion, s.FechaInicio, s.HoraInicio, s.Estado, t.NombreServicio AS TipoServicio, e.Nombre AS EquipoNombre FROM SolicitudServicio s INNER JOIN TipoServicio t ON s.TipoServicio = t.id_tipoServicio INNER JOIN EquipoTrabajo e ON s.EquipoId = e.Id WHERE s.Id = @id_solicitud", con))
            //{
            //    cmd.Parameters.AddWithValue("@id_solicitud", id_solicitud);

            //    using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            //    {
            //        if (rdr.Read())
            //        {
            //            DateTime fechaSolicitud = rdr.GetDateTime(rdr.GetOrdinal("FechaSolicitud"));
            //            tbFechaSolicitud.Text = fechaSolicitud.ToString();

            //            if (!rdr.IsDBNull(rdr.GetOrdinal("FechaInicio")))
            //            {
            //                DateTime fechaInicio = rdr.GetDateTime(rdr.GetOrdinal("FechaInicio"));
            //                tbFechaInicio.Text = fechaInicio.ToString();
            //            }

            //            if (!rdr.IsDBNull(rdr.GetOrdinal("HoraInicio")))
            //            {
            //                TimeSpan horaInicio = rdr.GetTimeSpan(rdr.GetOrdinal("HoraInicio"));
            //                tbHoraInicio.Text = horaInicio.ToString(@"hh\:mm\:ss");
            //            }

            //            tbCliente.Text = rdr.GetString(rdr.GetOrdinal("Cliente"));
            //            tbDescripcion.Text = rdr.GetString(rdr.GetOrdinal("Descripcion"));
            //            tbEstado.Text = rdr.GetString(rdr.GetOrdinal("Estado"));

            //            string tipoServicio = rdr.GetString(rdr.GetOrdinal("TipoServicio"));
            //            KeyValuePair<int, string> tipoServicioItem = cbTipoServicio.Items.Cast<KeyValuePair<int, string>>().FirstOrDefault(item => item.Value == tipoServicio);
            //            if (tipoServicioItem.Key != 0)
            //            {
            //                cbTipoServicio.SelectedItem = tipoServicioItem;
            //            }

            //            string equipoNombre = rdr.GetString(rdr.GetOrdinal("EquipoNombre"));
            //            KeyValuePair<int, string> equipoItem = cbEquipo.Items.Cast<KeyValuePair<int, string>>().FirstOrDefault(item => item.Value == equipoNombre);
            //            if (equipoItem.Key != 0)
            //            {
            //                cbEquipo.SelectedItem = equipoItem;
            //            }
            //        }
            //    }
            //}
        }
        #endregion

        #region Eliminar
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
        #endregion

        #region Actualizar
        private void Actualizar(object sender, RoutedEventArgs e)
        {
            //if (/*tbCliente.Text == "" || tbDescripcion.Text == "" || tbFechaInicio.Text == "" || tbHoraInicio.Text == "" || cbTipoServicio.SelectedItem == null || cbEquipo.SelectedItem == null*/)
            //{
            //    MessageBox.Show("Los campos no pueden quedar vacíos");
            //}
            //else
            //{
            //    con.Open();

            //    //using (SqlCommand cmd = new SqlCommand("UPDATE SolicitudServicio SET Cliente = @Cliente, Descripcion = @Descripcion, FechaInicio = @FechaInicio, HoraInicio = @HoraInicio, TipoServicio = @TipoServicio, EquipoId = @EquipoId WHERE Id = @id_solicitud", con))
            //    //{
            //    //    cmd.Parameters.AddWithValue("@id_solicitud", id_solicitud);
            //    //    //cmd.Parameters.AddWithValue("@Cliente", tbCliente.Text);
            //    //    //cmd.Parameters.AddWithValue("@Descripcion", tbDescripcion.Text);
            //    //    //cmd.Parameters.AddWithValue("@FechaInicio", tbFechaInicio.Text);

            //    //    TimeSpan horaInicio;
            //    //    if (TimeSpan.TryParse(/*tbHoraInicio.Text*/, out horaInicio))
            //    //    {
            //    //        cmd.Parameters.AddWithValue("@HoraInicio", horaInicio.ToString(@"hh\:mm\:ss"));
            //    //    }
            //    //    else
            //    //    {
            //    //        MessageBox.Show("La hora de inicio no es válida");
            //    //        return;
            //    //    }

            //    //    //cmd.Parameters.AddWithValue("@TipoServicio", ((KeyValuePair<int, string>)cbTipoServicio.SelectedItem).Key);
            //    //    //cmd.Parameters.AddWithValue("@EquipoId", ((KeyValuePair<int, string>)cbEquipo.SelectedItem).Key);

            //    //    cmd.ExecuteNonQuery();
            //    //}

            //    con.Close();

            //    Confirmar(id_solicitud);

            //    Content = new Customer();
            //}
        }
        #endregion

        #endregion

        private void Confirmar(int id)
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE SolicitudServicio SET Estado = 'Confirmado' WHERE Id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        private void Denegar(int id)
        {
            con.Open();
            using (SqlCommand cmd = new SqlCommand("UPDATE SolicitudServicio SET Estado = 'Denegado' WHERE Id = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            con.Close();
        }

        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Confirmar(id_solicitud);
            Content = new ControlSolicitudes();
        }

        private void BtnDenegar_Click(object sender, RoutedEventArgs e)
        {
            Denegar(id_solicitud);
            Content = new ControlSolicitudes();
        }
    }
}
