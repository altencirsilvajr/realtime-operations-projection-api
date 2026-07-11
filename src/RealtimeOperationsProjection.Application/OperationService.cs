using RealtimeOperationsProjection.Domain;

namespace RealtimeOperationsProjection.Application;

public sealed class OperationService(IOperationStore store, IOperationProjectionPublisher publisher)
{
    public async Task<OperationSnapshot> RegisterAsync(string name, CancellationToken cancellationToken)
    {
        var operation = Operation.Register(name);
        var now = DateTimeOffset.UtcNow;
        var snapshot = new OperationSnapshot(
            operation.Id,
            operation.Name,
            operation.Status,
            now,
            now,
            [new OperationTimelineEvent(null, operation.Status, now)]);

        await store.CreateAsync(snapshot, cancellationToken);
        await publisher.PublishAsync(snapshot, cancellationToken);
        return snapshot;
    }

    public async Task<OperationSnapshot> TransitionAsync(Guid operationId, OperationStatus nextStatus, CancellationToken cancellationToken)
    {
        var current = await store.FindAsync(operationId, cancellationToken)
            ?? throw new OperationNotFoundException(operationId);
        var operation = Operation.Rehydrate(current.Id, current.Name, current.Status);
        operation.TransitionTo(nextStatus);
        var domainEvent = operation.DomainEvents.Single();
        var snapshot = current with
        {
            Status = operation.Status,
            LastChangedAt = domainEvent.OccurredAt,
            Timeline = [.. current.Timeline, new OperationTimelineEvent(domainEvent.PreviousStatus, domainEvent.NewStatus, domainEvent.OccurredAt)]
        };

        await store.SaveTransitionAsync(snapshot, cancellationToken);
        await publisher.PublishAsync(snapshot, cancellationToken);
        return snapshot;
    }

    public async Task<OperationSnapshot> GetAsync(Guid operationId, CancellationToken cancellationToken) =>
        await store.FindAsync(operationId, cancellationToken)
        ?? throw new OperationNotFoundException(operationId);
}

public sealed class OperationNotFoundException(Guid operationId)
    : Exception($"Operation '{operationId}' was not found.");
