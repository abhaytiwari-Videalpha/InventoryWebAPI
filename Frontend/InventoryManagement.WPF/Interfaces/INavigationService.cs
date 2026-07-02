using System;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Interfaces
{
    /// <summary>
    /// Contract for switching the active module ViewModel displayed in the application shell.
    /// </summary>
    public interface INavigationService
    {
        event EventHandler<ViewModelBase?>? CurrentViewModelChanged;

        ViewModelBase? CurrentViewModel { get; }

        /// <summary>Resolves a fresh instance of TViewModel from DI and makes it current.</summary>
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;

        /// <summary>Makes an already-constructed ViewModel instance current (e.g. one built with runtime parameters, such as "Edit Product #5").</summary>
        void Navigate(ViewModelBase viewModel);
    }
}