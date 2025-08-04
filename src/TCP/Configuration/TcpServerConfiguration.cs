namespace Achilles.TCP.Configuration;

public class TcpServerConfiguration
{
    public int Port { get; set; }
    public string? Host { get; set; }
    public int MaxConnections { get; set; }
    public string MessageTerminator { get; set; } = "\n";
    public int PingInterval { get; set; } = 30;
}