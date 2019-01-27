using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Arcemi.Pathfinder.Kingmaker.SaveGameEditor.Models
{
    public class VectorToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var width = parameter == null ? 1.0 : (double)System.Convert.ChangeType(parameter, typeof(double));
            var vector = (VectorModel)value;
            var x = vector.X;
            var y = vector.Y * -1;
            var offset = width / 2;
            var left = x * 100.0 + 100 - offset;
            var top = y * 100.0 + 100 - offset;
            var right = 0;
            var bottom = 0;
            var thickness = new Thickness(left, top, right, bottom);
            return thickness;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
