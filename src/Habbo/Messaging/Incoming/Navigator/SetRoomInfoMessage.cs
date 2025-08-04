using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;

namespace Achilles.Habbo.Messaging.Incoming.Navigator;

[IncomingMessage(25)]
public class SetRoomInfoMessage : IncomingMessage
{
    public int RoomId { get; set; }
    public Dictionary<string, string> Info { get; set; } = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SetRoomInfoMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        if(content.Peek() == '/')
            content.Skip();
            
        this.RoomId = int.Parse(content.ReadUntil('/'));

        if(content.Peek() == (char)13)
            content.Skip();
        
        while(content.Remaining > 0)
        {
            string pair = content.ReadUntil((char) 13);
            if(pair.Length == 0)
                continue;
            if(pair.IndexOf('=') == -1)
                throw new Exception("Invalid pair: " + pair);

            string[] split = pair.Split('=');
            this.Info[split[0]] = split[1];
        }
    }
    
    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return;

        Room? room = ctx.Database.Rooms.FirstOrDefault(x => x.Id == this.RoomId);
        if(room is null)
        {
            await ctx.Connection.SendMessageAsync(new AlertMessage("Room not found"));
            return;
        }

        if(room.OwnerId != ctx.User.Id)
        {
            await ctx.Connection.SendMessageAsync(
                new AlertMessage("You are not the owner of this room")
            );
            return;
        }

        foreach(var pair in this.Info)
        {
            if(pair.Key == "description")
                room.Description = pair.Value;
            if(pair.Key == "allsuperuser")
                room.AllSuperUsers = int.Parse(pair.Value) == 1;
            if(pair.Key == "maxvisitors")
                room.MaxVisitors = int.Parse(pair.Value);
            if(pair.Key == "password")
                room.Password = pair.Value;
        }

        await ctx.Database.SaveChangesAsync();
    }
    
}
