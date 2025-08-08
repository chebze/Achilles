using Achilles.Habbo.Data;

namespace Achilles.Database.Models;

public enum RoomModelType
{
    PublicSpaceModel = 0,
    UserFlatModel = 1,
    UserFlatSpecialModel = 2
}
public enum RoomTileState
{
    Open = 0,
    Closed = 1
}

public class RoomModel : BaseModel<string>
{
    public required string Name { get; set; }

    public required RoomModelType Type { get; set; }

    public required int DoorX { get; set; }
    public required int DoorY { get; set; }
    public required double DoorZ { get; set; }

    public required string Heightmap { get; set; }
    public string[] HeightmapAxes => this.Heightmap.Split('|');

    public Position GetDoorPosition() => new Position(this.DoorX, this.DoorY, this.DoorZ);
}