﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    public class ItemRange
    {
        private readonly bool _UsingArray;
        private readonly IDictionary<(int, bool), bool> _UniqueCounts;
        private bool? _ItemsOneValue;
        private bool? _ItemsOneColour;
        protected readonly IEnumerable<Item> _ItemsEnum;
        protected readonly Item[] _ItemsArray;

        public int FirstItemIndex => FirstOrDefault()?.Index ?? -1;
        public int LastItemIndex => LastOrDefault()?.Index ?? -1;
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
            _UniqueCounts = new Dictionary<(int, bool), bool>(items.Count());
        }

        public ItemRange(params Item[] items)
        {
            _UsingArray = true;
            _ItemsEnum = items;
            _ItemsArray = items;
            _UniqueCounts = new Dictionary<(int, bool), bool>(items.Length);
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
            int Dots = 0;
            foreach ((Item, Item) Item in Pair(items))
            {
                if (Item.Item1.Green == Item.Item2.Green)
                    Dots++;
            }
            return Dots;
        }

        public bool UniqueCount(Block block)
        {
            bool Retval = false;

            if (_UniqueCounts.TryGetValue((block.SolidCount, block.Green), out bool Out))
                Retval = Out;
            else
            {
                Retval = _Items.Count(c => c >= block) == 1;
                _UniqueCounts[(block.SolidCount, block.Green)] = Retval;
            }

            return Retval;
        }

        public IEnumerable<Item> Skip(int count)
        {
            if (_UsingArray)
                return _ItemsArray.Skip(count);
            else
                return _ItemsEnum.Skip(count);
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
            return Any(func) && _Items.All(func);
        }

        public Item? FirstOrDefault(Func<Item, bool> func)
            => FirstOrDefault(_Items.Where(func));
        public Item? FirstOrDefault()
            => FirstOrDefault(_Items);
        public Item? LastOrDefault(Func<Item, bool> func)
            => FirstOrDefault(_Items.Reverse().Where(func));
        public Item? LastOrDefault()
            => FirstOrDefault(_Items.Reverse());
        public Item? FirstOrDefault(IEnumerable<Item> list)
        {
            Item? RetVal = null;

            foreach (Item Item in list)
            {
                RetVal = Item;
                break;
            }

            return RetVal;
        }
    }
}
