using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>
    /// Backend's RegisterDto only accepts Email + Password — no name/username fields
    /// exist at registration time in this system (those live in the Profile module).
    /// </summary>
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private string? successMessage;

        public RegisterViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
            Title = "Create Account";
        }

        [RelayCommand]
        private async System.Threading.Tasks.Task RegisterAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ClearError();
            SuccessMessage = null;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                SetError("Email and password are required.");
                return;
            }

            if (Password != ConfirmPassword)
            {
                SetError("Passwords do not match.");
                return;
            }

            IsBusy = true;

            try
            {
                var result = await _authService.RegisterAsync(new RegisterRequest
                {
                    Email = Email,
                    Password = Password
                });

                if (result.Success)
                {
                    SuccessMessage = result.Message ?? "Registration successful. Please log in.";
                    _navigationService.NavigateTo<LoginViewModel>();
                }
                else
                {
                    SetError(result.Message ?? "Registration failed.");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            _navigationService.NavigateTo<LoginViewModel>();
        }

        public void SetPassword(string value) => Password = value;

        public void SetConfirmPassword(string value) => ConfirmPassword = value;
    }
}