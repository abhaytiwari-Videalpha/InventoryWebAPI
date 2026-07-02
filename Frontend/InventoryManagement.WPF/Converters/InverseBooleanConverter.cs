using System;
using System.Globalization;
using System.Windows.Data;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// Inverts a boolean value. Commonly used for IsEnabled="{Binding IsBusy, Converter=...}"
    /// so controls disable while a request is in flight.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : value;
        }
    }
}