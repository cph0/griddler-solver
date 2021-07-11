namespace Griddlers.Library
{
    public class Block : Range, ICanBeItem
    {
        private readonly Item? _Item;

        public string Colour { get; private set; }
        public bool Complete { get; set; }
        public bool KnowItem => _Item.HasValue;

        public Block(int start, int end, string colour) : base(start, end)
        {
            Colour = colour;
        }

        public bool Is(Item item) 
            => item.Value == Size && item.Colour == Colour;

        public bool CanBe(Item item) 
            => item.Value >= Size && item.Colour == Colour;
        
        public bool IsOrCantBe(Item item) => Is(item) || !CanBe(item);

        public Block SetStart(int start)
        {
            Start = start;
            return this;
        }

        public Block SetEnd(int end)
        {
            End = end;
            return this;
        }
    }
}
