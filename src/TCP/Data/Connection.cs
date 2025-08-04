using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;
using Achilles.TCP.Abstractions;
using Achilles.TCP.Configuration;
using Achilles.Shared.Abstractions;
using Achilles.TCP.EventTypes;

namespace Achilles.TCP.Data;

public class Connection : IConnection
{
    public Guid Id { get; }
    public bool Connected { get => _client.Connected; }
    public List<object> Metadata { get => _metadata; }

    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Connection> _logger;
    private readonly List<object> _metadata;
    private readonly IMessageHandler _messageHandler;
    private readonly IEventManager _eventManager;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task? _receiveMessageTask;
    private bool _disposed = false;

    public Connection(Guid Id, TcpClient client, IServiceProvider serviceProvider)
    {
        this.Id = Id;
        _client = client;
        _stream = client.GetStream();
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<Connection>>();
        _messageHandler = serviceProvider.GetRequiredService<IMessageHandler>();
        _metadata = new List<object>();
        _eventManager = serviceProvider.GetRequiredService<IEventManager>();
        _receiveMessageTask = ReceiveMessageAsync(_cancellationTokenSource.Token);
    }

    private async Task ReceiveMessageAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested && !_disposed)
            {
                try
                {
                    var buffer = new byte[4096];
                    var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if(bytesRead == 0)
                    {
                        await this.CloseAsync();
                        break;
                    }

                    var receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                     var message = new Message(receivedData);
                    _logger.LogInformation($"Received message from connection {Id}: {receivedData}");
                    
                    // Handle the message using the injected handler
                    await _messageHandler.HandleMessageAsync(this, message);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Expected when shutting down
                    _logger.LogDebug($"Message receiving cancelled for connection {Id}");
                    break;
                }
                catch (ObjectDisposedException)
                {
                    // Stream was disposed, break cleanly
                    _logger.LogDebug($"Stream disposed for connection {Id}");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error receiving message from connection {Id}: {ex.Message}");
                    await this.CloseAsync();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fatal error in ReceiveMessageAsync for connection {Id}: {ex.Message}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            await CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during async disposal of connection {Id}: {ex.Message}");
        }

        GC.SuppressFinalize(this);
    }

    public async Task<bool> SendMessageAsync(IMessage message)
    {
        if (_disposed || !Connected)
            return false;

        try
        {
            var bytes = Encoding.ASCII.GetBytes(message.ToString()); 
            await _stream.WriteAsync(bytes, 0, bytes.Length);
            _logger.LogInformation($"Sent message to connection {Id}: `{message.ToString()}`");
            await _eventManager.Trigger(TcpEventTypes.MessageSent, this, message);
            return true;
        }
        catch (ObjectDisposedException)
        {
            _logger.LogDebug($"Cannot send message to disposed connection {Id}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending message to connection {Id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CloseAsync()
    {
        if (_disposed)
            return true;

        try
        {
            _cancellationTokenSource.Cancel();
            
            // Close network resources FIRST to force any blocking ReadAsync operations to throw
            try
            {
                _stream.Close();
                _client.Close();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error closing network resources for connection {Id}: {ex.Message}");
            }

            // Now wait for the receive task to complete (with timeout to prevent infinite wait)
            if(_receiveMessageTask is not null)
            {
                try
                {
                    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await _receiveMessageTask.WaitAsync(timeoutCts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation token is triggered or timeout occurs
                    _logger.LogDebug($"Receive task cancelled for connection {Id}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error waiting for receive task completion for connection {Id}: {ex.Message}");
                }
            }

            // Dispose resources
            try
            {
                _stream.Dispose();
                _client.Dispose();
                _cancellationTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error disposing resources for connection {Id}: {ex.Message}");
            }

            _logger.LogInformation($"Connection {Id} closed");
            await _eventManager.Trigger(TcpEventTypes.ConnectionClosed, this);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error closing connection {Id}: {ex.Message}");
            await _eventManager.Trigger(TcpEventTypes.ConnectionClosed, this);
            return false;
        }
    }
}