using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>Placeholder — reporting will be delivered alongside Module 8 (Dashboard Analytics).</summary>
    public partial class ReportsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string sampleContent = "Reports will be implemented alongside Module 8 (Dashboard Analytics).";

        public ReportsViewModel()
        {
            Title = "Reports";
        }
    }
}