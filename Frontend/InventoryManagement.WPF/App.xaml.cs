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