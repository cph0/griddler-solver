namespace Griddlers.Library
{
    public class Block : Range, IColour
    {
        private readonly Item? _Item;

        public int BlockIndex { get; private set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int SolidCount { get; set; }
        public bool Green { get; private set; }
        public string Colour { get; private set; }
        public bool Complete { get; set; }
        public bool KnowItem => _Item.HasValue;

        public Block(int index, bool g) : base(-1, -1)
        {
            BlockIndex = index;
            Green = g;
            Colour = g ? "green" : "black";
        }

        public Block(bool c, int sC) : base(-1, -1)
        {
            Complete = c;
            SolidCount = sC;
            Colour = string.Empty;
        }

        public Block(int start, int end, bool g) : base(start, end)
        {
            StartIndex = start;
            EndIndex = end;
            Green = g;
            Colour = g ? "green" : "black";
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

        public bool Is(Item item) 
            => item.Value == Size && item.Green == Green;

        public bool CanBe(Item item) 
            => item.Value >= Size && item.Green == Green;
        
        public bool IsOrCantBe(Item item) => Is(item) || !CanBe(item);

        public static bool operator ==(Block a, Item b)
            => a.SolidCount == b.Value && a.Green == b.Green;

        public static bool operator !=(Block a, Item b)
            => a.SolidCount != b.Value || a.Green != b.Green;

        public static bool operator <=(Item a, Block b)
            => a.Value <= b.SolidCount && a.Green == b.Green;

        public static bool operator >=(Item a, Block b)
            => a.Value >= b.SolidCount && a.Green == b.Green;

        public static bool operator <(Item a, Block b)
            => a.Value < b.SolidCount || a.Green != b.Green;

        public static bool operator >(Item a, Block b)
            => a.Value > b.SolidCount || a.Green != b.Green;

        public override bool Equals(object? obj)
            => base.Equals(obj);

        public override int GetHashCode()
            => base.GetHashCode();
    }
}
