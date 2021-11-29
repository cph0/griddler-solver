using Griddlers.Database;
using MultiKeyLookup;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library;

public class Skip
{
    public int Index { get; set; }
    public int BlockCount { get; set; }
    public Gap? LastGap { get; private set; }
    public Block? LastBlock { get; private set; }

    public void Continue(Block block)
    {
        BlockCount++;
        LastBlock = block;
    }

    public void Continue(Gap gap)
    {
        LastGap = gap;
    }
}

public class Line : ItemRange, IEnumerable<Item>
{
    private new Item[] _Items => (Item[])base._Items;
    private int? _LineValue;
    private int? _MinItem;
    private int? _MaxItem;
    private IDictionary<int, Line> _PairLines;
    private readonly MultiKeyLookup<Gap> _Gaps;

    public IDictionary<int, string> Points { get; private set; }
    public HashSet<int> Dots { get; private set; }
    public bool IsComplete => Points.Count + Dots.Count == LineLength;
    public bool IsRow { get; private set; }
    public int LineLength { get; private set; }
    public int LineIndex { get; private set; }
    public int LineItems => _Items.Length;
    public int LineValue
    {
        get
        {
            if (!_LineValue.HasValue)
                _LineValue = Sum();

            return _LineValue.Value;
        }
    }
    public int MinItem
    {
        get
        {
            if (!_MinItem.HasValue)
                _MinItem = Min();

            return _MinItem.Value;
        }
    }
    public int MaxItem
    {
        get
        {
            if (!_MaxItem.HasValue)
                _MaxItem = _Items.Max(m => m.Value);

            return _MaxItem.Value;
        }
    }

    public Item this[int index] => _Items[index];

    public Line(int index,
                bool isRow,
                int lL,
                Item[] items) : base(items, 0, items.Length - 1, true)
    {
        IsRow = isRow;
        LineLength = lL;
        LineIndex = index;

        _PairLines = new Dictionary<int, Line>();
        Points = new Dictionary<int, string>();
        Dots = new HashSet<int>();
        _Gaps = new MultiKeyLookup<Gap>(new[] { new Gap(0, LineLength - 1) },
                                        ("Start", k => k.Start),
                                        ("End", k => k.End),
                                        ("Size", k => k.Size));
    }

    public void SetPairLines(IEnumerable<Line> lines)
    {
        _PairLines = lines.ToDictionary(k => k.LineIndex);
    }

    public Gap? FindGapAtPos(int index, bool forward = true)
    {
        for (int i = index; forward ? i <= LineLength - 1 : i >= 0; i += (forward ? 1 : -1))
        {
            if (_Gaps.TryGetValue("Start", i, out Gap Gap))
                return Gap;
        }

        return null;
    }

    public IEnumerable<Gap> GetGapsBySize(int size)
    {
        foreach (Gap Gap in _Gaps.Get("Size", size))
            yield return Gap;
    }

    public int SumWhile(int start,
                        Gap gap,
                        Block? block = null,
                        bool forward = true,
                        bool? blockDir = null)
    {
        (int Sum, int Shift) = (0, 0);
        int StartPos = gap.Start;
        int EndPos = gap.End;
        int Range = gap.Size;

        if (block != null)
        {
            if (blockDir.GetValueOrDefault(forward))
                EndPos = block.Start;
            else
                StartPos = block.End;
            Range = EndPos - StartPos;
        }

        int CrashPos(int sum)
        {
            int AdjS = !blockDir.GetValueOrDefault(true) ? 1 : 0;
            int AdjE = blockDir.GetValueOrDefault() ? 1 : 0;
            return forward ? StartPos + sum + AdjS : EndPos - sum - AdjE;
        }

        for (int Index = start; forward ? Index < LineItems : Index >= 0; Index += forward ? 1 : -1)
        {
            Item Item = this[Index];
            var (Value, Colour) = Item;

            //point crash
            while (Points.TryGetValue(CrashPos(Sum), out string? Pt)
                && Pt != Colour)
            {
                Sum++;
            }

            Sum += Value;

            //dot crash
            while (Points.TryGetValue(CrashPos(Sum), out string? Pt)
                && Pt == Colour)
            {
                Sum++;
            }

            if (Sum < Range || (Sum == Range
                && (block == null || DotBetween(Item, block) == 0)))
                Shift++;
            else
            {
                //item can't be block as doesn't fit in gap!
                if (!blockDir.HasValue && block != null && Sum > gap.Size)
                    Shift--;

                break;
            }

            Sum += forward ? GetDotCount(Index) : GetDotCountB(Index);
        }

        return Shift;
    }

