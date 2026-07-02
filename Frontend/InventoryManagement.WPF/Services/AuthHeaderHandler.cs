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