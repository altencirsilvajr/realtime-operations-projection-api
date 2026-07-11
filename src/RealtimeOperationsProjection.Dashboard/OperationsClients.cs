using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;

namespace RealtimeOperationsProjection.Dashboard;

public sealed class OperationsApiClient(HttpClient httpClient)
{
    public async Task<OperationSnapshot> CreateAsync(string name) => await SendAsync(httpClient.PostAsJsonAsync("api/operations", new { name }));
    public async Task<OperationSnapshot> TransitionAsync(Guid id, string status) => await SendAsync(httpClient.PostAsJsonAsync($"api/operations/{id}/transitions", new { status }));
    public async Task<OperationSnapshot> GetAsync(Guid id) => await SendAsync(httpClient.GetAsync($"api/operations/{id}"));

    private static async Task<OperationSnapshot> SendAsync(Task<HttpResponseMessage> task)
    {
        var response = await task;
        if (!response.IsSuccessStatusCode) throw new OperationsApiException(await response.Content.ReadAsStringAsync());
        return await response.Content.ReadFromJsonAsync<OperationSnapshot>() ?? throw new OperationsApiException("Empty API payload.");
    }
}

public sealed class RealtimeOperationsClient : IAsyncDisposable
{
    private readonly HubConnection _connection = new HubConnectionBuilder().WithUrl("http://localhost:5308/hubs/operations").WithAutomaticReconnect().Build();
    public event Func<OperationSnapshot, Task>? ProjectionReceived;
    public event Func<Task>? Reconnected;
    public string State => _connection.State.ToString();

    public RealtimeOperationsClient()
    {
        _connection.On<OperationSnapshot>("operationProjectionUpdated", snapshot => ProjectionReceived?.Invoke(snapshot) ?? Task.CompletedTask);
        _connection.Reconnected += _ => Reconnected?.Invoke() ?? Task.CompletedTask;
    }

    public Task ConnectAsync() => _connection.State == HubConnectionState.Disconnected ? _connection.StartAsync() : Task.CompletedTask;
    public ValueTask DisposeAsync() => _connection.DisposeAsync();
}

public sealed class OperationsApiException(string message) : Exception(message);
public sealed record OperationSnapshot(Guid Id, string Name, string Status, DateTimeOffset CreatedAt, DateTimeOffset LastChangedAt, IReadOnlyList<OperationTimelineEvent> Timeline);
public sealed record OperationTimelineEvent(string? PreviousStatus, string NewStatus, DateTimeOffset OccurredAt);
