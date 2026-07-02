# Inventory Management System - WPF Frontend

## Project Information

Framework: .NET 9
UI: WPF
Pattern: MVVM
Toolkit: CommunityToolkit.Mvvm
DI: Microsoft.Extensions.DependencyInjection
Http: IHttpClientFactory

---

# Folder Structure

InventoryManagement.WPF/

├── App.xaml
├── App.xaml.cs

├── Configuration
│   ├── ApiSettings.cs
│   └── ApiRoutes.cs

├── Models
│   ├── ApiResponse.cs
│   ├── ServiceResult.cs
│   ├── AuthenticatedUser.cs
│   └── ...

├── Interfaces
│   ├── INavigationService.cs
│   ├── IAuthService.cs
│   └── ITokenStorageService.cs

├── Services
│   ├── NavigationService.cs
│   ├── AuthService.cs
│   ├── TokenStorageService.cs
│   └── AuthHeaderHandler.cs

├── ViewModels
│   ├── MainViewModel.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── DashboardViewModel.cs
│   ├── ProductsViewModel.cs
│   ├── CustomersViewModel.cs
│   ├── ReportsViewModel.cs
│   └── ProfileViewModel.cs

├── Views
│   ├── MainWindow.xaml
│   ├── LoginView.xaml
│   ├── RegisterView.xaml
│   ├── DashboardView.xaml
│   ├── ProductsView.xaml
│   ├── CustomersView.xaml
│   ├── ReportsView.xaml
│   └── ProfileView.xaml

├── Resources
│   ├── Themes
│   │   ├── Colors.xaml
│   │   └── Styles.xaml
│   └── Templates
│       └── ViewTemplates.xaml

├── Converters
│   ├── BoolToVisibilityConverter.cs
│   └── InverseBooleanConverter.cs






<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <UseWPF>true</UseWPF>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>InventoryManagement.WPF</RootNamespace>
    <AssemblyName>InventoryManagement.WPF</AssemblyName>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.4.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Http" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.0" />
    <PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="9.0.0" />
    <PackageReference Include="System.Security.Cryptography.ProtectedData" Version="9.0.0" />
  </ItemGroup>

  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>

----------

using System.Windows;

[assembly:ThemeInfo(
    ResourceDictionaryLocation.None,            //where theme specific resource dictionaries are located
                                                //(used if a resource is not found in the page,
                                                // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly   //where the generic resource dictionary is located
                                                //(used if a resource is not found in the page,
                                                // app, or any theme specific resource dictionaries)
)]



------------

{
  "ApiSettings": {
    "GatewayBaseUrl": "http://localhost:5078/",
    "TimeoutSeconds": 30
  }
}

----
using System;
using System.Net.Http.Headers;
using System.Windows;
using InventoryManagement.WPF.Configuration;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Services;
using InventoryManagement.WPF.ViewModels;
using InventoryManagement.WPF.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace InventoryManagement.WPF
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .Build();
        }

        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            // ---- Configuration ----
            services.Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName));

            // ---- Auth message handler ----
            services.AddTransient<AuthHeaderHandler>();

            // ---- HttpClient: AuthClient (login/register/refresh — no auth handler) ----
            services.AddHttpClient(ApiSettings.AuthClientName, (serviceProvider, client) =>
            {
                ConfigureGatewayClient(serviceProvider, client);
            });

            // ---- HttpClient: GatewayClient (all authenticated calls, incl. logout) ----
            services.AddHttpClient(ApiSettings.GatewayClientName, (serviceProvider, client) =>
            {
                ConfigureGatewayClient(serviceProvider, client);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

            // ---- Core Services ----
            services.AddSingleton<ITokenStorageService, TokenStorageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IAuthService, AuthService>();

            // ---- Shell (single window, single instance) ----
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();

            // ---- Authentication ViewModels (transient — fresh state per navigation) ----
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();

            // ---- Module placeholder ViewModels (transient for now) ----
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<ProductsViewModel>();
            services.AddTransient<CustomersViewModel>();
            services.AddTransient<InvoicesViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<ProfileViewModel>();
        }

        private static void ConfigureGatewayClient(IServiceProvider serviceProvider, System.Net.Http.HttpClient client)
        {
            var apiSettings = serviceProvider.GetRequiredService<IOptions<ApiSettings>>().Value;

            if (string.IsNullOrWhiteSpace(apiSettings.GatewayBaseUrl))
            {
                throw new InvalidOperationException(
                    "ApiSettings:GatewayBaseUrl is not configured. Check appsettings.json.");
            }

            client.BaseAddress = new Uri(apiSettings.GatewayBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected override async void OnStartup(StartupEventArgs e)
                {
                    await _host.StartAsync();

                    var navigationService = _host.Services.GetRequiredService<INavigationService>();
                    var authService = _host.Services.GetRequiredService<IAuthService>();

                    var autoLoginResult = await authService.TryAutoLoginAsync();

                    if (autoLoginResult.Success)
                    {
                        navigationService.NavigateTo<DashboardViewModel>();
                    }
                    else
                    {
                        navigationService.NavigateTo<LoginViewModel>();
                    }

                    var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                    mainWindow.Show();

                    base.OnStartup(e);
                }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}

---------

<Application x:Class="InventoryManagement.WPF.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Themes/Colors.xaml" />
                <ResourceDictionary Source="Resources/Themes/Styles.xaml" />
                <ResourceDictionary Source="Resources/Templates/ViewTemplates.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>

----------

S C:\Users\VideAlpha\InventoryManagementSystem\Frontend\InventoryManagement.WPF\views> tree /f
Folder PATH listing
Volume serial number is 3A50-6A56
C:.
    CustomersView.xaml
    CustomersView.xaml.cs
    DashboardView.xaml
    DashboardView.xaml.cs
    InvoicesView.xaml
    InvoicesView.xaml.cs
    LoginView.xaml
    LoginView.xaml.cs
    MainWindow.xaml
    MainWindow.xaml.cs
    ProductsView.xaml
    ProductsView.xaml.cs
    ProfileView.xaml
    ProfileView.xaml.cs
    RegisterView.xaml
    RegisterView.xaml.cs
    ReportsView.xaml
    ReportsView.xaml.cs
    
No subfolders exist 

<UserControl x:Class="InventoryManagement.WPF.Views.ProductsView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d">
    <StackPanel>
        <TextBlock Text="{Binding Title}" FontSize="22" FontWeight="Bold"
                   Foreground="{StaticResource TextPrimaryBrush}" Margin="0,0,0,12" />
        <TextBlock Text="{Binding SampleContent}" TextWrapping="Wrap"
                   Foreground="{StaticResource TextSecondaryBrush}" />
    </StackPanel>
</UserControl>

--------

using System.Windows.Controls;

namespace InventoryManagement.WPF.Views
{
    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
        }
    }
}

