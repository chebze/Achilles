namespace Achilles.TCP.EventTypes;

public static class TcpEventTypes
{
    public const string ServerStarted = "ServerStarted";
    public const string ServerStopped = "ServerStopped";
    public const string ServerListening = "ServerListening";

    public const string ConnectionEstablished = "ConnectionEstablished";
    public const string ConnectionFailed = "ConnectionFailed";
    public const string ConnectionClosed = "ConnectionClosed";
    public const string ConnectionPinged = "ConnectionPinged";

    public const string MessageReceived = "MessageReceived";
    public const string MessageSent = "MessageSent";
}