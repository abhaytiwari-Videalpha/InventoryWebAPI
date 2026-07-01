using System.Windows;

namespace InventoryManagement.WPF;

public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += (s, e) =>
        {
            MessageBox.Show(
                e.Exception.ToString(),
                "Unhandled Exception");

            e.Handled = true;
        };
    }
}