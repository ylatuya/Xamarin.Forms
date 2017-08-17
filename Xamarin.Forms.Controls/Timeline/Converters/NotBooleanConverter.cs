using System;
using System.Globalization;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Timeline.Converters
{
    public class NotBooleanConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new ArgumentException("Bad type conversion for NotBooleanConverter");

            bool flag = false;
            if (value != null && value is bool)
            {
                flag = (bool)value;
            }
            return !flag;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}