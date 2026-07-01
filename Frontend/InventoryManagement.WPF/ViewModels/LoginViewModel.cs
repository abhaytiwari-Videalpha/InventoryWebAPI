using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.WPF.Helpers;
using InventoryManagement.WPF.Models;
using InventoryManagement.WPF.Services;
using InventoryManagement.WPF.Views;
using System.Windows;

namespace InventoryManagement.WPF.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly AuthService _authService;

    public LoginViewModel()
    {
        _authService = new AuthService();
    }

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            var request = new LoginRequest
            {
                Email = Email,
                Password = Password
            };

            var result =
                await _authService.LoginAsync(
                    request);

            if (result == null)
            {
                MessageBox.Show(
                    "Invalid credentials");

                return;
            }

            TokenStorage.Token =
                result.Token;

            MessageBox.Show(
             $"TOKEN AFTER LOGIN:\n\n{TokenStorage.Token}");

            TokenStorage.RefreshToken =
                result.RefreshToken;

            NavigationService.Instance.CurrentView =
                new DashboardView();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
}