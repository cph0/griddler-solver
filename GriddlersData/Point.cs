using Griddlers.Database;
using System;

namespace Griddlers.Library
{
    public record Point(bool IsDot, int X, int Y, bool Green, int Xpos, int Ypos)
    {
        private const int sS = 20;
        public static short Group;

        public GriddlerPath.Action Action { get; private set; }
        public DateTime Time { get; private set; }
        public short Grp { get; private set; }

        public Point(): this(false, 0, 0, false, 0, 0) { }
        public Point(bool isDot, int ex, int why, bool g) : this(isDot, ex * sS, why * sS, g, ex, why)
        {

        }
        public Point(GriddlerSolid gs) : this(false, gs.x_position, gs.y_position, gs.green) { }
        public Point(bool isDot, (int, int) xy, bool g, GriddlerPath.Action a) : this(isDot, xy.Item1, xy.Item2, g)
        {
            Action = a;
            Time = DateTime.Now;
            Grp = Group;
        }
    }
}
