using CommunityToolkit.Mvvm.ComponentModel;
using InventoryManagement.WPF.Services;
using InventoryManagement.WPF.Views;

namespace InventoryManagement.WPF.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public NavigationService NavigationService { get; }

    public MainViewModel()
    {
        NavigationService =
          NavigationService.Instance;

        NavigationService.CurrentView =
            new LoginView();
    }
}