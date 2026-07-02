using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// Converts a bool to Visibility. Pass ConverterParameter="Invert" to flip the logic
    /// (useful for binding IsBusy to a Visibility.Collapsed content panel, etc.).
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                var invert = parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase);
                if (invert)
                {
                    boolValue = !boolValue;
                }

                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }

            return false;
        }
    }
}