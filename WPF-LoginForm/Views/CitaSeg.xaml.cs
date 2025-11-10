using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para CitaSeg.xaml
    /// </summary>
    public partial class CitaSeg : UserControl
    {
        // TODO: Inyectar servicios de API
        // private readonly CitaApiService _citaService;
        
        public CitaSeg()
        {
            InitializeComponent();
            // TODO: Migrar CargarDatos() a API
            // CargarDatosAsync();
        }
        
        // TODO: Migrar a API REST - Se eliminó conexión SQL directa
        
        #region buscar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Implementar búsqueda con API
        }
        #endregion
        
        private void Agregar(object sender, RoutedEventArgs e)
        {
            CrearCita ventana = new CrearCita();
            FrameCitaSeg.Content = ventana;
        }

        private void Presente(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Comenzar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            // TODO: Implementar con API
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
            FrameCitaSeg.Content = ventana;
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).CommandParameter;
            // TODO: Implementar con API
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
            FrameCitaSeg.Content = ventana;
        }
    }
}
