using System.Windows.Controls;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();

        DataContext =
            new LoginViewModel();
    }
}