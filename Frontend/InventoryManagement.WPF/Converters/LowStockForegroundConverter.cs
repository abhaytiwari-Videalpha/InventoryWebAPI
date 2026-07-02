using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// Colors the Quantity cell text based on stock level:
    /// 0 -> ErrorBrush (out of stock), &lt;= 10 -> WarningBrush (low stock),
    /// otherwise -> TextPrimaryBrush. Same threshold as ProductStatisticsConverter.
    /// </summary>
    public class LowStockForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity)
            {
                if (quantity <= 0)
                {
                    return Application.Current.Resources["ErrorBrush"] as Brush ?? Brushes.Red;
                }

                if (quantity <= ProductStatisticsConverter.LowStockThreshold)
                {
                    return Application.Current.Resources["WarningBrush"] as Brush ?? Brushes.Orange;
                }
            }

            return Application.Current.Resources["TextPrimaryBrush"] as Brush ?? Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}