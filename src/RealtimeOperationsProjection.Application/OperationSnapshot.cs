using RealtimeOperationsProjection.Domain;

namespace RealtimeOperationsProjection.Application;

public sealed record OperationSnapshot(
    Guid Id,
    string Name,
    OperationStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset LastChangedAt,
    IReadOnlyList<OperationTimelineEvent> Timeline);

public sealed record OperationTimelineEvent(
    OperationStatus? PreviousStatus,
    OperationStatus NewStatus,
    DateTimeOffset OccurredAt);
