using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>Placeholder — full invoice + line-item workflow wired in Module 6 (Invoice Management).</summary>
    public partial class InvoicesViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string sampleContent = "Invoice list, creation, and line items will be implemented in Module 6.";

        public InvoicesViewModel()
        {
            Title = "Invoices";
        }
    }
}