using System;
using System.Windows;
using System.Windows.Controls;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using WPF_LoginForm.Models;
using System.Linq;

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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var reporte = DataContext as ReporteModel;
            if (reporte == null)
            {
                MessageBox.Show("No hay datos para exportar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = "Reporte.xlsx"
            };
            if (saveDialog.ShowDialog() == true)
            {
                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Reporte");
                    ws.Cells["A1"].Value = "Paciente";
                    ws.Cells["B1"].Value = reporte.Paciente?.NombreCompleto;
                    ws.Cells["A2"].Value = "Psicólogo";
                    ws.Cells["B2"].Value = reporte.Psicologo?.Nombres;
                    ws.Cells["A3"].Value = "Fecha del Reporte";
                    ws.Cells["B3"].Value = reporte.FechaReporte;
                    ws.Cells["A4"].Value = "Tratamiento";
                    ws.Cells["B4"].Value = reporte.Tratamiento?.TipoTratamiento;
                    ws.Cells["A5"].Value = "Descripción";
                    ws.Cells["B5"].Value = reporte.Tratamiento?.Descripcion;
                    ws.Cells["A6"].Value = "Objetivos";
                    ws.Cells["B6"].Value = reporte.Tratamiento?.Objetivos;
                    ws.Cells["A7"].Value = "Seguimiento";
                    if (reporte.Seguimientos != null && reporte.Seguimientos.Count > 0)
                    {
                        ws.Cells["B7"].Value = string.Join("\n\n", reporte.Seguimientos.Select(s => $"Evolución de caso: {s.estado_animo}\nDescripción de avances: {s.adherencia_tratamiento}\nObservaciones: {s.observaciones}"));
                    }
                    else
                    {
                        ws.Cells["B7"].Value = "Sin seguimiento";
                    }
                    ws.Cells["A8"].Value = "Medicaciones";
                    ws.Cells["B8"].Value = (reporte.Medicaciones != null && reporte.Medicaciones.Count > 0) ? string.Join(", ", reporte.Medicaciones.Select(m => m.NombreMedicamento)) : "Sin medicación";
                    package.SaveAs(new FileInfo(saveDialog.FileName));
                }
                MessageBox.Show("Exportación a Excel exitosa.", "Excel", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Pdf(object sender, RoutedEventArgs e)
        {
            var reporte = DataContext as ReporteModel;
            if (reporte == null)
            {
                MessageBox.Show("No hay datos para exportar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = "Reporte.pdf"
            };
            if (saveDialog.ShowDialog() == true)
            {
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, new FileStream(saveDialog.FileName, FileMode.Create));
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var sectionFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var highlightFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
                var highlightBg = new BaseColor(255, 255, 128);

                var title = new Paragraph("INFORME DE ATENCIÓN", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 15f
                };
                doc.Add(title);

                PdfPTable table = new PdfPTable(2)
                {
                    WidthPercentage = 80,
                    SpacingAfter = 10f
                };
                table.SetWidths(new float[] { 1.2f, 2f });
                table.AddCell(new PdfPCell(new Phrase("Psicólogo:", labelFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase(reporte.Psicologo?.Nombres ?? "", valueFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase("Paciente:", labelFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase(reporte.Paciente?.NombreCompleto ?? "", valueFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase("Fecha del Reporte:", labelFont)) { Border = 0 });
                table.AddCell(new PdfPCell(new Phrase(reporte.FechaReporte ?? "", valueFont)) { Border = 0 });
                doc.Add(table);

                PdfPCell tratamientoHeader = new PdfPCell(new Phrase("TRATAMIENTO", sectionFont))
                {
                    BackgroundColor = highlightBg,
                    Colspan = 2,
                    Border = 0,
                    Padding = 5f
                };
                PdfPTable tratamientoTable = new PdfPTable(2)
                {
                    WidthPercentage = 80,
                    SpacingAfter = 5f
                };
                tratamientoTable.SetWidths(new float[] { 1.2f, 2f });
                tratamientoTable.AddCell(tratamientoHeader);
                tratamientoTable.AddCell(new PdfPCell(new Phrase("Tipo:", labelFont)) { Border = 0 });
                tratamientoTable.AddCell(new PdfPCell(new Phrase(reporte.Tratamiento?.TipoTratamiento ?? "", valueFont)) { Border = 0 });
                doc.Add(tratamientoTable);

                PdfPCell descripcionHeader = new PdfPCell(new Phrase("DESCRIPCIÓN", sectionFont))
                {
                    BackgroundColor = highlightBg,
                    Colspan = 2,
                    Border = 0,
                    Padding = 5f
                };
                PdfPTable descripcionTable = new PdfPTable(1)
                {
                    WidthPercentage = 80,
                    SpacingAfter = 5f
                };
                descripcionTable.AddCell(descripcionHeader);
                descripcionTable.AddCell(new PdfPCell(new Phrase(reporte.Tratamiento?.Descripcion ?? "", valueFont)) { Border = 0 });
                doc.Add(descripcionTable);

                PdfPCell objetivosHeader = new PdfPCell(new Phrase("OBJETIVOS", sectionFont))
                {
                    BackgroundColor = highlightBg,
                    Colspan = 2,
                    Border = 0,
                    Padding = 5f
                };
                PdfPTable objetivosTable = new PdfPTable(1)
                {
                    WidthPercentage = 80,
                    SpacingAfter = 5f
                };
                objetivosTable.AddCell(objetivosHeader);
                objetivosTable.AddCell(new PdfPCell(new Phrase(reporte.Tratamiento?.Objetivos ?? "", valueFont)) { Border = 0 });
                doc.Add(objetivosTable);

                PdfPCell seguimientoHeader = new PdfPCell(new Phrase("SEGUIMIENTO", sectionFont))
                {
                    BackgroundColor = highlightBg,
                    Colspan = 2,
                    Border = 0,
                    Padding = 5f
                };
                PdfPTable seguimientoTable = new PdfPTable(1)
                {
                    WidthPercentage = 80,
                    SpacingAfter = 5f
                };
                seguimientoTable.AddCell(seguimientoHeader);
                string seguimiento = (reporte.Seguimientos != null && reporte.Seguimientos.Count > 0)
                    ? string.Join("\n\n", reporte.Seguimientos.Select(s => $"Evolución de caso: {s.estado_animo}\nDescripción de avances: {s.adherencia_tratamiento}\nObservaciones: {s.observaciones}"))
                    : "Sin seguimiento";
                seguimientoTable.AddCell(new PdfPCell(new Phrase(seguimiento, valueFont)) { Border = 0 });
                doc.Add(seguimientoTable);

                PdfPCell medicacionHeader = new PdfPCell(new Phrase("MEDICACIONES", sectionFont))
                {
                    BackgroundColor = highlightBg,
                    Colspan = 2,
                    Border = 0,
                    Padding = 5f
                };
                PdfPTable medicacionTable = new PdfPTable(1)
                {
                    WidthPercentage = 80,
                    SpacingAfter = 10f
                };
                medicacionTable.AddCell(medicacionHeader);
                string medicacion = (reporte.Medicaciones != null && reporte.Medicaciones.Count > 0)
                    ? string.Join(", ", reporte.Medicaciones.Select(m => m.NombreMedicamento))
                    : "Sin medicación";
                medicacionTable.AddCell(new PdfPCell(new Phrase(medicacion, valueFont)) { Border = 0 });
                doc.Add(medicacionTable);

                var footer = new Paragraph("Se expide el presente a solicitud del interesado.", valueFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 20f
                };
                doc.Add(footer);
                var fecha = new Paragraph(DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"), valueFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 10f
                };
                doc.Add(fecha);

                doc.Close();
                MessageBox.Show("Exportación a PDF exitosa.", "PDF", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
