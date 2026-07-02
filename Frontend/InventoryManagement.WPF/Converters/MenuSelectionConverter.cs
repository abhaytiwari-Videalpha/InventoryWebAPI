using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// MultiValueConverter used by sidebar menu buttons to compute their own
    /// Background/Foreground based on whether they match the currently selected menu key.
    /// Usage: bind [0] = MainViewModel.SelectedMenuItem, [1] = the button's own Tag
    /// (e.g. "Dashboard"). ConverterParameter = "Background" or "Foreground".
    /// </summary>
    public class MenuSelectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = values.Length == 2
                && values[0] is string selectedKey
                && values[1] is string buttonKey
                && string.Equals(selectedKey, buttonKey, StringComparison.OrdinalIgnoreCase);

            var role = parameter as string;

            if (role == "Foreground")
            {
                return isSelected
                    ? Brushes.White
                    : (Brush)Application.Current.Resources["TextSecondaryBrush"];
            }

            return isSelected
                ? (Brush)Application.Current.Resources["AccentBrush"]
                : Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}