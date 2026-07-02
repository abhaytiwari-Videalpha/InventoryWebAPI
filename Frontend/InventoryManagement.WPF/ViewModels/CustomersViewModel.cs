using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>Placeholder — full CRUD wired in Module 5 (Customer Management).</summary>
    public partial class CustomersViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string sampleContent = "Customer list and CRUD will be implemented in Module 5.";

        public CustomersViewModel()
        {
            Title = "Customers";
        }
    }
}