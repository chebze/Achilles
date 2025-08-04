using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Incoming;
using Achilles.TCP.Abstractions;
using Achilles.Database.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Achilles.Habbo.Configuration;

namespace Achilles.Habbo.Messaging.Incoming.Register;

[IncomingMessage(43)]
public class RegistrationMessage : IncomingMessage
{
    public enum RegistrationValueType
    {
        ParentAgree = 1,
        Name = 2,
        Password = 3,
        Figure = 4,
        Sex = 5,
        CustomData = 6,
        Email = 7,
        Birthday = 8,
        DirectMail = 9,
        HasReadAgreement = 10,
        ISPId = 11,
        PartnerSite = 12,
        OldPassword = 13
    }

    public static Dictionary<RegistrationValueType, Type> ValueDataTypes { get; set; } = new() {
        {RegistrationValueType.ParentAgree, typeof(char)},
        {RegistrationValueType.Name, typeof(string)},
        {RegistrationValueType.Password, typeof(string)},
        {RegistrationValueType.Figure, typeof(string)},
        {RegistrationValueType.Sex, typeof(string)},
        {RegistrationValueType.CustomData, typeof(string)},
        {RegistrationValueType.Email, typeof(string)},
        {RegistrationValueType.Birthday, typeof(string)},
        {RegistrationValueType.DirectMail, typeof(char)},
        {RegistrationValueType.HasReadAgreement, typeof(char)},
        {RegistrationValueType.ISPId, typeof(string)},
        {RegistrationValueType.PartnerSite, typeof(string)},
        {RegistrationValueType.OldPassword, typeof(string)},
    };
    public Dictionary<RegistrationValueType, object> Values { get; set; } = [];

    public RegistrationMessage(MessageHeader header, string raw) : base(header, raw)
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

    public override async Task HandleAsync(IncomingMessageContext ctx)
    {
        HabboConfiguration configuration = ctx.Configuration;
        
        var user = new Achilles.Database.Models.User() {
            Username = (string) this.Values[RegistrationValueType.Name]!,
            Password = (string) this.Values[RegistrationValueType.Password]!,
            Figure = (string) this.Values[RegistrationValueType.Figure]!,
            Email = (string) this.Values[RegistrationValueType.Email]!,
                Birthday = (string) this.Values[RegistrationValueType.Birthday]!,
                Gender = (this.Values[RegistrationValueType.Sex] as string ?? "m")[0],
            Rank = configuration.DefaultRankName
        };
        await ctx.Database.Users.AddAsync(user);
        await ctx.Database.SaveChangesAsync();

        ctx.Connection.Metadata.Add(user);
    }
}
