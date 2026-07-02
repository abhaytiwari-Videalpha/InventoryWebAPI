using System;
using System.Threading.Tasks;
using System.Windows;

namespace InventoryManagement.WPF.Helpers
{
    /// <summary>
    /// Safely awaits a fire-and-forget Task (e.g. from an async void event handler or
    /// AsyncRelayCommand execution) and routes any exception to a handler instead of
    /// crashing the application via an unobserved exception.
    /// </summary>
    public static class AsyncCommandHelper
    {
        public static async void FireAndForget(this Task task, Action<Exception>? onException = null)
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                if (onException is not null)
                {
                    onException(ex);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
            }
        }
    }
}