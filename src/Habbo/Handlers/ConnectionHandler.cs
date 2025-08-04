using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.Handshake;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.Shared.Abstractions;
using Achilles.TCP.Abstractions;
using Achilles.TCP.EventTypes;

namespace Achilles.Habbo.Handlers;

public class ConnectionHandler
{
    private ILogger<ConnectionHandler> _logger { get; }
    private IServiceProvider _serviceProvider { get; }
    private IEventManager _eventManager { get; }

    public ConnectionHandler(IServiceProvider serviceProvider, IEventManager eventManager, ILogger<ConnectionHandler> logger)
    {
        this._serviceProvider = serviceProvider;
        this._eventManager = eventManager;
        this._logger = logger;

        this.RegisterEvents();
    }

    private void RegisterEvents()
    {
        this._eventManager.Add<IConnection>(TcpEventTypes.ConnectionEstablished, this.OnConnectionEstablished);
        this._eventManager.Add<IConnection>(TcpEventTypes.ConnectionPinged, this.OnConnectionPinged);
        this._eventManager.Add<IConnection, IMessage>(TcpEventTypes.MessageReceived, this.OnMessageReceived);
    }

    private async Task OnMessageReceived(IConnection connection, IMessage message)
    {
        _logger.LogInformation(
            "Received: {msg}",
            message.ToString()
                .Replace(((char)1).ToString(), "[chr(1)]")
                .Replace(((char)2).ToString(), "[chr(2)]")
                .Replace(((char)9).ToString(), "[chr(9)]")
                .Replace(((char)13).ToString(), "[chr(13)]")
        );

        List<IncomingMessage> incomingMessages = [];
        
        try
        {
            incomingMessages.AddRange(MessageTypeResolver.Resolve(message));
        }
        catch(AggregateException ex)
        {
            foreach(Exception exception in ex.InnerExceptions)
            {
                this._logger.LogError(
                    exception,
                    "Error resolving message {msg}",
                    message.ToString()
                );
            }
        }

        using IncomingMessageContext context = new IncomingMessageContext(
            this._serviceProvider.CreateScope(),
            connection
        );

        if(context.User is not null)
        {
            context.User.LastOnline = DateTime.Now;
            await context.Database.SaveChangesAsync();
        }

        foreach(IncomingMessage incomingMessage in incomingMessages)
        {
            try
            {

                incomingMessage.Handle(context);
                await incomingMessage.HandleAsync(context);

                OutgoingMessage? outgoingMessage;
                
                outgoingMessage = incomingMessage.Respond(context);
                if(outgoingMessage is not null)
                    await connection.SendMessageAsync(outgoingMessage);
                
                outgoingMessage = await incomingMessage.RespondAsync(context);
                if(outgoingMessage is not null)
                    await connection.SendMessageAsync(outgoingMessage);

            } catch(Exception ex) {
                this._logger.LogError(
                    ex,
                    "Error handling message #{id}{nl}Packet: {msg}",
                    incomingMessage.Header.Value,
                    Environment.NewLine,
                    incomingMessage.Header.Encoded + incomingMessage.Raw
                );
            }
        }
    }

    private async Task OnConnectionEstablished(IConnection connection)
    {
        connection.Metadata.Add(new ConnectionPingState());
        connection.Metadata.Add(this._serviceProvider);
        connection.Metadata.Add(new ConnectionPingState());

        await connection.SendMessageAsync(new HelloMessage());
    }    
    private async Task OnConnectionPinged(IConnection connection)
    {
        ConnectionPingState pingState = connection.Metadata.OfType<ConnectionPingState>().FirstOrDefault() ?? throw new Exception("No connection ping state found in connection metadata");

        if (pingState.LastPing is not null && pingState.LastPong is null)
            return;

        if (pingState.LastPing is null && pingState.LastPong is not null)
        {
            if(DateTime.Now - pingState.LastPong > TimeSpan.FromSeconds(30))
            {
                this._logger.LogWarning("Connection {id} timed out", connection.Id);
                await connection.CloseAsync();
                return;
            }
        }

        pingState.LastPing = DateTime.Now;
        await connection.SendMessageAsync(new PingMessage());
    }
}