---------

<UserControl x:Class="InventoryManagement.WPF.Views.DashboardView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
            xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
            xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
mc:Ignorable="d">
    <StackPanel>
        <TextBlock Text="{Binding Title}" FontSize="22" FontWeight="Bold"
                   Foreground="{StaticResource TextPrimaryBrush}" Margin="0,0,0,12" />
        <TextBlock Text="{Binding SampleContent}" TextWrapping="Wrap"
                   Foreground="{StaticResource TextSecondaryBrush}" />
    </StackPanel>
</UserControl>

-------
using System.Windows.Controls;

namespace InventoryManagement.WPF.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }
    }
}

---------
<Window x:Class="InventoryManagement.WPF.Views.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:converters="clr-namespace:InventoryManagement.WPF.Converters"
mc:Ignorable="d"
        Title="{Binding Title}"
        Height="800" Width="1400"
        MinHeight="600" MinWidth="1000"
        WindowStartupLocation="CenterScreen">

    <Window.Resources>
        <converters:BoolToVisibilityConverter x:Key="BoolToVisibilityConverter" />
        <converters:MenuSelectionConverter x:Key="MenuSelectionConverter" />

        <Style x:Key="MenuButtonStyle" TargetType="Button">
            <Setter Property="Height" Value="46" />
            <Setter Property="Margin" Value="12,3" />
            <Setter Property="HorizontalContentAlignment" Value="Left" />
            <Setter Property="BorderThickness" Value="0" />
            <Setter Property="Cursor" Value="Hand" />
            <Setter Property="FontSize" Value="14" />
            <Setter Property="FontWeight" Value="SemiBold" />
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="Button">
                        <Border Background="{TemplateBinding Background}"
                                CornerRadius="8"
                                Padding="16,0">
                            <ContentPresenter VerticalAlignment="Center" />
                        </Border>
                        <ControlTemplate.Triggers>
                            <Trigger Property="IsMouseOver" Value="True">
                                <Setter Property="Opacity" Value="0.85" />
                            </Trigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
    </Window.Resources>

    <Grid>

        <!-- ===================== AUTH LAYOUT (Login / Register) ===================== -->
        <Grid Background="{StaticResource BackgroundBrush}"
              Visibility="{Binding IsAuthenticated, Converter={StaticResource BoolToVisibilityConverter}, ConverterParameter=Invert}">
            <ContentControl Content="{Binding CurrentViewModel}" />
        </Grid>

        <!-- ===================== DASHBOARD SHELL LAYOUT ===================== -->
        <Grid Visibility="{Binding IsAuthenticated, Converter={StaticResource BoolToVisibilityConverter}}">
            <Grid.RowDefinitions>
                <RowDefinition Height="64" />
                <RowDefinition Height="*" />
            </Grid.RowDefinitions>

<!-- TOP HEADER -->
            <Border Grid.Row="0" Background="{StaticResource SurfaceBrush}"
                    BorderBrush="{StaticResource BorderBrush}" BorderThickness="0,0,0,1">
                <Grid Margin="20,0">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="Auto" />
                        <ColumnDefinition Width="Auto" />
                        <ColumnDefinition Width="Auto" />
                    </Grid.ColumnDefinitions>

                    <TextBlock Grid.Column="0"
                               Text="{Binding Title}"
                               FontSize="18" FontWeight="Bold"
                               Foreground="{StaticResource PrimaryBrush}"
                               VerticalAlignment="Center" />

                    <TextBlock Grid.Column="1"
                               Text="{Binding CurrentDate}"
                               Foreground="{StaticResource TextSecondaryBrush}"
                               VerticalAlignment="Center"
                               Margin="0,0,24,0" />

                    <Border Grid.Column="2"
                            Width="36" Height="36"
                            CornerRadius="18"
                            Background="{StaticResource BackgroundBrush}"
                            Margin="0,0,12,0">
                        <TextBlock Text="&#xE7E7;"
                                   FontFamily="Segoe MDL2 Assets"
                                   FontSize="16"
                                   Foreground="{StaticResource TextSecondaryBrush}"
                                   HorizontalAlignment="Center"
                                   VerticalAlignment="Center" />
                    </Border>

                    <TextBlock Grid.Column="3"
                               Text="{Binding UserName}"
                               FontWeight="SemiBold"
                               Foreground="{StaticResource TextPrimaryBrush}"
                               VerticalAlignment="Center" />
                </Grid>
            </Border>

