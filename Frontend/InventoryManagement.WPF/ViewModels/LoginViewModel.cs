using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            Title = "Sign In";
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task LoginAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ClearError();

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                SetError("Please enter both email and password.");
                return;
            }

            IsBusy = true;

            try
            {
                var result = await _authService.LoginAsync(new LoginRequest
                {
                    Email = Email,
                    Password = Password
                });

                if (result.Success)
                {
                    _navigationService.NavigateTo<DashboardViewModel>();
                }
                else
                {
                    SetError(result.Message ?? "Login failed. Please check your credentials.");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            _navigationService.NavigateTo<RegisterViewModel>();
        }

        public void SetPassword(string value) => Password = value;
    }
}