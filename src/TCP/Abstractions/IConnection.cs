namespace Achilles.TCP.Abstractions;

public interface IConnection : IAsyncDisposable
{
    Guid Id { get; }
    bool Connected { get; }

    List<object> Metadata { get; }

    Task<bool> SendMessageAsync(IMessage message);
    Task<bool> CloseAsync();
}