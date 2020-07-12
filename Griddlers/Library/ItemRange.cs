using System;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    public class ItemRange
    {
        private readonly bool _UsingArray;
        private bool? _ItemsOneValue;
        private bool? _ItemsOneColour;
        protected readonly IEnumerable<Item> _ItemsEnum;
        protected readonly Item[] _ItemsArray;

        public int FirstItemIndex => _Items.First()?.Index ?? -1;
        public int LastItemIndex => _Items.Last()?.Index ?? -1;
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
                    _ItemsOneColour = _Items.GroupBy(g => g.Green).Count() == 1;

                return _ItemsOneColour.Value;
            }
        }

        private IEnumerable<Item> _Items => _UsingArray ? _ItemsArray : _ItemsEnum;

        public ItemRange(IEnumerable<Item> items)
        {
            _UsingArray = false;
            _ItemsEnum = items;
            _ItemsArray = new Item[] { };
        }

        public ItemRange(Item[] items)
        {
            _UsingArray = true;
            _ItemsEnum = items;
            _ItemsArray = items;
        }

        public IEnumerable<(Item, Item)> Pair()
            => Pair(_Items);
        public static IEnumerable<(Item, Item)> Pair(IEnumerable<Item> items)
        {
            Item? Previous = null;

            foreach (Item Item in items)
            {
                if (Previous != (object?)null)
                    yield return (Previous, Item);

                Previous = Item;
            }
        }

        public int GetDotCount()
            => GetDotCount(_Items);
        public static int GetDotCount(IEnumerable<Item> items)
        {
            int Dots = 0;
            foreach ((Item, Item) Item in Pair(items))
            {
                if (Item.Item1.Green == Item.Item2.Green)
                    Dots++;
            }
            return Dots;
        }

        public IEnumerable<Item> Reverse(int? start = null)
        {
            if (_UsingArray)
                return _ItemsArray.Reverse(start);
            else
                return _ItemsEnum.Reverse();
        }

        public int Sum(bool includeDots = true)
        {
            return _Items.Sum(s => s.Value) + (includeDots ? GetDotCount(_Items) : 0);
        }

        public bool Any() => _Items.Any();
        public bool Any(Func<Item, bool> func) => _Items.Any(func);

        public bool All(Func<Item, bool> func)
        {
            return _Items.All(func);
        }
    }
}
