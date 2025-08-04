using Achilles.Shared.Abstractions;
using Achilles.TCP.Abstractions;
using Achilles.TCP.EventTypes;

namespace Achilles.TCP.Handlers;

public class ConnectionHandler : IConnectionHandler
{
    private readonly IConnectionManager _connectionManager;
    private readonly IEventManager _eventManager;
    private readonly ILogger<ConnectionHandler> _logger;

    public ConnectionHandler(IConnectionManager connectionManager, IEventManager eventManager, ILogger<ConnectionHandler> logger)
    {
        _connectionManager = connectionManager;
        _eventManager = eventManager;
        _logger = logger;
    }

    public async Task<bool> HandleConnectionAsync(IConnection connection)
    {
        try
        {
            if (await _connectionManager.CanConnectAsync(connection))
            {
                await _eventManager.Trigger(TcpEventTypes.ConnectionEstablished, connection);
                await _connectionManager.AddConnectionAsync(connection);

                return true;
            }

            await _eventManager.Trigger(TcpEventTypes.ConnectionFailed, connection);
            await connection.CloseAsync();
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error handling connection {connection.Id}: {ex.Message}");
        }

        return false;
    }
}