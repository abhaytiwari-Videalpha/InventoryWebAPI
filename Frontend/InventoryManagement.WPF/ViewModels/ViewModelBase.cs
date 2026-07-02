using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>
    /// Common base class for all module ViewModels: busy state, error state, and a title
    /// for shell/header binding.
    /// </summary>
    public abstract partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private string title = string.Empty;

        protected void SetError(string? message)
        {
            ErrorMessage = message;
        }

        protected void ClearError()
        {
            ErrorMessage = null;
        }
    }
}