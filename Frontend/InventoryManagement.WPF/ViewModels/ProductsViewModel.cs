using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>Placeholder — full CRUD + pagination wired in Module 4 (Product Management).</summary>
    public partial class ProductsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string sampleContent = "Product list, search, filtering, and CRUD will be implemented in Module 4.";

        public ProductsViewModel()
        {
            Title = "Products";
        }
    }
}