<!-- SIDEBAR + CONTENT -->
            <Grid Grid.Row="1">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="250" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>

                <!-- SIDEBAR -->
                <Border Grid.Column="0" Background="{StaticResource PrimaryBrush}">
                    <StackPanel Margin="0,20,0,0">

                        <Button Command="{Binding NavigateDashboardCommand}" Tag="Dashboard" Style="{StaticResource MenuButtonStyle}">
                            <Button.Background>
                                <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Background">
                                    <Binding Path="SelectedMenuItem" />
                                    <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                </MultiBinding>
                            </Button.Background>
                            <StackPanel Orientation="Horizontal">
                                <TextBlock Text="&#xE80F;" FontFamily="Segoe MDL2 Assets" FontSize="16" Width="24">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                                <TextBlock Text="Dashboard" Margin="8,0,0,0">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                            </StackPanel>
                        </Button>

                        <Button Command="{Binding NavigateProductsCommand}" Tag="Products" Style="{StaticResource MenuButtonStyle}">
                            <Button.Background>
                                <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Background">
                                    <Binding Path="SelectedMenuItem" />
                                    <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                </MultiBinding>
                            </Button.Background>
                            <StackPanel Orientation="Horizontal">
                                <TextBlock Text="&#xE719;" FontFamily="Segoe MDL2 Assets" FontSize="16" Width="24">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                                <TextBlock Text="Products" Margin="8,0,0,0">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                            </StackPanel>
                        </Button>

                        <Button Command="{Binding NavigateCustomersCommand}" Tag="Customers" Style="{StaticResource MenuButtonStyle}">
                            <Button.Background>
                                <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Background">
                                    <Binding Path="SelectedMenuItem" />
                                    <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                </MultiBinding>
                            </Button.Background>
                            <StackPanel Orientation="Horizontal">
                                <TextBlock Text="&#xE716;" FontFamily="Segoe MDL2 Assets" FontSize="16" Width="24">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                                <TextBlock Text="Customers" Margin="8,0,0,0">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                            </StackPanel>
                        </Button>

                        <Button Command="{Binding NavigateInvoicesCommand}" Tag="Invoices" Style="{StaticResource MenuButtonStyle}">
                            <Button.Background>
                                <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Background">
                                    <Binding Path="SelectedMenuItem" />
                                    <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                </MultiBinding>
                            </Button.Background>
                            <StackPanel Orientation="Horizontal">
                                <TextBlock Text="&#xE9F9;" FontFamily="Segoe MDL2 Assets" FontSize="16" Width="24">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                                <TextBlock Text="Invoices" Margin="8,0,0,0">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                            </StackPanel>
                        </Button>

                        <Button Command="{Binding NavigateReportsCommand}" Tag="Reports" Style="{StaticResource MenuButtonStyle}">
                            <Button.Background>
                                <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Background">
                                    <Binding Path="SelectedMenuItem" />
                                    <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                </MultiBinding>
                            </Button.Background>
                            <StackPanel Orientation="Horizontal">
                                <TextBlock Text="&#xE9D2;" FontFamily="Segoe MDL2 Assets" FontSize="16" Width="24">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                                <TextBlock Text="Reports" Margin="8,0,0,0">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                            </StackPanel>
                        </Button>

                        <Button Command="{Binding NavigateProfileCommand}" Tag="Profile" Style="{StaticResource MenuButtonStyle}">
                            <Button.Background>
                                <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Background">
                                    <Binding Path="SelectedMenuItem" />
                                    <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                </MultiBinding>
                            </Button.Background>
                            <StackPanel Orientation="Horizontal">
                                <TextBlock Text="&#xE77B;" FontFamily="Segoe MDL2 Assets" FontSize="16" Width="24">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                                <TextBlock Text="Profile" Margin="8,0,0,0">
                                    <TextBlock.Foreground>
                                        <MultiBinding Converter="{StaticResource MenuSelectionConverter}" ConverterParameter="Foreground">
                                            <Binding Path="SelectedMenuItem" />
                                            <Binding Path="Tag" RelativeSource="{RelativeSource AncestorType=Button}" />
                                        </MultiBinding>
                                    </TextBlock.Foreground>
                                </TextBlock>
                            </StackPanel>
                        </Button>

                        <Border Height="1" Background="#33FFFFFF" Margin="16,12" />

                        <Button Command="{Binding LogoutCommand}" Style="{StaticResource MenuButtonStyle}"
                                Background="Transparent">
                            <StackPanel Orientation="Horizontal">
                                <TextBlock Text="&#xE7E8;" FontFamily="Segoe MDL2 Assets" FontSize="16" Width="24" Foreground="White" />
                                <TextBlock Text="Logout" Margin="8,0,0,0" Foreground="White" />
                            </StackPanel>
                        </Button>

                    </StackPanel>
                </Border>

                <!-- CONTENT AREA -->
                <Border Grid.Column="1" Background="{StaticResource BackgroundBrush}">
                    <Border Margin="24"
                            Background="{StaticResource SurfaceBrush}"
                            CornerRadius="12"
                            Padding="24">
                        <Border.Effect>
                            <DropShadowEffect BlurRadius="16" ShadowDepth="2" Opacity="0.08" Color="Black" />
                        </Border.Effect>
                        <ContentControl Content="{Binding CurrentViewModel}" />
                    </Border>
                </Border>

            </Grid>
        </Grid>
    </Grid>
</Window>

---------
using System.Windows;
using InventoryManagement.WPF.ViewModels;

namespace InventoryManagement.WPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
-------

PS C:\Users\VideAlpha\InventoryManagementSystem\Frontend\InventoryManagement.WPF\ViewModels> tree /f   
Folder PATH listing
Volume serial number is 3A50-6A56
C:.
    CustomersViewModel.cs
    DashboardViewModel.cs
    InvoicesViewModel.cs
    LoginViewModel.cs
    MainViewModel.cs
    ProductsViewModel.cs
    ProfileViewModel.cs
    RegisterViewModel.cs
    ReportsViewModel.cs
    ViewModelBase.cs
    
No subfolders exist 


using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>Placeholder — real analytics wired in Module 8 (Dashboard Analytics).</summary>
    public partial class DashboardViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string sampleContent = "Dashboard analytics (charts, KPIs, recent activity) will be implemented in Module 8.";

        public DashboardViewModel()
        {
            Title = "Dashboard";
        }
    }
}

----------

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

------------

using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>Placeholder — full CRUD + pagination wired in Module 4 (Product Management).</summary>
    public partial class ProductsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string sampleContent = "Product list, search, filtering, and CRUD will be implemented in Module 4.";

        public ProductsViewModel()
        {
            Title = "Products";
        }
    }
}

