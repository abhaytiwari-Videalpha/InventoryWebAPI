using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>Placeholder — real analytics wired in Module 8 (Dashboard Analytics).</summary>
    public partial class DashboardViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string sampleContent = "Dashboard analytics (charts, KPIs, recent activity) will be implemented in Module 8.";

        public DashboardViewModel()
        {
            Title = "Dashboard";
        }
    }
}