    private bool RanOutOfItems(int item, bool forward = true)
        => forward ? item >= LineItems : item < 0;

    private (int, bool, int) AdjustItemIndexes(Gap gap,
                                               int ei,
                                               (int, bool) ie,
                                               bool forward = true)
    {
        int ItemShift = SumWhile(ie.Item1, gap, null, forward);
        var Iei = (ie.Item1, ie.Item2, ItemShift);

        if (!gap.IsFull(false)
            && (Iei.ItemShift > 1 || (Iei.ItemShift == 1 && !gap.HasPoints)))
            Iei.Item2 = false;

        if (gap.IsFull() && (!Iei.Item2 || RanOutOfItems(Iei.Item1, forward)
            || !gap.Is(this[Iei.Item1])))
        {
            IEnumerable<Item> Items = Where(ei, Iei.Item1, true);
            Item[] UniqueItems = Items.Where(gap.Is).ToArray();
            Item? Item = null;

            if (UniqueItems.Length == 1)
                Item = UniqueItems[0];
            else
            {
                Gap? LastGap = FindGapAtPos(gap.Start + (forward ? -1 : 1), !forward);
                if (LastGap?.IsFull() == true)
                {
                    UniqueItems = Pair()
                        .Where(w => LastGap.Is(forward ? w.Item1 : w.Item2)
                        && gap.Is(forward ? w.Item2 : w.Item1))
                        .Select(s => forward ? s.Item2 : s.Item1)
                        .ToArray();

                    if (UniqueItems.Length == 1)
                        Item = UniqueItems[0];
                }
            }

            if (!Item.HasValue)
                Item = !forward ? Items.FirstOrDefault(gap.Is)
                    : Items.LastOrDefault(gap.Is);

            Iei = (Item?.Index ?? -1, UniqueItems.Length == 1, 1);
        }
        else if (!gap.IsFull() && gap.HasPoints && !Iei.Item2)
        {
            Block? LastBlock = forward ? gap.GetLastBlock(gap.End) : gap.GetNextBlock(gap.Start);
            if (LastBlock != null)
            {
                int ToItem = Iei.Item1 + ((forward ? 1 : -1) * (Iei.ItemShift - 1));
                if (UniqueCount(Where(ei, ToItem, true), LastBlock, out Item UniqueItem))
                {
                    int Itm = UniqueItem.Index;
                    bool Eq = Itm == ToItem;

                    if (!Eq)
                    {
                        int Start = UniqueItem.Index + (forward ? 1 : -1);
                        Itm += (forward ? 1 : -1) * SumWhile(Start,
                                                             gap,
                                                             LastBlock,
                                                             forward,
                                                             !forward);
                    }

                    Iei = (Itm, Eq, 1);
                }
            }
        }

        return Iei;
    }

    public IEnumerable<(Gap, LineSegment, Skip)> GetGaps(bool includeItems = false)
    {
        (int Item, int EqualityItem, bool Equality) = (0, 0, true);
        Skip Skip = new Skip();

        for (Skip.Index = 0; Skip.Index <= LineLength - 1; Skip.Index++)
        {
            if (_Gaps.TryGetValue("Start", Skip.Index, out Gap Gap))
            {
                Item? TheItem = Item < LineItems ? this[Item] : null;
                LineSegment Ls = new LineSegment(_Items, true, TheItem, Item, EqualityItem);
                yield return (Gap, Ls, Skip);

                if (includeItems)
                {
                    int ItemShift;
                    (Item, Equality, ItemShift) = AdjustItemIndexes(Gap, EqualityItem, (Item, Equality));

                    if (Equality)
                        EqualityItem = Item + ItemShift;
                    else if (Gap.HasPoints)
                        EqualityItem++;

                    Item += ItemShift;
                }

                Skip.Continue(Gap);
            }
        }
    }

