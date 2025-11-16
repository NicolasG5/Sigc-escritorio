using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
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
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;
using Newtonsoft.Json;

namespace WPF_LoginForm.Views
{
   
    public partial class CrudSolicitudServicio : Page
    {
        private readonly CitaApiService _citaService = new CitaApiService();
        private readonly ServicioApiService _servicioService = new ServicioApiService();
        private readonly PsicologoApiService _psicologoService = new PsicologoApiService();
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://147.182.240.177:8000/") };

        public CrudSolicitudServicio()
        {
            InitializeComponent();
            _ = CargarServiciosYEmpleadosAsync();
            cbEmpleado.SelectionChanged += cbEmpleado_SelectionChanged;
            dpFechaCita.SelectedDateChanged += dpFechaCita_SelectedDateChanged;
            cbHoraInicio.SelectionChanged += cbHoraInicio_SelectionChanged;
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            // Navega a la vista principal de clientes/solicitudes
            if (this.Parent is Frame frame)
            {
                frame.Content = new Customer();
            }
            else
            {
                // Fallback: reemplaza el contenido de la página
                this.Content = new Customer();
            }
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

        private async Task CargarServiciosYEmpleadosAsync()
        {
            // Servicios activos
            var servicios = await _servicioService.GetServiciosActivosAsync();
            cbServicio.ItemsSource = servicios.ToList();
            cbServicio.DisplayMemberPath = "DisplayName";
            cbServicio.SelectedValuePath = "IdServicio";

            // Empleados disponibles
            var token = Repositories.ApiTokenStore.Instance.Token;
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/empleados/disponibles");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var empleados = JsonConvert.DeserializeObject<List<PsicologoModel>>(json);
                cbEmpleado.ItemsSource = empleados;
                cbEmpleado.DisplayMemberPath = "DisplayName";
                cbEmpleado.SelectedValuePath = "IdEmpleado";
            }
        }

        private async void cbEmpleado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await ActualizarHorasDisponiblesAsync();
        }

        private async void dpFechaCita_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            await ActualizarHorasDisponiblesAsync();
        }

        private async Task ActualizarHorasDisponiblesAsync()
        {
            cbHoraInicio.ItemsSource = null;
            tbHoraFin.Text = "";
            if (cbEmpleado.SelectedItem is PsicologoModel empleado && dpFechaCita.SelectedDate.HasValue)
            {
                var token = Repositories.ApiTokenStore.Instance.Token;
                var fecha = dpFechaCita.SelectedDate.Value.ToString("yyyy-MM-dd"); // Formato correcto
                var url = $"/api/v1/empleados/disponibilidad?psicologo_id={empleado.IdEmpleado}&fecha={fecha}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("accept", "application/json");
                request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var horas = JsonConvert.DeserializeObject<List<HoraDisponibilidadModel>>(json);
                    var horasDisponibles = horas.Where(h => h.disponible && !h.ocupado && !h.pasado).Select(h => h.hora).ToList();
                    cbHoraInicio.ItemsSource = horasDisponibles;
                }
            }
        }

        private void cbHoraInicio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbHoraFin.Text = "";
            if (cbHoraInicio.SelectedItem is string horaInicio && cbServicio.SelectedItem is ServicioModel servicio)
            {
                TimeSpan inicio;
                // Intentar parsear HH:mm:ss, luego HH:mm
                if (TimeSpan.TryParseExact(horaInicio, "hh\\:mm\\:ss", null, out inicio) ||
                    TimeSpan.TryParseExact(horaInicio, "hh\\:mm", null, out inicio))
                {
                    var fin = inicio.Add(TimeSpan.FromMinutes(servicio.DuracionMinutos));
                    tbHoraFin.Text = fin.ToString("hh\\:mm");
                }
                else
                {
                    tbHoraFin.Text = "Formato de hora inválido";
                }
            }
        }

        public class HoraDisponibilidadModel
        {
            public string hora { get; set; }
            public bool disponible { get; set; }
            public bool ocupado { get; set; }
            public bool pasado { get; set; }
        }

        #region CRUD (create, read, update, delete)
        public int id_solicitud;

        #region Crear
        private async void CrearCita_Click(object sender, RoutedEventArgs e)
        {
            if (cbServicio.SelectedItem == null || cbEmpleado.SelectedItem == null || dpFechaCita.SelectedDate == null || cbHoraInicio.SelectedItem == null || string.IsNullOrWhiteSpace(tbHoraFin.Text))
    {
        MessageBox.Show("Completa todos los campos obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    // Convertir fecha de nacimiento a yyyy-MM-dd
    string fechaNacimientoFormateada = null;
    if (!string.IsNullOrWhiteSpace(tbFechaNacimiento.Text))
    {
        DateTime fechaNacimiento;
        if (DateTime.TryParse(tbFechaNacimiento.Text.Trim(), out fechaNacimiento))
        {
            fechaNacimientoFormateada = fechaNacimiento.ToString("yyyy-MM-dd");
        }
        else
        {
            MessageBox.Show("La fecha de nacimiento debe tener formato válido (dd/MM/yyyy o yyyy-MM-dd)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }

    // Usar solo los campos del formulario
    var nuevoPaciente = new PacienteFormularioModel
    {
        Rut = tbRut.Text?.Trim(),
        Nombres = tbNombres.Text?.Trim(),
        ApellidoPaterno = tbApellidoPaterno.Text?.Trim(),
        ApellidoMaterno = tbApellidoMaterno.Text?.Trim(),
        FechaNacimiento = fechaNacimientoFormateada,
        Telefono = tbTelefono.Text?.Trim(),
        Email = tbEmail.Text?.Trim(),
        Estado = "activo"
    };

    var pacienteService = new PacienteApiService();
    var pacienteCreado = await pacienteService.CreatePacienteDesdeFormularioAsync(nuevoPaciente);
    if (pacienteCreado == null || pacienteCreado.IdPaciente <= 0)
    {
        MessageBox.Show("No se pudo crear el paciente. Verifica los datos ingresados.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
    }

    var solicitud = new SolicitudCreateModel
    {
        Rut = pacienteCreado.Rut,
        Nombres = pacienteCreado.Nombres,
        ApellidoPaterno = pacienteCreado.ApellidoPaterno,
        ApellidoMaterno = pacienteCreado.ApellidoMaterno,
        Telefono = pacienteCreado.Telefono,
        Email = pacienteCreado.Email,
        FechaNacimiento = pacienteCreado.FechaNacimiento,
        IdServicio = (int)cbServicio.SelectedValue,
        IdEmpleado = (int)cbEmpleado.SelectedValue, // Cambiado de IdPsicologo a IdEmpleado
        FechaCita = dpFechaCita.SelectedDate.Value.ToString("yyyy-MM-dd"),
        HoraInicio = cbHoraInicio.SelectedItem as string,
        HoraFin = tbHoraFin.Text,
        MotivoConsulta = tbMotivoConsulta.Text
    };

    var result = await _citaService.CreateSolicitudAsync(solicitud);
    if (result != null)
    {
        MessageBox.Show("Cita creada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        Content = new Customer();
    }
    else
    {
        MessageBox.Show("Error al crear la cita. Verifica los datos ingresados.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
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
