namespace Achilles.TCP.Abstractions;

public interface IConnectionPingingService : IHostedService, IAsyncDisposable
{
    Task PingConnectionsAsync(CancellationToken cancellationToken);
}