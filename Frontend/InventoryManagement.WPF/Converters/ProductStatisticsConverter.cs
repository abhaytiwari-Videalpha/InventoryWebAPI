using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// Computes read-only KPI statistics (count, inventory value, low-stock count,
    /// average price) directly from the Products collection for display on the
    /// Products page KPI cards. Pure view-layer computation — ProductsViewModel is
    /// never modified.
    ///
    /// Bind as a MultiBinding with two inputs:
    ///   [0] = Products (ObservableCollection&lt;ProductDto&gt;)
    ///   [1] = IsBusy (bool) — included only to force re-evaluation after each
    ///         load/add/edit/delete cycle, since WPF does not automatically refresh
    ///         a binding when items are added/removed from a collection reference
    ///         (ObservableCollection.Count has no PropertyChanged notification of
    ///         its own that a plain path-binding would pick up).
    /// ConverterParameter selects which statistic to return:
    ///   "Count" | "Value" | "LowStock" | "AveragePrice"
    ///
    /// ASSUMPTION: "Low Stock" threshold is Quantity &lt;= 10. This is a UI-only
    /// heuristic — the backend does not currently expose a configurable threshold.
    /// </summary>
    public class ProductStatisticsConverter : IMultiValueConverter
    {
        public const int LowStockThreshold = 10;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var products = (values.Length > 0 ? values[0] as IEnumerable : null)?
                .Cast<ProductDto>()
                .ToList() ?? new List<ProductDto>();

            var statistic = parameter as string ?? string.Empty;

            return statistic switch
            {
                "Count" => products.Count.ToString(culture),
                "Value" => products.Sum(p => p.Price * p.Quantity).ToString("N2", culture),
                "LowStock" => products.Count(p => p.Quantity <= LowStockThreshold).ToString(culture),
                "AveragePrice" => (products.Count > 0 ? products.Average(p => p.Price) : 0m).ToString("N2", culture),
                _ => string.Empty
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}