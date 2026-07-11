namespace RealtimeOperationsProjection.Domain;

public sealed class InvalidOperationTransitionException(OperationStatus currentStatus, OperationStatus requestedStatus)
    : Exception($"Cannot transition from {currentStatus} to {requestedStatus}.");
