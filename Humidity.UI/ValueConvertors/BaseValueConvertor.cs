using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Humidity.UI.ValueConvertors
{
    public abstract class BaseValueConvertor<T> : MarkupExtension, IValueConverter where T : class, new()
    {
        #region Private Members

        /// <summary>
        /// A single instance of this value convertor
        /// </summary>
        private static T Convertor = null;

        #endregion

        #region Markup Extension Methods

        /// <summary>
        /// Provides a static instance of the value convertor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Convertor ?? (Convertor = new T());
        }

        #endregion

        #region Value Convertors

        /// <summary>
        /// The method to convert one type to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// The method to convert a value back to it's source type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        #endregion
    }
}
