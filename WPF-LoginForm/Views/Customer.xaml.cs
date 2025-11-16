using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_LoginForm.Models;
using WPF_LoginForm.Services;

namespace WPF_LoginForm.Views
{
    public partial class Customer : UserControl
    {
        private readonly CitaApiService _citaService = new CitaApiService();
        private readonly PacienteApiService _pacienteService = new PacienteApiService();
        private readonly PsicologoApiService _empleadoService = new PsicologoApiService();
        private readonly ServicioApiService _servicioService = new ServicioApiService();
        private List<CitaGridModel> _citasGrid = new List<CitaGridModel>();

        public Customer()
        {
            InitializeComponent();
            _ = CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            try
            {
                GridDatos.ItemsSource = null;
                var citas = await GetCitasFromApiAsync();
                var pacientes = (await _pacienteService.GetAllPacientesAsync()).ToList();
                var empleados = (await _empleadoService.GetAllPsicologosAsync()).ToList();
                var servicios = (await _servicioService.GetAllServiciosAsync()).ToList();
                var estados = new Dictionary<int, string> {
                    {1, "Pendiente"}, {2, "Confirmada"}, {3, "En Curso"}, {4, "Completada"}, {5, "Cancelada"}
                };
                _citasGrid = citas.Select(c => new CitaGridModel {
                    IdCita = c.IdCita,
                    FechaCita = c.FechaCita,
                    HoraInicio = c.HoraInicio,
                    HoraFin = c.HoraFin,
                    MotivoConsulta = c.MotivoConsulta,
                    Observaciones = c.Observaciones,
                    NombrePaciente = pacientes.FirstOrDefault(p => p.IdPaciente == c.IdPaciente)?.NombreCompleto ?? c.IdPaciente.ToString(),
                    NombreEmpleado = empleados.FirstOrDefault(e => e.IdEmpleado == c.IdEmpleado)?.NombreCompleto ?? c.IdEmpleado.ToString(),
                    NombreServicio = servicios.FirstOrDefault(s => s.IdServicio == c.IdServicio)?.NombreServicio ?? c.IdServicio.ToString(),
                    NombreEstado = estados.ContainsKey(c.IdEstadoCita) ? estados[c.IdEstadoCita] : c.IdEstadoCita.ToString(),
                    CodigoConfirmacion = c.CodigoConfirmacion,
                    FechaCreacion = c.FechaCreacion,
                    FechaModificacion = c.FechaModificacion
                }).ToList();
                GridDatos.ItemsSource = _citasGrid;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar citas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<IEnumerable<CitaApiModel>> GetCitasFromApiAsync()
        {
            var token = Repositories.ApiTokenStore.Instance.Token;
            if (string.IsNullOrEmpty(token))
                return new List<CitaApiModel>();
            var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, "/api/v1/citas/?skip=0&limit=1000");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");
            using (var client = new System.Net.Http.HttpClient { BaseAddress = new Uri("http://147.182.240.177:8000/") })
            {
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var citasResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<CitasApiResponse>(json);
                    return citasResponse?.Data ?? new List<CitaApiModel>();
                }
            }
            return new List<CitaApiModel>();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_citasGrid == null || !_citasGrid.Any())
                return;
            string textoBusqueda = Buscar.Text.ToLower().Trim();
            if (string.IsNullOrEmpty(textoBusqueda))
            {
                GridDatos.ItemsSource = _citasGrid;
            }
            else
            {
                var citasFiltradas = _citasGrid.Where(c =>
                    (c.MotivoConsulta != null && c.MotivoConsulta.ToLower().Contains(textoBusqueda)) ||
                    (c.Observaciones != null && c.Observaciones.ToLower().Contains(textoBusqueda)) ||
                    (c.NombrePaciente != null && c.NombrePaciente.ToLower().Contains(textoBusqueda)) ||
                    (c.NombreEmpleado != null && c.NombreEmpleado.ToLower().Contains(textoBusqueda)) ||
                    (c.NombreServicio != null && c.NombreServicio.ToLower().Contains(textoBusqueda))
                ).ToList();
                GridDatos.ItemsSource = citasFiltradas;
            }
        }

        private void Agregar(object sender, RoutedEventArgs e)
        {
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            FrameCustomer.Content = ventana;
            FrameCustomer.Visibility = Visibility.Visible;
            GridCliente.Visibility = Visibility.Collapsed;
            
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            ventana.id_solicitud = id;
            ventana.Consultar();
            FrameCustomer.Content = ventana;
            ventana.Titulo.Text = "Consultar Servicio";
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            ventana.id_solicitud = id;
            ventana.Consultar();
            FrameCustomer.Content = ventana;
            ventana.Titulo.Text = "Actualizar Servicio";
            // ventana.BtnActualizar.Visibility = Visibility.Visible;
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            CrudSolicitudServicio ventana = new CrudSolicitudServicio();
            ventana.id_solicitud = id;
            ventana.Consultar();
            FrameCustomer.Content = ventana;
            ventana.Titulo.Text = "Eliminar Servicio";
            // ventana.BtnEliminar.Visibility = Visibility.Visible;
        }

        public class CitasApiResponse
        {
            [Newtonsoft.Json.JsonProperty("data")]
            public List<CitaApiModel> Data { get; set; }
            [Newtonsoft.Json.JsonProperty("count")]
            public int Count { get; set; }
        }
    }
}

































