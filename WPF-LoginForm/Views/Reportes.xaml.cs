using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para Reportes.xaml
    /// </summary>
    public partial class Reportes : Page
    {
        // TODO: Implementar servicios de API
        // private readonly CitaApiService _citaService;
        // private readonly ReporteApiService _reporteService;
        
        public Reportes()
        {
            InitializeComponent();
            // TODO: Migrar CargarDatos() a API REST
            // CargarDatosAsync();
        }
        
        // TODO: Migrar a API REST
        // Se eliminó conexión SQL directa (conexionDB2)
        // Implementar servicio de API para obtener datos de citas/reportes
        // private async Task CargarDatosAsync()
        // {
        //     try
        //     {
        //         var citas = await _citaService.GetAllCitasAsync();
        //         // Procesar datos para reportes
        //         GridDatos.ItemsSource = citas;
        //     }
        //     catch (Exception ex)
        //     {
        //         MessageBox.Show($"Error al cargar datos: {ex.Message}", 
        //             "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //     }
        // }
        
        private void Crear(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar creación de reporte
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new Atencion();
        }
        
        #region buscar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Implementar búsqueda local en datos cargados
            // O usar filtros de API si están disponibles
        }
        #endregion
        
        private void Agregar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar consulta con API
            // int id = (int)((Button)sender).CommandParameter;
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar actualización con API
            // int id = (int)((Button)sender).CommandParameter;
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar eliminación con API
            // int id = (int)((Button)sender).CommandParameter;
        }

        private void Guardar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar guardado de reporte
        }

        private void Excel(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar exportación a Excel
            // Usar biblioteca como EPPlus o ClosedXML
        }

        private void Pdf(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar exportación a PDF
            // Usar biblioteca como iTextSharp o PdfSharp
        }
    }
}
