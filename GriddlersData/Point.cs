using Griddlers.Database;
using System;

namespace Griddlers.Library
{
    public record Point(bool IsDot, int X, int Y, string Colour)
    {
        public static short Group;

        public GriddlerPath.Action Action { get; private set; }
        public DateTime Time { get; private set; }
        public short Grp { get; private set; }

        public Point(): this(false, 0, 0, "black") { }
        public Point(GriddlerSolid gs) : this(false, gs.x_position, gs.y_position, gs.green ? "lightgreen" : "black") { }
        public Point(bool isDot, (int, int) xy, string c, GriddlerPath.Action a) : this(isDot, xy.Item1, xy.Item2, c)
        {
            Action = a;
            Time = DateTime.Now;
            Grp = Group;
        }

        public static bool operator ==(Point a, Item b)
        {
            return a.Colour == b.Colour;
        }

        public static bool operator !=(Point? a, Item b)
        {
            return a != (Point?)null && a.Colour != b.Colour;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
