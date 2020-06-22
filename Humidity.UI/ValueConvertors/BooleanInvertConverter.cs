using System;
using System.Globalization;

namespace Humidity.UI.ValueConvertors
{
    public class BooleanInvertConverter : BaseValueConvertor<BooleanInvertConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