----------
using CommunityToolkit.Mvvm.ComponentModel;

namespace InventoryManagement.WPF.ViewModels
{
    /// <summary>
    /// Common base class for all module ViewModels: busy state, error state, and a title
    /// for shell/header binding.
    /// </summary>
    public abstract partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private string title = string.Empty;

        protected void SetError(string? message)
        {
            ErrorMessage = message;
        }

        protected void ClearError()
        {
            ErrorMessage = null;
        }
    }
}


-----------
PS C:\Users\VideAlpha\InventoryManagementSystem\Frontend\InventoryManagement.WPF\Services> tree /f    
Folder PATH listing
Volume serial number is 3A50-6A56
C:.
    AuthHeaderHandler.cs
    AuthService.cs
    NavigationService.cs
    TokenStorageService.cs
    
No subfolders exist 


using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagement.WPF.Interfaces;

namespace InventoryManagement.WPF.Services
{
    /// <summary>
    /// DelegatingHandler attached to the "GatewayClient" HttpClient. Attaches the current
    /// JWT access token to outgoing requests. On a 401 response, attempts exactly one
    /// silent token refresh and retries the original request once.
    /// </summary>
    public class AuthHeaderHandler : DelegatingHandler
    {
        private static readonly HttpRequestOptionsKey<bool> RetriedKey = new("X-Auth-Retried");

        private readonly ITokenStorageService _tokenStorageService;
        private readonly IAuthService _authService;

        public AuthHeaderHandler(ITokenStorageService tokenStorageService, IAuthService authService)
        {
            _tokenStorageService = tokenStorageService;
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            AttachAccessToken(request);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusCode.Unauthorized || HasAlreadyRetried(request))
            {
                return response;
            }

            var refreshed = await _authService.RefreshTokenAsync();

            if (!refreshed)
            {
                return response;
            }

            var retryRequest = await CloneRequestAsync(request);
            retryRequest.Options.Set(RetriedKey, true);
            AttachAccessToken(retryRequest);

            response.Dispose();

            return await base.SendAsync(retryRequest, cancellationToken);
        }

