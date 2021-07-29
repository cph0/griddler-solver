namespace Griddlers.Library
{
    public class Block : ICanBeItem
    {
        private readonly Item? _Item;
        private readonly Point[] _Points;

        public int Start { get; set; }
        public int End { get; set; }
        public int Size => End - Start + 1;

        public int BlockIndex { get; private set; }
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
            Start = start;
            End = end;
            Colour = colour;
        }

        public Block(Item item, bool complete, int? start, int? end) : this(item.Index, item.Colour)
        {
            _Item = item;
            SolidCount = item.Value;
            Complete = complete;

            if (start.HasValue)
                Start = start.Value;

            if (end.HasValue)
                End = end.Value;
        }

        public bool Is(Item item)
            => item.Value == SolidCount && item.Colour == Colour;

        public bool CanBe(Item item)
            => item.Value >= SolidCount && item.Colour == Colour;

        public bool IsOrCantBe(Item item) => Is(item) || !CanBe(item);
    }
}
