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