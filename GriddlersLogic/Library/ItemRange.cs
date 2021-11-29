using System;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library;

public interface ICanBeItem : IColour
{
    int Size { get; }
    bool CanBe(Item item);
}

public class ItemRange
{
    private readonly bool _UsingArray;
    private readonly IDictionary<(int, string), Item> _UniqueCounts;
    private bool? _ItemsOneValue;
    private bool? _ItemsOneColour;
    private readonly Item[] _ItemsArray;
    private IEnumerable<Item> ItemsEnum => Where(Start, End);
    protected IEnumerable<Item> _Items => _UsingArray ? _ItemsArray : ItemsEnum;
    public int Start { get; private set; }
    public int End { get; private set; }
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

    protected ItemRange(Item[] items, int start, int end, bool usingArray = false)
    {
        _UsingArray = usingArray;
        _ItemsArray = items;
        _UniqueCounts = new Dictionary<(int, string), Item>(items.Length);

        Start = start;
        End = end;
    }

    public ItemRange CreateRange(int start, int end)
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

    public (bool, bool) ShouldAddDots(int index)
    {
        bool Start = index > 0 && _ItemsArray[index].Colour == _ItemsArray[index - 1].Colour;
        bool End = index < _ItemsArray.Length - 1 && _ItemsArray[index].Colour == _ItemsArray[index + 1].Colour;
        return (Start || index == 0, End || index == _ItemsArray.Length - 1);
    }
    public (bool, bool) ShouldAddDots(Block block,
                                      int? veryStart = null,
                                      int? veryEnd = null)
    {
        Item[] Cache = _Items.AsArray();
        int Start = Cache.FirstOrDefault(block.Is).Index;
        int End = Cache.LastOrDefault(block.Is).Index;
        int VeryStart = veryStart.GetValueOrDefault(Start - 1);
        int VeryEnd = veryEnd.GetValueOrDefault(End + 1);
        bool AddStart = Where(VeryStart, End).GroupBy(g => g.Colour).Count() == 1;
        bool AddEnd = Where(Start, VeryEnd).GroupBy(g => g.Colour).Count() == 1;
        return (AddStart, AddEnd);
    }

    public int GetDotCount(int pos)
    {
        return ShouldAddDots(pos).Item2 ? 1 : 0;
    }

    public int GetDotCountB(int pos)
    {
        return ShouldAddDots(pos).Item1 ? 1 : 0;
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

    public bool UniqueCount(ICanBeItem block, out Item item)
    {
        bool Retval;

        if (_UniqueCounts.TryGetValue((block.Size, block.Colour), out Item Out))
        {
            item = Out;
            Retval = true;
        }
        else
        {
            Retval = UniqueCount(_Items, block, out item);
            if (Retval)
                _UniqueCounts[(block.Size, block.Colour)] = item;
        }

        return Retval;
    }

    public static bool UniqueCount(IEnumerable<Item> items,
                                   ICanBeItem block,
                                   out Item item)
    {
        Item[] Cache = items.AsArray();
        item = Cache.FirstOrDefault(block.CanBe);
        return item != null && Cache.Count(block.CanBe) == 1;
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

    public int Sum(bool includeDots = true) => Sum(_Items, includeDots);
    public static int Sum(IEnumerable<Item> items, bool includeDots = true)
    {
        Item[] Cache = items.AsArray();
        return Cache.Sum(s => s.Value) + (includeDots ? GetDotCount(Cache) : 0);
    }

    public bool Any(Func<Item, bool> func) => _Items.Any(func);

    public bool All(Func<Item, bool> func) => All(_Items, func);
    public static bool All(IEnumerable<Item> items, Func<Item, bool> func)
    {
        Item[] Cache = items.AsArray();
        return Cache.Any(func) && Cache.All(func);
    }

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
