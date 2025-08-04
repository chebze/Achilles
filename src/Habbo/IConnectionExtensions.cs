using System.Security.Cryptography.X509Certificates;
using Achilles.Database.Abstractions;
using Achilles.Database.Models;
using Achilles.Habbo.Configuration;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo;

public static partial class IConnectionExtensions
{
    public static async Task SendMessageAsync(this IConnection connection, OutgoingMessage message)
    {
        await connection.SendMessageAsync(message.ToMessage());
    }
}
