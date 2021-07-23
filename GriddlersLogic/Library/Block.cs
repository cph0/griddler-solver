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
        public string Colour { get; private set; }
        public bool Complete { get; set; }
        public bool KnowItem => _Item.HasValue;

        public Block(int index, string colour)
        {
            _Points = new Point[] { };
            BlockIndex = index;
            Colour = colour;
        }

        public Block(bool c, int sC)
        {
            _Points = new Point[] { };
            Complete = c;
            SolidCount = sC;
            Colour = "black";
        }

        public Block(int start, int end, string colour)
        {
            _Points = new Point[] { };
            StartIndex = start;
            EndIndex = end;
            Colour = colour;
        }

        public Block(Item item, bool complete, int? start, int? end) : this(item.Index, item.Colour)
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
            return a.SolidCount == b.Value && a.Colour == b.Colour;
        }

        public static bool operator !=(Block a, Item b)
        {
            return a.SolidCount != b.Value || a.Colour != b.Colour;
        }

        public static bool operator <=(Item a, Block b)
        {
            return a.Value <= b.SolidCount && a.Colour == b.Colour;
        }

        public static bool operator >=(Item a, Block b)
        {
            return a.Value >= b.SolidCount && a.Colour == b.Colour;
        }

        public static bool operator <(Item a, Block b)
        {
            return a.Value < b.SolidCount || a.Colour != b.Colour;
        }

        public static bool operator >(Item a, Block b)
        {
            return a.Value > b.SolidCount || a.Colour != b.Colour;
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
