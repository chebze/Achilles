using Achilles.TCP.Abstractions;

namespace Achilles.TCP.Services;

public class TcpServerHostedService : ITcpServerHostedService
{
    private readonly ITcpServerService _tcpServerService;
    private readonly ILogger<TcpServerHostedService> _logger;

    public TcpServerHostedService(ITcpServerService tcpServerService, ILogger<TcpServerHostedService> logger)
    {
        _tcpServerService = tcpServerService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting TCP worker");
        return _tcpServerService.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping TCP worker");
        return _tcpServerService.StopAsync();
    }
}