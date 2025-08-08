using System.Net;
using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.Rooms;
using Achilles.Habbo.Utilities;
using Achilles.TCP.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Achilles.Habbo.Messaging.Incoming.Rooms;

[IncomingMessage(57)]
public class TryEnterRoomMessage : IncomingMessage
{
    public int RoomId { get; set; }
    public string? Password { get; set; }

    public TryEnterRoomMessage(MessageHeader header, string content) : base(header, content)
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        if(content.Length == 0)
            return;

        if(content.Contains('/'))
        {
            string[] parts = content.ReadRemainingString().Split('/');
            this.RoomId = int.Parse(parts[0]);
            this.Password = parts[1];
        }else{
            this.RoomId = int.Parse(content.ReadRemainingString());
        }
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return null;

        List<string> fuseRights = await UserUtilities.GetFuseRights(ctx);

        Database.Models.Room? room = await ctx.Database.Rooms.FirstOrDefaultAsync(r => r.Id == this.RoomId);
        if(room is null)
            return null;

        bool hasRightsInRoom = await ctx.Database.RoomUserRights.AnyAsync(r => r.RoomId == room.Id && r.UserId == ctx.User.Id);
        RoomAccessType accessType = room.AccessType;

        if(fuseRights.Contains("fuse_enter_locked_rooms") || room.OwnerId == ctx.User.Id || hasRightsInRoom)
            accessType = RoomAccessType.Open;

        if(accessType == Database.Models.RoomAccessType.Password)
        {
            if(room.Password != this.Password)
                return new LocalizedErrorMessage("Incorrect flat password");
        }

        if(accessType == Database.Models.RoomAccessType.Closed)
        {
            bool doorbellRung = await TryRingDoorbell(ctx, room);
            return doorbellRung ? new RoomDoorbellMessage() : new RoomEntryNotAllowedMessage();
        }
        
        return new RoomEntryGrantedMessage();
    }

    private async Task<bool> TryRingDoorbell(IncomingMessageContext ctx, Database.Models.Room room)
    {
        if(ctx.User is null)
            return false;
            
        bool sent = false;

        List<RoomUserRight> rights = await ctx.Database.RoomUserRights
            .Where(r => r.RoomId == room.Id)
        .ToListAsync();

        var usersInRoom = RoomUtilities.GetUserConnectionsInRoom(ctx, room);
        var usersInRoomWithRights = usersInRoom.Where(u => rights.Any(r => r.UserId == u.User.Id)).ToList();

        foreach(var user in usersInRoomWithRights)
        {
            if(user.Connection is null)
                continue;

            await user.Connection.SendMessageAsync(
                new RoomDoorbellMessage(ctx.User.Username)
            );
            sent = true;
        }

        return sent;
    }
}