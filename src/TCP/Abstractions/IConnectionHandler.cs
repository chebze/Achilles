namespace Achilles.TCP.Abstractions;

public interface IConnectionHandler
{
    Task<bool> HandleConnectionAsync(IConnection connection);
}