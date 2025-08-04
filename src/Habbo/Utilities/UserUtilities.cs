using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Utilities;

public static class UserUtilities
{
    public static IConnection? GetConnection(IncomingMessageContext ctx, User user)
    {
        return ctx.ConnectionManager.Connections
            .Where(c => c.Metadata.OfType<User>().Any(u => u.Id == user.Id))
            .FirstOrDefault();
    }
}
