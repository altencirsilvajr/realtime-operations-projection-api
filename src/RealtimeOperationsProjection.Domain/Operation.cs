namespace RealtimeOperationsProjection.Domain;

public sealed class Operation
{
    private readonly List<OperationStatusChanged> _domainEvents = [];

    private Operation(Guid id, string name, OperationStatus status)
    {
        Id = id;
        Name = name;
        Status = status;
    }

    public Guid Id { get; }
    public string Name { get; }
    public OperationStatus Status { get; private set; }
    public IReadOnlyCollection<OperationStatusChanged> DomainEvents => _domainEvents.AsReadOnly();

    public static Operation Register(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Operation name is required.", nameof(name));
        }

        return new Operation(Guid.NewGuid(), name.Trim(), OperationStatus.Created);
    }

    public static Operation Rehydrate(Guid id, string name, OperationStatus status) => new(id, name, status);

    public void TransitionTo(OperationStatus nextStatus)
    {
        if (!CanTransitionTo(nextStatus))
        {
            throw new InvalidOperationTransitionException(Status, nextStatus);
        }

        var previousStatus = Status;
        Status = nextStatus;
        _domainEvents.Add(new OperationStatusChanged(Id, previousStatus, nextStatus, DateTimeOffset.UtcNow));
    }

    private bool CanTransitionTo(OperationStatus nextStatus) => (Status, nextStatus) switch
    {
        (OperationStatus.Created, OperationStatus.Processing) => true,
        (OperationStatus.Processing, OperationStatus.Completed) => true,
        (OperationStatus.Processing, OperationStatus.Failed) => true,
        _ => false
    };
}
