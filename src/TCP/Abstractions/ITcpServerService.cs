namespace Achilles.TCP.Abstractions;

public interface ITcpServerService : IAsyncDisposable
{
    Task StartAsync();
    Task StopAsync();
}