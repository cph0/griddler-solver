using System;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    public class ItemRange
    {
        private readonly bool _UsingArray;
        private readonly IDictionary<(int, string), Item> _UniqueCounts;
        private bool? _ItemsOneValue;
        private bool? _ItemsOneColour;
        private readonly Item[] _ItemsArray;
        public int Start { get; private set; }
        public int End { get; private set; }

        private IEnumerable<Item> ItemsEnum => Where(Start, End);
        public bool ItemsOneValue
        {
            get
            {
                if (!_ItemsOneValue.HasValue)
                    _ItemsOneValue = _Items.GroupBy(g => g.Value).Count() == 1;

                return _ItemsOneValue.Value;
            }
        }
        public bool ItemsOneColour
        {
            get
            {
                if (!_ItemsOneColour.HasValue)
                    _ItemsOneColour = _Items.GroupBy(g => g.Colour).Count() == 1;

                return _ItemsOneColour.Value;
            }
        }

        protected IEnumerable<Item> _Items => _UsingArray ? _ItemsArray : ItemsEnum;

        protected ItemRange CreateRange(int start, int end)
            => new ItemRange(_ItemsArray, start, end);

        protected void SetStart(int start)
        {
            Start = start;
            _UniqueCounts.Clear();
        }

        protected void SetEnd(int end)
        {
            End = end;
            _UniqueCounts.Clear();
        }

        public ItemRange(IEnumerable<Item> items, int start = 0, int end = 0)
        {
            _UsingArray = false;
            //_ItemsEnum = items;
            _ItemsArray = items is Item[] v ? v : items.ToArray();
            _UniqueCounts = new Dictionary<(int, string), Item>(items.Count());

            Start = start;
            End = end;
        }

        public ItemRange(params Item[] items)
        {
            _UsingArray = true;
            //_ItemsEnum = items;
            _ItemsArray = items;
            _UniqueCounts = new Dictionary<(int, string), Item>(items.Length);
        }

        public IEnumerable<Item> Where(int start, int end)
            => Where(start, end, false);

        protected IEnumerable<Item> Where(int start, int end, bool allowSwap)
        {
            (int Start, int End) = (Math.Min(start, end), Math.Max(start, end));
            (Start, End) = !allowSwap ? (start, end) : (Start, End);
            return _ItemsArray.Where(w => w.Index >= Start && w.Index <= End);
        }

        public IEnumerable<(Item, Item)> Pair()
            => Pair(_Items);
        public static IEnumerable<(Item, Item)> Pair(IEnumerable<Item> items)
        {
            Item? Previous = null;

            foreach (Item Item in items)
            {
                if (Previous.HasValue)
                    yield return (Previous.Value, Item);

                Previous = Item;
            }
        }

        public IEnumerable<(Item, Item, Item)> Triple()
            => Triple(_Items);
        public static IEnumerable<(Item, Item, Item)> Triple(IEnumerable<Item> items)
        {
            Item? First = null;
            Item? Second = null;

            foreach (Item Item in items)
            {
                if (First.HasValue && Second.HasValue)
                {
                    yield return (First.Value, Second.Value, Item);
                    First = Second;
                }

                if (First.HasValue)
                    Second = Item;
                else
                    First = Item;
            }
        }

        public int GetDotCount()
            => GetDotCount(_Items);
        public static int GetDotCount(IEnumerable<Item> items)
        {
            return Pair(items)
                .Aggregate(0, (acc, a) => acc + DotBetween(a.Item1, a.Item2));
        }

        public bool UniqueCount(Block block, out Item item)
        {
            bool Retval = false;

            if (_UniqueCounts.TryGetValue((block.Size, block.Colour), out Item Out))
            {
                item = Out;
                Retval = true;
            }
            else
            {
                Retval = _Items.Count(block.CanBe) == 1;
                item = FirstOrDefault(block.CanBe) ?? default;
                if (Retval)
                    _UniqueCounts[(block.Size, block.Colour)] = item;
            }

            return Retval;
        }

        public IEnumerable<Item> Skip(int count)
        {
            if (_UsingArray)
                return _ItemsArray.Skip(count);
            else
                return ItemsEnum.Skip(count);
        }

        public IEnumerable<Item> Reverse(int? start = null)
        {
            if (_UsingArray)
                return _ItemsArray.Reverse(start);
            else
                return ItemsEnum.Reverse();
        }

        public static int DotBetween(IColour first, IColour second)
            => first.Colour == second.Colour ? 1 : 0;

        public int Sum(bool includeDots = true)
        {
            return _Items.Sum(s => s.Value) + (includeDots ? GetDotCount() : 0);
        }

        public bool Any() => _Items.Any();
        public bool Any(Func<Item, bool> func) => _Items.Any(func);

        public bool All(Func<Item, bool> func) => Any(func) && _Items.All(func); 

        public int Min(Block? block = null) 
        {
            return _Items.Where(w => block == null || block.CanBe(w)).Min(m => m.Value);
        }

        public Item? FirstOrDefault(Func<Item, bool> func)
            => FirstOrDefault(_Items.Where(func));
        public Item? FirstOrDefault()
            => FirstOrDefault(_Items);
        public Item? LastOrDefault(Func<Item, bool> func)
            => FirstOrDefault(Reverse().Where(func));
        public Item? LastOrDefault()
            => FirstOrDefault(Reverse());
        public static Item? FirstOrDefault(IEnumerable<Item> list)
            => list.FirstOrDefault();
    }
}
