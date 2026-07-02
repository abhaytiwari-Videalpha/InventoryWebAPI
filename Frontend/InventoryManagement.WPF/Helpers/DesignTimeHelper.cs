using System.ComponentModel;
using System.Windows;

namespace InventoryManagement.WPF.Helpers
{
    /// <summary>
    /// Detects whether code is running inside the XAML designer (Blend/Visual Studio)
    /// so ViewModels can supply design-time sample data safely.
    /// </summary>
    public static class DesignTimeHelper
    {
        public static bool IsInDesignMode =>
            DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}