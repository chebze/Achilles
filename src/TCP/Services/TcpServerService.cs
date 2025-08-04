using System.Net;
using System.Net.Sockets;
using Achilles.Shared.Abstractions;
using Achilles.TCP.Abstractions;
using Achilles.TCP.Configuration;
using Achilles.TCP.Data;
using Achilles.TCP.EventTypes;

namespace Achilles.TCP.Services;

public class TcpServerService : ITcpServerService
{
    private readonly TcpServerConfiguration _configuration;
    private readonly IConnectionManager _connectionManager;
    private readonly IConnectionHandler _connectionHandler;
    private readonly IEventManager _eventManager;
    private readonly ILogger<TcpServerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly TcpListener _listener;
    private Task? _acceptClientsTask;
    private bool _disposed = false;

    public TcpServerService(TcpServerConfiguration configuration, IConnectionManager connectionManager, IConnectionHandler connectionHandler, IEventManager eventManager, ILogger<TcpServerService> logger, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _connectionManager = connectionManager;
        _connectionHandler = connectionHandler;
        _eventManager = eventManager;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _listener = new TcpListener(IPAddress.Parse(_configuration.Host ?? "0.0.0.0"), _configuration.Port);
    }

    public async Task StartAsync()
    {
        if (_acceptClientsTask is not null || _disposed)
            return;

        _logger.LogInformation("Starting TCP server");

        try
        {
            _listener.Start();
            _logger.LogInformation($"TCP server started on {_configuration.Host}:{_configuration.Port}");
            await _eventManager.Trigger(TcpEventTypes.ServerStarted, this);

            _acceptClientsTask = AcceptClientsAsync(_cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error starting TCP server: {ex.Message}");
            throw;
        }
    }

    public async Task StopAsync()
    {
        if (_acceptClientsTask is null || _disposed)
            return;

        _logger.LogInformation("Stopping TCP server");
        _cancellationTokenSource.Cancel();

        if (_acceptClientsTask != null)
        {
            try
            {
                await _acceptClientsTask;
            }
            catch (OperationCanceledException) when (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Expected when shutting down
                _logger.LogDebug("Accept clients task cancelled due to shutdown");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error stopping accept clients task: {ex.Message}");
            }
        }

        try
        {
            _listener.Stop();
            _logger.LogInformation("TCP server stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error stopping TCP server: {ex.Message}");
        }

        await _eventManager.Trigger(TcpEventTypes.ServerStopped, this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            await StopAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during TCP server disposal: {ex.Message}");
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

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Accepting clients on {_configuration.Host}:{_configuration.Port}");
        await _eventManager.Trigger(TcpEventTypes.ServerListening, this);
        
        try
        {
            while (!cancellationToken.IsCancellationRequested && !_disposed)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                    var connection = new Connection(Guid.NewGuid(), client, _serviceProvider);
                    _ = _connectionHandler.HandleConnectionAsync(connection);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Expected when shutting down
                    _logger.LogDebug("Client acceptance cancelled due to shutdown");
                    break;
                }
                catch (ObjectDisposedException) when (_disposed)
                {
                    // Expected when listener is disposed
                    _logger.LogDebug("Client acceptance stopped due to disposed listener");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error accepting client: {ex.Message}");
                    // Continue accepting other clients unless cancellation is requested
                    if (cancellationToken.IsCancellationRequested)
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Fatal error in AcceptClientsAsync: {ex.Message}");
        }
        finally
        {
            _logger.LogInformation("Client acceptance has stopped");
        }
    }
}