        private void AttachAccessToken(HttpRequestMessage request)
        {
            var accessToken = _tokenStorageService.GetAccessToken();

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        private static bool HasAlreadyRetried(HttpRequestMessage request)
        {
            return request.Options.TryGetValue(RetriedKey, out var retried) && retried;
        }

        private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
        {
            var clone = new HttpRequestMessage(original.Method, original.RequestUri)
            {
                Version = original.Version
            };

            if (original.Content is not null)
            {
                var contentBytes = await original.Content.ReadAsByteArrayAsync();
                clone.Content = new ByteArrayContent(contentBytes);

                foreach (var header in original.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            foreach (var header in original.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var option in original.Options)
            {
                clone.Options.TryAdd(option.Key, option.Value);
            }

            return clone;
        }
    }
}

--------

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using InventoryManagement.WPF.Configuration;
using InventoryManagement.WPF.Helpers;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Services
{
    /// <summary>
    /// Handles login, registration, token refresh, auto-login, and logout against the
    /// Identity endpoints exposed by the Gateway. Uses a dedicated "AuthClient" HttpClient
    /// (no auth handler attached) for login/register/refresh to prevent recursive refresh
    /// loops, and the authenticated "GatewayClient" for logout, since that endpoint
    /// requires a valid Bearer token.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenStorageService _tokenStorageService;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        private AuthenticatedUser? _currentUser;

        public AuthService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _tokenStorageService = tokenStorageService;
        }

        public AuthenticatedUser? CurrentUser => _currentUser;

        public bool IsAuthenticated => _currentUser is not null;

        public event EventHandler<AuthenticatedUser?>? AuthenticationStateChanged;

        public async Task<ServiceResult<AuthenticatedUser>> LoginAsync(LoginRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(ApiSettings.AuthClientName);

                var httpResponse = await client.PostAsJsonAsync(ApiRoutes.Auth.Login, request, JsonOptionsProvider.Default);
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(JsonOptionsProvider.Default);

                if (!httpResponse.IsSuccessStatusCode || apiResponse?.Success != true || apiResponse.Data is null)
                {
                    return ServiceResult<AuthenticatedUser>.Fail(
                        apiResponse?.Message ?? "Invalid email or password.");
                }

                var user = ApplyAuthResult(apiResponse.Data);
                return ServiceResult<AuthenticatedUser>.Ok(user, apiResponse.Message);
            }
            catch (HttpRequestException ex)
            {
                return ServiceResult<AuthenticatedUser>.Fail($"Unable to reach the server: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthenticatedUser>.Fail($"Unexpected error during login: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(ApiSettings.AuthClientName);

                var httpResponse = await client.PostAsJsonAsync(ApiRoutes.Auth.Register, request, JsonOptionsProvider.Default);

                // Backend returns ApiResponse<string> on register (NOT AuthResponseDto) —
                // there is no token issued at registration time.
                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<string>>(JsonOptionsProvider.Default);

                if (!httpResponse.IsSuccessStatusCode || apiResponse?.Success != true)
                {
                    return ServiceResult.Fail(apiResponse?.Message ?? "Registration failed.");
                }

                return ServiceResult.Ok(apiResponse.Message ?? "Registration successful. Please log in.");
            }
            catch (HttpRequestException ex)
            {
                return ServiceResult.Fail($"Unable to reach the server: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"Unexpected error during registration: {ex.Message}");
            }
        }

        public async Task<bool> RefreshTokenAsync()
        {
            await _refreshLock.WaitAsync();
            try
            {
                var storedAccessToken = _tokenStorageService.GetAccessToken();
                var storedRefreshToken = _tokenStorageService.GetRefreshToken();

                if (string.IsNullOrWhiteSpace(storedRefreshToken))
                {
                    return false;
                }

                var client = _httpClientFactory.CreateClient(ApiSettings.AuthClientName);

                var payload = new RefreshTokenRequest
                {
                    Token = storedAccessToken ?? string.Empty,
                    RefreshToken = storedRefreshToken
                };

                var httpResponse = await client.PostAsJsonAsync(ApiRoutes.Auth.RefreshToken, payload, JsonOptionsProvider.Default);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    ClearLocalSession();
                    return false;
                }

                var apiResponse = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(JsonOptionsProvider.Default);

                if (apiResponse?.Success != true || apiResponse.Data is null || string.IsNullOrWhiteSpace(apiResponse.Data.Token))
                {
                    ClearLocalSession();
                    return false;
                }

                ApplyAuthResult(apiResponse.Data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        public async Task<ServiceResult<AuthenticatedUser>> TryAutoLoginAsync()
        {
            if (!_tokenStorageService.HasStoredSession())
            {
                return ServiceResult<AuthenticatedUser>.Fail("No stored session.");
            }

            var refreshed = await RefreshTokenAsync();

            if (!refreshed || _currentUser is null)
            {
                return ServiceResult<AuthenticatedUser>.Fail("Session expired. Please log in again.");
            }

            return ServiceResult<AuthenticatedUser>.Ok(_currentUser);
        }

        public async Task LogoutAsync()
        {
            try
            {
                // Authenticated call — GatewayClient attaches the Bearer token automatically
                // via AuthHeaderHandler. Backend identifies the user from the token's email
                // claim and invalidates their refresh token server-side.
                var client = _httpClientFactory.CreateClient(ApiSettings.GatewayClientName);
                await client.PostAsync(ApiRoutes.Auth.Logout, content: null);
            }
            catch (Exception)
            {
                // Even if the server call fails (offline, already-expired token, etc.),
                // we still clear the local session below so the user isn't stuck.
            }
            finally
            {
                ClearLocalSession();
            }
        }

        private void ClearLocalSession()
        {
            _tokenStorageService.ClearTokens();
            _currentUser = null;
            AuthenticationStateChanged?.Invoke(this, null);
        }

        /// <summary>
        /// Persists tokens, decodes JWT claims to build the current user (the backend
        /// never returns a separate user/profile object on login or refresh), and raises
        /// the authentication state changed event.
        /// </summary>
        private AuthenticatedUser ApplyAuthResult(AuthResponse authResponse)
        {
            _tokenStorageService.SaveTokens(authResponse.Token, authResponse.RefreshToken);

            var user = new AuthenticatedUser
            {
                Id = JwtHelper.GetClaimValue(authResponse.Token, "sub", "nameid", "id"),
                Email = JwtHelper.GetClaimValue(
                    authResponse.Token,
                    "email",
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"),
                Username = JwtHelper.GetClaimValue(authResponse.Token, "unique_name", "username", "name"),
                Roles = JwtHelper.GetRoles(authResponse.Token)
            };

            _currentUser = user;
            AuthenticationStateChanged?.Invoke(this, user);

            return user;
        }
    }
}

-------

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

--------
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using InventoryManagement.WPF.Interfaces;

namespace InventoryManagement.WPF.Services
{
    /// <summary>
    /// Persists JWT access/refresh tokens to disk encrypted with Windows DPAPI
    /// (CurrentUser scope), so tokens are unreadable outside the logged-in Windows user.
    /// </summary>
    public class TokenStorageService : ITokenStorageService
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("InventoryManagement.WPF.TokenStorage");

        private readonly string _storageFilePath;

        private string? _cachedAccessToken;
        private string? _cachedRefreshToken;

        public TokenStorageService()
        {
            var appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InventoryManagement.WPF");

            Directory.CreateDirectory(appDataFolder);

            _storageFilePath = Path.Combine(appDataFolder, "session.dat");

            LoadFromDisk();
        }

        public void SaveTokens(string accessToken, string refreshToken)
        {
            _cachedAccessToken = accessToken;
            _cachedRefreshToken = refreshToken;

            var payload = new TokenPayload
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var json = JsonSerializer.Serialize(payload);
            var plainBytes = Encoding.UTF8.GetBytes(json);
            var encryptedBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser);

            File.WriteAllBytes(_storageFilePath, encryptedBytes);
        }

        public string? GetAccessToken() => _cachedAccessToken;

        public string? GetRefreshToken() => _cachedRefreshToken;

        public bool HasStoredSession() =>
            !string.IsNullOrWhiteSpace(_cachedAccessToken) && !string.IsNullOrWhiteSpace(_cachedRefreshToken);

        public void ClearTokens()
        {
            _cachedAccessToken = null;
            _cachedRefreshToken = null;

            if (File.Exists(_storageFilePath))
            {
                File.Delete(_storageFilePath);
            }
        }

        private void LoadFromDisk()
        {
            if (!File.Exists(_storageFilePath))
            {
                return;
            }

            try
            {
                var encryptedBytes = File.ReadAllBytes(_storageFilePath);
                var plainBytes = ProtectedData.Unprotect(encryptedBytes, Entropy, DataProtectionScope.CurrentUser);
                var json = Encoding.UTF8.GetString(plainBytes);

                var payload = JsonSerializer.Deserialize<TokenPayload>(json);

                if (payload is not null)
                {
                    _cachedAccessToken = payload.AccessToken;
                    _cachedRefreshToken = payload.RefreshToken;
                }
            }
            catch (CryptographicException)
            {
                // Data cannot be decrypted (different user/machine, corrupted file) — discard it.
                ClearTokens();
            }
        }

        private sealed class TokenPayload
        {
            public string? AccessToken { get; set; }
            public string? RefreshToken { get; set; }
        }
    }
}

------------
PS C:\Users\VideAlpha\InventoryManagementSystem\Frontend\InventoryManagement.WPF\Resources> tree /f    
Folder PATH listing
Volume serial number is 3A50-6A56
C:.
├───Templates
│       ViewTemplates.xaml
│       
└───Themes
        Colors.xaml
        Styles.xaml

        <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                     xmlns:vm="clr-namespace:InventoryManagement.WPF.ViewModels"
                     xmlns:views="clr-namespace:InventoryManagement.WPF.Views">

    <DataTemplate DataType="{x:Type vm:LoginViewModel}">
        <views:LoginView />
    </DataTemplate>

    <DataTemplate DataType="{x:Type vm:RegisterViewModel}">
        <views:RegisterView />
    </DataTemplate>

    <DataTemplate DataType="{x:Type vm:DashboardViewModel}">
        <views:DashboardView />
    </DataTemplate>

    <DataTemplate DataType="{x:Type vm:ProductsViewModel}">
        <views:ProductsView />
    </DataTemplate>

    <DataTemplate DataType="{x:Type vm:CustomersViewModel}">
        <views:CustomersView />
    </DataTemplate>

    <DataTemplate DataType="{x:Type vm:InvoicesViewModel}">
        <views:InvoicesView />
    </DataTemplate>

    <DataTemplate DataType="{x:Type vm:ReportsViewModel}">
        <views:ReportsView />
    </DataTemplate>

    <DataTemplate DataType="{x:Type vm:ProfileViewModel}">
        <views:ProfileView />
    </DataTemplate>

</ResourceDictionary>

--------

<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Color x:Key="PrimaryColor">#FF1E3A5F</Color>
    <Color x:Key="PrimaryLightColor">#FF2E5A8F</Color>
    <Color x:Key="AccentColor">#FF00A8E8</Color>
    <Color x:Key="BackgroundColor">#FFF5F6F8</Color>
    <Color x:Key="SurfaceColor">#FFFFFFFF</Color>
    <Color x:Key="TextPrimaryColor">#FF1B1F23</Color>
    <Color x:Key="TextSecondaryColor">#FF5A6472</Color>
    <Color x:Key="BorderColor">#FFD8DCE1</Color>
    <Color x:Key="ErrorColor">#FFD64545</Color>
    <Color x:Key="SuccessColor">#FF2E9E5B</Color>
    <Color x:Key="WarningColor">#FFE0A32E</Color>

    <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource PrimaryColor}" />
    <SolidColorBrush x:Key="PrimaryLightBrush" Color="{StaticResource PrimaryLightColor}" />
    <SolidColorBrush x:Key="AccentBrush" Color="{StaticResource AccentColor}" />
    <SolidColorBrush x:Key="BackgroundBrush" Color="{StaticResource BackgroundColor}" />
    <SolidColorBrush x:Key="SurfaceBrush" Color="{StaticResource SurfaceColor}" />
    <SolidColorBrush x:Key="TextPrimaryBrush" Color="{StaticResource TextPrimaryColor}" />
    <SolidColorBrush x:Key="TextSecondaryBrush" Color="{StaticResource TextSecondaryColor}" />
    <SolidColorBrush x:Key="BorderBrush" Color="{StaticResource BorderColor}" />
    <SolidColorBrush x:Key="ErrorBrush" Color="{StaticResource ErrorColor}" />
    <SolidColorBrush x:Key="SuccessBrush" Color="{StaticResource SuccessColor}" />
    <SolidColorBrush x:Key="WarningBrush" Color="{StaticResource WarningColor}" />

</ResourceDictionary>

----------

<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Style TargetType="{x:Type Window}">
        <Setter Property="Background" Value="{StaticResource BackgroundBrush}" />
        <Setter Property="FontFamily" Value="Segoe UI" />
        <Setter Property="FontSize" Value="14" />
        <Setter Property="TextElement.Foreground" Value="{StaticResource TextPrimaryBrush}" />
    </Style>

