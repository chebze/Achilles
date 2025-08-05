# TCP System Documentation

## Overview

The Achilles TCP system provides a robust, event-driven framework for handling TCP server functionality with connection management, message processing, and lifecycle management. The system is built around asynchronous operations, dependency injection, and comprehensive error handling.

## Architecture

### Core Components

1. **TCP Server Service** - Main server that listens for incoming connections
2. **Connection Management** - Handles connection lifecycle and limits
3. **Message Handling** - Processes incoming and outgoing messages
4. **Event System** - Event-driven architecture for extensibility
5. **Ping Service** - Monitors connection health
6. **Configuration** - Centralized server settings

### Directory Structure

```
src/TCP/
├── Abstractions/
│   ├── IConnection.cs              # Connection interface
│   ├── IConnectionManager.cs       # Connection management interface
│   ├── IConnectionHandler.cs       # Connection event handling
│   ├── IMessage.cs                 # Message interface
│   ├── IMessageHandler.cs          # Message processing interface
│   ├── ITcpServerService.cs        # Server service interface
│   ├── ITcpServerHostedService.cs  # Hosted service interface
│   └── IConnectionPingingService.cs # Connection health monitoring
├── Data/
│   ├── Connection.cs               # Connection implementation
│   └── Message.cs                  # Message implementation
├── Services/
│   ├── TcpServerService.cs         # Main TCP server implementation
│   ├── TcpServerHostedService.cs   # .NET hosted service wrapper
│   └── ConnectionPingingService.cs # Connection health monitoring
├── Managers/
│   └── ConnectionManager.cs        # Connection lifecycle management
├── Handlers/
│   ├── ConnectionHandler.cs        # Connection establishment handling
│   └── MessageHandler.cs           # Message processing
├── Configuration/
│   └── TcpServerConfiguration.cs   # Server configuration model
├── Extensions/
│   └── HostingExtensions.cs        # Dependency injection setup
└── EventTypes.cs                   # Event type constants
```

## Event System

### Event Types

The TCP system uses an event-driven architecture with predefined event types:

#### Server Events
- `ServerStarted` - Fired when the TCP server starts
- `ServerStopped` - Fired when the TCP server stops
- `ServerListening` - Fired when the server begins accepting connections

#### Connection Events
- `ConnectionEstablished` - Fired when a new connection is successfully established
- `ConnectionFailed` - Fired when a connection attempt fails
- `ConnectionClosed` - Fired when a connection is closed
- `ConnectionPinged` - Fired during connection health checks

#### Message Events
- `MessageReceived` - Fired when a message is received from a client
- `MessageSent` - Fired when a message is sent to a client

### Event Usage

```csharp
// Events are triggered through the IEventManager
await _eventManager.Trigger(TcpEventTypes.ConnectionEstablished, connection);
await _eventManager.Trigger(TcpEventTypes.MessageReceived, connection, message);
```

## TCP Server Service

### Main Server Implementation

The `TcpServerService` is the core component that handles TCP server operations:

#### Key Features:
- **Asynchronous Operations** - All operations are async for better performance
- **Graceful Shutdown** - Proper cleanup and resource disposal
- **Error Handling** - Comprehensive exception handling with logging
- **Event Integration** - Fires events for monitoring and extensibility

#### Server Lifecycle

```csharp
public class TcpServerService : ITcpServerService
{
    // Start the server
    public async Task StartAsync()
    {
        _listener.Start();
        _acceptClientsTask = AcceptClientsAsync(_cancellationTokenSource.Token);
        await _eventManager.Trigger(TcpEventTypes.ServerStarted, this);
    }

    // Stop the server
    public async Task StopAsync()
    {
        _cancellationTokenSource.Cancel();
        await _acceptClientsTask;
        _listener.Stop();
        await _eventManager.Trigger(TcpEventTypes.ServerStopped, this);
    }
}
```

#### Client Acceptance

The server continuously accepts new clients in a background task:

1. **Accept Connection** - Waits for incoming TCP connections
2. **Create Connection** - Wraps TcpClient in Connection object
3. **Handle Connection** - Delegates to ConnectionHandler for processing
4. **Error Recovery** - Continues accepting clients even if individual connections fail

### Hosted Service Integration

The `TcpServerHostedService` integrates the TCP server with .NET's hosted service lifecycle:

