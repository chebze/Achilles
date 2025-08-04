namespace Achilles.Database.Models;

public enum RoomModelType
{
    PublicSpaceModel = 0,
    UserFlatModel = 1,
    UserFlatSpecialModel = 2
}

public class RoomModel
{
    public required string Id { get; set; }
    public required RoomModelType Type { get; set; }

    public required int DoorX { get; set; }
    public required int DoorY { get; set; }
    public required double DoorZ { get; set; }

    public required string Heightmap { get; set; }
    public string[] HeightmapAxes => this.Heightmap.Split('|');
}