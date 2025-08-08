namespace Achilles.Habbo.Data;

public class Position : IEquatable<Position>
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public double Z { get; private set; }

    public int HeadRotation { get; set; }
    public int BodyRotation { get; set; }

    public Position(int x, int y, double z, int headRotation, int bodyRotation)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.HeadRotation = headRotation;
        this.BodyRotation = bodyRotation;
    }
    public Position(int x, int y, double z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
    public Position(int x, int y)
    {
        this.X = x;
        this.Y = y;
        this.Z = 0;
    }

    public void SetHeadRotation(int amount)
    {
        this.HeadRotation = amount;
    }
    public void SetBodyRotation(int amount)
    {
        this.BodyRotation = amount;
    }
    public void SetRotation(int amount)
    {
        this.HeadRotation = amount;
        this.BodyRotation = amount;
    }

    public int GetDistanceSquared(Position point)
    {
        int dx = this.X - point.X;
        int dy = this.Y - point.Y;

        return (int) Math.Sqrt((dx * dx) + (dy * dy));
    }
    public bool Touches(Position position)
        => this.GetDistanceSquared(position) <= 1;

    public Position GetPositionLeft()
    {
        Position square = this.Clone();

        if (this.BodyRotation == 0) {
            square.X--;
        } else if (this.BodyRotation == 1) {
            square.X--;
            square.Y--;
        } else if (this.BodyRotation == 2) {
            square.Y--;
        } else if (this.BodyRotation == 3) {
            square.X++;
            square.Y--;
        }  else if (this.BodyRotation == 4) {
            square.X++;
        } else if (this.BodyRotation == 5) {
            square.X++;
            square.Y++;
        } else if (this.BodyRotation == 6) {
            square.Y++;
        } else if (this.BodyRotation == 7) {
            square.X--;
            square.Y++;
        }

        return square;
    }
    public Position GetPositionRight()
    {
        Position square = this.Clone();

        if (this.BodyRotation == 0) {
            square.X++;
        } else if (this.BodyRotation == 1) {
            square.X++;
            square.Y++;
        } else if (this.BodyRotation == 2) {
            square.Y++;
        } else if (this.BodyRotation == 3) {
            square.X--;
            square.Y++;
        }  else if (this.BodyRotation == 4) {
            square.X--;
        } else if (this.BodyRotation == 5) {
            square.X--;
            square.Y--;
        } else if (this.BodyRotation == 6) {
            square.Y--;
        } else if (this.BodyRotation == 7) {
            square.X++;
            square.Y--;
        }

        return square;
    }
    public Position GetPositionBehind()
    {
        Position square = this.Clone();

        if (this.BodyRotation == 0) {
            square.Y++;
        } else if (this.BodyRotation == 1) {
            square.X--;
            square.Y++;
        } else if (this.BodyRotation == 2) {
            square.X--;
        } else if (this.BodyRotation == 3) {
            square.X--;
            square.Y--;
        }  else if (this.BodyRotation == 4) {
            square.Y--;
        } else if (this.BodyRotation == 5) {
            square.X++;
            square.Y--;
        } else if (this.BodyRotation == 6) {
            square.X++;
        } else if (this.BodyRotation == 7) {
            square.X++;
            square.Y++;
        }

        return square;
    }
    public Position GetPositionForward() {
        Position square = this.Clone();

        if (this.BodyRotation == 0) {
            square.Y--;
        } else if (this.BodyRotation == 1) {
            square.X++;
            square.Y--;
        } else if (this.BodyRotation == 2) {
            square.X++;
        } else if (this.BodyRotation == 3) {
            square.X++;
            square.Y++;
        }  else if (this.BodyRotation == 4) {
            square.Y++;
        } else if (this.BodyRotation == 5) {
            square.X--;
            square.Y++;
        } else if (this.BodyRotation == 6) {
            square.X--;
        } else if (this.BodyRotation == 7) {
            square.X--;
            square.Y--;
        }

        return square;
    }

    public Position Clone() => new Position(this.X, this.Y, this.Z, this.HeadRotation, this.BodyRotation);

    public bool Equals(Position? other)
    {
        if(other is null)
            return false;

        return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
    }

    public override string ToString()
    {
        return $"{this.X}, {this.Y}";
    }
}