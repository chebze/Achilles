using Achilles.Shared.Abstractions;
using Achilles.TCP.Abstractions;
using Achilles.TCP.EventTypes;

namespace Achilles.TCP.Handlers;

public class MessageHandler : IMessageHandler
{
    private readonly ILogger<MessageHandler> _logger;
    private readonly IEventManager _eventManager;

    public MessageHandler(ILogger<MessageHandler> logger, IEventManager eventManager)
    {
        _logger = logger;
        _eventManager = eventManager;
    }

    public async Task<bool> HandleMessageAsync(IConnection connection, IMessage message)
    {
        _logger.LogInformation($"Received message from connection {connection.Id}: {message.ToString()}");
        await _eventManager.Trigger(TcpEventTypes.MessageReceived, connection, message);
        return true;
    }
}