    public IEnumerable<Gap> GetGapsB(int pos)
    {
        for (int Index = LineLength - 1; Index >= pos; Index--)
        {
            if (_Gaps.TryGetValue("Start", Index, out Gap Gap))
                yield return Gap;
        }
    }

    public IEnumerable<(Block, Gap, LineSegment, Skip)> GetBlocks(bool includeItems = false)
    {
        foreach (var (Gap, Ls, Skip) in GetGaps(includeItems))
        {
            bool SkipGap = false;//Gap.IsFull(false);
            foreach (Block Block in Gap.GetBlocks())
            {
                if (!SkipGap)
                {
                    Ls.SetIndexAtBlock(SumWhile(Ls.Index, Gap, Block));
                    yield return (Block, Gap, Ls, Skip);
                }
                Skip.Continue(Block);
            }
        }
    }

    public LineSegment GetItemAtPosB(Gap currentGap, Block? block = null)
    {
        (int Item, bool Equality) = (LineItems - 1, true);
        int EqualityItem = Item;

        foreach (Gap Gap in GetGapsB(currentGap.End + 1))
        {
            int ItemShift;
            (Item, Equality, ItemShift) = AdjustItemIndexes(Gap, EqualityItem, (Item, Equality), false);

            if (Equality)
                EqualityItem = Item - ItemShift;
            else if (Gap.HasPoints)
                EqualityItem--;

            Item -= ItemShift;
        }

        Item? TheItem = Item >= 0 ? this[Item] : null;
        LineSegment LsEnd = new LineSegment(_Items, false, TheItem, Item, EqualityItem);

        if (block != null)
            LsEnd.SetIndexAtBlock(SumWhile(Item, currentGap, block, false));

        return LsEnd;
    }

    private void AddGap(int index)
    {
        Gap? Gap = FindGapAtPos(index, false);

        if (Gap != null)
        {
            _Gaps.Remove("Start", Gap.Start);
            if (index > Gap.Start && index < Gap.End)
            {
                Gap RightGap = Gap.SplitGap(index);
                _Gaps.Add(Gap);
                _Gaps.Add(RightGap);
            }
            else if (index != Gap.End)
                _Gaps.Add(Gap.SetStart(index + 1));
            else if (index != Gap.Start)
                _Gaps.Add(Gap.SetEnd(index - 1));
        }
    }

    private void RemoveGap(int index)
    {
        Gap? LeftGap = _Gaps.GetValueOrDefault("End", index - 1);
        Gap? RightGap = _Gaps.GetValueOrDefault("Start", index + 1);
        int Start = index;
        int End = index;

        if (LeftGap != null)
        {
            _Gaps.Remove("Start", LeftGap.Start);
            Start = LeftGap.Start;
        }

        if (RightGap != null)
        {
            _Gaps.Remove("Start", RightGap.Start);
            End = RightGap.End;
        }

        _Gaps.Add(new Gap(Start, End));
    }

    public static (int, int, int) SpaceBetween(Range block,
                                               Range nextBlock,
                                               Item item)
    {
        int End = nextBlock.Start;
        int Start = block.End;
        int LeftDotCount = 0;
        int RightDotCount = 0;

        if (block is Block)
            LeftDotCount = DotBetween(block as Block, item);
        else
            Start = block.Start - 1;

        if (nextBlock is Block)
            RightDotCount = DotBetween(nextBlock as Block, item);
        else
            End = nextBlock.End + 1;

        return (End - Start - 1 - LeftDotCount - RightDotCount, LeftDotCount, RightDotCount);
    }

    public static bool FitsInSpace(Range block, Range nextBlock, Item item)
    {
        return item.Value <= SpaceBetween(block, nextBlock, item).Item1;
    }

