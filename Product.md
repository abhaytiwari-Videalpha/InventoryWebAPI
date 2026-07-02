namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Mirrors ProductService.API.DTOs.CreateProductDto.
    /// Used for POST /api/v1/Products.
    /// </summary>
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}

---------

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Mirrors ProductService.API.Responses.PagedResponse<T>.
    /// </summary>
    /// <typeparam name="T">Response payload type.</typeparam>
    public class PagedResponse<T> : ApiResponse<T>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages { get; set; }
    }
}

----------

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Mirrors ProductService.API.DTOs.ProductDto.
    /// DO NOT MODIFY without backend changes.
    /// </summary>
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}

------------

namespace InventoryManagement.WPF.Models
{
    /// <summary>
    /// Mirrors ProductService.API.DTOs.UpdateProductDto.
    /// Used for PUT /api/v1/Products/{id}.
    /// </summary>
    public class UpdateProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}

--------

using System.Net.Http.Json;
using InventoryManagement.WPF.Configuration;
using InventoryManagement.WPF.Interfaces;
using InventoryManagement.WPF.Models;
using System.Net.Http;


namespace InventoryManagement.WPF.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GatewayClient");
        }

        public async Task<PagedResponse<List<ProductDto>>> GetProductsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            string? sortOrder = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var queryParams = new List<string>
            {
                $"PageNumber={pageNumber}",
                $"PageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"Search={Uri.EscapeDataString(search)}");

            if (!string.IsNullOrWhiteSpace(sortBy))
                queryParams.Add($"SortBy={sortBy}");

            if (!string.IsNullOrWhiteSpace(sortOrder))
                queryParams.Add($"SortOrder={sortOrder}");

            if (minPrice.HasValue)
                queryParams.Add($"MinPrice={minPrice}");

            if (maxPrice.HasValue)
                queryParams.Add($"MaxPrice={maxPrice}");

            var endpoint =
                $"{ApiRoutes.Products}?{string.Join("&", queryParams)}";

            var response =
                await _httpClient.GetFromJsonAsync<
                    PagedResponse<List<ProductDto>>>(endpoint);

            return response ?? new PagedResponse<List<ProductDto>>
            {
                Success = false,
                Message = "Failed to retrieve products",
                Data = new List<ProductDto>()
            };
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var response =
                await _httpClient.GetFromJsonAsync<
                    ApiResponse<ProductDto>>
                    ($"{ApiRoutes.Products}/{id}");

            return response?.Data;
        }

        public async Task<bool> CreateProductAsync(CreateProductDto product)
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    ApiRoutes.Products,
                    product);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(
            int id,
            UpdateProductDto product)
        {
            var response =
                await _httpClient.PutAsJsonAsync(
                    $"{ApiRoutes.Products}/{id}",
                    product);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var response =
                await _httpClient.DeleteAsync(
                    $"{ApiRoutes.Products}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}

----------

<Window x:Class="InventoryManagement.WPF.Views.ProductDialog"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Product"
        Height="350"
        Width="450"
        ResizeMode="NoResize"
        WindowStartupLocation="CenterOwner">

    <Grid Margin="20">

        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <TextBlock Text="Product Name"
                   Margin="0,0,0,5"/>

        <TextBox Grid.Row="1"
                 Height="35"
                 Text="{Binding Name}" />

        <StackPanel Grid.Row="2"
                    Orientation="Horizontal"
                    Margin="0,15,0,0">

            <StackPanel Width="180">

                <TextBlock Text="Price"/>
                <TextBox Height="35"
                         Text="{Binding Price}" />

            </StackPanel>

            <StackPanel Width="180"
                        Margin="20,0,0,0">

                <TextBlock Text="Quantity"/>
                <TextBox Height="35"
                         Text="{Binding Quantity}" />

            </StackPanel>

        </StackPanel>

        <StackPanel Grid.Row="4"
                    Orientation="Horizontal"
                    HorizontalAlignment="Right">

            <Button Content="Save"
                    Width="100"
                    Margin="0,0,10,0"
                    Click="Save_Click"/>

            <Button Content="Cancel"
                    Width="100"
                    Click="Cancel_Click"/>

        </StackPanel>

    </Grid>

</Window>

--------

using System.Windows;
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Views
{
    public partial class ProductDialog : Window
    {
        public ProductDto Product { get; }

        public ProductDialog(ProductDto product)
        {
            InitializeComponent();

            Product = product;
            DataContext = Product;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

---------
<UserControl x:Class="InventoryManagement.WPF.Views.ProductsView"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             mc:Ignorable="d">

    <Grid Margin="10">

        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Header -->
        <TextBlock Grid.Row="0"
                   Text="{Binding Title}"
                   FontSize="24"
                   FontWeight="Bold"
                   Margin="0,0,0,15"/>

        <!-- Search Area -->
        <StackPanel Grid.Row="1"
                    Orientation="Horizontal"
                    Margin="0,0,0,15">

            <TextBox Width="250"
                     Height="35"
                     Margin="0,0,10,0"
                     VerticalContentAlignment="Center"
                     Text="{Binding SearchText, UpdateSourceTrigger=PropertyChanged}" />

            <Button Content="Search"
                    Width="100"
                    Height="35"
                    Margin="0,0,10,0"
                    Command="{Binding SearchCommand}" />

            <Button Content="Refresh"
                    Width="100"
                    Height="35"
                    Margin="0,0,10,0"
                    Command="{Binding RefreshCommand}" />

            <Button Content="Add Product"
                    Width="120"
                    Height="35"
                    Command="{Binding AddProductCommand}" />

        </StackPanel>

        <!-- Product Grid -->
        <DataGrid Grid.Row="2"
                  ItemsSource="{Binding Products}"
                  SelectedItem="{Binding SelectedProduct}"
                  AutoGenerateColumns="False"
                  CanUserAddRows="False"
                  IsReadOnly="True"
                  Margin="0,0,0,10">

            <DataGrid.Columns>

                <DataGridTextColumn Header="ID"
                                    Binding="{Binding ProductId}"
                                    Width="100"/>

                <DataGridTextColumn Header="Name"
                                    Binding="{Binding Name}"
                                    Width="*"/>

                <DataGridTextColumn Header="Price"
                                    Binding="{Binding Price, StringFormat={}{0:N2}}"
                                    Width="150"/>

                <DataGridTextColumn Header="Quantity"
                                    Binding="{Binding Quantity}"
                                    Width="150"/>

                <DataGridTemplateColumn Header="Actions"
                                        Width="200">

                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>

                            <StackPanel Orientation="Horizontal">

                                <Button Content="Edit"
                                        Width="60"
                                        Margin="0,0,5,0"
                                        Command="{Binding DataContext.EditProductCommand,
                                                          RelativeSource={RelativeSource AncestorType=DataGrid}}"
                                        CommandParameter="{Binding}" />

                                <Button Content="Delete"
                                        Width="60"
                                        Command="{Binding DataContext.DeleteProductCommand,
                                                          RelativeSource={RelativeSource AncestorType=DataGrid}}"
                                        CommandParameter="{Binding}" />

                            </StackPanel>

                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>

                </DataGridTemplateColumn>

            </DataGrid.Columns>

        </DataGrid>

        <!-- Footer -->
        <StackPanel Grid.Row="3"
                    Orientation="Horizontal"
                    HorizontalAlignment="Center">

            <Button Content="Previous"
                    Width="100"
                    Margin="5"
                    Command="{Binding PreviousPageCommand}" />

            <TextBlock Margin="15,0"
                       VerticalAlignment="Center"
                       FontWeight="SemiBold"
                       Text="{Binding CurrentPage}" />

            <Button Content="Next"
                    Width="100"
                    Margin="5"
                    Command="{Binding NextPageCommand}" />

            <TextBlock Margin="20,0,0,0"
                       VerticalAlignment="Center"
                       Text="{Binding TotalRecords,
                                      StringFormat=Total Records: {0}}" />

        </StackPanel>

        <!-- Loading Overlay -->
        <Border Background="#80000000"
                Visibility="{Binding IsBusy,
                                     Converter={StaticResource BooleanToVisibilityConverter}}">

            <TextBlock Text="Loading..."
                       FontSize="24"
                       Foreground="White"
                       HorizontalAlignment="Center"
                       VerticalAlignment="Center"/>

        </Border>

    </Grid>

</UserControl>

---------

using System.Windows.Controls;

namespace InventoryManagement.WPF.Views
{
    public partial class ProductsView : UserControl
    {
        public ProductsView()
        {
            InitializeComponent();
        }
    }
}


------
using InventoryManagement.WPF.Models;

namespace InventoryManagement.WPF.Interfaces
{
    public interface IProductService
    {
        Task<PagedResponse<List<ProductDto>>> GetProductsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            string? sortBy = null,
            string? sortOrder = null,
            decimal? minPrice = null,
            decimal? maxPrice = null);

        Task<ProductDto?> GetProductByIdAsync(int id);

        Task<bool> CreateProductAsync(CreateProductDto product);

        Task<bool> UpdateProductAsync(int id, UpdateProductDto product);

        Task<bool> DeleteProductAsync(int id);
    }
}

-----
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

        public const string Products = "api/v1/Products";
    }
}

--------

<Application x:Class="InventoryManagement.WPF.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Application.Resources>
        <ResourceDictionary>

            <BooleanToVisibilityConverter x:Key="BooleanToVisibilityConverter"/>

            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="Resources/Themes/Colors.xaml"/>
                <ResourceDictionary Source="Resources/Themes/Styles.xaml"/>
                <ResourceDictionary Source="Resources/Templates/ViewTemplates.xaml"/>
            </ResourceDictionary.MergedDictionaries>

        </ResourceDictionary>
    </Application.Resources>

</Application>

---------

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

            //Product Module 
            services.AddTransient<IProductService, ProductService>();

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