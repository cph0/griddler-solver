using System.Collections.Generic;

namespace Griddlers.Library
{
    public class Gap : Range, ICanBeItem
    {
        public string Colour { get; set; }

        public Gap(int start, int end, IEnumerable<Block>? blocks = null) : base(start, end)
        {
            Colour = "black";
        }

        public bool IsFull(int size, bool oneBlock = true)
            => size == Size && (!oneBlock || 1 == 1);

        public bool Is(Item a)
            => Size == a.Value && Colour == a.Colour;

        public bool CanBe(Item a)
            => Size >= a.Value && Colour == a.Colour;

    }
}
