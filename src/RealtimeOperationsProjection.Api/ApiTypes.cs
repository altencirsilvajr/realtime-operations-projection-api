using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RealtimeOperationsProjection.Application;
using RealtimeOperationsProjection.Domain;

namespace RealtimeOperationsProjection.Api;

public sealed record CreateOperationRequest(string Name);
public sealed record TransitionOperationRequest(OperationStatus Status);
public sealed record OperationResponse(Guid Id, string Name, OperationStatus Status, DateTimeOffset CreatedAt, DateTimeOffset LastChangedAt, IReadOnlyList<OperationTimelineResponse> Timeline);
public sealed record OperationTimelineResponse(OperationStatus? PreviousStatus, OperationStatus NewStatus, DateTimeOffset OccurredAt);

public static class OperationResponseMapper
{
    public static OperationResponse ToResponse(this OperationSnapshot snapshot) => new(snapshot.Id, snapshot.Name, snapshot.Status, snapshot.CreatedAt, snapshot.LastChangedAt,
        snapshot.Timeline.Select(item => new OperationTimelineResponse(item.PreviousStatus, item.NewStatus, item.OccurredAt)).ToList());
}

public sealed class OperationsHub : Microsoft.AspNetCore.SignalR.Hub;

public sealed class SignalROperationProjectionPublisher(Microsoft.AspNetCore.SignalR.IHubContext<OperationsHub> hubContext) : IOperationProjectionPublisher
{
    public Task PublishAsync(OperationSnapshot snapshot, CancellationToken cancellationToken) =>
        hubContext.Clients.All.SendAsync("operationProjectionUpdated", snapshot.ToResponse(), cancellationToken);
}

public sealed class DatabaseHealthCheck(RealtimeOperationsProjection.Infrastructure.OperationsDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await dbContext.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy("SQLite projection store is reachable.")
            : HealthCheckResult.Unhealthy("SQLite projection store is unavailable.");
}

public sealed class OperationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            OperationNotFoundException => (StatusCodes.Status404NotFound, "Operation not found"),
            InvalidOperationTransitionException => (StatusCodes.Status409Conflict, "Invalid operation transition"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid operation request"),
            _ => (0, string.Empty)
        };
        if (status == 0)
        {
            return false;
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status, Title = title, Detail = exception.Message, Instance = httpContext.Request.Path
        }, cancellationToken);
        return true;
    }
}
