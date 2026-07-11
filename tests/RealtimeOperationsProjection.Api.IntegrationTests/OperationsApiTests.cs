using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RealtimeOperationsProjection.Api.IntegrationTests;

public sealed class OperationsApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Creating_operation_returns_persisted_projection()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/operations", new { name = "integration-pick" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var snapshot = await response.Content.ReadFromJsonAsync<OperationResponse>();
        Assert.NotNull(snapshot);
        Assert.Equal("Created", snapshot.Status);
        var reloaded = await client.GetAsync($"/api/operations/{snapshot.Id}");
        Assert.Equal(HttpStatusCode.OK, reloaded.StatusCode);
    }

    private sealed record OperationResponse(Guid Id, string Status);
}
