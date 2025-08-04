using Achilles.Database.Models;
using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.Alert;
using Achilles.Habbo.Messaging.Outgoing.Navigator;

namespace Achilles.Habbo.Messaging.Incoming.Roomomatic;

[IncomingMessage(29)]
public class CreateRoomMessage : IncomingMessage
{
    public string Floor { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }
    public RoomAccessType AccessType { get; set; }
    public bool ShowOwnerName { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public CreateRoomMessage(MessageHeader header, string raw) : base(header, raw)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected override void Parse(IncomingMessageContent content)
    {
        string[] data = content.ReadRemainingString().Split('/');

        this.Floor = data[1];
        this.Name = data[2];
        this.Model = data[3];
        this.AccessType = Enum.Parse<RoomAccessType>(data[4], true);
        this.ShowOwnerName = data[5] == "1";
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        if(ctx.User is null)
            return null;

        if(this.Floor != "first floor")
            return new AlertMessage("Invalid floor");

        RoomModel? roomModel = ctx.Database.RoomModels.FirstOrDefault(x => x.Id == this.Model);
        if(roomModel is null)
            return new AlertMessage("Invalid room model");
        if(roomModel.Type < RoomModelType.UserFlatModel)
            return new AlertMessage("Invalid room model");
        if(!ctx.User.IsClubMember && roomModel.Type > RoomModelType.UserFlatModel)
            return new AlertMessage("Invalid room model");

        Room room = new Room
        {
            Name = this.Name,
            Description = "",
            AccessType = this.AccessType,
            ShowOwnerName = this.ShowOwnerName,
            OwnerId = ctx.User.Id,
            AllSuperUsers = false,
            RoomCategoryId = 0,
            RoomModelId = roomModel.Id
        };

        ctx.Database.Rooms.Add(room);
        await ctx.Database.SaveChangesAsync();

        await ctx.Connection.SendMessageAsync(new AlertMessage($"Room '{this.Name}' created!"));
        return new NavigateToRoomMessage(ctx, room);
    }
}

