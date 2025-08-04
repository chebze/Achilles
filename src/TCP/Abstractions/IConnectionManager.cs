namespace Achilles.TCP.Abstractions;

public interface IConnectionManager : IAsyncDisposable
{
    IConnection? this[Guid id] { get; }
    IConnection[] Connections { get; }

    Task<bool> CanConnectAsync(IConnection connection);
    Task<bool> AddConnectionAsync(IConnection connection);
    Task<bool> RemoveConnectionAsync(IConnection connection);
    Task<bool> CloseConnectionAsync(IConnection connection);
    Task<bool> CloseAllConnectionsAsync();
    
    Task<bool> SendMessageAsync(IMessage message);
    Task<bool> SendMessageAsync(IConnection connection, IMessage message);
}