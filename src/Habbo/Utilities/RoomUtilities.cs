using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Database.Models;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Utilities;

public static class RoomUtilities
{
    public static int GetUserCountInCategoryRooms(IncomingMessageContext ctx, RoomCategory category)
    {
        return ctx.Database.Rooms.Where(room => room.RoomCategoryId == category.Id)
            .ToList()
            .Sum(room => GetUserCountInRoom(ctx, room));
    }

    public static int GetUserCountInRoom(IncomingMessageContext ctx, Room room)
    {
        return ctx.ConnectionManager.Connections
            .Where(user => user.Metadata.OfType<Room>().FirstOrDefault() is Room userRoom && userRoom.Id == room.Id)
            .Count();
    }

    public static IEnumerable<IConnection> GetUsersInRoom(IncomingMessageContext ctx, Room room)
    {
        return ctx.ConnectionManager.Connections
            .Where(user => user.Metadata.OfType<Room>().FirstOrDefault() is Room userRoom && userRoom.Id == room.Id);
    }
    public static IEnumerable<(User User, IConnection Connection)> GetUserConnectionsInRoom(IncomingMessageContext ctx, Room room)
    {
        return ctx.ConnectionManager.Connections
            .Where(c => c.Metadata.OfType<Room>().FirstOrDefault() is Room userRoom && userRoom.Id == room.Id)
            .Select(c => (c.Metadata.OfType<User>().First(), c));
    }
}