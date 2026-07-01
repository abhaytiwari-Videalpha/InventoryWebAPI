using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace IdentityService.Tests.Integration;

public class AuthControllerIntegrationTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthEndpoint_ShouldReturnSuccess()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}