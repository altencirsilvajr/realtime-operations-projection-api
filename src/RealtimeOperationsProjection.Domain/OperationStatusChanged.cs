namespace RealtimeOperationsProjection.Domain;

public sealed record OperationStatusChanged(
    Guid OperationId,
    OperationStatus PreviousStatus,
    OperationStatus NewStatus,
    DateTimeOffset OccurredAt);
