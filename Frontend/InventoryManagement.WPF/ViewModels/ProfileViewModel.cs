using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>Placeholder — real profile view/edit wired in Module 7 (Profile).</summary>
    public partial class ProfileViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string sampleContent = "Profile viewing and editing will be implemented in Module 7.";

        public ProfileViewModel()
        {
            Title = "Profile";
        }
    }
}