namespace Achilles.Habbo.Configuration;

public class HabboConfiguration
{
    public required FigurePartsConfiguration FigureParts { get; set; }
    public required RoomomaticConfiguration Roomomatic { get; set; }

    public required List<RankConfiguration> Ranks { get; set; }
    public required string DefaultRankName { get; set; }

    public required MessengerConfiguration Messenger { get; set; }

    public required HabboClubConfiguration Club { get; set; }
}