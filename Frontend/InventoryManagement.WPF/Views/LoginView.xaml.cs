using System.Windows.Controls;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBoxControl_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.SetPassword(PasswordBoxControl.Password);
            }
        }
    }
}