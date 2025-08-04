using System.Diagnostics.CodeAnalysis;

namespace Achilles.Habbo.Configuration;

public class MessengerConfiguration
{
    public required int NormalFriendLimit { get; set; }
    public required int ClubFriendLimit { get; set; }
}