```csharp
public class TcpServerHostedService : ITcpServerHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _tcpServerService.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _tcpServerService.StopAsync();
    }
}
```

## Connection Management

### Connection Interface

The `IConnection` interface defines the contract for TCP connections:

```csharp
public interface IConnection : IAsyncDisposable
{
    Guid Id { get; }                    // Unique connection identifier
    bool Connected { get; }             // Connection status
    List<object> Metadata { get; }      // Extensible metadata storage

    Task<bool> SendMessageAsync(IMessage message);
    Task<bool> CloseAsync();
}
```

### Connection Implementation

The `Connection` class provides the full implementation:

#### Key Features:
- **Unique Identification** - Each connection has a GUID
- **Metadata Support** - Extensible metadata for storing connection-specific data
- **Automatic Message Receiving** - Background task for processing incoming messages
- **Graceful Cleanup** - Proper resource disposal and error handling

#### Connection Lifecycle

```csharp
public class Connection : IConnection
{
    public Connection(Guid Id, TcpClient client, IServiceProvider serviceProvider)
    {
        // Initialize connection
        // Start background message receiving task
        _receiveMessageTask = ReceiveMessageAsync(_cancellationTokenSource.Token);
    }

    private async Task ReceiveMessageAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !_disposed)
        {
            // Read from network stream
            var buffer = new byte[4096];
            var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            
            // Process received data
            var receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            var message = new Message(receivedData);
            
            // Handle message through injected handler
            await _messageHandler.HandleMessageAsync(this, message);
        }
    }
}
```

#### Message Sending

```csharp
public async Task<bool> SendMessageAsync(IMessage message)
{
    if (_disposed || !Connected)
        return false;

    try
    {
        var bytes = Encoding.ASCII.GetBytes(message.ToString());
        await _stream.WriteAsync(bytes, 0, bytes.Length);
        await _eventManager.Trigger(TcpEventTypes.MessageSent, this, message);
        return true;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error sending message to connection {Id}: {ex.Message}");
        return false;
    }
}
```

### Connection Manager

The `ConnectionManager` handles the lifecycle of all active connections:

#### Key Features:
- **Thread-Safe Collection** - Uses `ConcurrentDictionary` for connection storage
- **Connection Limits** - Enforces maximum connection limits
- **Bulk Operations** - Support for broadcasting and bulk connection management
- **Indexer Access** - Easy connection lookup by ID

#### Connection Management Operations

```csharp
public class ConnectionManager : IConnectionManager
{
    // Check if new connection can be accepted
    public Task<bool> CanConnectAsync(IConnection connection)
    {
        if (_connections.Count >= _configuration.MaxConnections)
        {
            _logger.LogWarning($"Max connections reached ({_connections.Count}/{_configuration.MaxConnections})");
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

    // Add new connection
    public async Task<bool> AddConnectionAsync(IConnection connection)
    {
        if (_connections.TryAdd(connection.Id, connection))
        {
            _logger.LogInformation($"Connection {connection.Id} added");
            return true;
        }
        return false;
    }

    // Remove connection
    public async Task<bool> RemoveConnectionAsync(IConnection connection)
    {
        if (_connections.TryRemove(connection.Id, out _))
        {
            _logger.LogInformation($"Connection {connection.Id} removed");
            return true;
        }
        return false;
    }

    // Broadcast message to all connections
    public async Task<bool> SendMessageAsync(IMessage message)
    {
        foreach (var connection in _connections.Values)
        {
            await connection.SendMessageAsync(message);
        }
        return true;
    }
}
```

#### Connection Access

```csharp
// Access by indexer
IConnection? connection = _connectionManager[connectionId];

// Get all connections
IConnection[] allConnections = _connectionManager.Connections;
```

## Message System

### Message Interface

The `IMessage` interface defines the contract for all messages:

```csharp
public interface IMessage
{
    public string ToString();
}
```

### Message Implementation

The `Message` class provides a simple string-based message implementation:

```csharp
public class Message : IMessage
{
    public string Content { get; set; }

    public Message(string content)
    {
        Content = content;
    }

    public override string ToString() => Content;
}
```

### Message Handling

#### Message Handler Interface

```csharp
public interface IMessageHandler
{
    Task<bool> HandleMessageAsync(IConnection connection, IMessage message);
}
```

#### Default Message Handler

The default `MessageHandler` logs received messages and triggers events:

