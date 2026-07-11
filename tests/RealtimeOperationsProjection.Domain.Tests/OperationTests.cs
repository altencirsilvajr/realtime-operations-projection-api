using RealtimeOperationsProjection.Domain;

namespace RealtimeOperationsProjection.Domain.Tests;

public sealed class OperationTests
{
    [Fact]
    public void Transitioning_created_operation_to_processing_records_domain_event()
    {
        var operation = Operation.Register("warehouse-pick");

        operation.TransitionTo(OperationStatus.Processing);

        Assert.Equal(OperationStatus.Processing, operation.Status);
        var domainEvent = Assert.Single(operation.DomainEvents);
        Assert.Equal(OperationStatus.Processing, domainEvent.NewStatus);
    }
}
