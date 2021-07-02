using Griddlers.Database;
using MultiKeyLookup;
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
        private readonly IDictionary<int, Block> _Blocks;
        private readonly IDictionary<int, Block> _BlocksByStart;
        private readonly IDictionary<int, Block> _BlocksByEnd;
        private readonly IDictionary<int, LineSegment> _ItemsAtPos;
        private readonly IDictionary<int, LineSegment> _ItemsAtPosB;
        private readonly IDictionary<(int, int, int), (bool, bool, IDictionary<int, int>)> _Isolations;
        private int? _LineValue;
        private int? _MinItem;
        private int? _MaxItem;

        private IDictionary<int, Line> _PairLines;
        private readonly MultiKeyLookup<Gap> _Gaps;
        public IDictionary<int, string> Points { get; private set; }
        public HashSet<int> Dots { get; private set; }

        public bool IsComplete { get; set; }
        public int CompletePos { get; set; }
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
                    _MinItem = _Items.Min(m => m.Value);

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
        public int StartOfFirstGap => FindGapAtPos(-1)?.Start ?? 0;
        public int EndOfLastGap => FindGapAtPos(LineLength, false)?.End ?? LineLength - 1;

        public Block CurrentBlock => _Blocks.Values.Last();

        public Item this[int index] => _Items[index];

        public Line(int index, bool isRow, int lL, Item[] items, Logic logic) : base(items)
        {
            Logic = logic;
            IsRow = isRow;
            LineLength = lL;
            LineIndex = index;
            _Blocks = new Dictionary<int, Block>(items.Length);
            _BlocksByStart = new Dictionary<int, Block>(items.Length);
            _BlocksByEnd = new Dictionary<int, Block>(items.Length);
            _ItemsAtPos = new Dictionary<int, LineSegment>(lL);
            _ItemsAtPosB = new Dictionary<int, LineSegment>(lL);
            _Isolations = new Dictionary<(int, int, int), (bool, bool, IDictionary<int, int>)>();

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

        public int SumWhile(int start, Gap gap, Block? block = null, bool forward = true)
        {
            (int Sum, int Shift) = (0, 0);
            int StartPos = gap.Start;
            int EndPos = gap.End;
            int Range = gap.Size;

            if (block != null)
            {
                if (forward)
                    EndPos = block.StartIndex;
                else
                    StartPos = block.EndIndex;
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

        private (int, bool, int) AdjustItemIndexes(Gap gap, int ei, (int, bool) ie, bool forward = true)
        {
            int ItemShift = SumWhile(ie.Item1, gap, null, forward);
            var Iei = (ie.Item1, ie.Item2, ItemShift);

            if (!gap.IsFull && (Iei.ItemShift > 1 || (Iei.ItemShift == 1 && !gap.HasPoints)))
                Iei.Item2 = false;

            if (gap.IsFull && (!Iei.Item2 || RanOutOfItems(Iei.Item1, forward)
                || gap != this[Iei.Item1]))
            {
                IEnumerable<Item> Items = Where(ei, Iei.Item1);
                Item[] UniqueItems = Items.Where(w => gap == w).ToArray();
                Item? Item = null;

                if (UniqueItems.Length == 1)
                    Item = UniqueItems[0];
                else
                {
                    Gap? LastGap = FindGapAtPos(gap.Start + (forward ? 1 : -1), forward);
                    if (LastGap?.IsFull == true)
                    {
                        UniqueItems = Pair()
                            .Where(w => LastGap == (forward ? w.Item1 : w.Item2)
                            && gap == (forward ? w.Item2 : w.Item1))
                            .Select(s => forward ? s.Item2 : s.Item1)
                            .ToArray();

                        if (UniqueItems.Length == 1)
                            Item = UniqueItems[0];
                    }
                }

                if (Item == null)
                    Item = !forward ? FirstOrDefault(f => gap == f)
                        : LastOrDefault(f => gap == f);

                Iei = (Item?.Index ?? -1, UniqueItems.Length == 1, 1);
            }
            else if (!gap.IsFull && gap.HasPoints && !Iei.Item2)
            {
                Block? LastBlock = gap.GetLastBlock(gap.End);
                if (LastBlock != null)
                {
                    int ToItem = Iei.Item1 + ((forward ? 1 : -1) * (Iei.ItemShift - 1));
                    Item[] UniqueItems = Where(ei, ToItem)
                                        .Where(w => gap == w).ToArray();

                    if (UniqueItems.Length == 1 && UniqueItems[0].Index == ToItem)
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
                    //IEnumerable<Item> Next = Where(EqualityItem, Item);
                    //IEnumerable<Item> After = _Items.Where((w, wi) => wi >= Item);
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

            //IEnumerable<Item> Next = Where(Item, EqualityItem);
            //IEnumerable<Item> After = _Items.Where((w, wi) => wi <= Item);
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

        public static (int, int, int) SpaceBetween(Range block, Range nextBlock, Item item) 
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

        public IEnumerable<Point> AddDots(int start, int end, GriddlerPath.Action action)
        {
            for (int i = start; i <= end; i++)
            {
                Point? Dot = AddDot(i, action);

                if (Dot != null)
                    yield return Dot;
            }
        }

        public Point? AddDot(int index, GriddlerPath.Action action, bool fromPair = false)
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
                return Dot;
            }

            return null;
        }

        public IEnumerable<Point> AddPoints(int start, int end, string colour, GriddlerPath.Action action, int? item = null)
        {
            for (int i = start; i <= end; i++)
            {
                Point? Pt = AddPoint(i, colour, action, item);

                if (Pt != null)
                    yield return Pt;
            }
        }

        public Point? AddPoint(int index, string colour, GriddlerPath.Action action, int? item = null, bool fromPair = false)
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
                return Pt;
            }

            return null;
        }

        public static bool IsolatedPart(Item item, Block lastBlock, Block block, bool forward = true)
        {
            if (forward && block.End <= lastBlock.Start + item.Value - 1)
                return false;
            else if (!forward && block.Start >= lastBlock.End - item.Value + 1)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the possible items at a position along the line working forwards
        /// </summary>
        /// <param name="position">The position of the line to start at</param>
        /// <param name="untilDot">Position is on a dot</param>
        /// <returns></returns>
        private LineSegment GetItemAtPosition(int position, bool untilDot = true)
        {
            Logic.AddMethodCount("GetItemAtPosition");
            int Item = 0, MaxItem = 0, LastItemAtEquality = -1, ItemAtEquality = -1, ItemAtLastGap = -1;
            bool Valid = true, Equality = untilDot, ScValid = true, PartEquality = false;
            int LastFullGapCount = 0, LastGapIndex = -1;
            Block LastBlock = new Block(0, false);
            HashSet<(int, bool)> PossibleSolids = new HashSet<(int, bool)>();

            foreach ((int Pos, _, Block Block, bool WasSolid, int GapSize, int SolidCount) in Logic.ForEachLinePos(this, (ns, sC, i, xy, gS) => gS > 0 && (Logic.dots.ContainsKey(xy) || i == LineLength - 1 || (!untilDot && i == position - 1)), (i) => i == position))
            {
                int Sum = 0, ItemShift = 0, GapMaxItem = 0;
                ItemAtLastGap = Item;
                LastGapIndex = Pos - GapSize - 1;

                if (WasSolid && !untilDot)
                    PossibleSolids = FindPossibleSolids(Pos - Block.SolidCount);

                if (Block.SolidCount == GapSize && Item < LineItems && _Items[Item].Value == Block.SolidCount
                    && _Items[Item].Value > MaxItem)
                {
                    Equality = true;
                }

                if (untilDot && Item >= LineItems && Block.SolidCount > 0 && Block.SolidCount == GapSize)
                {
                    for (int c = LineItems - 1; c >= 0; c--)
                    {
                        if (_Items[c].Value >= Block.SolidCount && _Items[c].Value <= GapSize)
                        {
                            Item = c;
                            break;
                        }
                    }
                }

                if (Item >= LineItems && Block.SolidCount > 0)
                    Item = LineItems - 1;

                foreach (Item Itm in Skip(Item))
                {
                    Sum += Itm.Value;
                    var Pos2 = IsRow ? (Pos - GapSize + Sum, LineIndex) : (LineIndex, Pos - GapSize + Sum);

                    if (Logic.points.TryGetValue(Pos2, out Point? Pt) && Pt.Green == Itm.Green)
                    {
                        Sum++;

                        if (WasSolid && Sum == GapSize
                            && Item + ItemShift >= 0
                            && Block.SolidCount > _Items[Item + ItemShift].Value)
                            Item--;
                        else if ((untilDot || !PossibleSolids.Contains((_Items[Item + ItemShift].Value, false)))
                            && WasSolid && Sum > GapSize
                            && Item + ItemShift - 1 >= 0
                            && Block.SolidCount > _Items[Item + ItemShift - 1].Value)
                            Item--;
                    }

                    if (Sum <= GapSize)
                    {
                        ItemShift++;

                        if (Itm.Value > GapMaxItem)
                            GapMaxItem = Itm.Value;
                    }
                    else
                        break;

                    Sum += GetDotCount(Itm.Index);
                }

                if (SolidCount < GapSize && (ItemShift > 1 || (ItemShift == 1 && SolidCount == 0)))
                    Equality = false;

                if (SolidCount < GapSize)
                    PartEquality = false;

                if (untilDot && Block.SolidCount == GapSize && Item < LineItems)
                {
                    int UniqueCount = 0, TempItem = 0;

                    foreach (Item Itm in Skip(LastItemAtEquality + 1).Take(Item - LastItemAtEquality))
                    {
                        if (Block == Itm)
                        {
                            if (UniqueCount > 0 && Itm.Index > TempItem + 1)
                            {
                                UniqueCount = 0;
                                break;
                            }

                            TempItem = Itm.Index;
                            UniqueCount++;
                        }
                    }

                    if (UniqueCount == 1
                        || (UniqueCount == 2 && PartEquality //osterich15x15
                        && _Items[TempItem - 1].Value == LastFullGapCount)
                        )
                    {
                        Item = TempItem;
                        ItemShift = 1;
                        Equality = true;
                    }
                }

                if (SolidCount == GapSize)
                    PartEquality = true;

                if (Equality && Pos < LineLength - 1 && Pos < position - 1)
                    LastItemAtEquality = Item; //????

                if (Equality && Pos < LineLength - 1 && Pos < position - 1)
                    ItemAtEquality = Item + ItemShift;

                if (GapMaxItem > MaxItem)
                    MaxItem = GapMaxItem;

                if (SolidCount == GapSize)
                    LastFullGapCount = Block.SolidCount;

                LastBlock = Block;
                if (SolidCount < GapSize)
                    ScValid = false;
                else
                    ScValid = true;

                Item += ItemShift;
            }

            if (Item > LineItems - 1)
                Valid = false;

            if (!untilDot)
            {
                Equality = false;
            }

            Item? TheItem = null;

            if (Valid)
                TheItem = _Items[Item];

            LastBlock.Complete = ScValid;
            IEnumerable<Item> Next = _Items.Where((w, wi) => wi >= ItemAtEquality && ((!Equality && wi <= Item) || wi == Item));
            ItemRange Before = new ItemRange(_Items.Where((w, wi) => wi >= ItemAtEquality && wi < Item));
            IEnumerable<Item> After = _Items.Where((w, wi) => wi >= Item);
            //HashSet<Item> RightBefore = FindPossibleSolids(IsRow, LineLength, lineIndex, 1);
            HashSet<Block> RightBefore = new Block[] { LastBlock }.ToHashSet();
            ItemRange Gap = new ItemRange(_Items.Where((w, wi) => wi >= ItemAtLastGap && wi < Item));

            return new LineSegment(Next, true, TheItem, Equality, Before, After, RightBefore, Gap, ItemAtLastGap, ItemAtEquality);
        }

        /// <summary>
        /// Finds the possible solids from the start to the next dot
        /// </summary>
        /// <param name="start">The position in the line to start</param>
        /// <param name="onlyFirstSolid">If true the solids must consume the first block</param>
        /// <returns>
        /// A dictionary of 
        /// 
        /// </returns>
        private Dictionary<int, Dictionary<(int, bool), int>> FindPossibleSolidsI(int start, bool onlyFirstSolid = true)
        {
            Logic.AddMethodCount("FindPossibleSolids");
            Dictionary<int, Dictionary<(int, bool), int>> PossibleSolids = new Dictionary<int, Dictionary<(int, bool), int>>(50);
            bool WasSolid = false, AfterFirstSolid = false, PrevColour = false;

            foreach ((int Pos, (int, int) Xy, _, _, _, _) in Logic.ForEachLinePos(this, start: start))
            {
                if (Logic.points.TryGetValue(Xy, out Point? Pt))
                {
                    if (!WasSolid || PrevColour != Pt.Green)
                    {
                        foreach (int PosSolid in PossibleSolids.Keys)
                            PossibleSolids[PosSolid].Remove((Pos - PosSolid, Pt.Green));

                        if (!AfterFirstSolid)
                            PossibleSolids.Add(Pos, new Dictionary<(int, bool), int>(50));

                        if (!WasSolid)
                            PrevColour = Pt.Green;
                    }
                }
                else
                {
                    if (!WasSolid && !AfterFirstSolid)
                        PossibleSolids.Add(Pos, new Dictionary<(int, bool), int>(50));
                    else if (WasSolid && onlyFirstSolid)
                        AfterFirstSolid = true;
                }

                if (!Logic.points.TryGetValue(Xy, out Pt) || PrevColour != Pt.Green || Pos == LineLength - 1)
                {
                    foreach (int PosSolid in PossibleSolids.Keys)
                    {
                        int ConsumeCount = 0;
                        bool ConsumeColour = false;
                        bool Valid = true, NextValid = true;
                        for (int Pos2 = PosSolid; Pos2 <= Pos; Pos2++)
                        {
                            var PosXy = IsRow ? (Pos2, LineIndex) : (LineIndex, Pos2);
                            if (Logic.points.TryGetValue(PosXy, out Point? Pt2))
                            {
                                if (ConsumeCount > 0 && Pt2.Green != ConsumeColour)
                                {
                                    Valid = false;
                                    break;
                                }

                                ConsumeCount++;
                                ConsumeColour = Pt2.Green;

                                if (Pt != (object?)null && Pt2.Green != Pt.Green)
                                {
                                    NextValid = false;
                                    break;
                                }
                            }
                        }

                        if (WasSolid && Valid)
                            PossibleSolids[PosSolid].TryAdd((Pos - PosSolid, ConsumeColour), ConsumeCount);

                        if (Valid && NextValid)
                            PossibleSolids[PosSolid].TryAdd((Pos - PosSolid + 1, ConsumeColour), ConsumeCount);

                        if (ConsumeCount == 0)
                        {
                            if (WasSolid)
                                PossibleSolids[PosSolid].TryAdd((Pos - PosSolid, !ConsumeColour), ConsumeCount);

                            PossibleSolids[PosSolid].TryAdd((Pos - PosSolid + 1, !ConsumeColour), ConsumeCount);
                        }
                    }
                }

                if (Logic.points.TryGetValue(Xy, out Pt))
                {
                    WasSolid = true;
                    PrevColour = Pt.Green;
                }
                else
                    WasSolid = false;

                if (Logic.dots.ContainsKey(Xy))
                    break;
            }

            return PossibleSolids;
        }
        /// <summary>
        /// Finds the possible solids from the start to the next dot
        /// <para>Uses: <see cref="FindPossibleSolidsI(int, bool)"/></para>
        /// </summary>
        /// <param name="start">The position in the line to start</param>
        /// <param name="onlyFirstSolid">If true the solids must consume the first block</param>
        /// <returns>
        /// A HashSet of the possible item values and colours
        /// </returns>        
        public HashSet<(int, bool)> FindPossibleSolids(int start, bool onlyFirstSolid = true)
        {
            Dictionary<int, Dictionary<(int, bool), int>> PossibleSolids = FindPossibleSolidsI(start, onlyFirstSolid);
            HashSet<(int, bool)> Return = new HashSet<(int, bool)>();

            foreach (Dictionary<(int, bool), int> Item in PossibleSolids.Values)
                Return.UnionWith(Item.Keys);

            return Return;
        }
        
        public void AddBlock(Item item, bool complete = false, int? start = null, int? end = null)
        {
            if (!_Blocks.TryGetValue(item.Index, out Block? Block))
            {
                Block = new Block(item, complete, start, end);
                _Blocks.TryAdd(item.Index, Block);
            }

            if (complete && start.HasValue)
            {
                Block.StartIndex = start.Value;
                int StartPos = 0;

                if (item.Index > 0)
                    StartPos = start.Value - _Items[item.Index - 1].Value + 1;

                for (int Pos = start.Value; Pos >= StartPos; Pos--)
                    _BlocksByStart.TryAdd(Pos, Block);
            }

            if (complete && end.HasValue)
            {
                Block.EndIndex = end.Value;
                int EndPos = LineLength - 1;

                if (item.Index < LineItems - 1)
                    EndPos = end.Value + _Items[item.Index + 1].Value - 1;

                for (int Pos = end.Value; Pos <= EndPos; Pos++)
                    _BlocksByEnd.TryAdd(Pos, Block);
            }
        }

        public int GetLinePointsValue(bool includeDots = false)
        {
            int LineValue = 0;

            LineValue = Logic.points.Keys.Count(w => IsRow ? w.Item2 == LineIndex : w.Item1 == LineIndex);

            if (includeDots)
                LineValue += Logic.dots.Keys.Count(w => IsRow ? w.Item2 == LineIndex : w.Item1 == LineIndex);

            return LineValue;
        }

        public (bool, bool) ShouldAddDots(int index)
        {
            bool Start = index > 0 && _Items[index].Green == _Items[index - 1].Green;
            bool End = index < _Items.Length - 1 && _Items[index].Green == _Items[index + 1].Green;
            return (Start || index == 0, End || index == _Items.Length - 1);
        }

        public int GetDotCount(int pos)
        {
            return ShouldAddDots(pos).Item2 ? 1 : 0;
        }

        public int GetDotCountB(int pos)
        {
            return ShouldAddDots(pos).Item1 ? 1 : 0;
        }

        public static int DotBetween(IColour first, IColour second)
            => first.Colour == second.Colour ? 1 : 0;

        public LineSegment GetItemAtPos(int pos, bool untilDot = true)
        {
            LineSegment Ls;

            if (_ItemsAtPos.TryGetValue(pos, out LineSegment? Out))
                Ls = Out;
            //else if (_BlocksByEnd.TryGetValue(pos - 2, out Block? BlkOut))
            //{
            //    bool Valid = BlkOut.BlockIndex + 1 < LineItems;
            //    IEnumerable<Item> Next = new Item[] { };
            //    Item? TheItem = null;

            //    if (Valid)
            //    {                
            //        Next = new Item[] { _Items[BlkOut.BlockIndex + 1] };
            //        TheItem = _Items[BlkOut.BlockIndex + 1];
            //    }

            //    ItemRange Before = new ItemRange(this.Where((w, wi) => wi < BlkOut.BlockIndex + 1));
            //    IEnumerable<Item> After = this.Where((w, wi) => wi >= BlkOut.BlockIndex + 1);
            //    HashSet<Block> RightBefore = new HashSet<Block>() { BlkOut };
            //    Ls = new LineSegment(Next, true, TheItem, BlkOut.BlockIndex + 1, untilDot, Before, After, RightBefore, new Item[] { });
            //}
            else
            {
                Ls = GetItemAtPosition(pos, untilDot);
                _ItemsAtPos.TryAdd(pos, Ls);
            }

            return Ls;
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
        /// <param name="position">The position of the line to start at</param>
        /// <param name="startItem">The item index to start from</param>
        /// <param name="endItem">The item index to end with</param>
        /// <returns>
        /// A boolean if the part is isolated.  A boolean if the dictionary is valid.  
        /// A dictionary of item index by block index
        /// </returns>
        private (bool, bool, IDictionary<int, int>) IsolatedPart(int position, int startItem, int endItem)
        {
            Logic.AddMethodCount("IsolatedPart");
            //2,3,4,5//--00--0---0---- isolated 2,3,4 - invalid as could be 3,4,5
            bool IsIsolated = true, Valid = false, NotSolid = false, RestIsolated = false;
            bool Green = false, AllOneColour = ItemsOneColour;
            int SolidBlockCount = 0, SolidCount = 0, CurrentItem = startItem, ReachIndex = 0, StartItem = startItem;
            Dictionary<int, bool> Isolations = new Dictionary<int, bool>();
            Dictionary<int, int> IsolatedItems = new Dictionary<int, int>();
            Dictionary<int, Block> BlockIndexes = new Dictionary<int, Block>();
            Dictionary<int, bool> CanJoin = new Dictionary<int, bool>();
            Dictionary<int, int> Pushes = new Dictionary<int, int>();

            for (int c = position; c < LineLength; c++)
            {
                var xy = IsRow ? (c, LineIndex) : (LineIndex, c);
                if (Logic.points.TryGetValue(xy, out Point? Pt))
                {
                    if (Pt.Green != Green)
                    {
                        if (SolidCount > 0)
                            SolidBlockCount++;

                        SolidCount = 0;
                        NotSolid = true;
                    }

                    if (BlockIndexes.ContainsKey(SolidBlockCount))
                        BlockIndexes[SolidBlockCount].EndIndex = c;
                    else
                        BlockIndexes.TryAdd(SolidBlockCount, new Block(c, c, Pt.Green));

                    if ((!Logic.points.TryGetValue(IsRow ? (c + 1, LineIndex) : (LineIndex, c + 1), out Point? Pt2)
                        || Pt2.Green != Pt.Green)
                        && SolidBlockCount > 0
                        && CurrentItem < endItem + 1)
                    {
                        int ReachIndexCurrent = BlockIndexes[SolidBlockCount - 1].StartIndex
                            + _Items[CurrentItem].Value - 1;
                        if (ReachIndexCurrent > ReachIndex)
                            ReachIndex = ReachIndexCurrent;
                        //previous block reach current
                        if (c <= ReachIndexCurrent)
                        {
                            Isolations.TryAdd(SolidBlockCount, false);
                            CanJoin.TryAdd(SolidBlockCount - 1, false);
                            CanJoin.TryAdd(SolidBlockCount, false);
                            Pushes.TryAdd(SolidBlockCount, CurrentItem + 1);
                            RestIsolated = true;
                            IsIsolated = false;
                        }
                        else if (CurrentItem == endItem)
                        {
                            //does not reach & no more items => change isolations
                            RestIsolated = false;
                            if (Isolations.ContainsKey(SolidBlockCount))
                                Isolations.Remove(SolidBlockCount);
                            if (Isolations.ContainsKey(SolidBlockCount - 2)
                                && !Pushes.ContainsKey(SolidBlockCount - 2))
                                Isolations.Remove(SolidBlockCount - 2);

                            // just one push
                            if (Pushes.Count == 1)
                            {
                                int Stop = Pushes.Keys.First();
                                bool Flag = false;
                                //start at previous block and current item minus 1 - current item is previous block
                                for (int d = SolidBlockCount - 1; d >= 0; d--)
                                {
                                    int ItemIndex = CurrentItem - (SolidBlockCount - d);

                                    if (BlockIndexes.TryGetValue(d, out Block? First)
                                        && BlockIndexes.TryGetValue(d + 1, out Block? Second))
                                    {
                                        if (First.StartIndex + _Items[ItemIndex].Value - 1 >= Second.EndIndex
                                            || Second.EndIndex - _Items[ItemIndex + 1].Value + 1 <= First.StartIndex)
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
                                                && Previous.EndIndex + _Items[ItemIndex].Value + 2 >= Second.StartIndex)
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
                                    for (int d = Stop; d < SolidBlockCount; d++)
                                    {
                                        Isolations.Remove(d);
                                    }
                                }
                            }
                        }

                        CurrentItem++;

                        //check solid count
                        if (CurrentItem < endItem + 1
                            && _Items[CurrentItem] < new Block(-1, Pt.Green) { SolidCount = SolidCount })
                        {
                            bool NoMoreItems = UniqueCount(new Block(-1, Pt.Green) { SolidCount = SolidCount }, out Item q);
                            bool Flag = NoMoreItems;
                            int StartIndex = -1;
                            int Stop = 0;

                            if (NoMoreItems)
                            {
                                StartIndex = FirstOrDefault(w => w >= new Block(-1, Pt.Green) { SolidCount = SolidCount })?.Index ?? -1;

                                for (int d = SolidBlockCount - 1; d >= 0; d--)
                                {
                                    int ItemIndex = StartIndex - (SolidBlockCount - d);

                                    if (ItemIndex == -1)
                                    {
                                        Flag = false;
                                        break;
                                    }

                                    if (BlockIndexes.TryGetValue(d, out Block? First)
                                        && BlockIndexes.TryGetValue(d + 1, out Block? Second))
                                    {
                                        if (First.StartIndex + _Items[ItemIndex].Value - 1 >= Second.EndIndex
                                            || Second.EndIndex - _Items[ItemIndex + 1].Value + 1 <= First.StartIndex)
                                        {
                                            Flag = false;
                                            break;
                                        }
                                        else if (Second.StartIndex - First.EndIndex - 1 >= (AllOneColour ? 3 : 0))
                                        {
                                            Stop = d + 1;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (Flag)
                            {
                                for (int d = Stop; d <= SolidBlockCount; d++)
                                {
                                    int ItemIndex = StartIndex - (SolidBlockCount - d);
                                    IsolatedItems.TryAdd(d, ItemIndex);
                                }
                            }
                            else if (CurrentItem == endItem && Pushes.Count == 1)
                            {
                                int Key = Pushes.Keys.First();
                                CanJoin[Key] = true;
                                CanJoin[Key - 1] = true;
                            }
                        }

                        //previous reach current && no more items
                        if (c <= ReachIndexCurrent && CurrentItem == endItem + 1)
                            CurrentItem--;

                        if (CurrentItem < endItem + 1)
                        {
                            int BackReach = (c - _Items[CurrentItem].Value) + 1;
                            //current block reach previous
                            if (BackReach <= BlockIndexes[SolidBlockCount - 1].StartIndex)
                            {
                                for (int d = SolidBlockCount - 1; d >= 0; d--)
                                {
                                    Isolations.TryAdd(d, false);
                                    if (BlockIndexes.TryGetValue(d, out Block? Value)
                                        && Value.EndIndex >= BackReach)
                                        CanJoin.TryAdd(d, false);
                                }

                                IsIsolated = false;
                            }
                            //else
                            //{
                            //    int Start = BlockIndexes[SolidBlockCount - 1].Item2 + 2;
                            //    HashSet<int> PossibleSolids = FindPossibleSolids(IsRow, lineLength, LineIndex, Start);
                            //    //does not reach and no space -> pull or push
                            //    if (!PossibleSolids.Contains(_Items[CurrentItem].Value))
                            //    {

                            //    }
                            //}
                        }
                    }

                    if (NotSolid && Valid)
                        Valid = false;

                    if (IsIsolated && SolidBlockCount > 0
                        && SolidCount > 0 && CurrentItem < endItem + 1
                        && _Items[CurrentItem].Value >= SolidCount
                        && UniqueCount(new Block(-1, Pt.Green) { SolidCount = SolidCount }, out Item z))
                    {
                        Valid = true;
                    }

                    if (NotSolid && c < ReachIndex)
                        CanJoin.TryAdd(SolidBlockCount, false);

                    if (NotSolid && SolidBlockCount > 1)
                    {
                        if (Pushes.TryGetValue(SolidBlockCount - 2, out int Item)
                            && Item < LineItems
                            && c < BlockIndexes[SolidBlockCount - 1].StartIndex
                            + _Items[Item].Value)
                            CanJoin.TryAdd(SolidBlockCount, false);
                    }

                    Green = Pt.Green;
                    NotSolid = false;
                    SolidCount++;

                    if (RestIsolated)
                        Isolations.TryAdd(SolidBlockCount, false);
                }
                else
                {
                    if (SolidCount > 0)
                    {
                        SolidBlockCount++;

                        //start - skip to correct solid count
                        if (SolidBlockCount == 1 && _Items[CurrentItem].Value < SolidCount)
                        {
                            CurrentItem = _Items.Select((w, wi) => (w, wi))
                                               .FirstOrDefault(w => w.w.Value >= SolidCount).wi;
                            StartItem = CurrentItem;
                            IsIsolated = false;
                        }
                    }

                    NotSolid = true;
                    SolidCount = 0;
                }
            }

            if (!NotSolid)
                SolidBlockCount++;

            //4 items, 5 blocks, 3 isolations => other 2 blocks join
            if (startItem == 0 && endItem == LineItems - 1 && false
                && SolidBlockCount >= (endItem + 1) - StartItem
                )
            {
                bool Default = SolidBlockCount > (endItem + 1) - StartItem && CanJoin.Count == 2;

                for (int Pos = 0; Pos < SolidBlockCount; Pos++)
                {
                    if (CanJoin.TryGetValue(Pos, out bool First) && (First || Default))
                    {
                        if (CanJoin.TryGetValue(Pos + 1, out bool Second) && (Second || Default))
                        {
                            Point.Group++;
                            int FirstIndex = BlockIndexes[Pos].EndIndex;
                            int SecondIndex = BlockIndexes[Pos + 1].StartIndex;
                            foreach (Point Point in Logic.AddPoints(this, FirstIndex, BlockIndexes[Pos].Green, GriddlerPath.Action.MustJoin, SecondIndex))
                                ;
                        }
                    }
                }
            }

            if (IsolatedItems.Count > 0)
            {
                Valid = true;
            }
            else
            {
                for (int Pos = 0; Pos <= SolidBlockCount; Pos++)
                {
                    if (!Isolations.ContainsKey(Pos))
                        IsolatedItems.TryAdd(Pos, Pos + StartItem);
                }
            }

            if (SolidBlockCount == (endItem + 1) - StartItem)
                Valid = true;
            else
                IsIsolated = false;

            return (IsIsolated, Valid, IsolatedItems);
        }
        /// <summary>
        /// Determines if the block index is equivalent to the item index
        /// <para>
        /// Uses: <see cref="IsolatedPart(int, int, int)"/>
        /// </para>
        /// </summary>
        /// <returns>
        /// A boolean if the part is isolated.  A boolean if the dictionary is valid.  
        /// A dictionary of item index by block index
        /// </returns>
        public (bool, bool, IDictionary<int, int>) IsLineIsolated()
            => GetIsolatedPart(0, 0, LineItems - 1);
        /// <summary>
        /// Determines if the block index is equivalent to the item index
        /// <para>
        /// Uses: <see cref="IsolatedPart(int, int, int)"/>
        /// </para>
        /// </summary>
        /// <returns>
        /// A boolean if the part is isolated.  A boolean if the dictionary is valid.  
        /// A dictionary of item index by block index
        /// </returns>
        public (bool, bool, IDictionary<int, int>) GetIsolatedPart(int pos, int startItem, int endItem)
        {
            (bool, bool, IDictionary<int, int>) ItemKey;

            if (_Isolations.TryGetValue((pos, startItem, endItem), out var Out))
                ItemKey = Out;
            //else if (pos == 0)
            //{
            //    bool IsolatedItemsValid = false;
            //    Dictionary<int, int> IsolatedItems = new Dictionary<int, int>();



            //    ItemKey = (false, IsolatedItemsValid, IsolatedItems);
            //}
            else
            {
                ItemKey = IsolatedPart(pos, startItem, endItem);
                _Isolations[(pos, startItem, endItem)] = ItemKey;
            }

            return ItemKey;
        }

        public void ClearCaches(int pos)
        {
            _Isolations.Clear();

            foreach (int Key in _ItemsAtPos.Keys.Where(w => w >= pos))
                _ItemsAtPos.Remove(Key);

            _ItemsAtPosB.Clear();

            //breaks Highlights20x20
            //foreach (int Key in _ItemsAtPosB.Keys.Where(w => w <= pos))
            //    _ItemsAtPosB.Remove(Key);
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
