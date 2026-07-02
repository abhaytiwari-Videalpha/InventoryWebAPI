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