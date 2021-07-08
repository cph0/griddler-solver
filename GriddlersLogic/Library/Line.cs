using Griddlers.Database;
using MultiKeyLookup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    public class Skip
    {
        public int Index { get; set; }
    }

    public class Line : ItemRange, IEnumerable<Item>
    {
        //TEMP - until points/dots are in this class
        private Logic Logic;

        private new Item[] _Items => (Item[])base._Items;
        private int? _LineValue;
        private int? _MinItem;
        private int? _MaxItem;

        private IDictionary<int, Line> _PairLines;
        private readonly MultiKeyLookup<Gap> _Gaps;
        public IDictionary<int, string> Points { get; private set; }
        public HashSet<int> Dots { get; private set; }

        public bool IsComplete { get; set; }
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
                    Item[] items,
                    Logic logic) : base(items, 0, items.Length - 1, true)
        {
            Logic = logic;
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
            for (int i = index; forward ? i <= LineItems - 1 : i >= 0; i += (forward ? 1 : -1))
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
                            bool forward = true)
        {
            (int Sum, int Shift) = (0, 0);
            int StartPos = gap.Start;
            int EndPos = gap.End;
            int Range = gap.Size;

            if (block != null)
            {
                if (forward)
                    EndPos = block.Start;
                else
                    StartPos = block.End;
                Range = EndPos - StartPos;
            }

            for (int Index = start; forward ? Index < LineItems : Index >= 0; Index += forward ? 1 : -1)
            {
                Item Item = this[Index];
                var (Value, Colour) = Item;
                Sum += Value;

                while (Points.TryGetValue(forward ? StartPos + Sum : EndPos - Sum, out string? Pt)
                    && Pt == Colour)
                {
                    Sum++;
                }

                if (Sum < Range || (Sum == Range
                    && (block == null || DotBetween(Item, block) == 0)))
                    Shift++;
                else
                {
                    if (block != null && Sum > gap.Size)
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

            if (!gap.IsFull && (Iei.ItemShift > 1 || (Iei.ItemShift == 1 && !gap.HasPoints)))
                Iei.Item2 = false;

            if (gap.IsFull && (!Iei.Item2 || RanOutOfItems(Iei.Item1, forward)
                || !gap.Is(this[Iei.Item1])))
            {
                IEnumerable<Item> Items = Where(ei, Iei.Item1, true);
                Item[] UniqueItems = Items.Where(w => gap.Is(w)).ToArray();
                Item? Item = null;

                if (UniqueItems.Length == 1)
                    Item = UniqueItems[0];
                else
                {
                    Gap? LastGap = FindGapAtPos(gap.Start + (forward ? -1 : 1), !forward);
                    if (LastGap?.IsFull == true)
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
                    Item = !forward ? Items.FirstOrDefault(f => gap.Is(f))
                        : Items.LastOrDefault(f => gap.Is(f));

                Iei = (Item?.Index ?? -1, UniqueItems.Length == 1, 1);
            }
            else if (!gap.IsFull && gap.HasPoints && !Iei.Item2)
            {
                Block? LastBlock = forward ? gap.GetLastBlock(gap.End) : gap.GetNextBlock(gap.Start);
                if (LastBlock != null)
                {
                    int ToItem = Iei.Item1 + ((forward ? 1 : -1) * (Iei.ItemShift - 1));                    
                    if (UniqueCount(Where(ei, ToItem, true), gap, out Item UniqueItem) 
                        && UniqueItem.Index == ToItem)
                        Iei.Item2 = true;
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
                foreach (Block Block in Gap.GetBlocks())
                {
                    Ls.SetIndexAtBlock(SumWhile(Ls.Index, Gap, Block));
                    yield return (Block, Gap, Ls, Skip);
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

        public IEnumerable<Point> AddDots(int start,
                                          int end,
                                          GriddlerPath.Action action)
        {
            for (int i = start; i <= end; i++)
            {
                Point? Dot = AddDot(i, action).SingleOrDefault();

                if (Dot != null)
                    yield return Dot;
            }
        }

        public IEnumerable<Point> AddDot(int index,
                                         GriddlerPath.Action action,
                                         bool fromPair = false)
        {
            if (!Dots.Contains(index) && index >= 0 && index <= LineLength - 1)
            {
                Dots.Add(index);
                AddGap(index);

                if (!fromPair && _PairLines.TryGetValue(index, out Line? line))
                    line.AddDot(LineIndex, action, true);

                var Xy = IsRow ? (index, LineIndex) : (LineIndex, index);
                Point Dot = new Point(true, Xy, false, action);
                Logic.dots.TryAdd(Xy, Dot);
                return new[] { Dot };
            }

            return Array.Empty<Point>();
        }

        public IEnumerable<Point> AddPoints(int start,
                                            int end,
                                            string colour,
                                            GriddlerPath.Action action,
                                            int? item = null)
        {
            for (int i = start; i <= end; i++)
            {
                Point? Pt = AddPoint(i, colour, action, item).SingleOrDefault();

                if (Pt != null)
                    yield return Pt;
            }
        }

        public IEnumerable<Point> AddPoint(int index,
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

                var Xy = IsRow ? (index, LineIndex) : (LineIndex, index);
                Point Pt = new Point(false, Xy, colour == "green", action);
                Logic.points.TryAdd(Xy, Pt);
                return new[] { Pt };
            }

            return Array.Empty<Point>();
        }

        public static bool IsolatedPart(Item item,
                                        Block lastBlock,
                                        Block block,
                                        bool forward = true)
        {
            if (forward && block.End <= lastBlock.Start + item.Value - 1)
                return false;
            else if (!forward && block.Start >= lastBlock.End - item.Value + 1)
                return false;

            return true;
        }

        public int GetLinePointsValue(bool includeDots = false)
            => Points.Count + (includeDots ? Dots.Count : 0);

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
        public (bool, bool, IDictionary<int, int>) IsLineIsolated()
        {
            Logic.AddMethodCount("IsolatedPart");
            //2,3,4,5//--00--0---0---- isolated 2,3,4 - invalid as could be 3,4,5
            bool IsIsolated = true, Valid = false, NotSolid = false, RestIsolated = false;
            bool Green = false, AllOneColour = ItemsOneColour;
            int BlockCount = 0;
            int SolidCount = 0;
            int CurrentItem = 0, ReachIndex = 0, StartItem = 0;
            Block? LastBlock = null;
            Dictionary<int, bool> Isolations = new Dictionary<int, bool>();
            Dictionary<int, int> IsolatedItems = new Dictionary<int, int>();
            Dictionary<int, Block> BlockIndexes = new Dictionary<int, Block>();
            Dictionary<int, bool> CanJoin = new Dictionary<int, bool>();
            Dictionary<int, int> Pushes = new Dictionary<int, int>();

            foreach (var (Block, _, _, _) in GetBlocks())
            {
                BlockIndexes[BlockCount] = Block;

                if (LastBlock != null && CurrentItem < LineItems)
                {
                    int ReachIndexCurrent = LastBlock.Start + _Items[CurrentItem].Value - 1;
                    
                    if (ReachIndexCurrent > ReachIndex)
                        ReachIndex = ReachIndexCurrent;

                    //previous block reach current
                    if (Block.End <= ReachIndexCurrent)
                    {
                        Isolations.TryAdd(BlockCount, false);
                        CanJoin.TryAdd(BlockCount - 1, false);
                        CanJoin.TryAdd(BlockCount, false);
                        Pushes.TryAdd(BlockCount, CurrentItem + 1);
                        RestIsolated = true;
                        IsIsolated = false;
                    }
                    else if (CurrentItem == LineItems - 1)
                    {
                        //does not reach & no more items => change isolations
                        RestIsolated = false;
                        if (Isolations.ContainsKey(BlockCount))
                            Isolations.Remove(BlockCount);
                        if (Isolations.ContainsKey(BlockCount - 2)
                            && !Pushes.ContainsKey(BlockCount - 2))
                            Isolations.Remove(BlockCount - 2);

                        // just one push
                        if (Pushes.Count == 1)
                        {
                            int Stop = Pushes.Keys.First();
                            bool Flag = false;
                            //start at previous block and current item minus 1 - current item is previous block
                            for (int d = BlockCount - 1; d >= 0; d--)
                            {
                                int ItemIndex = CurrentItem - (BlockCount - d);

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
                                for (int d = Stop; d < BlockCount; d++)
                                {
                                    Isolations.Remove(d);
                                }
                            }
                        }
                    }

                    CurrentItem++;

                    //check solid count
                    //if (CurrentItem < endItem + 1
                    //    && !new Block(-1, Pt.Green) { SolidCount = SolidCount }.CanBe(_Items[CurrentItem]))
                    //{
                    //    bool NoMoreItems = UniqueCount(new Block(-1, Pt.Green) { SolidCount = SolidCount }, out Item q);
                    //    bool Flag = NoMoreItems;
                    //    int StartIndex = -1;
                    //    int Stop = 0;

                    //    if (NoMoreItems)
                    //    {
                    //        StartIndex = FirstOrDefault(w => new Block(-1, Pt.Green) { SolidCount = SolidCount }.CanBe(w))?.Index ?? -1;

                    //        for (int d = BlockCount - 1; d >= 0; d--)
                    //        {
                    //            int ItemIndex = StartIndex - (BlockCount - d);

                    //            if (ItemIndex == -1)
                    //            {
                    //                Flag = false;
                    //                break;
                    //            }

                    //            if (BlockIndexes.TryGetValue(d, out Block? First)
                    //                && BlockIndexes.TryGetValue(d + 1, out Block? Second))
                    //            {
                    //                if (First.StartIndex + _Items[ItemIndex].Value - 1 >= Second.EndIndex
                    //                    || Second.EndIndex - _Items[ItemIndex + 1].Value + 1 <= First.StartIndex)
                    //                {
                    //                    Flag = false;
                    //                    break;
                    //                }
                    //                else if (Second.StartIndex - First.EndIndex - 1 >= (AllOneColour ? 3 : 0))
                    //                {
                    //                    Stop = d + 1;
                    //                    break;
                    //                }
                    //            }
                    //        }
                    //    }

                    //    if (Flag)
                    //    {
                    //        for (int d = Stop; d <= BlockCount; d++)
                    //        {
                    //            int ItemIndex = StartIndex - (BlockCount - d);
                    //            IsolatedItems.TryAdd(d, ItemIndex);
                    //        }
                    //    }
                    //    else if (CurrentItem == endItem && Pushes.Count == 1)
                    //    {
                    //        int Key = Pushes.Keys.First();
                    //        CanJoin[Key] = true;
                    //        CanJoin[Key - 1] = true;
                    //    }
                    //}

                    //previous reach current && no more items
                    if (Block.End <= ReachIndexCurrent && CurrentItem == LineItems)
                        CurrentItem--;

                    if (CurrentItem < LineItems)
                    {
                        int BackReach = (Block.End - _Items[CurrentItem].Value) + 1;
                        //current block reach previous
                        if (BackReach <= LastBlock.Start)
                        {
                            for (int d = BlockCount - 1; d >= 0; d--)
                            {
                                Isolations.TryAdd(d, false);
                                if (BlockIndexes.TryGetValue(d, out Block? Value)
                                    && Value.End >= BackReach)
                                    CanJoin.TryAdd(d, false);
                            }

                            IsIsolated = false;
                        }
                    }
                }

                BlockCount++;
                LastBlock = Block;

                //start - skip to correct solid count
                if (BlockCount == 1 && _Items[CurrentItem].Value < SolidCount)
                {
                    CurrentItem = FirstOrDefault(w => w.Value >= SolidCount)?.Index ?? 0;
                    StartItem = CurrentItem;
                }
            }

            if (BlockCount != LineItems || StartItem > 0)
                IsIsolated = false;

            return (IsIsolated, Valid, IsolatedItems);
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
}