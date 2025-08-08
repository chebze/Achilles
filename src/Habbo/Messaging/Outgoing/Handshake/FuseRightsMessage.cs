using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.Handshake;

public class FuseRightsMessage : OutgoingMessage
{
    public FuseRightsMessage(List<string> rights) : base(2)
    {
        foreach(string right in rights)
            this.AppendString(right);
    }
}