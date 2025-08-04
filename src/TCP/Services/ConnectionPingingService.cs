using Achilles.TCP.Abstractions;
using Achilles.TCP.Configuration;
using Achilles.Shared.Abstractions;
using Achilles.TCP.EventTypes;

namespace Achilles.TCP.Services;

public class ConnectionPingingService : IConnectionPingingService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task? connectionStatusTask;
    private bool _disposed = false;

    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<ConnectionPingingService> _logger;
    private readonly TcpServerConfiguration _configuration;
    private readonly IEventManager _eventManager;

    public ConnectionPingingService(IConnectionManager connectionManager, ILogger<ConnectionPingingService> logger, TcpServerConfiguration configuration, IEventManager eventManager)
    {
        _connectionManager = connectionManager;
        _logger = logger;
        _configuration = configuration;
        _eventManager = eventManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_disposed)
            return Task.CompletedTask;

        connectionStatusTask = PingConnectionsAsync(_cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_disposed)
            return;

        _cancellationTokenSource.Cancel();

        if (connectionStatusTask != null)
        {
            try
            {
                await connectionStatusTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || _cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Expected when shutting down
                _logger.LogDebug("Connection pinging service stopped due to cancellation");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error stopping connection pinging service: {ex.Message}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            await StopAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during async disposal of connection pinging service: {ex.Message}");
        }

        try
        {
            _cancellationTokenSource.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Error disposing cancellation token source: {ex.Message}");
        }

        GC.SuppressFinalize(this);
    }

    public async Task PingConnectionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && !_disposed)
            {
                try
                {
                    foreach (var connection in _connectionManager.Connections)
                    {
                        if(cancellationToken.IsCancellationRequested || _disposed)
                            break;

                        _logger.LogInformation($"Pinging connection {connection.Id}");

                        await _eventManager.Trigger(TcpEventTypes.ConnectionPinged, connection);

                        if (!connection.Connected)
                        {
                            _logger.LogWarning($"Connection {connection.Id} no longer connected");
                            await _connectionManager.RemoveConnectionAsync(connection);
                            continue;
                        }
                    }

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(_configuration.PingInterval), cancellationToken);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        // Expected when shutting down
                        _logger.LogDebug("Ping delay cancelled due to shutdown");
                        break;
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Expected when shutting down
                    _logger.LogDebug("Connection pinging cancelled due to shutdown");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error during connection pinging iteration: {ex.Message}");
                    // Continue with next iteration after a brief delay
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    }
                    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fatal error in PingConnectionsAsync: {ex.Message}");
        }
        finally
        {
            _logger.LogInformation("Connection pinging service has stopped");
        }
    }
}