    <Style TargetType="{x:Type Button}">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}" />
        <Setter Property="Foreground" Value="White" />
        <Setter Property="Padding" Value="14,8" />
        <Setter Property="BorderThickness" Value="0" />
        <Setter Property="Cursor" Value="Hand" />
        <Setter Property="FontWeight" Value="SemiBold" />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="{x:Type Button}">
                    <Border Background="{TemplateBinding Background}"
                            CornerRadius="4"
                            Padding="{TemplateBinding Padding}">
                        <ContentPresenter HorizontalAlignment="Center"
                                           VerticalAlignment="Center" />
                    </Border>
                    <ControlTemplate.Triggers>
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter Property="Background" Value="{StaticResource PrimaryLightBrush}" />
                        </Trigger>
                        <Trigger Property="IsEnabled" Value="False">
                            <Setter Property="Opacity" Value="0.5" />
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <Style TargetType="{x:Type TextBox}">
        <Setter Property="Padding" Value="8,6" />
        <Setter Property="BorderBrush" Value="{StaticResource BorderBrush}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="Background" Value="{StaticResource SurfaceBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="VerticalContentAlignment" Value="Center" />
    </Style>

    <Style TargetType="{x:Type PasswordBox}">
        <Setter Property="Padding" Value="8,6" />
        <Setter Property="BorderBrush" Value="{StaticResource BorderBrush}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="Background" Value="{StaticResource SurfaceBrush}" />
        <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}" />
        <Setter Property="VerticalContentAlignment" Value="Center" />
    </Style>

</ResourceDictionary>

----------

Volume serial number is 3A50-6A56
C:.
    ApiResponse.cs
    AuthenticatedUser.cs
    AuthResponse.cs
    LoginRequest.cs
    RefreshTokenRequest.cs
    RegisterRequest.cs
    ServiceResult.cs
    
No subfolders exist 

using System.Collections.Generic;

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Generic envelope used to deserialize standard API responses from the Gateway.
    /// Adjust property names in Module 2 if your backend's actual envelope differs.
    /// </summary>
    /// <typeparam name="T">Type of the payload returned in the "Data" field.</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public T? Data { get; set; }

        public List<string>? Errors { get; set; }
    }
}