```csharp
public class MessageHandler : IMessageHandler
{
    public async Task<bool> HandleMessageAsync(IConnection connection, IMessage message)
    {
        _logger.LogInformation($"Received message from connection {connection.Id}: {message.ToString()}");
        await _eventManager.Trigger(TcpEventTypes.MessageReceived, connection, message);
        return true;
    }
}
```

#### Custom Message Handlers

You can implement custom message handlers for specific processing logic:

```csharp
public class CustomMessageHandler : IMessageHandler
{
    public async Task<bool> HandleMessageAsync(IConnection connection, IMessage message)
    {
        // Custom message processing logic
        if (message.ToString().StartsWith("COMMAND:"))
        {
            await ProcessCommand(connection, message);
        }
        
        return true;
    }
}
```

## Connection Health Monitoring

### Ping Service

The `ConnectionPingingService` monitors connection health by periodically checking all active connections:

#### Key Features:
- **Periodic Health Checks** - Configurable ping intervals
- **Automatic Cleanup** - Removes disconnected connections
- **Event Integration** - Fires ping events for monitoring
- **Graceful Shutdown** - Proper service lifecycle management

#### Implementation

```csharp
public class ConnectionPingingService : IConnectionPingingService
{
    public async Task PingConnectionsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && !_disposed)
        {
            foreach (var connection in _connectionManager.Connections)
            {
                _logger.LogInformation($"Pinging connection {connection.Id}");
                await _eventManager.Trigger(TcpEventTypes.ConnectionPinged, connection);

                if (!connection.Connected)
                {
                    _logger.LogWarning($"Connection {connection.Id} no longer connected");
                    await _connectionManager.RemoveConnectionAsync(connection);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(_configuration.PingInterval), cancellationToken);
        }
    }
}
```

## Connection Handling

### Connection Handler

The `ConnectionHandler` manages the initial connection establishment process:

```csharp
public class ConnectionHandler : IConnectionHandler
{
    public async Task<bool> HandleConnectionAsync(IConnection connection)
    {
        try
        {
            // Check if connection can be accepted
            if (await _connectionManager.CanConnectAsync(connection))
            {
                await _eventManager.Trigger(TcpEventTypes.ConnectionEstablished, connection);
                await _connectionManager.AddConnectionAsync(connection);
                return true;
            }

            // Reject connection if limits exceeded
            await _eventManager.Trigger(TcpEventTypes.ConnectionFailed, connection);
            await connection.CloseAsync();
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error handling connection {connection.Id}: {ex.Message}");
            return false;
        }
    }
}
```

### Custom Connection Handling

You can implement custom connection handlers for specific logic:

```csharp
public class CustomConnectionHandler : IConnectionHandler
{
    public async Task<bool> HandleConnectionAsync(IConnection connection)
    {
        // Custom connection validation
        if (!await ValidateConnection(connection))
        {
            await connection.CloseAsync();
            return false;
        }

        // Add custom metadata
        connection.Metadata.Add(new ConnectionTimestamp { ConnectedAt = DateTime.UtcNow });

        // Standard connection handling
        if (await _connectionManager.CanConnectAsync(connection))
        {
            await _connectionManager.AddConnectionAsync(connection);
            return true;
        }

        return false;
    }
}
```

## Configuration

### TCP Server Configuration

The `TcpServerConfiguration` class defines server settings:

```csharp
public class TcpServerConfiguration
{
    public int Port { get; set; }                    // TCP port to listen on
    public string? Host { get; set; }                // Host address (default: 0.0.0.0)
    public int MaxConnections { get; set; }          // Maximum concurrent connections
    public string MessageTerminator { get; set; } = "\n";  // Message delimiter
    public int PingInterval { get; set; } = 30;      // Ping interval in seconds
}
```

### Configuration Usage

Configuration is typically loaded from appsettings.json:

```json
{
  "Server": {
    "Port": 30000,
    "Host": "0.0.0.0",
    "MaxConnections": 100,
    "MessageTerminator": "\n",
    "PingInterval": 30
  }
}
```

## Dependency Injection Setup

### Service Registration

The `HostingExtensions` class provides easy setup for dependency injection:

