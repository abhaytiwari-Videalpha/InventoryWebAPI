using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// Returns Visibility.Visible when the product list is empty AND the ViewModel
    /// is not currently loading — used to toggle the empty state vs. the DataGrid.
    /// Bind as a MultiBinding: [0] = Products, [1] = IsBusy.
    /// Pass ConverterParameter="Invert" to get the opposite (used to hide the
    /// DataGrid itself while the empty state is showing).
    /// </summary>
    public class ProductListEmptyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var products = (values.Length > 0 ? values[0] as IEnumerable : null)?.Cast<object>().ToList();
            var isBusy = values.Length > 1 && values[1] is bool busy && busy;

            var isEmpty = (products is null || products.Count == 0) && !isBusy;

            var invert = parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase);
            if (invert)
            {
                isEmpty = !isEmpty;
            }

            return isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}