-------
using System.Collections.Generic;

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Represents the currently authenticated user, built primarily from JWT claims.
    /// </summary>
    public class AuthenticatedUser
    {
        public string? Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}

-------

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Payload returned inside ApiResponse&lt;AuthResponseDto&gt;.Data by the login and
    /// refresh-token endpoints. Matches IdentityService.API.Auth.DTOs.AuthResponseDto
    /// EXACTLY: property is "Token" (not "AccessToken"), plus "RefreshToken".
    /// No User object is ever returned — identity is derived client-side from JWT claims
    /// (see JwtHelper) since the backend never echoes a user/profile object here.
    /// </summary>
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}

-------
namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Payload for POST /api/v1/auth/login.
    /// ASSUMPTION: backend expects Email + Password. Adjust here if your DTO differs.
    /// </summary>
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

--------
namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Payload for POST /api/v1/auth/refresh-token.
    /// ASSUMPTION: your snippet shows RefreshTokenController usage
    /// (RefreshTokenRequestDto dto) but not the DTO's own property definitions.
    /// This shape mirrors AuthResponseDto's naming convention (Token + RefreshToken),
    /// which is the most likely match given the rest of the codebase's consistency.
    /// If your actual RefreshTokenRequestDto uses different property names
    /// (e.g. just "RefreshToken" alone), tell me and this is a one-file fix.
    /// </summary>
    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}

--------


namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Payload for POST /api/v1/auth/register.
    /// Matches IdentityService.API.Auth.DTOs.RegisterDto EXACTLY: Email + Password only.
    /// There is no FirstName/LastName/Username at registration time in this backend —
    /// those fields belong to the Profile module (UpdateProfileDto) instead.
    /// </summary>
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}

---------

using System.Collections.Generic;

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Normalized result returned by application services to ViewModels, decoupling
    /// ViewModels from the raw ApiResponse&lt;T&gt; wire envelope.
    /// </summary>
    public class ServiceResult
    {
        public bool Success { get; init; }

        public string? Message { get; init; }

        public List<string>? Errors { get; init; }

        public static ServiceResult Ok(string? message = null) =>
            new() { Success = true, Message = message };

        public static ServiceResult Fail(string? message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; init; }

        public static ServiceResult<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static new ServiceResult<T> Fail(string? message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }
}
------------

PS C:\Users\VideAlpha\InventoryManagementSystem\Frontend\InventoryManagement.WPF\interfaces> tree /f   
Folder PATH listing
Volume serial number is 3A50-6A56
C:.
    IAuthService.cs
    INavigationService.cs
    ITokenStorageService.cs
    
No subfolders exist 

using System;
using System.Threading.Tasks;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Interfaces
{
    /// <summary>
    /// Contract for authentication operations: login, registration, silent token refresh,
    /// auto-login on startup, and logout. Also exposes the current authenticated identity.
    /// </summary>
    public interface IAuthService
    {
        AuthenticatedUser? CurrentUser { get; }

        bool IsAuthenticated { get; }

        event EventHandler<AuthenticatedUser?>? AuthenticationStateChanged;

        Task<ServiceResult<AuthenticatedUser>> LoginAsync(LoginRequest request);

        /// <summary>
        /// Registers a new account. NOTE: the backend does not return tokens on register —
        /// the user must log in separately afterward.
        /// </summary>
        Task<ServiceResult> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Attempts to refresh the access token using the stored refresh token.
        /// Used both by AuthHeaderHandler (on 401) and TryAutoLoginAsync (on startup).
        /// </summary>
        Task<bool> RefreshTokenAsync();

        Task<ServiceResult<AuthenticatedUser>> TryAutoLoginAsync();

        /// <summary>
        /// Calls the backend logout endpoint (invalidates the refresh token server-side)
        /// then clears local storage regardless of whether the server call succeeded.
        /// </summary>
        Task LogoutAsync();
    }
}

----------

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


-------
namespace InventoryManagement.WPF.Interfaces
{
    /// <summary>
    /// Contract for securely persisting and retrieving JWT access/refresh tokens
    /// across application sessions.
    /// </summary>
    public interface ITokenStorageService
    {
        void SaveTokens(string accessToken, string refreshToken);

        string? GetAccessToken();

        string? GetRefreshToken();

        void ClearTokens();

        bool HasStoredSession();
    }
}


------
PS C:\Users\VideAlpha\InventoryManagementSystem\Frontend\InventoryManagement.WPF\Helpers> tree /f   
Folder PATH listing
Volume serial number is 3A50-6A56
C:.
    AsyncCommandHelper.cs
    DesignTimeHelper.cs
    JsonOptionsProvider.cs
    JwtHelper.cs
    
No subfolders exist 


using System;
using System.Threading.Tasks;
using System.Windows;

namespace InventoryManagement.WPF.Helpers
{
    /// <summary>
    /// Safely awaits a fire-and-forget Task (e.g. from an async void event handler or
    /// AsyncRelayCommand execution) and routes any exception to a handler instead of
    /// crashing the application via an unobserved exception.
    /// </summary>
    public static class AsyncCommandHelper
    {
        public static async void FireAndForget(this Task task, Action<Exception>? onException = null)
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                if (onException is not null)
                {
                    onException(ex);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
            }
        }
    }
}


-----------

using System.ComponentModel;
using System.Windows;

