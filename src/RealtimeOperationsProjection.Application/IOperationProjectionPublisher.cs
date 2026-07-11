namespace RealtimeOperationsProjection.Application;

public interface IOperationProjectionPublisher
{
    Task PublishAsync(OperationSnapshot snapshot, CancellationToken cancellationToken);
}
