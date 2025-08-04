namespace Achilles.TCP.Abstractions;

public interface IMessageHandler
{
    Task<bool> HandleMessageAsync(IConnection connection, IMessage message);
}