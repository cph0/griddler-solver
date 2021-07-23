using Griddlers.Database;
using System;

namespace Griddlers.Library
{
    public record Point(bool IsDot, int X, int Y, string Colour, int Xpos, int Ypos)
    {
        private const int sS = 20;
        public static short Group;

        public GriddlerPath.Action Action { get; private set; }
        public DateTime Time { get; private set; }
        public short Grp { get; private set; }

        public Point(): this(false, 0, 0, "black", 0, 0) { }
        public Point(bool isDot, int ex, int why, string c) : this(isDot, ex * sS, why * sS, c, ex, why)
        {

        }
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
