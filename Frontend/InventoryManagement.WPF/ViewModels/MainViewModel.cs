using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>
    /// ViewModel for the single application shell window. Drives two mutually-exclusive
    /// layout states (auth screen vs. dashboard shell) via IsAuthenticated, and hosts the
    /// sidebar navigation commands + logged-in user header info.
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly DispatcherTimer _clockTimer;

        [ObservableProperty]
        private ViewModelBase? currentViewModel;

        [ObservableProperty]
        private bool isAuthenticated;

        [ObservableProperty]
        private string userName = string.Empty;

        [ObservableProperty]
        private string currentDate = string.Empty;

        [ObservableProperty]
        private string selectedMenuItem = "Dashboard";

        public MainViewModel(INavigationService navigationService, IAuthService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
            Title = "Inventory Management System";

            _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _authService.AuthenticationStateChanged += OnAuthenticationStateChanged;

            CurrentViewModel = _navigationService.CurrentViewModel;
            IsAuthenticated = _authService.IsAuthenticated;
            RefreshUserName();

            CurrentDate = DateTime.Now.ToString("dddd, dd MMMM yyyy");

            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _clockTimer.Tick += (_, _) => CurrentDate = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            _clockTimer.Start();
        }

        private void OnCurrentViewModelChanged(object? sender, ViewModelBase? viewModel)
        {
            CurrentViewModel = viewModel;

            SelectedMenuItem = viewModel switch
            {
                DashboardViewModel => "Dashboard",
                ProductsViewModel => "Products",
                CustomersViewModel => "Customers",
                InvoicesViewModel => "Invoices",
                ReportsViewModel => "Reports",
                ProfileViewModel => "Profile",
                _ => SelectedMenuItem
            };
        }

        private void OnAuthenticationStateChanged(object? sender, AuthenticatedUser? user)
        {
            IsAuthenticated = user is not null;
            RefreshUserName();
        }

        private void RefreshUserName()
        {
            // This backend identifies users by email — there is no separate username.
            UserName = _authService.CurrentUser?.Email ?? string.Empty;
        }

        [RelayCommand]
        private void NavigateDashboard() => _navigationService.NavigateTo<DashboardViewModel>();

        [RelayCommand]
        private void NavigateProducts() => _navigationService.NavigateTo<ProductsViewModel>();

        [RelayCommand]
        private void NavigateCustomers() => _navigationService.NavigateTo<CustomersViewModel>();

        [RelayCommand]
        private void NavigateInvoices() => _navigationService.NavigateTo<InvoicesViewModel>();

        [RelayCommand]
        private void NavigateReports() => _navigationService.NavigateTo<ReportsViewModel>();

        [RelayCommand]
        private void NavigateProfile() => _navigationService.NavigateTo<ProfileViewModel>();

        [RelayCommand]
        private async Task LogoutAsync()
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            try
            {
                await _authService.LogoutAsync();
                _navigationService.NavigateTo<LoginViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}