```csharp
public static class HostingExtensions
{
    public static IServiceCollection UseTcpServer(this IServiceCollection services)
    {
        // Configuration
        services.AddSingleton<TcpServerConfiguration>(s => 
            s.GetRequiredService<IConfiguration>()
                .GetSection("Server")
                .Get<TcpServerConfiguration>() ?? 
            throw new Exception("Server configuration missing"));

        // Core services
        services.AddSingleton<IConnectionManager, ConnectionManager>();
        services.AddSingleton<IConnectionHandler, ConnectionHandler>();
        services.AddSingleton<IMessageHandler, MessageHandler>();
        
        // Server services
        services.AddSingleton<ITcpServerService, TcpServerService>();
        services.AddSingleton<ITcpServerHostedService, TcpServerHostedService>();
        
        // Hosted services
        services.AddHostedService<TcpServerHostedService>();
        services.AddHostedService<ConnectionPingingService>();

        return services;
    }
}
```

### Usage in Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add TCP server services
builder.Services.UseTcpServer();

var app = builder.Build();
app.Run();
```

## Error Handling and Resilience

### Connection Error Handling

The system includes comprehensive error handling:

#### Connection Level
- **Graceful Disconnection** - Proper cleanup when connections are lost
- **Resource Disposal** - Automatic cleanup of network resources
- **Exception Logging** - Detailed error logging with context

#### Server Level
- **Fault Isolation** - Individual connection failures don't affect the server
- **Graceful Shutdown** - Proper cleanup during application shutdown
- **Recovery Mechanisms** - Server continues operating despite individual failures

### Error Patterns

```csharp
// Connection error handling
public async Task<bool> SendMessageAsync(IMessage message)
{
    try
    {
        // Send logic
        return true;
    }
    catch (ObjectDisposedException)
    {
        // Connection already disposed
        return false;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error sending message to connection {Id}");
        return false;
    }
}

// Server error handling
private async Task AcceptClientsAsync(CancellationToken cancellationToken)
{
    try
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                // Handle client
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error accepting client: {ex.Message}");
                // Continue accepting other clients
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Fatal error in AcceptClientsAsync: {ex.Message}");
    }
}
```

## Best Practices

### Connection Management

1. **Resource Cleanup** - Always properly dispose connections and resources
2. **Metadata Usage** - Use connection metadata for storing connection-specific data
3. **Connection Limits** - Enforce reasonable connection limits
4. **Health Monitoring** - Use the ping service to monitor connection health

### Message Handling

1. **Async Processing** - Use async/await for all I/O operations
2. **Error Handling** - Handle message processing errors gracefully
3. **Event Integration** - Use events for decoupled message processing
4. **Custom Handlers** - Implement custom message handlers for specific logic

### Service Configuration

1. **Configuration Validation** - Validate configuration at startup
2. **Environment-Specific Settings** - Use different settings per environment
3. **Security** - Configure appropriate connection limits and timeouts
4. **Monitoring** - Log important events and metrics

### Performance Optimization

1. **Connection Pooling** - Consider connection reuse patterns
2. **Buffer Management** - Use appropriate buffer sizes for message handling
3. **Async Operations** - Leverage async/await for better scalability
4. **Resource Limits** - Set appropriate limits to prevent resource exhaustion

## Common Usage Patterns

### Basic Server Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.UseTcpServer();
var app = builder.Build();
app.Run();
```

### Custom Message Processing

```csharp
public class GameMessageHandler : IMessageHandler
{
    public async Task<bool> HandleMessageAsync(IConnection connection, IMessage message)
    {
        var content = message.ToString();
        
        if (content.StartsWith("LOGIN:"))
        {
            await ProcessLogin(connection, content);
        }
        else if (content.StartsWith("MOVE:"))
        {
            await ProcessMove(connection, content);
        }
        
        return true;
    }
}
```

### Connection Metadata Usage

```csharp
// Add user data to connection metadata
connection.Metadata.Add(new User { Id = userId, Username = username });

// Retrieve user data
var user = connection.Metadata.OfType<User>().FirstOrDefault();
```

### Broadcasting Messages

```csharp
// Send to all connections
await _connectionManager.SendMessageAsync(new Message("Server announcement"));

// Send to specific connections
var gameConnections = _connectionManager.Connections
    .Where(c => c.Metadata.OfType<GameSession>().Any())
    .ToList();

foreach (var connection in gameConnections)
{
    await connection.SendMessageAsync(new Message("Game update"));
}
```

This TCP system provides a robust, scalable foundation for building networked applications with comprehensive connection management, message handling, and monitoring capabilities. 