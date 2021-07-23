using Griddlers.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    public class Line : ItemRange, IEnumerable<Item>
    {
        //TEMP - until points/dots are in this class
        private Logic Logic;

        private Item[] _Items => _ItemsArray;
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
            Block LastBlock = new Block(0, "black");
            HashSet<(int, string)> PossibleSolids = new HashSet<(int, string)>();

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

                    if (Logic.points.TryGetValue(Pos2, out Point? Pt) && Pt.Colour == Itm.Colour)
                    {
                        Sum++;

                        if (WasSolid && Sum == GapSize
                            && Item + ItemShift >= 0
                            && Block.SolidCount > _Items[Item + ItemShift].Value)
                            Item--;
                        else if ((untilDot || !PossibleSolids.Contains((_Items[Item + ItemShift].Value, "black")))
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
        private Dictionary<int, Dictionary<(int, string), int>> FindPossibleSolidsI(int start, bool onlyFirstSolid = true)
        {
            Logic.AddMethodCount("FindPossibleSolids");
            Dictionary<int, Dictionary<(int, string), int>> PossibleSolids = new Dictionary<int, Dictionary<(int, string), int>>(50);
            bool WasSolid = false, AfterFirstSolid = false;
            string PrevColour = "black";

            foreach ((int Pos, (int, int) Xy, _, _, _, _) in Logic.ForEachLinePos(this, start: start))
            {
                if (Logic.points.TryGetValue(Xy, out Point? Pt))
                {
                    if (!WasSolid || PrevColour != Pt.Colour)
                    {
                        foreach (int PosSolid in PossibleSolids.Keys)
                            PossibleSolids[PosSolid].Remove((Pos - PosSolid, Pt.Colour));

                        if (!AfterFirstSolid)
                            PossibleSolids.Add(Pos, new Dictionary<(int, string), int>(50));

                        if (!WasSolid)
                            PrevColour = Pt.Colour;
                    }
                }
                else
                {
                    if (!WasSolid && !AfterFirstSolid)
                        PossibleSolids.Add(Pos, new Dictionary<(int, string), int>(50));
                    else if (WasSolid && onlyFirstSolid)
                        AfterFirstSolid = true;
                }

                if (!Logic.points.TryGetValue(Xy, out Pt) || PrevColour != Pt.Colour || Pos == LineLength - 1)
                {
                    foreach (int PosSolid in PossibleSolids.Keys)
                    {
                        int ConsumeCount = 0;
                        string ConsumeColour = "black";
                        bool Valid = true, NextValid = true;
                        for (int Pos2 = PosSolid; Pos2 <= Pos; Pos2++)
                        {
                            var PosXy = IsRow ? (Pos2, LineIndex) : (LineIndex, Pos2);
                            if (Logic.points.TryGetValue(PosXy, out Point? Pt2))
                            {
                                if (ConsumeCount > 0 && Pt2.Colour != ConsumeColour)
                                {
                                    Valid = false;
                                    break;
                                }

                                ConsumeCount++;
                                ConsumeColour = Pt2.Colour;

                                if (Pt != (object?)null && Pt2.Colour != Pt.Colour)
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
                                PossibleSolids[PosSolid].TryAdd((Pos - PosSolid, ConsumeColour == "black" ? "lightgreen" : "black"), ConsumeCount);

                            PossibleSolids[PosSolid].TryAdd((Pos - PosSolid + 1, ConsumeColour == "black" ? "lightgreen" : "black"), ConsumeCount);
                        }
                    }
                }

                if (Logic.points.TryGetValue(Xy, out Pt))
                {
                    WasSolid = true;
                    PrevColour = Pt.Colour;
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
        public HashSet<(int, string)> FindPossibleSolids(int start, bool onlyFirstSolid = true)
        {
            Dictionary<int, Dictionary<(int, string), int>> PossibleSolids = FindPossibleSolidsI(start, onlyFirstSolid);
            HashSet<(int, string)> Return = new HashSet<(int, string)>();

            foreach (Dictionary<(int, string), int> Item in PossibleSolids.Values)
                Return.UnionWith(Item.Keys);

            return Return;
        }
        /// <summary>
        /// Determines if the items from start are the only ones that consume the blocks
        /// until the next dot
        /// <para>Uses: <see cref="FindPossibleSolidsI(int, string)"/></para>
        /// </summary>
        /// <param name="start">The position in the line to start</param>
        /// <param name="items">The items to use from start to dot</param>
        /// <param name="newItem">The first item index</param>
        /// <returns>
        /// A boolean if the items consume the blocks.
        /// </returns>
        private bool FindPossibleSolids(int start, IEnumerable<Item> items, out int newItem)
        {
            Dictionary<int, Dictionary<(int, string), int>> PossibleSolids = FindPossibleSolidsI(start, false);
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
                    foreach (KeyValuePair<int, Dictionary<(int, string), int>> Items in PossibleSolids)
                    {
                        if (Items.Key >= PossiblePos
                            && Items.Value.TryGetValue((Item.Value, Item.Colour), out int Consumes)
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
            HashSet<(int, string)> PossibleSolids = new HashSet<(int, string)>();

            foreach ((int Pos, (int, int) Xy, Block Block, bool WasSolid, int GapSize, int Sc) in Logic.ForEachLinePos(this, (nS, sC, i, xy, gS) => (Logic.dots.ContainsKey(xy) || i == position + 1), (i) => i == position, true))
            {
                int Sum = 0, ItemShift = 0, IsPoint = Logic.points.ContainsKey(Xy) ? 1 : 0;
                int GapSizeCopy = GapSize;
                ItemAtLastGap = Item;
                LastGapIndex = Pos + GapSizeCopy + 1;
                GapSizeCopy += IsPoint;

                if (GapSizeCopy > 0)
                {

                    if (Item < 0 && Block.SolidCount > 0 && Block.SolidCount == GapSizeCopy)
                    {
                        Item? FirstItem = _Items.FirstOrDefault(f => f.Value >= Block.SolidCount && f.Value <= GapSizeCopy);
                        Item = FirstItem.HasValue ? FirstItem.Value.Index : -1;

                        if (Item < LineItems - 1
                            && !PossibleSolids.Contains((_Items[Item + 1].Value, "black"))
                            && PossibleSolids.Count > 0
                            && PossibleSolids.Max(m => m.Item1) >= _Items[Item + 1].Value)
                        {
                            Item++;
                        }
                    }

                    if (WasSolid && !untilDot)
                        PossibleSolids = FindPossibleSolids(Pos - Block.SolidCount);

                    for (int c = Item; c >= 0; c--)
                    {
                        Sum += _Items[c].Value;
                        var PosXy = IsRow ? (Pos + GapSizeCopy - IsPoint - Sum, LineIndex) : (LineIndex, Pos + GapSizeCopy - IsPoint - Sum);

                        if (Logic.points.TryGetValue(PosXy, out Point? Pt) && Pt.Colour == _Items[c].Colour)
                        {
                            Sum++;

                            if (WasSolid && Pt.Colour == _Items[c].Colour
                                && Sum <= GapSizeCopy
                                && Pos + GapSizeCopy - IsPoint - Sum <= Block.EndIndex
                                && Item - ItemShift <= LineItems - 1
                                && Block.SolidCount > _Items[Item - ItemShift].Value)
                                Item++;
                            else if ((untilDot || (!PossibleSolids.Contains((_Items[Item - ItemShift].Value, "black")) && false))
                                && WasSolid && Sum > GapSizeCopy && Pt.Colour == _Items[c].Colour
                                && Item - (ItemShift - 1) <= LineItems - 1
                                && Block.SolidCount > _Items[Item - (ItemShift - 1)].Value)
                                Item++;
                        }

                        if (Sum <= GapSizeCopy)
                            ItemShift++;
                        else
                            break;

                        Sum += GetDotCountB(c);
                    }

                    //TO FIX
                    if (Sc < GapSizeCopy && (ItemShift != 1 || (ItemShift == 1 && Sc == 0)))
                    {
                        Equality = false;
                    }

                    if (Sc < GapSizeCopy)
                        PartEquality = false;

                    //unique colour item
                    if (untilDot && !Equality && WasSolid && Item - ItemShift < LineItems - 1)
                    {
                        //ItemRange R = new ItemRange(_Items.Where(w => w.Index > Item + 1));
                        IEnumerable<Item> R2 = _Items.Where(w => w.Index > Item - ItemShift && w.Index <= LastItemAtEquality
                                        && w >= Block);
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
                        && R.Any() && FindPossibleSolids(Pos + 1, R, out int NewItem))
                    {
                        ItemShift = 1;
                        Item = NewItem;
                        Equality = true;
                    }

                    if (Logic.dots.ContainsKey(Xy) && Block.SolidCount == GapSizeCopy && Item < LineItems)
                    {
                        int UniqueCount = 0, TempItem = 0;

                        for (int Pos2 = LastItemAtEquality - 1; Pos2 >= Item; Pos2--)
                        {
                            if (Block == _Items[Pos2])
                            {
                                if (UniqueCount > 0 && Pos2 < TempItem - 1)
                                {
                                    UniqueCount = 0;
                                    break;
                                }

                                TempItem = Pos2;
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

                    if (Sc == GapSizeCopy)
                        PartEquality = true;

                    if (Equality)
                        LastItemAtEquality = Item;

                    if (Equality && Pos > 0 && Pos > position + 1)
                        ItemAtEquality = Item - ItemShift;

                    if (Sc == GapSizeCopy)
                        LastFullGapCount = Sc;

                    SolidCount = Block.SolidCount;
                    if (Block.SolidCount < GapSizeCopy)
                        ScValid = false;
                    else
                        ScValid = true;

                    if (Logic.dots.ContainsKey(Xy))
                    {
                        ItemAtLastGap = Item - ItemShift;
                        LastGapIndex = Pos;
                    }

                    Item -= ItemShift;
                    PossibleSolids = FindPossibleSolids(Pos + 1);
                }
            }

            if (Item < 0)
                Valid = false;

            if (!untilDot)
                Equality = false;

            Item? TheItem = null;

            if (Valid)
                TheItem = _Items[Item];

            IEnumerable<Item> Next = _Items.Where((w, wi) => ((!Equality && wi >= Item) || wi == Item) && wi <= ItemAtEquality);
            ItemRange Before = new ItemRange(_Items.Where((w, wi) => wi > Item && wi <= ItemAtEquality));
            IEnumerable<Item> After = _Items.Where((w, wi) => wi <= Item);
            //HashSet<Item> RightBefore = FindPossibleSolids(IsRow, LineLength, lineIndex, 1);
            HashSet<Block> RightBefore = new Block[] { new Block(ScValid, SolidCount) }.ToHashSet();
            ItemRange Gap = new ItemRange(_Items.Where((w, wi) => wi > Item && wi <= ItemAtLastGap));

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
            bool Start = index == 0 || _Items[index].Colour == _Items[index - 1].Colour;
            bool End = index == _Items.Length - 1 || _Items[index].Colour == _ItemsArray[index + 1].Colour;
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
            => sum > 0 && end - start - sum <= GetDotCount(index);

        public bool IsEqB(ItemRange u, int index, int start, int end)
            => IsEqB(index, start, end, u.Sum());
        public bool IsEqB(int index, int start, int end, int sum)
            => sum > 0 && end - start - sum <= GetDotCountB(index);

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

        public LineSegment GetItemAtPosB(int pos, bool untilDot = true)
        {
            LineSegment Ls;

            if (_ItemsAtPosB.TryGetValue(pos, out LineSegment? Out))
                Ls = Out;
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
            else
            {
                Ls = GetItemAtPositionB(pos, untilDot);
                _ItemsAtPosB.TryAdd(pos, Ls);
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
            string Colour = "black";
            bool AllOneColour = ItemsOneColour;
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
                    if (Pt.Colour != Colour)
                    {
                        if (SolidCount > 0)
                            SolidBlockCount++;

                        SolidCount = 0;
                        NotSolid = true;
                    }

                    if (BlockIndexes.ContainsKey(SolidBlockCount))
                        BlockIndexes[SolidBlockCount].EndIndex = c;
                    else
                        BlockIndexes.TryAdd(SolidBlockCount, new Block(c, c, Pt.Colour));

                    if ((!Logic.points.TryGetValue(IsRow ? (c + 1, LineIndex) : (LineIndex, c + 1), out Point? Pt2)
                        || Pt2.Colour != Pt.Colour)
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
                            && _Items[CurrentItem] < new Block(-1, Pt.Colour) { SolidCount = SolidCount })
                        {
                            bool NoMoreItems = UniqueCount(new Block(-1, Pt.Colour) { SolidCount = SolidCount });
                            bool Flag = NoMoreItems;
                            int StartIndex = -1;
                            int Stop = 0;

                            if (NoMoreItems)
                            {
                                StartIndex = FirstOrDefault(w => w >= new Block(-1, Pt.Colour) { SolidCount = SolidCount })?.Index ?? -1;

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
                        && UniqueCount(new Block(-1, Pt.Colour) { SolidCount = SolidCount }))
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

                    Colour = Pt.Colour;
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
            if (startItem == 0 && endItem == LineItems - 1
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
                            foreach (Point Point in Logic.AddPoints(this, FirstIndex, BlockIndexes[Pos].Colour, GriddlerPath.Action.MustJoin, SecondIndex))
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

            foreach ((int i, (int, int) Xy, Block Block, bool WasSolid, _, _) in Logic.ForEachLinePos(this))
            {
                if (i >= EndOfGap)
                    EndOfGap = FindGapStartEnd(i).Item2;

                LineSegment Ls = GetItemAtPos(i + 1, false);
                LineSegment LsEnd = GetItemAtPosB(i - 1, Logic.dots.ContainsKey(Xy));

                if (Ls.Valid || LsEnd.Valid || (Ls.ItemAtStartOfGap == Ls.LastItemAtEquality))
                {
                    Item Min = new Item(MaxItem, "black"), Max = new Item(0, "black");
                    int Start = 0, End = LineItems - 1;
                    bool WholeGap = false;

                    if (Ls.Valid)
                        End = Ls.Index;

                    if (Ls.Valid && IsEq(Ls.Gap, Ls.Index - 1, GapIndex, i - 1))
                        End--;

                    if (LsEnd.Before.Any() && End > LsEnd.Before.LastItemIndex)
                        End = LsEnd.Before.LastItemIndex;

                    if (LsEnd.Valid)
                        Start = LsEnd.Index;

                    //2,1,2//--00---.
                    if (LsEnd.Valid && Start < LineItems - 1
                            && IsEqB(LsEnd.Gap, LsEnd.Index + 1, i, EndOfGap + 1))
                        Start++;

                    if (Start > 0 && Start >= End
                    //&& _Items[Start].Value == 1 
                    && _Items[Start].Colour != Block.Colour)
                        Start--;

                    if (Logic.dots.ContainsKey(Xy) && LsEnd.Valid && LsEnd.Eq)
                        End = LsEnd.Index;

                    if (End > Start && End < LineItems && false
                        && !FindPossibleSolids(i - Block.SolidCount)
                            .Contains((_Items[End].Value, _Items[End].Colour)))
                    {
                        End--;
                    }

                    if (End > Start)
                    {
                        for (int Pos = End; Pos >= Start;)
                        {
                            if (Block.Colour != _Items[Pos].Colour)
                                End--;
                            //else
                            break;
                        }
                    }


                    //colour check TEMP - should be in find possible solids
                    if (End > Start && End > 0 && End < LineItems - 1 && !Logic.points.ContainsKey(Xy))
                    {
                        //for (int Pos = End + 1; Pos >= 1; Pos--)
                        //{
                        for (int LPos = i + GetDotCount(End); LPos <= i + _Items[End + 1].Value - 1 + GetDotCount(End); LPos++)
                        {
                            var PosXY = IsRow ? (LPos, LineIndex) : (LineIndex, LPos);
                            if (Logic.points.TryGetValue(PosXY, out Point? Pt)
                                && Pt.Colour != _Items[End + 1].Colour)
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
                            if (Block.Colour != _Items[Pos].Colour)
                                Start++;
                            //else
                            break;
                        }
                    }


                    if (End > 0)
                    {
                        for (int Pos = i; Pos < LineLength; Pos++)
                        {
                            var PosXy = IsRow ? (Pos, LineIndex) : (LineIndex, Pos);
                            if (Logic.dots.ContainsKey(PosXy))
                            {
                                if (Pos - GapIndex - 1 < _Items[End].Value)
                                    End--;

                                break;
                            }
                        }
                    }

                    //pull out end
                    if (WasSolid && End > 0)
                    {
                        (bool IsolatedEnd, _, _) = GetIsolatedPart(i - Block.SolidCount, End - 1, LineItems - 1);

                        if (IsolatedEnd)
                            End--;
                    }

                    //pull out start
                    if (WasSolid && Start < LineItems - 1 && false)
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
                            var PosXy = IsRow ? (Pos, LineIndex) : (LineIndex, Pos);

                            if (!Logic.points.ContainsKey(PosXy))
                                NotSolid = true;
                            else if (NotSolid)
                            {
                                if (Pos + _Items[Start].Value - 1 < i - 1)
                                    Start++;

                                break;
                            }
                        }
                    }

                    if (Start > End)
                    {
                        //int Temp = Start;
                        Start = Ls.Index;//End;
                        End = LsEnd.Index; //Temp;
                    }

                    //TEMP
                    if (Skip(Start).Take(End - Start + 1).All(a => a.Value < Block.SolidCount))
                    {
                        Min = new Item(0, "black");
                        Max = new Item(MaxItem, "black");
                    }
                    else if (Start <= End)
                        (Min, Max) = FindMinMax(Start, End);

                    MinMaxItems.TryAdd(i, (Min, Max));
                    WholeGap = true;

                    if (Ls.ItemAtStartOfGap > -1 && Ls.ItemAtStartOfGap == Ls.LastItemAtEquality)
                        Start = Ls.ItemAtStartOfGap;
                    else
                        WholeGap = false;

                    if ((Logic.dots.ContainsKey(Xy) || i == LineLength - 1) && LsEnd.Valid && LsEnd.Eq)
                        End = LsEnd.Index;
                    else
                        WholeGap = false;

                    (Item, Item) FindMinMax(int s, int e)
                    {
                        Item Min = new Item(MaxItem, "black"), Max = new Item(0, "black");

                        foreach (Item Item in Skip(s).Take(e - s + 1))
                        {
                            if (Item.Value > Max.Value)
                                Max = Item;

                            if (Item.Value < Min.Value && (WholeGap || !WasSolid || Item >= Block))
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

                if (!Logic.dots.ContainsKey(Xy))
                {
                    if (!WasGap)
                        GapIndex = i - 1;

                    WasGap = true;
                }
                else if (Logic.dots.ContainsKey(Xy))
                    WasGap = false;

                if (!Logic.points.ContainsKey(Xy) && WasSolid && Block.SolidCount > MinItem)
                {
                    Item[] ItemsGTSolid = _Items.Where(w => w.Value >= Block.SolidCount).ToArray();

                    if (ItemsGTSolid.Length > 0)
                    {
                        if (MinMaxItems.TryGetValue(i, out (Item, Item) Val)
                            && Val.Item1.Value < ItemsGTSolid.Min(m => m.Value))
                            MinMaxItems[i] = (new Item(ItemsGTSolid.Min(m => m.Value), "black"), Val.Item2);

                        MinMaxItems.TryAdd(i, (new Item(ItemsGTSolid.Min(m => m.Value), "black"), new Item(MaxItem, "black")));
                    }

                }

            }

            return MinMaxItems;
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
