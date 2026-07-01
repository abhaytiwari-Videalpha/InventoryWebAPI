using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.Services;

public partial class NavigationService
    : ObservableObject
{
    private static readonly NavigationService _instance =
        new();

    public static NavigationService Instance =>
        _instance;

    [ObservableProperty]
    private object? currentView;
}