namespace InventoryManagement.WPF.Helpers
{
    /// <summary>
    /// Detects whether code is running inside the XAML designer (Blend/Visual Studio)
    /// so ViewModels can supply design-time sample data safely.
    /// </summary>
    public static class DesignTimeHelper
    {
        public static bool IsInDesignMode =>
            DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}
-------------
using System.Text.Json;

namespace InventoryManagement.WPF.Helpers
{
    /// <summary>
    /// Shared JsonSerializerOptions for all HTTP communication with the Gateway,
    /// tolerant of camelCase (typical ASP.NET Core default) vs PascalCase payloads.
    /// </summary>
    public static class JsonOptionsProvider
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace InventoryManagement.WPF.Helpers
{
    /// <summary>
    /// Minimal, dependency-free JWT payload decoder. Reads claims directly from the
    /// access token so the client can determine identity/roles/expiry without requiring
    /// the backend to echo a separate "user" object on every auth response.
    /// </summary>
    public static class JwtHelper
    {
        public static Dictionary<string, JsonElement> GetClaims(string jwtToken)
        {
            var claims = new Dictionary<string, JsonElement>();

            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                return claims;
            }

            var parts = jwtToken.Split('.');
            if (parts.Length < 2)
            {
                return claims;
            }

            var payloadJson = DecodeBase64Url(parts[1]);

            using var document = JsonDocument.Parse(payloadJson);
            foreach (var property in document.RootElement.EnumerateObject())
            {
                claims[property.Name] = property.Value.Clone();
            }

            return claims;
        }

        public static DateTime? GetExpiryUtc(string jwtToken)
        {
            var claims = GetClaims(jwtToken);

            if (claims.TryGetValue("exp", out var expElement) && expElement.TryGetInt64(out var expUnix))
            {
                return DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            }

            return null;
        }

        public static string? GetClaimValue(string jwtToken, params string[] possibleClaimNames)
        {
            var claims = GetClaims(jwtToken);

            foreach (var claimName in possibleClaimNames)
            {
                if (claims.TryGetValue(claimName, out var value))
                {
                    return value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
                }
            }

            return null;
        }

        public static List<string> GetRoles(string jwtToken)
        {
            var roles = new List<string>();
            var claims = GetClaims(jwtToken);

            string[] roleClaimNames =
            {
                "role",
                "roles",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };

            foreach (var claimName in roleClaimNames)
            {
                if (!claims.TryGetValue(claimName, out var value))
                {
                    continue;
                }

                if (value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in value.EnumerateArray())
                    {
                        var role = item.GetString();
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            roles.Add(role);
                        }
                    }
                }
                else if (value.ValueKind == JsonValueKind.String)
                {
                    var role = value.GetString();
                    if (!string.IsNullOrWhiteSpace(role))
                    {
                        roles.Add(role);
                    }
                }
            }

            return roles;
        }

        private static string DecodeBase64Url(string input)
        {
            var base64 = input.Replace('-', '+').Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}

PS C:\Users\VideAlpha\InventoryManagementSystem\Frontend\InventoryManagement.WPF\Converters> tree /f   
Folder PATH listing
Volume serial number is 3A50-6A56
C:.
    BoolToVisibilityConverter.cs
    InverseBooleanConverter.cs
    MenuSelectionConverter.cs
    
No subfolders exist

------------
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// Converts a bool to Visibility. Pass ConverterParameter="Invert" to flip the logic
    /// (useful for binding IsBusy to a Visibility.Collapsed content panel, etc.).
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                var invert = parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase);
                if (invert)
                {
                    boolValue = !boolValue;
                }

                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }

            return false;
        }
    }
}

-------

using System;
using System.Globalization;
using System.Windows.Data;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// Inverts a boolean value. Commonly used for IsEnabled="{Binding IsBusy, Converter=...}"
    /// so controls disable while a request is in flight.
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue ? !boolValue : value;
        }
    }
}

-------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace InventoryManagement.WPF.Converters
{
    /// <summary>
    /// MultiValueConverter used by sidebar menu buttons to compute their own
    /// Background/Foreground based on whether they match the currently selected menu key.
    /// Usage: bind [0] = MainViewModel.SelectedMenuItem, [1] = the button's own Tag
    /// (e.g. "Dashboard"). ConverterParameter = "Background" or "Foreground".
    /// </summary>
    public class MenuSelectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var isSelected = values.Length == 2
                && values[0] is string selectedKey
                && values[1] is string buttonKey
                && string.Equals(selectedKey, buttonKey, StringComparison.OrdinalIgnoreCase);

            var role = parameter as string;

            if (role == "Foreground")
            {
                return isSelected
                    ? Brushes.White
                    : (Brush)Application.Current.Resources["TextSecondaryBrush"];
            }

            return isSelected
                ? (Brush)Application.Current.Resources["AccentBrush"]
                : Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

-----------

PS C:\Users\VideAlpha\InventoryManagementSystem\Frontend\InventoryManagement.WPF\configuration> tree /f

Folder PATH listing
Volume serial number is 3A50-6A56
C:.
    ApiRoutes.cs
    ApiSettings.cs
    
No subfolders exist 

namespace InventoryManagement.WPF.Configuration
{
    /// <summary>
    /// Centralized relative route definitions for Gateway-routed microservice endpoints.
    /// Verified against IdentityService.API's AuthController + Gateway ReverseProxy config.
    /// Route matching is case-insensitive in ASP.NET Core / YARP, so lowercase is safe
    /// even though the controller segment resolves to "Auth" server-side.
    /// </summary>
    public static class ApiRoutes
    {
        public static class Auth
        {
            public const string Register = "api/v1/auth/register";
            public const string Login = "api/v1/auth/login";
            public const string RefreshToken = "api/v1/auth/refresh-token";
            public const string Logout = "api/v1/auth/logout";
        }
    }
}


namespace InventoryManagement.WPF.Configuration
{
    /// <summary>
    /// Strongly typed configuration bound from the "ApiSettings" section of appsettings.json.
    /// </summary>
    public class ApiSettings
    {
        public const string SectionName = "ApiSettings";

        /// <summary>Named HttpClient used for authenticated calls to feature modules (attaches JWT, auto-refreshes on 401).</summary>
        public const string GatewayClientName = "GatewayClient";

        /// <summary>Named HttpClient used for login/register/refresh-token calls. Has NO auth handler attached, to avoid recursive refresh loops.</summary>
        public const string AuthClientName = "AuthClient";

        public string GatewayBaseUrl { get; set; } = string.Empty;

        public int TimeoutSeconds { get; set; } = 30;
    }
}