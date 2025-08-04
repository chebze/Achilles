using Achilles.TCP.Abstractions;

namespace Achilles.Habbo;

public class ConnectionPingState
{
    public DateTime? LastPing { get; set; } = null;
    public DateTime? LastPong { get; set; } = null;
}