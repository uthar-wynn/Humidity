using System;
using System.Globalization;

namespace Humidity.UI.ValueConvertors
{
    public class EqualValueToParameterConvertor : BaseValueConvertor<EqualValueToParameterConvertor>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
