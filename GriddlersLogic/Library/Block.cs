namespace Griddlers.Library
{
    public class Block
    {
        private readonly Item? _Item;
        private readonly Point[] _Points;

        public int BlockIndex { get; private set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int SolidCount { get; set; }
        public bool Green { get; private set; }
        public bool Complete { get; set; }
        public bool KnowItem => _Item != (object?)null;

        public Block(int index, bool g)
        {
            _Points = new Point[] { };
            BlockIndex = index;
            Green = g;
        }

        public Block(bool c, int sC)
        {
            _Points = new Point[] { };
            Complete = c;
            SolidCount = sC;
        }

        public Block(int start, int end, bool g)
        {
            _Points = new Point[] { };
            StartIndex = start;
            EndIndex = end;
            Green = g;
        }

        public Block(Item item, bool complete, int? start, int? end) : this(item.Index, item.Green)
        {
            _Item = item;
            SolidCount = item.Value;
            Complete = complete;

            if (start.HasValue)
                StartIndex = start.Value;

            if (end.HasValue)
                EndIndex = end.Value;
        }

        public static bool operator ==(Block a, Item b)
        {
            return a.SolidCount == b.Value && a.Green == b.Green;
        }

        public static bool operator !=(Block a, Item b)
        {
            return a.SolidCount != b.Value || a.Green != b.Green;
        }

        public static bool operator <=(Item a, Block b)
        {
            return a.Value <= b.SolidCount && a.Green == b.Green;
        }

        public static bool operator >=(Item a, Block b)
        {
            return a.Value >= b.SolidCount && a.Green == b.Green;
        }

        public static bool operator <(Item a, Block b)
        {
            return a.Value < b.SolidCount || a.Green != b.Green;
        }

        public static bool operator >(Item a, Block b)
        {
            return a.Value > b.SolidCount || a.Green != b.Green;
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
