using Microsoft.EntityFrameworkCore;
using RealtimeOperationsProjection.Application;

namespace RealtimeOperationsProjection.Infrastructure;

public sealed class SqliteOperationStore(OperationsDbContext dbContext) : IOperationStore
{
    public async Task CreateAsync(OperationSnapshot snapshot, CancellationToken cancellationToken)
    {
        dbContext.Operations.Add(new OperationEntity
        {
            Id = snapshot.Id, Name = snapshot.Name, Status = snapshot.Status,
            CreatedAt = snapshot.CreatedAt, LastChangedAt = snapshot.LastChangedAt
        });
        dbContext.Projections.Add(new OperationProjectionEntity
        {
            OperationId = snapshot.Id, Name = snapshot.Name, Status = snapshot.Status,
            CreatedAt = snapshot.CreatedAt, LastChangedAt = snapshot.LastChangedAt
        });
        AddEvent(snapshot.Id, snapshot.Timeline.Single());
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<OperationSnapshot?> FindAsync(Guid operationId, CancellationToken cancellationToken)
    {
        var projection = await dbContext.Projections.AsNoTracking()
            .SingleOrDefaultAsync(item => item.OperationId == operationId, cancellationToken);
        if (projection is null)
        {
            return null;
        }

        var eventEntities = await dbContext.Events.AsNoTracking()
            .Where(item => item.OperationId == operationId)
            .ToListAsync(cancellationToken);
        // SQLite cannot translate DateTimeOffset ordering; the operation timeline is intentionally small in this lab.
        var events = eventEntities.OrderBy(item => item.OccurredAt)
            .Select(item => new OperationTimelineEvent(item.PreviousStatus, item.NewStatus, item.OccurredAt))
            .ToList();
        return new OperationSnapshot(projection.OperationId, projection.Name, projection.Status, projection.CreatedAt, projection.LastChangedAt, events);
    }

    public async Task SaveTransitionAsync(OperationSnapshot snapshot, CancellationToken cancellationToken)
    {
        var operation = await dbContext.Operations.SingleAsync(item => item.Id == snapshot.Id, cancellationToken);
        var projection = await dbContext.Projections.SingleAsync(item => item.OperationId == snapshot.Id, cancellationToken);
        operation.Status = snapshot.Status;
        operation.LastChangedAt = snapshot.LastChangedAt;
        projection.Status = snapshot.Status;
        projection.LastChangedAt = snapshot.LastChangedAt;
        AddEvent(snapshot.Id, snapshot.Timeline.Last());
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void AddEvent(Guid operationId, OperationTimelineEvent timelineEvent) => dbContext.Events.Add(new OperationEventEntity
    {
        Id = Guid.NewGuid(), OperationId = operationId, PreviousStatus = timelineEvent.PreviousStatus,
        NewStatus = timelineEvent.NewStatus, OccurredAt = timelineEvent.OccurredAt
    });
}
