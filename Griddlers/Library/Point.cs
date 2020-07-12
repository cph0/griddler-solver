using Griddlers.Database;
using System;

namespace Griddlers.Library
{
    public class Point
    {
        private const int sS = 20;
        public static short Group;

        public int X { get; private set; }
        public int Y { get; private set; }
        public bool Green { get; private set; }

        public int Xpos { get; private set; }
        public int Ypos { get; private set; }
        public GriddlerPath.Action Action { get; private set; }
        public DateTime Time { get; private set; }
        public short Grp { get; private set; }

        public Point() { }
        public Point(int ex, int why, bool g)
        {
            X = ex * sS;
            Y = why * sS;
            Green = g;

            Xpos = ex;
            Ypos = why;
        }
        public Point(GriddlerSolid gs) : this(gs.x_position, gs.y_position, gs.green) { }
        public Point((int, int) xy, bool g, GriddlerPath.Action a) : this(xy.Item1, xy.Item2, g)
        {
            Action = a;
            Time = DateTime.Now;
            Grp = Group;
        }

        public static bool operator ==(Point a, Item b)
        {
            return a.Green == b.Green;
        }

        public static bool operator !=(Point? a, Item b)
        {
            return a != (Point?)null && a.Green != b.Green;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
    }
}
