using Microsoft.EntityFrameworkCore;
using RealtimeOperationsProjection.Domain;

namespace RealtimeOperationsProjection.Infrastructure;

public sealed class OperationsDbContext(DbContextOptions<OperationsDbContext> options) : DbContext(options)
{
    public DbSet<OperationEntity> Operations => Set<OperationEntity>();
    public DbSet<OperationProjectionEntity> Projections => Set<OperationProjectionEntity>();
    public DbSet<OperationEventEntity> Events => Set<OperationEventEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OperationEntity>(entity =>
        {
            entity.HasKey(operation => operation.Id);
            entity.Property(operation => operation.Name).HasMaxLength(120);
        });
        modelBuilder.Entity<OperationProjectionEntity>(entity =>
        {
            entity.HasKey(projection => projection.OperationId);
            entity.Property(projection => projection.Name).HasMaxLength(120);
        });
        modelBuilder.Entity<OperationEventEntity>(entity =>
        {
            entity.HasKey(domainEvent => domainEvent.Id);
            entity.HasIndex(domainEvent => new { domainEvent.OperationId, domainEvent.OccurredAt });
        });
    }
}

public sealed class OperationEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public OperationStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastChangedAt { get; set; }
}

public sealed class OperationProjectionEntity
{
    public Guid OperationId { get; set; }
    public required string Name { get; set; }
    public OperationStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastChangedAt { get; set; }
}

public sealed class OperationEventEntity
{
    public Guid Id { get; set; }
    public Guid OperationId { get; set; }
    public OperationStatus? PreviousStatus { get; set; }
    public OperationStatus NewStatus { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
}
