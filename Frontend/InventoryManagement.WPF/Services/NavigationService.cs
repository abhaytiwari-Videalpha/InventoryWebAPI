using System;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.WPF.Services
{
    /// <summary>
    /// Resolves module ViewModels from the DI container (or accepts pre-built instances)
    /// and publishes the active one for the shell to display via DataTemplates.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private ViewModelBase? _currentViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public event EventHandler<ViewModelBase?>? CurrentViewModelChanged;

        public ViewModelBase? CurrentViewModel => _currentViewModel;

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            Navigate(viewModel);
        }

        public void Navigate(ViewModelBase viewModel)
        {
            _currentViewModel = viewModel;
            CurrentViewModelChanged?.Invoke(this, _currentViewModel);
        }
    }
}