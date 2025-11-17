using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using WPF_LoginForm.Services;
using WPF_LoginForm.Models;
using System.Collections.Generic;
using System.Linq;

namespace WPF_LoginForm.Converters
{
    public class FirstSeguimientoObservacionesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lista = value as IList<SeguimientoResponse>;
            if (lista != null && lista.Count > 0)
            {
                return string.Join("\n\n", lista.Select(s =>
                    $"Evolución de caso: {s.estado_animo}\nDescripción de avances: {s.adherencia_tratamiento}\nObservaciones: {s.observaciones}"));
            }
            return "Sin seguimiento";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FirstMedicacionNombreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lista = value as IList<MedicacionModel>;
            if (lista != null && lista.Count > 0)
                return lista[0].NombreMedicamento;
            return "Sin medicación";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
