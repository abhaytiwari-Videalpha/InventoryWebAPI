using System.Windows.Controls;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Views
{
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        private void PasswordBoxControl_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.SetPassword(PasswordBoxControl.Password);
            }
        }

        private void ConfirmPasswordBoxControl_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.SetConfirmPassword(ConfirmPasswordBoxControl.Password);
            }
        }
    }
}