    public void AddDot(int index,
                       GriddlerPath.Action action,
                       bool fromPair = false)
    {
        if (!Dots.Contains(index) && index >= 0 && index <= LineLength - 1)
        {
            Dots.Add(index);
            AddGap(index);

            if (!fromPair && _PairLines.TryGetValue(index, out Line? line))
                line.AddDot(LineIndex, action, true);
        }
    }

    public void RemoveDot(int index, bool fromPair = false)
    {
        Dots.Remove(index);
        RemoveGap(index);

        if (!fromPair && _PairLines.TryGetValue(index, out Line? line))
            line.RemoveDot(LineIndex, true);
    }

    public void AddPoint(int index,
                         string colour,
                         GriddlerPath.Action action,
                         int? item = null,
                         bool fromPair = false)
    {
        if (!Points.ContainsKey(index) && index >= 0 && index <= LineLength - 1)
        {
            Points.Add(index, colour);
            FindGapAtPos(index, false)?.AddPoint(index, colour, item);

            if (!fromPair && _PairLines.TryGetValue(index, out Line? line))
                line.AddPoint(LineIndex, colour, action, item, true);

        }
    }

    public void RemovePoint(int index, bool fromPair = false)
    {
        Points.Remove(index);
        FindGapAtPos(index, false)?.RemovePoint(index);

        if (!fromPair && _PairLines.TryGetValue(index, out Line? line))
            line.RemovePoint(LineIndex, true);
    }

    public static bool IsolatedPart(Item item,
                                    Block block,
                                    Block lastBlock,
                                    bool forward = true)
    {
        if (forward && block.End <= lastBlock.Start + item.Value - 1)
            return false;
        else if (!forward && block.Start >= lastBlock.End - item.Value + 1)
            return false;

        return true;
    }

