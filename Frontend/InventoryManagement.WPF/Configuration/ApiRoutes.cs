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