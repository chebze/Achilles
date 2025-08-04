using System.Collections.Concurrent;
using System.Threading.Tasks;
using Achilles.TCP.Abstractions;
using Achilles.TCP.Configuration;

namespace Achilles.TCP.Managers;

public class ConnectionManager : IConnectionManager
{
    private readonly TcpServerConfiguration _configuration;
    private readonly ConcurrentDictionary<Guid, IConnection> _connections = new();
    private readonly ILogger<ConnectionManager> _logger;
    private bool _disposed = false;

    public IConnection? this[Guid id] => _connections.TryGetValue(id, out var connection) ? connection : null;
    public IConnection[] Connections => _connections.Values.ToArray();

    public ConnectionManager(TcpServerConfiguration configuration, ILogger<ConnectionManager> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            await CloseAllConnectionsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during async disposal of connection manager: {ex.Message}");
        }

        GC.SuppressFinalize(this);
    }

    public Task<bool> CanConnectAsync(IConnection connection)
    {
        if (_disposed)
            return Task.FromResult(false);

        if (_connections.Count >= _configuration.MaxConnections)
        {
            _logger.LogWarning($"Max connections reached ({_connections.Count}/{_configuration.MaxConnections})");
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public async Task<bool> AddConnectionAsync(IConnection connection)
    {
        if (_disposed)
            return false;

        if(_connections.TryAdd(connection.Id, connection))
        {
            _logger.LogInformation($"Connection {connection.Id} added");
            return true;
        }

        return await this.AddConnectionAsync(connection);
    }

    public async Task<bool> RemoveConnectionAsync(IConnection connection)
    {
        if(_connections.TryRemove(connection.Id, out _))
        {
            _logger.LogInformation($"Connection {connection.Id} removed");
            return true;
        }

        return await this.RemoveConnectionAsync(connection);
    }

    public async Task<bool> CloseConnectionAsync(IConnection connection)
    {
        try
        {
            await connection.CloseAsync();
            _logger.LogInformation($"Connection {connection.Id} closed");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error closing connection {connection.Id}: {ex.Message}");
            return false;
        }

        return await RemoveConnectionAsync(connection);
    }

    public async Task<bool> CloseAllConnectionsAsync()
    {
        bool result = true;

        _logger.LogInformation($"Closing all {_connections.Count} connections");
        
        // Create a list of disposal tasks
        var disposalTasks = new List<Task>();
        
        foreach (var connection in _connections.Values)
        {
            disposalTasks.Add(CloseConnectionWithLoggingAsync(connection));
        }

        // Wait for all connections to close
        try
        {
            await Task.WhenAll(disposalTasks);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error closing some connections: {ex.Message}");
            result = false;
        }

        return result;
    }

    private async Task CloseConnectionWithLoggingAsync(IConnection connection)
    {
        try
        {
            await CloseConnectionAsync(connection);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to close connection {connection.Id}: {ex.Message}");
        }
    }

    public async Task<bool> SendMessageAsync(IMessage message)
    {
        if (_disposed)
            return false;

        foreach (var connection in _connections.Values)
        {
            try
            {
                await connection.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending message to connection {connection.Id}: {ex.Message}");
                return false;
            }
        }

        return true;
    }

    public async Task<bool> SendMessageAsync(IConnection connection, IMessage message)
    {
        if (_disposed)
            return false;

        try
        {
            return await connection.SendMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending message to connection {connection.Id}: {ex.Message}");
            return false;
        }
    }
}   