    /// <summary>
    /// Determines if the block index is equivalent to the item index by:
    /// <list type="number">
    ///     <item>Starting from a position and item index</item>
    ///     <item>Taking each block and item in turn</item>
    ///     <item>See if any item consumes two blocks</item>
    ///     <item>If not and the item count equals the block count then the part is isolated</item>
    /// </list>
    /// </summary>
    /// <returns>
    /// A boolean if the part is isolated.  A boolean if the dictionary is valid.  
    /// A dictionary of item index by block index
    /// </returns>
    public (bool, IDictionary<int, int>) IsLineIsolated()
    {
        //2,3,4,5//--00--0---0---- isolated 2,3,4 - invalid as could be 3,4,5
        bool IsIsolated = true, NotSolid = false, RestIsolated = false;
        bool AllOneColour = ItemsOneColour;
        int BlockCount = 0, CurrentItem = 0, ReachIndex = 0, StartItem = 0;
        Dictionary<int, bool> Isolations = new Dictionary<int, bool>();
        Dictionary<int, int> IsolatedItems = new Dictionary<int, int>();
        Dictionary<int, Block> BlockIndexes = new Dictionary<int, Block>();
        Dictionary<int, bool> CanJoin = new Dictionary<int, bool>();
        Dictionary<int, int> Pushes = new Dictionary<int, int>();

        foreach (var (Block, _, _, Skip) in GetBlocks())
        {
            BlockIndexes[Skip.BlockCount] = Block;

            if (Skip.LastBlock != null && CurrentItem < LineItems)
            {
                int ReachIndexCurrent = Skip.LastBlock.Start + _Items[CurrentItem].Value - 1;

                if (ReachIndexCurrent > ReachIndex)
                    ReachIndex = ReachIndexCurrent;

                //previous block reach current
                if (Block.End <= ReachIndexCurrent)
                {
                    Isolations.TryAdd(Skip.BlockCount, false);
                    CanJoin.TryAdd(Skip.BlockCount - 1, false);
                    CanJoin.TryAdd(Skip.BlockCount, false);
                    Pushes.TryAdd(Skip.BlockCount, CurrentItem + 1);
                    RestIsolated = true;
                    IsIsolated = false;
                }
                else if (CurrentItem == LineItems - 1)
                {
                    //does not reach & no more items => change isolations
                    RestIsolated = false;
                    if (Isolations.ContainsKey(Skip.BlockCount))
                        Isolations.Remove(Skip.BlockCount);
                    if (Isolations.ContainsKey(Skip.BlockCount - 2)
                        && !Pushes.ContainsKey(Skip.BlockCount - 2))
                        Isolations.Remove(Skip.BlockCount - 2);

                    // just one push
                    if (Pushes.Count == 1)
                    {
                        int Stop = Pushes.Keys.First();
                        bool Flag = true;
                        //start at previous block and current item minus 1 - current item is previous block
                        for (int d = Skip.BlockCount - 1; d >= 0; d--)
                        {
                            int ItemIndex = CurrentItem - (Skip.BlockCount - d);

                            if (BlockIndexes.TryGetValue(d, out Block? First)
                                && BlockIndexes.TryGetValue(d + 1, out Block? Second))
                            {
                                if (First.Start + _Items[ItemIndex].Value - 1 >= Second.End
                                    || Second.End - _Items[ItemIndex + 1].Value + 1 <= First.Start)
                                {
                                    Flag = false;
                                    break;
                                }
                                else if (d == Stop)// at pushed (First), item does not reach Second
                                {
                                    //---{P:end}.{F != Item}.{S:start}---
                                    //Item is too big to fit between Previous and Second
                                    //Therefore Previous and First Join as ONLY ONE PUSH
                                    if (BlockIndexes.TryGetValue(d - 1, out Block? Previous)
                                        && Previous.End + _Items[ItemIndex].Value + 2 >= Second.Start)
                                    {
                                        CanJoin[d] = true;
                                        CanJoin[d - 1] = true;
                                    }

                                    break;
                                }
                            }
                        }

                        if (Flag)
                        {
                            int ItmIdx = StartItem;
                            for (int i = 0; i <= Skip.BlockCount; i++)
                                IsolatedItems.TryAdd(i, Pushes.ContainsKey(i) ? ItmIdx - 1 : ItmIdx++);

                            for (int d = Stop; d < Skip.BlockCount; d++)
                                Isolations.Remove(d);

                            break;
                        }
                    }
                }

                CurrentItem++;

                //previous reach current && no more items
                if (Block.End <= ReachIndexCurrent && CurrentItem == LineItems)
                    CurrentItem--;

                if (CurrentItem < LineItems)
                {
                    int BackReach = (Block.End - _Items[CurrentItem].Value) + 1;
                    //current block reach previous
                    if (BackReach <= Skip.LastBlock.Start)
                    {
                        for (int d = Skip.BlockCount - 1; d >= 0; d--)
                        {
                            Isolations.TryAdd(d, false);
                            if (BlockIndexes.TryGetValue(d, out Block? Value)
                                && Value.End >= BackReach)
                                CanJoin.TryAdd(d, false);
                        }

                        Pushes.TryAdd(1, 1);
                        Pushes.TryAdd(2, 2);
                        IsIsolated = false;
                    }
                }
            }

            //start - skip to correct solid count
            if (Skip.BlockCount == 0 && !Block.CanBe(_Items[CurrentItem]))
            {
                CurrentItem = FirstOrDefault(Block.CanBe)?.Index ?? 0;
                StartItem = CurrentItem;
            }

            BlockCount = Skip.BlockCount;
        }

        if (IsIsolated && StartItem > 0 && IsolatedItems.Count == 0
            && BlockCount == LineItems - StartItem - 1)
        {
            for (int i = 0; i <= BlockCount; i++)
                IsolatedItems.TryAdd(i, i + StartItem);
        }

        if (BlockCount != LineItems - 1 || StartItem > 0)
            IsIsolated = false;

        return (IsIsolated, IsolatedItems);
    }

    public IEnumerator<Item> GetEnumerator()
    {
        return ((IEnumerable<Item>)_Items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _Items.GetEnumerator();
    }
}
