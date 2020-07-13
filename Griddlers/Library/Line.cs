using Griddlers.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    public class Line : ItemRange, IEnumerable<Item>
    {
        private Item[] _Items => _ItemsArray;
        private readonly IDictionary<(int, bool), bool> _UniqueCounts;
        private readonly IDictionary<int, Block> _Blocks;
        private readonly IDictionary<int, Block> _BlocksByStart;
        private readonly IDictionary<int, Block> _BlocksByEnd;
        private readonly IDictionary<int, LineSegment> _ItemsAtPos;
        private readonly IDictionary<int, LineSegment> _ItemsAtPosB;
        private readonly IDictionary<(int, int, int), (bool, bool, IDictionary<int, int>)> _Isolations;
        private int? _LineValue;
        private int? _MinItem;
        private int? _MaxItem;

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
                    _LineValue = _Items.Sum(s => s.Value) + GetDotCount();

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

        public Block CurrentBlock => _Blocks.Values.Last();


        public Item this[int index]
        {
            get { return _Items[index]; }
            set { _Items[index] = value; }
        }


        public Line(int index, bool isRow, int lL, Item[] items) : base(items)
        {
            IsRow = isRow;
            LineLength = lL;
            LineIndex = index;
            _UniqueCounts = new Dictionary<(int, bool), bool>(items.Length);
            _Blocks = new Dictionary<int, Block>(items.Length);
            _BlocksByStart = new Dictionary<int, Block>(items.Length);
            _BlocksByEnd = new Dictionary<int, Block>(items.Length);
            _ItemsAtPos = new Dictionary<int, LineSegment>(lL);
            _ItemsAtPosB = new Dictionary<int, LineSegment>(lL);
            _Isolations = new Dictionary<(int, int, int), (bool, bool, IDictionary<int, int>)>();
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

            Logic.ForEachLinePos(this, (int i, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) =>
            {
                int Sum = 0, ItemShift = 0, GapMaxItem = 0;
                ItemAtLastGap = Item;
                LastGapIndex = i - gapSize - 1;

                if (wasSolid && !untilDot)
                    PossibleSolids = FindPossibleSolids(i - block.SolidCount);

                if (block.SolidCount == gapSize && Item < LineItems && _Items[Item].Value == block.SolidCount
                    && _Items[Item].Value > MaxItem)
                {
                    Equality = true;
                }

                if (untilDot && Item >= LineItems && block.SolidCount > 0 && block.SolidCount == gapSize)
                {
                    for (int c = LineItems - 1; c >= 0; c--)
                    {
                        if (_Items[c].Value >= block.SolidCount && _Items[c].Value <= gapSize)
                        {
                            Item = c;
                            break;
                        }
                    }
                }

                if (Item >= LineItems && block.SolidCount > 0)
                    Item = LineItems - 1;

                foreach (Item Itm in _Items.Skip(Item))
                {
                    Sum += Itm.Value;
                    var Pos = IsRow ? (i - gapSize + Sum, LineIndex) : (LineIndex, i - gapSize + Sum);

                    if (Logic.points.TryGetValue(Pos, out Point? Pt) && Pt.Green == Itm.Green)
                    {
                        Sum++;

                        if (wasSolid && Sum == gapSize
                            && Item + ItemShift >= 0
                            && block.SolidCount > _Items[Item + ItemShift].Value)
                            Item--;
                        else if ((untilDot || !PossibleSolids.Contains((_Items[Item + ItemShift].Value, false)))
                            && wasSolid && Sum > gapSize
                            && Item + ItemShift - 1 >= 0
                            && block.SolidCount > _Items[Item + ItemShift - 1].Value)
                            Item--;
                    }

                    if (Sum <= gapSize)
                    {
                        ItemShift++;

                        if (Itm.Value > GapMaxItem)
                            GapMaxItem = Itm.Value;
                    }
                    else
                        break;

                    Sum += GetDotCount(Itm.Index);
                }

                if (solidCount < gapSize && (ItemShift > 1 || (ItemShift == 1 && solidCount == 0)))
                    Equality = false;

                if (solidCount < gapSize)
                    PartEquality = false;

                if (untilDot && block.SolidCount == gapSize && Item < LineItems)
                {
                    int UniqueCount = 0, TempItem = 0;

                    foreach (Item Itm in _Items.Skip(LastItemAtEquality + 1).Take(Item - LastItemAtEquality))
                    {
                        if (block == Itm)
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

                if (solidCount == gapSize)
                    PartEquality = true;

                if (Equality && i < LineLength - 1 && i < position - 1)
                    LastItemAtEquality = Item; //????

                if (Equality && i < LineLength - 1 && i < position - 1)
                    ItemAtEquality = Item + ItemShift;

                if (GapMaxItem > MaxItem)
                    MaxItem = GapMaxItem;

                if (solidCount == gapSize)
                    LastFullGapCount = solidCount;

                LastBlock = block;
                if (solidCount < gapSize)
                    ScValid = false;
                else
                    ScValid = true;

                Item += ItemShift;
                return 0;
            }, (ns, sC, i, xy, gS) => gS > 0 && (Logic.dots.ContainsKey(xy) || i == LineLength - 1 || (!untilDot && i == position - 1)), (i) => i == position);

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
            IEnumerable<Item> Next = this.Where((w, wi) => wi >= ItemAtEquality && ((!Equality && wi <= Item) || wi == Item));
            ItemRange Before = new ItemRange(this.Where((w, wi) => wi >= ItemAtEquality && wi < Item));
            IEnumerable<Item> After = this.Where((w, wi) => wi >= Item);
            //HashSet<Item> RightBefore = FindPossibleSolids(IsRow, LineLength, lineIndex, 1);
            HashSet<Block> RightBefore = new Block[] { LastBlock }.ToHashSet();
            ItemRange Gap = new ItemRange(this.Where((w, wi) => wi >= ItemAtLastGap && wi < Item));

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

            Logic.ForEachLinePos(this, (int i, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) =>
            {
                if (Logic.points.TryGetValue(xy, out Point? Pt))
                {
                    if (!WasSolid || PrevColour != Pt.Green)
                    {
                        foreach (int PosSolid in PossibleSolids.Keys)
                            PossibleSolids[PosSolid].Remove((i - PosSolid, Pt.Green));

                        if (!AfterFirstSolid)
                            PossibleSolids.Add(i, new Dictionary<(int, bool), int>(50));

                        if (!WasSolid)
                            PrevColour = Pt.Green;
                    }
                }
                else
                {
                    if (!WasSolid && !AfterFirstSolid)
                        PossibleSolids.Add(i, new Dictionary<(int, bool), int>(50));
                    else if (WasSolid && onlyFirstSolid)
                        AfterFirstSolid = true;
                }

                if (!Logic.points.TryGetValue(xy, out Pt) || PrevColour != Pt.Green || i == LineLength - 1)
                {
                    foreach (int PosSolid in PossibleSolids.Keys)
                    {
                        int ConsumeCount = 0;
                        bool ConsumeColour = false;
                        bool Valid = true, NextValid = true;
                        for (int Pos = PosSolid; Pos <= i; Pos++)
                        {
                            var Xy = IsRow ? (Pos, LineIndex) : (LineIndex, Pos);
                            if (Logic.points.TryGetValue(Xy, out Point? Pt2))
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
                            PossibleSolids[PosSolid].TryAdd((i - PosSolid, ConsumeColour), ConsumeCount);

                        if (Valid && NextValid)
                            PossibleSolids[PosSolid].TryAdd((i - PosSolid + 1, ConsumeColour), ConsumeCount);

                        if (ConsumeCount == 0)
                        {
                            if (WasSolid)
                                PossibleSolids[PosSolid].TryAdd((i - PosSolid, !ConsumeColour), ConsumeCount);

                            PossibleSolids[PosSolid].TryAdd((i - PosSolid + 1, !ConsumeColour), ConsumeCount);
                        }
                    }
                }

                if (Logic.points.TryGetValue(xy, out Pt))
                {
                    WasSolid = true;
                    PrevColour = Pt.Green;
                }
                else
                    WasSolid = false;

                if (Logic.dots.ContainsKey(xy))
                    return LineLength;

                return 0;
            }, start: start);

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
        private HashSet<(int, bool)> FindPossibleSolids(int start, bool onlyFirstSolid = true)
        {
            Dictionary<int, Dictionary<(int, bool), int>> PossibleSolids = FindPossibleSolidsI(start, onlyFirstSolid);
            HashSet<(int, bool)> Return = new HashSet<(int, bool)>();

            foreach (Dictionary<(int, bool), int> Item in PossibleSolids.Values)
                Return.UnionWith(Item.Keys);

            return Return;
        }
        /// <summary>
        /// Determines if the items from start are the only ones that consume the blocks
        /// until the next dot
        /// <para>Uses: <see cref="FindPossibleSolidsI(int, bool)"/></para>
        /// </summary>
        /// <param name="start">The position in the line to start</param>
        /// <param name="items">The items to use from start to dot</param>
        /// <param name="newItem">The first item index</param>
        /// <returns>
        /// A boolean if the items consume the blocks.
        /// </returns>
        private bool FindPossibleSolids(int start, IEnumerable<Item> items, out int newItem)
        {
            Dictionary<int, Dictionary<(int, bool), int>> PossibleSolids = FindPossibleSolidsI(start, false);
            int SolidCount = 0;
            bool Consume = true;
            newItem = items.First().Index;

            for (int Pos = start; Pos < LineLength; Pos++)
            {
                var Xy = IsRow ? (Pos, LineIndex) : (LineIndex, Pos);

                if (Logic.points.ContainsKey(Xy))
                    SolidCount++;

                if (Logic.dots.ContainsKey(Xy) || Pos == LineLength - 1)
                    break;
            }

            while (!FitsThese(newItem, items) && newItem < LineItems)
                newItem++;

            if (!FitsThese(newItem, items, true))
                return false;

            int NewItem = newItem;
            foreach (Item Item in items.Where(w => w.Index > NewItem))
            {
                if (FitsThese(Item.Index, items, true))
                {
                    Consume = false;
                    break;
                }
            }

            bool FitsThese(int ItemStart, IEnumerable<Item> itms, bool consume = false)
            {
                int PossiblePos = start, ItemCount = 0, PossibleItems = 0;
                int ConsumeCount = 0;
                foreach (Item Item in itms.Where(w => w.Index >= ItemStart))
                {
                    ItemCount++;
                    foreach (KeyValuePair<int, Dictionary<(int, bool), int>> Items in PossibleSolids)
                    {
                        if (Items.Key >= PossiblePos
                            && Items.Value.TryGetValue((Item.Value, Item.Green), out int Consumes)
                            && (!consume || Consumes > 0))
                        {
                            PossiblePos = Items.Key + Item.Value + GetDotCount(Item.Index);
                            PossibleItems++;
                            ConsumeCount += Consumes;
                            break;
                        }
                    }
                }

                return PossibleItems == ItemCount && (!consume || ConsumeCount == SolidCount);
            }

            return Consume;
        }

        /// <summary>
        /// Gets the possible items at a position along the line working backwards
        /// </summary>
        /// <param name="position">The position of the line to start at</param>
        /// <param name="untilDot">Position is on a dot</param>
        /// <returns></returns>
        private LineSegment GetItemAtPositionB(int position, bool untilDot = true)
        {
            Logic.AddMethodCount("GetItemAtPositionB");
            int Item = LineItems - 1, SolidCount = 0, LastItemAtEquality = LineItems, ItemAtEquality = LineItems, ItemAtLastGap = LineItems;
            bool Valid = true, Equality = true, ScValid = true, PartEquality = false;
            int LastFullGapCount = 0, LastGapIndex = LineLength;
            HashSet<(int, bool)> PossibleSolids = new HashSet<(int, bool)>();

            Logic.ForEachLinePos(this, (int i, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) =>
            {
                int Sum = 0, ItemShift = 0, IsPoint = Logic.points.ContainsKey(xy) ? 1 : 0;
                ItemAtLastGap = Item;
                LastGapIndex = i + gapSize + 1;
                gapSize += IsPoint;

                if (gapSize == 0)
                    return 0;

                if (Item < 0 && block.SolidCount > 0 && block.SolidCount == gapSize)
                {
                    Item FirstItem = _Items.FirstOrDefault(f => f.Value >= block.SolidCount && f.Value <= gapSize);
                    Item = FirstItem != (object?)null ? FirstItem.Index : -1;

                    if (Item < LineItems - 1
                        && !PossibleSolids.Contains((_Items[Item + 1].Value, false))
                        && PossibleSolids.Count > 0
                        && PossibleSolids.Max(m => m.Item1) >= _Items[Item + 1].Value)
                    {
                        Item++;
                    }
                }

                if (wasSolid && !untilDot)
                    PossibleSolids = FindPossibleSolids(i - block.SolidCount);

                for (int c = Item; c >= 0; c--)
                {
                    Sum += _Items[c].Value;
                    var Pos = IsRow ? (i + gapSize - IsPoint - Sum, LineIndex) : (LineIndex, i + gapSize - IsPoint - Sum);

                    if (Logic.points.TryGetValue(Pos, out Point? Pt) && Pt.Green == _Items[c].Green)
                    {
                        Sum++;

                        if (wasSolid && Pt.Green == _Items[c].Green
                            && Sum <= gapSize
                            && i + gapSize - IsPoint - Sum <= block.EndIndex
                            && Item - ItemShift <= LineItems - 1
                            && block.SolidCount > _Items[Item - ItemShift].Value)
                            Item++;
                        else if ((untilDot || (!PossibleSolids.Contains((_Items[Item - ItemShift].Value, false)) && false))
                            && wasSolid && Sum > gapSize && Pt.Green == _Items[c].Green
                            && Item - (ItemShift - 1) <= LineItems - 1
                            && block.SolidCount > _Items[Item - (ItemShift - 1)].Value)
                            Item++;
                    }

                    if (Sum <= gapSize)
                        ItemShift++;
                    else
                        break;

                    Sum += GetDotCountB(c);
                }

                //TO FIX
                if (solidCount < gapSize && (ItemShift != 1 || (ItemShift == 1 && solidCount == 0)))
                {
                    Equality = false;
                }

                if (solidCount < gapSize)
                    PartEquality = false;

                //unique colour item
                if (untilDot && !Equality && wasSolid && Item - ItemShift < LineItems - 1)
                {
                    //ItemRange R = new ItemRange(_Items.Where(w => w.Index > Item + 1));
                    IEnumerable<Item> R2 = _Items.Where(w => w.Index > Item - ItemShift && w.Index <= LastItemAtEquality
                                        && w >= block);
                    if (R2.Count() == 1)
                    {
                        Item = R2.First().Index;
                        ItemShift = 1;
                        Equality = true;
                    }
                }

                //mutliple possible solids
                IEnumerable<Item> R = _Items.Where(w => w.Index > Item - ItemShift && w.Index <= ItemAtEquality);
                if (untilDot && !Equality && ItemAtLastGap == ItemAtEquality
                    && R.Any() && FindPossibleSolids(i + 1, R, out int NewItem))
                {
                    ItemShift = 1;
                    Item = NewItem;
                    Equality = true;
                }

                if (untilDot && block.SolidCount == gapSize && Item < LineItems)
                {
                    int UniqueCount = 0, TempItem = 0;

                    for (int Pos = LastItemAtEquality - 1; Pos >= Item; Pos--)
                    {
                        if (block == _Items[Pos])
                        {
                            if (UniqueCount > 0 && Pos < TempItem - 1)
                            {
                                UniqueCount = 0;
                                break;
                            }

                            TempItem = Pos;
                            UniqueCount++;
                        }
                    }

                    if (UniqueCount == 1
                        || (UniqueCount == 2 && PartEquality
                        && _Items[TempItem + 1].Value == LastFullGapCount))
                    {
                        Item = TempItem;
                        ItemShift = 1;
                        Equality = true;
                    }
                }

                if (solidCount == gapSize)
                    PartEquality = true;

                if (Equality)
                    LastItemAtEquality = Item;

                if (Equality && i > 0 && i > position + 1)
                    ItemAtEquality = Item - ItemShift;

                if (solidCount == gapSize)
                    LastFullGapCount = solidCount;

                SolidCount = block.SolidCount;
                if (block.SolidCount < gapSize)
                    ScValid = false;
                else
                    ScValid = true;

                if (Logic.dots.ContainsKey(xy))
                {
                    ItemAtLastGap = Item - ItemShift;
                    LastGapIndex = i;
                }

                Item -= ItemShift;
                PossibleSolids = FindPossibleSolids(i + 1);
                return 0;
            }, (nS, sC, i, xy, gS) => (Logic.dots.ContainsKey(xy) || i == position + 1), (i) => i == position, true);

            if (Item < 0)
                Valid = false;

            if (!untilDot)
                Equality = false;

            Item? TheItem = null;

            if (Valid)
                TheItem = _Items[Item];

            IEnumerable<Item> Next = this.Where((w, wi) => ((!Equality && wi >= Item) || wi == Item) && wi <= ItemAtEquality);
            ItemRange Before = new ItemRange(this.Where((w, wi) => wi > Item && wi <= ItemAtEquality));
            IEnumerable<Item> After = this.Where((w, wi) => wi <= Item);
            //HashSet<Item> RightBefore = FindPossibleSolids(IsRow, LineLength, lineIndex, 1);
            HashSet<Block> RightBefore = new Block[] { new Block(ScValid, SolidCount) }.ToHashSet();
            ItemRange Gap = new ItemRange(this.Where((w, wi) => wi > Item && wi <= ItemAtLastGap));

            return new LineSegment(Next, false, TheItem, Equality, Before, After, RightBefore, Gap, ItemAtLastGap, ItemAtEquality);
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

        public bool UniqueCount(Block block)
        {
            bool Retval = false;

            if (_UniqueCounts.TryGetValue((block.SolidCount, block.Green), out bool Out))
                Retval = Out;
            else
            {
                Retval = this.Count(c => c >= block) == 1;
                _UniqueCounts[(block.SolidCount, block.Green)] = Retval;
            }

            return Retval;
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
            bool Start = index == 0 || _Items[index].Green == _Items[index - 1].Green;
            bool End = index == _Items.Length - 1 || _Items[index].Green == _ItemsArray[index + 1].Green;
            return (Start, End);
        }

        public int GetDotCount(int pos)
        {
            return ShouldAddDots(pos).Item2 ? 1 : 0;
        }

        public int GetDotCountB(int pos)
        {
            return ShouldAddDots(pos).Item1 ? 1 : 0;
        }

        public bool IsEq(ItemRange u, int index, int start, int end)
            => IsEq(index, start, end, u.Sum());
        public bool IsEq(int index, int start, int end, int sum)
        {
            return end - start - sum <= GetDotCount(index);
            //return end - dotCount <= start + sum;
        }

        public bool IsEqB(ItemRange u, int index, int start, int end)
            => IsEqB(index, start, end, u.Sum());
        public bool IsEqB(int index, int start, int end, int sum)
        {
            return end - start - sum <= GetDotCountB(index);
            //return end - dotCount <= start + sum;
        }

        public (int, int, bool) FindGapStartEnd(int pos, int? pos2 = null)
        {
            int GapStart = -1, GapEnd = LineLength;
            bool WasSolid = false;

            for (int Pos = pos; Pos >= 0; Pos--)
            {
                var Xy = IsRow ? (Pos, LineIndex) : (LineIndex, Pos);
                if (Logic.dots.ContainsKey(Xy))
                {
                    GapStart = Pos;
                    break;
                }
            }
            for (int Pos = pos2.GetValueOrDefault(pos); Pos < LineLength; Pos++)
            {
                var Xy = IsRow ? (Pos, LineIndex) : (LineIndex, Pos);
                if (Logic.dots.ContainsKey(Xy))
                {
                    GapEnd = Pos;
                    break;
                }
                else if (Logic.points.ContainsKey(Xy))
                    WasSolid = true;
            }

            return (GapStart, GapEnd, WasSolid);
        }

        public LineSegment GetItemAtPos(int pos, bool untilDot = true)
        {
            LineSegment Ls;

            //if (_ItemsAtPos.TryGetValue(pos, out LineSegment? Out))
            //    Ls = Out;
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
            //else
            //{
            Ls = GetItemAtPosition(pos, untilDot);
            _ItemsAtPos.TryAdd(pos, Ls);
            //}

            return Ls;
        }

        public LineSegment GetItemAtPosB(int pos, bool untilDot = true)
        {
            LineSegment Ls;

            //if (_ItemsAtPosB.TryGetValue(pos, out LineSegment? Out))
            //    Ls = Out;
            //else if (untilDot && _BlocksByStart.TryGetValue(pos + 2, out Block? BlkOut))
            //{
            //    bool Valid = BlkOut.BlockIndex - 1 >= 0;
            //    IEnumerable<Item> Next = new Item[] { };
            //    Item? TheItem = null;

            //    if (Valid)
            //    {
            //        Next = new Item[] { _Items[BlkOut.BlockIndex - 1] };
            //        TheItem = _Items[BlkOut.BlockIndex - 1];
            //    }

            //    ItemRange Before = new ItemRange(this.Where((w, wi) => wi > BlkOut.BlockIndex - 1));
            //    IEnumerable<Item> After = this.Where((w, wi) => wi <= BlkOut.BlockIndex - 1);
            //    HashSet<Block> RightBefore = new HashSet<Block>() { BlkOut };
            //    Ls = new LineSegment(Next, false, TheItem, BlkOut.BlockIndex - 1, true, Before, After, RightBefore);
            //}

            //    Ls = (BlkOut.BlockIndex - 1, BlkOut.BlockIndex - 1 >= 0, true, BlkOut.SolidCount, true);
            //else
            //{
            Ls = GetItemAtPositionB(pos, untilDot);
            _ItemsAtPosB.TryAdd(pos, Ls);
            //}

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

                    //if (BlockIndexes.ContainsKey(SolidBlockCount))
                    //    BlockIndexes[SolidBlockCount] = (BlockIndexes[SolidBlockCount].Item1, c);
                    //else
                    //    BlockIndexes.TryAdd(SolidBlockCount, (c, c));

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

                        bool NoMoreItems = UniqueCount(new Block(-1, Pt.Green) { SolidCount = SolidCount });

                        //check solid count
                        if (NoMoreItems && CurrentItem < endItem + 1
                            && _Items[CurrentItem] < new Block(-1, Pt.Green) { SolidCount = SolidCount })
                        {
                            bool Flag = true;
                            int StartIndex = -1;
                            int Stop = 0;

                            StartIndex = this.FirstOrDefault(w => w >= new Block(-1, Pt.Green) { SolidCount = SolidCount }).Index;

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

                            if (Flag)
                            {
                                for (int d = Stop; d <= SolidBlockCount; d++)
                                {
                                    int ItemIndex = StartIndex - (SolidBlockCount - d);
                                    IsolatedItems.TryAdd(d, ItemIndex);
                                }
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
                        && UniqueCount(new Block(-1, Pt.Green) { SolidCount = SolidCount }))
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
                            CurrentItem = this.Select((w, wi) => (w, wi))
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
            if (startItem == 0 && endItem == LineItems - 1
                && SolidBlockCount > (endItem + 1) - StartItem)
            {
                for (int Pos = 0; Pos < SolidBlockCount; Pos++)
                {
                    if (CanJoin.TryGetValue(Pos, out bool First) && (First || CanJoin.Count == 2))
                    {
                        if (CanJoin.TryGetValue(Pos + 1, out bool Second) && (Second || CanJoin.Count == 2))
                        {
                            Point.Group++;
                            int FirstIndex = BlockIndexes[Pos].EndIndex;
                            int SecondIndex = BlockIndexes[Pos + 1].StartIndex;
                            Logic.AddPoints(this, FirstIndex, BlockIndexes[Pos].Green, GriddlerPath.Action.MustJoin, SecondIndex);
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

        /// <summary>
        /// Finds the minimum and maximum items at each position along the line
        /// </summary>
        /// <returns>A dictionary of the minimum/maximum items by position</returns>
        public IDictionary<int, (Item, Item)> GetMinMaxItems()
        {
            Logic.AddMethodCount("GetLineMinMaxItems2");
            Dictionary<int, (Item, Item)> MinMaxItems = new Dictionary<int, (Item, Item)>();
            int GapIndex = -1, EndOfGap = -1;
            bool WasGap = false;

            Logic.ForEachLinePos(this, (int i, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) =>
            {
                if (i >= EndOfGap)
                    EndOfGap = FindGapStartEnd(i).Item2;

                LineSegment Ls = GetItemAtPos(i + 1, false);
                LineSegment LsEnd = GetItemAtPosB(i - 1, false);

                if (Ls.Valid || LsEnd.Valid || (Ls.ItemAtStartOfGap == Ls.LastItemAtEquality))
                {
                    Item Min = new Item(this.Max(m => m.Value), false), Max = new Item(0, false);
                    int Start = 0, End = LineItems - 1;
                    bool WholeGap = false;

                    if (Ls.Valid)
                        End = Ls.Index;

                    if (Ls.Valid && Ls.Gap.Any() && IsEq(Ls.Gap, Ls.Index - 1, GapIndex, i - 1))
                        End--;

                    if (LsEnd.Before.Any() && End > LsEnd.Before.LastItemIndex)
                        End = LsEnd.Before.LastItemIndex;

                    if (LsEnd.Valid)
                        Start = LsEnd.Index;

                    //2,1,2//--00---.
                    if (LsEnd.Valid && Start < LineItems - 1 && LsEnd.Gap.Any()
                            && IsEqB(LsEnd.Gap, LsEnd.Index + 1, i, EndOfGap + 1))
                        Start++;

                    if (Start > 0 && Start >= End
                    //&& _Items[Start].Value == 1 
                    && _Items[Start].Green != block.Green)
                        Start--;

                    if (End > Start)
                    {
                        for (int Pos = End; Pos >= Start;)
                        {
                            if (block.Green != _Items[Pos].Green)
                                End--;
                            //else
                            break;
                        }
                    }


                    //colour check TEMP - should be in find possible solids
                    if (End > Start && End > 0 && End < LineItems - 1 && !Logic.points.ContainsKey(xy))
                    {
                        //for (int Pos = End + 1; Pos >= 1; Pos--)
                        //{
                        for (int LPos = i + GetDotCount(End); LPos <= i + _Items[End + 1].Value - 1 + GetDotCount(End); LPos++)
                        {
                            var PosXY = IsRow ? (LPos, LineIndex) : (LineIndex, LPos);
                            if (Logic.points.TryGetValue(PosXY, out Point? Pt)
                                && Pt.Green != _Items[End + 1].Green)
                            {
                                End--;
                                break;
                            }
                        }
                        //}
                    }

                    //TEMP - colour check
                    if (Start < End)
                    {
                        for (int Pos = Start; Pos <= End;)
                        {
                            if (block.Green != _Items[Pos].Green)
                                Start++;
                            //else
                            break;
                        }
                    }


                    if (End > 0)
                    {
                        for (int Pos = i; Pos < LineLength; Pos++)
                        {
                            xy = IsRow ? (Pos, LineIndex) : (LineIndex, Pos);
                            if (Logic.dots.ContainsKey(xy))
                            {
                                if (Pos - GapIndex - 1 < _Items[End].Value)
                                    End--;

                                break;
                            }
                        }
                        xy = IsRow ? (i, LineIndex) : (LineIndex, i);
                    }

                    //pull out end
                    if (wasSolid && End > 0)
                    {
                        (bool IsolatedEnd, _, _) = GetIsolatedPart(i - block.SolidCount, End - 1, LineItems - 1);

                        if (IsolatedEnd)
                            End--;
                    }

                    //pull out start
                    if (wasSolid && Start < LineItems - 1 && false)
                    {
                        (bool IsolatedEnd, _, _) = GetIsolatedPart(0, 0, Start + 1);

                        if (IsolatedEnd)
                            Start++;
                    }

                    if (Start == 0 && Ls.Sc > 0)
                    {
                        bool NotSolid = false;
                        for (int Pos = i - 1; Pos >= 0; Pos--)
                        {
                            xy = IsRow ? (Pos, LineIndex) : (LineIndex, Pos);

                            if (!Logic.points.ContainsKey(xy))
                                NotSolid = true;
                            else if (NotSolid)
                            {
                                if (Pos + _Items[Start].Value - 1 < i - 1)
                                    Start++;

                                break;
                            }
                        }
                        xy = IsRow ? (i, LineIndex) : (LineIndex, i);
                    }

                    if (Start > End)
                    {
                        //int Temp = Start;
                        Start = Ls.Index;//End;
                        End = LsEnd.Index; //Temp;
                    }

                    //TEMP
                    if (_Items.Skip(Start).Take(End - Start + 1).All(a => a.Value < block.SolidCount))
                    {
                        Min = new Item(0, false);
                        Max = new Item(this.Max(m => m.Value), false);
                    }
                    else if (Start <= End)
                        (Min, Max) = FindMinMax(Start, End);

                    MinMaxItems.TryAdd(i, (Min, Max));
                    WholeGap = true;

                    if (Ls.ItemAtStartOfGap == Ls.LastItemAtEquality)
                        Start = Ls.ItemAtStartOfGap;
                    else
                        WholeGap = false;

                    if ((Logic.dots.ContainsKey(xy) || i == LineLength - 1) && LsEnd.Eq)
                        End = LsEnd.Index;
                    else
                        WholeGap = false;

                    (Item, Item) FindMinMax(int s, int e)
                    {
                        Item Min = new Item(this.Max(m => m.Value), false), Max = new Item(0, false);

                        foreach (Item Item in _Items.Skip(s).Take(e - s + 1))
                        {
                            if (Item.Value > Max.Value)
                                Max = Item;

                            if (Item.Value < Min.Value && (WholeGap || !wasSolid || Item.Value >= block.SolidCount))
                                Min = Item;
                        }

                        return (Min, Max);
                    };

                    if (WholeGap)
                    {
                        (Min, Max) = FindMinMax(Start, End);

                        for (int Pos = GapIndex + 1; Pos <= i; Pos++)
                        {
                            if (MinMaxItems.TryGetValue(Pos, out (Item, Item) Val)
                                && (Val.Item1.Value < Min.Value || Val.Item2.Value > Max.Value))
                                MinMaxItems[Pos] = (Min, Max);

                            MinMaxItems.TryAdd(Pos, (Min, Max));
                        }
                    }

                }

                if (!Logic.dots.ContainsKey(xy))
                {
                    if (!WasGap)
                        GapIndex = i - 1;

                    WasGap = true;
                }
                else if (Logic.dots.ContainsKey(xy))
                    WasGap = false;

                if (!Logic.points.ContainsKey(xy) && wasSolid && block.SolidCount > this.Min(m => m.Value))
                {
                    Item[] ItemsGTSolid = this.Where(w => w.Value >= block.SolidCount).ToArray();

                    if (ItemsGTSolid.Length > 0)
                    {
                        if (MinMaxItems.TryGetValue(i, out (Item, Item) Val)
                            && Val.Item1.Value < ItemsGTSolid.Min(m => m.Value))
                            MinMaxItems[i] = (new Item(ItemsGTSolid.Min(m => m.Value), false), Val.Item2);

                        MinMaxItems.TryAdd(i, (new Item(ItemsGTSolid.Min(m => m.Value), false), new Item(this.Max(m => m.Value), false)));
                    }

                }

                return 0;
            });

            return MinMaxItems;
        }

        public void ClearCaches(int pos)
        {
            _Isolations.Clear();

            foreach (int Key in _ItemsAtPos.Keys.Where(w => w >= pos))
                _ItemsAtPos.Remove(Key);

            foreach (int Key in _ItemsAtPosB.Keys.Where(w => w <= pos))
                _ItemsAtPosB.Remove(Key);
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
