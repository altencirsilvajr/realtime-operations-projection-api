using RealtimeOperationsProjection.Application;
using RealtimeOperationsProjection.Domain;

namespace RealtimeOperationsProjection.Application.Tests;

public sealed class OperationServiceTests
{
    [Fact]
    public async Task Register_persists_projection_before_publishing_it()
    {
        var sequence = new List<string>();
        var service = new OperationService(new RecordingStore(sequence), new RecordingPublisher(sequence));

        var snapshot = await service.RegisterAsync("warehouse-pick", CancellationToken.None);

        Assert.Equal("persist", sequence[0]);
        Assert.Equal("publish", sequence[1]);
        Assert.Equal(OperationStatus.Created, snapshot.Status);
    }

    private sealed class RecordingStore(List<string> sequence) : IOperationStore
    {
        public Task CreateAsync(OperationSnapshot snapshot, CancellationToken cancellationToken)
        {
            sequence.Add("persist");
            return Task.CompletedTask;
        }

        public Task<OperationSnapshot?> FindAsync(Guid operationId, CancellationToken cancellationToken) => Task.FromResult<OperationSnapshot?>(null);

        public Task SaveTransitionAsync(OperationSnapshot snapshot, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class RecordingPublisher(List<string> sequence) : IOperationProjectionPublisher
    {
        public Task PublishAsync(OperationSnapshot snapshot, CancellationToken cancellationToken)
        {
            sequence.Add("publish");
            return Task.CompletedTask;
        }
    }
}
