using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.Habbo.Messaging.Outgoing.User;
using Achilles.TCP.Abstractions;
using static Achilles.Habbo.Messaging.Incoming.Register.RegistrationMessage;

namespace Achilles.Habbo.Messaging.Incoming.User;

[IncomingMessage(44)]
public class UpdateUserInfoMessage : IncomingMessage
{
    
    public Dictionary<RegistrationValueType, object> Values { get; set; } = [];

    public UpdateUserInfoMessage(MessageHeader header, string raw) : base(header, raw)
    {
    }
    
    protected override void Parse(IncomingMessageContent content)
    {
        while(content.Remaining > 0)
        {
            RegistrationValueType type = (RegistrationValueType) content.ReadBase64Int();
            if(!ValueDataTypes.ContainsKey(type))
                throw new Exception("Invalid registration value type: " + type);

            Type dataType = ValueDataTypes[type];
            if(dataType == typeof(bool))
                this.Values[type] = content.ReadBase64Int() == 1;
            else if(dataType == typeof(string))
                this.Values[type] = content.ReadString();
            else if(dataType == typeof(char))
                this.Values[type] = content.ReadChar();
            else if(dataType == typeof(int))
                this.Values[type] = content.ReadWiredInt();
            else
                throw new Exception("Invalid registration value type: " + type);
        }
    }

    public override async Task<OutgoingMessage?> RespondAsync(IncomingMessageContext ctx)
    {
        var user = ctx.User;
        if(user is null)
            return null;

        if(this.Values.ContainsKey(RegistrationValueType.CustomData))
            user.Motto = (string) this.Values[RegistrationValueType.CustomData]!;

        if(this.Values.ContainsKey(RegistrationValueType.Figure))
            user.Figure = (string) this.Values[RegistrationValueType.Figure]!;

        if(this.Values.ContainsKey(RegistrationValueType.Sex))
            user.Gender = (this.Values[RegistrationValueType.Sex] as string ?? "m")[0];

        ctx.Database.Users.Update(user);
        await ctx.Database.SaveChangesAsync();

        // TODO: if in room, refresh appearance

        return new SendUserInfoMessage(ctx);
    }

}
