namespace RealtimeOperationsProjection.Application;

public interface IOperationStore
{
    Task CreateAsync(OperationSnapshot snapshot, CancellationToken cancellationToken);
    Task<OperationSnapshot?> FindAsync(Guid operationId, CancellationToken cancellationToken);
    Task SaveTransitionAsync(OperationSnapshot snapshot, CancellationToken cancellationToken);
}
