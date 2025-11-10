using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF_LoginForm.Views
{
    /// <summary>
    /// Lógica de interacción para Medicacion.xaml
    /// </summary>
    public partial class Medicacion : Page
    {
        // TODO: Inyectar servicios de API
        
        public Medicacion()
        {
            InitializeComponent();
            // TODO: Migrar a API
        }
        
        // TODO: Migrar a API REST - Se eliminó conexión SQL directa
        
        private void Crear(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Regresar(object sender, RoutedEventArgs e)
        {
            Content = new Atencion();
        }
        
        #region buscar
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TODO: Implementar búsqueda con API
        }
        #endregion
        
        private void Agregar(object sender, RoutedEventArgs e)
        {
            ConfirmarSolicitud ventana = new ConfirmarSolicitud();
        }

        private void Consultar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Actualizar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Eliminar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }

        private void Guardar(object sender, RoutedEventArgs e)
        {
            // TODO: Implementar con API
        }
    }
}
