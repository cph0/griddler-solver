﻿using Griddlers.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Griddlers.Library
{
    public class Line : ItemRange, IEnumerable<Item>
    {
        private new Item[] _Items => (Item[])base._Items;
        private readonly IDictionary<int, Block> _Blocks;
        private readonly IDictionary<int, Block> _BlocksByStart;
        private readonly IDictionary<int, Block> _BlocksByEnd;
        private int? _LineValue;
        private int? _MinItem;
        private int? _MaxItem;
        private IDictionary<int, Line> _PairLines;

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

        public Item this[int index] => _Items[index];

        public Line(int index,
                    bool isRow,
                    int lL,
                    Item[] items) : base(items, 0, items.Length - 1, true)
        {
            IsRow = isRow;
            LineLength = lL;
            LineIndex = index;
            _Blocks = new Dictionary<int, Block>(items.Length);
            _BlocksByStart = new Dictionary<int, Block>(items.Length);
            _BlocksByEnd = new Dictionary<int, Block>(items.Length);
            _PairLines = new Dictionary<int, Line>();

            Points = new Dictionary<int, string>(lL);
            Dots = new HashSet<int>(lL);
        }

        public void SetPairLines(IEnumerable<Line> lines)
        {
            _PairLines = lines.ToDictionary(k => k.LineIndex);
        }

        public IEnumerable<(int, Block, bool, int, int)> ForEachLinePos(Func<bool, bool, int, int, bool>? run = null,
                                                                                    Predicate<int>? stop = null,
                                                                                    bool b = false,
                                                                                    int start = 0,
                                                                                    Func<int>? advance = null)
        {
            bool WasSolid = false;
            int SolidBlockCount = 0, GapSize = 0, Size = 0;
            Block Block = new Block(0, "black");
            for (int Pos = b ? LineLength - 1 : start; b ? Pos >= start : Pos < LineLength; Pos += (b ? -1 : 1))
            {
                bool ChangedColour = false;

                if (stop != null && stop(Pos))
                    break;

                if (Points.TryGetValue(Pos, out string? Pt)
                    && Block.Colour != Pt)
                    ChangedColour = true;

                if (run == null || run(Pt == (object?)null || ChangedColour, WasSolid, Pos, GapSize))
                    yield return (Pos, Block, WasSolid, GapSize, Size);
                //Pos += (b ? -1 : 1) * logic(Pos, Xy, Block, WasSolid, GapSize, Size);

                if (advance != null)
                    Pos += (b ? -1 : 1) * advance();

                if (!Dots.Contains(Pos))
                    GapSize++;
                else
                    GapSize = 0;

                if (Points.TryGetValue(Pos, out Pt))
                {
                    if (Block.Colour != Pt)
                    {
                        if (Block.Size > 0)
                            SolidBlockCount++;

                        Block = new Block(SolidBlockCount, Pt);
                    }

                    if (!WasSolid || ChangedColour)
                        Block.Start = Pos;

                    Block.End = Pos;
                    Block.Size++;
                    Size++;

                    WasSolid = true;
                }
                else
                {
                    if (WasSolid)
                    {
                        SolidBlockCount++;
                        Block = new Block(SolidBlockCount, "black");
                        Size = 0;
                    }

                    WasSolid = false;
                }
            }
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

        //private (int, bool, int) AdjustItemIndexes(Gap gap,
        //                                           int ei,
        //                                           (int, bool) ie,
        //                                           bool forward = true)
        //{
        //    int ItemShift = SumWhile(ie.Item1, gap, null, forward);
        //    var Iei = (ie.Item1, ie.Item2, ItemShift);

        //    if (!gap.IsFull(false)
        //        && (Iei.ItemShift > 1 || (Iei.ItemShift == 1 && !gap.HasPoints)))
        //        Iei.Item2 = false;

        //    if (gap.IsFull() && (!Iei.Item2 || RanOutOfItems(Iei.Item1, forward)
        //        || !gap.Is(this[Iei.Item1])))
        //    {
        //        IEnumerable<Item> Items = Where(ei, Iei.Item1, true);
        //        Item[] UniqueItems = Items.Where(gap.Is).ToArray();
        //        Item? Item = null;

        //        if (UniqueItems.Length == 1)
        //            Item = UniqueItems[0];
        //        else
        //        {
        //            Gap? LastGap = FindGapAtPos(gap.Start + (forward ? -1 : 1), !forward);
        //            if (LastGap?.IsFull() == true)
        //            {
        //                UniqueItems = Pair()
        //                    .Where(w => LastGap.Is(forward ? w.Item1 : w.Item2)
        //                    && gap.Is(forward ? w.Item2 : w.Item1))
        //                    .Select(s => forward ? s.Item2 : s.Item1)
        //                    .ToArray();

        //                if (UniqueItems.Length == 1)
        //                    Item = UniqueItems[0];
        //            }
        //        }

        //        if (!Item.HasValue)
        //            Item = !forward ? Items.FirstOrDefault(gap.Is)
        //                : Items.LastOrDefault(gap.Is);

        //        Iei = (Item?.Index ?? -1, UniqueItems.Length == 1, 1);
        //    }
        //    else if (!gap.IsFull() && gap.HasPoints && !Iei.Item2)
        //    {
        //        Block? LastBlock = forward ? gap.GetLastBlock(gap.End) : gap.GetNextBlock(gap.Start);
        //        if (LastBlock != null)
        //        {
        //            int ToItem = Iei.Item1 + ((forward ? 1 : -1) * (Iei.ItemShift - 1));
        //            if (UniqueCount(Where(ei, ToItem, true), LastBlock, out Item UniqueItem))
        //            {
        //                int Itm = UniqueItem.Index;
        //                bool Eq = Itm == ToItem;

        //                if (!Eq)
        //                {
        //                    int Start = UniqueItem.Index + (forward ? 1 : -1);
        //                    Itm += (forward ? 1 : -1) * SumWhile(Start,
        //                                                         gap,
        //                                                         LastBlock,
        //                                                         forward,
        //                                                         !forward);
        //                }

        //                Iei = (Itm, Eq, 1);
        //            }
        //        }
        //    }

        //    return Iei;
        //}

        private LineSegment GetItemAtPosition1(int position)
        {
            int Item = 0, MaxItem = 0, LastItemAtEquality = -1, ItemAtEquality = -1, ItemAtLastGap = -1;
            bool Valid = true, Equality = true, ScValid = true, PartEquality = false;
            int LastFullGapCount = 0, LastGapIndex = -1;
            Block LastBlock = new Block(0, "black");
            HashSet<(int, string)> PossibleSolids = new HashSet<(int, string)>();

            foreach ((int Pos, Block Block, bool WasSolid, int GapSize, int Size) in ForEachLinePos((ns, sC, i, gS) => gS > 0
            && (Dots.Contains(i) || i == LineLength - 1), (i) => i == position))
            {
                int Sum = 0, ItemShift = 0, GapMaxItem = 0;
                ItemAtLastGap = Item;
                LastGapIndex = Pos - GapSize - 1;

                if (Block.Size == GapSize && Item < LineItems && _Items[Item].Value == Block.Size
                    && _Items[Item].Value > MaxItem)
                {
                    Equality = true;
                }

                //until dot
                if (Item >= LineItems && Block.Size > 0 && Block.Size == GapSize)
                {
                    for (int c = LineItems - 1; c >= 0; c--)
                    {
                        if (_Items[c].Value >= Block.Size && _Items[c].Value <= GapSize)
                        {
                            Item = c;
                            break;
                        }
                    }
                }

                if (Item >= LineItems && Block.Size > 0)
                    Item = LineItems - 1;

                foreach (Item Itm in Skip(Item))
                {
                    Sum += Itm.Value;
                    if (Points.TryGetValue(Pos - GapSize + Sum, out string? Pt) 
                        && Pt == Itm.Colour)
                    {
                        Sum++;

                        //item cannot be block
                        if (WasSolid && Sum == GapSize
                            && Item + ItemShift >= 0
                            && Block.Size > _Items[Item + ItemShift].Value) //at end of gap
                            Item--;
                        else if (WasSolid && Sum > GapSize //until dot
                            && Item + ItemShift - 1 >= 0
                            && Block.Size > _Items[Item + ItemShift - 1].Value)
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

                if (Size < GapSize && (ItemShift > 1 || (ItemShift == 1 && Size == 0)))
                    Equality = false;

                if (Size < GapSize)
                    PartEquality = false;

                //until dot
                if (Block.Size == GapSize && Item < LineItems && LastItemAtEquality + 1 >= 0)
                {
                    int UniqueCount = 0, TempItem = 0;

                    foreach (Item Itm in Skip(LastItemAtEquality + 1).Take(Item - LastItemAtEquality))
                    {
                        if (Block.Is(Itm))
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

                if (Size == GapSize)
                    PartEquality = true;

                if (Equality && Pos < LineLength - 1 && Pos < position - 1)
                    LastItemAtEquality = Item; //????

                if (Equality && Pos < LineLength - 1)
                    ItemAtEquality = Item + ItemShift;

                if (GapMaxItem > MaxItem)
                    MaxItem = GapMaxItem;

                if (Size == GapSize)
                    LastFullGapCount = Block.Size;

                LastBlock = Block;
                if (Size < GapSize)
                    ScValid = false;
                else
                    ScValid = true;

                Item += ItemShift;
            }

            if (Item > LineItems - 1)
                Valid = false;

            Item? TheItem = null;

            if (Valid)
                TheItem = _Items[Item];

            LastBlock.Complete = ScValid;
            IEnumerable<Item> Next = _Items.Where((w, wi) => wi >= ItemAtEquality && ((!Equality && wi <= Item) || wi == Item));
            ItemRange Before = CreateRange(ItemAtEquality, Item - 1);
            IEnumerable<Item> After = _Items.Where((w, wi) => wi >= Item);
            HashSet<Block> RightBefore = new Block[] { LastBlock }.ToHashSet();
            ItemRange Gap = CreateRange(ItemAtLastGap, Item - 1);

            return new LineSegment(Next, true, TheItem, Equality, Before, After, RightBefore, Gap, ItemAtLastGap, ItemAtEquality);
        }

        private LineSegment GetItemAtPosition2(LineSegment ls,
                                              int gapPos,
                                              Block block,
                                              int gapEnd)
        {
            int Range = block.End - (gapPos);//block.Start - (gapPos + 1);
            int GapSize = gapEnd - (gapPos + 1);
            int Item = ls.Index;
            bool Valid = true;
            HashSet<(int, string)> PossibleSolids = new HashSet<(int, string)>();
            int Sum = 0, ItemShift = 0;

            //!until dot
            PossibleSolids = FindPossibleSolids(block.Start);

            int CrashPos(int sum)
            {
                return gapPos + sum + 1;
            };

            foreach (Item Itm in Skip(Item))
            {
                //while (Logic.points.TryGetValue(CrashPos(Sum), out Point? Pt)
                //    && Pt.Colour != Itm.Colour)
                //{
                //    Sum++;
                //}

                Sum += Itm.Value;
                //var Pos2 = IsRow ? (gapPos + Sum + 1, LineIndex) 
                //    : (LineIndex, gapPos + Sum + 1);

                if (Points.TryGetValue(CrashPos(Sum), out string? Pt)
                    && Pt == Itm.Colour)
                {
                    Sum++;

                    //item cannot be block
                    if (Sum == Range
                        && Item + ItemShift >= 0
                        && block.Size > _Items[Item + ItemShift].Value) //at end of gap
                        Item--;
                    else if (!PossibleSolids.Contains((_Items[Item + ItemShift].Value, "black"))
                        && Sum > Range
                        && Item + ItemShift - 1 >= 0
                        && block.Size > _Items[Item + ItemShift - 1].Value)
                        Item--;
                }

                if (Sum <= Range)
                {
                    ItemShift++;
                }
                else
                {
                    //item can't be block as doesn't fit in gap!
                    //if (block != null && Sum > GapSize)
                    //    ItemShift--;

                    //item can't be block as too small! - while?
                    //if (block != null && Item + ItemShift >= 0
                    //    && block.Size > _Items[Item + ItemShift].Value)
                    //    ItemShift--;

                    break;
                }

                Sum += GetDotCount(Itm.Index);
            }

            Item += ItemShift;

            if (Item > LineItems - 1)
                Valid = false;

            Item? TheItem = null;

            if (Valid)
                TheItem = _Items[Item];

            IEnumerable<Item> Next = Where(ls.EqualityIndex, Item);
            ItemRange Before = CreateRange(ls.EqualityIndex, Item - 1);
            IEnumerable<Item> After = _Items.Where((w, wi) => wi >= Item);
            ItemRange Gap = CreateRange(ls.Index, Item - 1);
            return ls.SetIndexAtBlock(Item, TheItem, Next, Before, After, Gap);
        }

        public LineSegment GetItemAtPosition(int gapPos,
                                             Block? block = null,
                                             int gapEnd = -1)
        {
            LineSegment Ls = GetItemAtPosition1(gapPos + 1);

            if (block != null)
                Ls = GetItemAtPosition2(Ls, gapPos, block, gapEnd);

            return Ls;
        }

        private LineSegment GetItemAtPositionB1(int position)
        {
            int Item = LineItems - 1, Size = 0, LastItemAtEquality = LineItems,
                ItemAtEquality = LineItems, ItemAtLastGap = LineItems;
            bool Valid = true, Equality = true, ScValid = true, PartEquality = false;
            int LastFullGapCount = 0, LastGapIndex = LineLength;
            HashSet<(int, string)> PossibleSolids = new HashSet<(int, string)>();

            foreach ((int Pos, Block Block, bool WasSolid, int GapSize, int Sc) in ForEachLinePos((nS, sC, i, gS) 
                => (Dots.Contains(i) || i == position + 1), (i) => i == position, true))
            {
                int Sum = 0, ItemShift = 0, IsPoint = Points.ContainsKey(Pos) ? 1 : 0;
                int GapSizeCopy = GapSize;
                ItemAtLastGap = Item;
                LastGapIndex = Pos + GapSizeCopy + 1;
                GapSizeCopy += IsPoint;

                if (GapSizeCopy > 0)
                {
                    if (Item < 0 && Block.Size > 0 && Block.Size == GapSizeCopy)
                    {
                        Item? FirstItem = _Items.FirstOrDefault(f => f.Value >= Block.Size && f.Value <= GapSizeCopy);
                        Item = FirstItem.HasValue ? FirstItem.Value.Index : -1;

                        if (Item < LineItems - 1
                            && !PossibleSolids.Contains((_Items[Item + 1].Value, "black"))
                            && PossibleSolids.Count > 0
                            && PossibleSolids.Max(m => m.Item1) >= _Items[Item + 1].Value)
                        {
                            Item++;
                        }
                    }

                    for (int c = Item; c >= 0; c--)
                    {
                        Sum += _Items[c].Value;
                        var Pos2 = Pos + GapSizeCopy - IsPoint - Sum;
                        if (Points.TryGetValue(Pos2, out string? Pt) && Pt == _Items[c].Colour)
                        {
                            Sum++;

                            if (WasSolid && Pt == _Items[c].Colour
                                && Sum <= GapSizeCopy
                                && Pos + GapSizeCopy - IsPoint - Sum <= Block.End
                                && Item - ItemShift <= LineItems - 1
                                && Block.Size > _Items[Item - ItemShift].Value)
                                Item++;
                            else if (WasSolid && Sum > GapSizeCopy && Pt == _Items[c].Colour
                                && Item - (ItemShift - 1) <= LineItems - 1
                                && Block.Size > _Items[Item - (ItemShift - 1)].Value)
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

                    //unique colour item - untildot
                    if (!Equality && WasSolid && Item - ItemShift < LineItems - 1)
                    {
                        //ItemRange R = new ItemRange(_Items.Where(w => w.Index > Item + 1));
                        IEnumerable<Item> R2 = _Items.Where(w => w.Index > Item - ItemShift && w.Index <= LastItemAtEquality
                                        && Block.CanBe(w));
                        if (R2.Count() == 1)
                        {
                            Item = R2.First().Index;
                            ItemShift = 1;
                            Equality = true;
                        }
                    }

                    //mutliple possible solids = until dot
                    IEnumerable<Item> R = _Items.Where(w => w.Index > Item - ItemShift && w.Index <= ItemAtEquality);
                    if (!Equality && ItemAtLastGap == ItemAtEquality
                        && R.Any() && FindPossibleSolids(Pos + 1, R, out int NewItem))
                    {
                        ItemShift = 1;
                        Item = NewItem;
                        Equality = true;
                    }

                    if (Dots.Contains(Pos) && Block.Size == GapSizeCopy && Item < LineItems)
                    {
                        int UniqueCount = 0, TempItem = 0;

                        for (int Pos2 = LastItemAtEquality - 1; Pos2 >= Item; Pos2--)
                        {
                            if (Block.Is(_Items[Pos2]))
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

                    Size = Block.Size;
                    if (Block.Size < GapSizeCopy)
                        ScValid = false;
                    else
                        ScValid = true;

                    if (Dots.Contains(Pos))
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

            Item? TheItem = null;

            if (Valid)
                TheItem = _Items[Item];

            IEnumerable<Item> Next = _Items.Where((w, wi) => ((!Equality && wi >= Item) || wi == Item) && wi <= ItemAtEquality);
            ItemRange Before = CreateRange(Item + 1, ItemAtEquality);
            IEnumerable<Item> After = _Items.Where((w, wi) => wi <= Item);
            HashSet<Block> RightBefore = new Block[] { new Block(ScValid, Size) }.ToHashSet();
            ItemRange Gap = CreateRange(Item + 1, ItemAtLastGap);
            return new LineSegment(Next,
                                   false,
                                   TheItem,
                                   Equality,
                                   Before,
                                   After,
                                   RightBefore,
                                   Gap,
                                   ItemAtLastGap,
                                   ItemAtEquality);
        }

        private LineSegment GetItemAtPositionB2(LineSegment ls,
                                                int gapPos,
                                                Block block,
                                                int gapStart)
        {
            int Range = block.End - (gapPos);
            int GapSize = gapPos - (gapStart + 1);
            int Item = ls.Index;
            bool Valid = true;
            int Sum = 0, ItemShift = 0;

            int CrashPos(int sum)
            {
                return gapPos - sum - 1;
            };

            for (int c = Item; c >= 0; c--)
            {
                Sum += _Items[c].Value;
                if (Points.TryGetValue(CrashPos(Sum), out string? Pt) 
                    && Pt == _Items[c].Colour)
                {
                    Sum++;

                    if (Sum == Range
                        && Item - ItemShift <= LineItems - 1
                        && block.Size > _Items[Item - ItemShift].Value)
                        Item++;
                }

                if (Sum <= Range)
                    ItemShift++;
                else
                    break;

                Sum += GetDotCountB(c);
            }

            Item -= ItemShift;

            if (Item < 0)
                Valid = false;

            Item? TheItem = null;

            if (Valid)
                TheItem = _Items[Item];

            IEnumerable<Item> Next = Where(Item, ls.EqualityIndex);
            ItemRange Before = CreateRange(Item + 1, ls.EqualityIndex);
            IEnumerable<Item> After = _Items.Where((w, wi) => wi <= Item);
            ItemRange Gap = CreateRange(Item + 1, ls.Index);
            return ls.SetIndexAtBlock(Item, TheItem, Next, Before, After, Gap);
        }

        public LineSegment GetItemAtPositionB(int gapPos,
                                              Block? block = null,
                                              int gapStart = -1)
        {
            LineSegment Ls = GetItemAtPositionB1(gapPos - 1);

            if (block != null)
                Ls = GetItemAtPositionB2(Ls, gapPos, block, gapStart);

            return Ls;
        }

        private LineSegment GetItemAtPosition(int position, bool untilDot = true)
        {
            int Item = 0, MaxItem = 0, LastItemAtEquality = -1, ItemAtEquality = -1, ItemAtLastGap = -1;
            bool Valid = true, Equality = untilDot, ScValid = true, PartEquality = false;
            int LastFullGapCount = 0, LastGapIndex = -1;
            Block LastBlock = new Block(0, "black");
            HashSet<(int, string)> PossibleSolids = new HashSet<(int, string)>();

            foreach ((int Pos, Block Block, bool WasSolid, int GapSize, int Size) in ForEachLinePos((ns, sC, i, gS) => gS > 0
            && (Dots.Contains(i) || i == LineLength - 1 || (!untilDot && i == position - 1)), (i) => i == position))
            {
                int Sum = 0, ItemShift = 0, GapMaxItem = 0;
                ItemAtLastGap = Item;
                LastGapIndex = Pos - GapSize - 1;

                if (WasSolid && !untilDot)
                    PossibleSolids = FindPossibleSolids(Pos - Block.Size);

                if (Block.Size == GapSize && Item < LineItems && _Items[Item].Value == Block.Size
                    && _Items[Item].Value > MaxItem)
                {
                    Equality = true;
                }

                if (untilDot && Item >= LineItems && Block.Size > 0 && Block.Size == GapSize)
                {
                    for (int c = LineItems - 1; c >= 0; c--)
                    {
                        if (_Items[c].Value >= Block.Size && _Items[c].Value <= GapSize)
                        {
                            Item = c;
                            break;
                        }
                    }
                }

                if (Item >= LineItems && Block.Size > 0)
                    Item = LineItems - 1;

                foreach (Item Itm in Skip(Item))
                {
                    Sum += Itm.Value;
                    if (Points.TryGetValue(Pos - GapSize + Sum, out string? Pt) 
                        && Pt == Itm.Colour)
                    {
                        Sum++;

                        //item cannot be block
                        if (WasSolid && Sum == GapSize
                            && Item + ItemShift >= 0
                            && Block.Size > _Items[Item + ItemShift].Value) //at end of gap
                            Item--;
                        else if ((untilDot || !PossibleSolids.Contains((_Items[Item + ItemShift].Value, "black")))
                            && WasSolid && Sum > GapSize
                            && Item + ItemShift - 1 >= 0
                            && Block.Size > _Items[Item + ItemShift - 1].Value)
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

                if (Size < GapSize && (ItemShift > 1 || (ItemShift == 1 && Size == 0)))
                    Equality = false;

                if (Size < GapSize)
                    PartEquality = false;

                if (untilDot && Block.Size == GapSize && Item < LineItems)
                {
                    int UniqueCount = 0, TempItem = 0;

                    foreach (Item Itm in Skip(LastItemAtEquality + 1).Take(Item - LastItemAtEquality))
                    {
                        if (Block.Is(Itm))
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

                if (Size == GapSize)
                    PartEquality = true;

                if (Equality && Pos < LineLength - 1 && Pos < position - 1)
                    LastItemAtEquality = Item; //????

                if (Equality && Pos < LineLength - 1 && Pos < position - 1)
                    ItemAtEquality = Item + ItemShift;

                if (GapMaxItem > MaxItem)
                    MaxItem = GapMaxItem;

                if (Size == GapSize)
                    LastFullGapCount = Block.Size;

                LastBlock = Block;
                if (Size < GapSize)
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
            ItemRange Before = CreateRange(ItemAtEquality, Item - 1);
            IEnumerable<Item> After = _Items.Where((w, wi) => wi >= Item);
            HashSet<Block> RightBefore = new Block[] { LastBlock }.ToHashSet();
            ItemRange Gap = CreateRange(ItemAtLastGap, Item - 1);
            return new LineSegment(Next, true, TheItem, Equality, Before, After, RightBefore, Gap, ItemAtLastGap, ItemAtEquality);
        }

        /// <summary>
        /// Gets the possible items at a position along the line working backwards
        /// </summary>
        /// <param name="position">The position of the line to start at</param>
        /// <param name="untilDot">Position is on a dot</param>
        /// <returns></returns>
        public LineSegment GetItemAtPositionB(int position, bool untilDot = true)
        {
            int Item = LineItems - 1, Size = 0, LastItemAtEquality = LineItems, ItemAtEquality = LineItems, ItemAtLastGap = LineItems;
            bool Valid = true, Equality = true, ScValid = true, PartEquality = false;
            int LastFullGapCount = 0, LastGapIndex = LineLength;
            HashSet<(int, string)> PossibleSolids = new HashSet<(int, string)>();

            foreach ((int Pos, Block Block, bool WasSolid, int GapSize, int Sc) in ForEachLinePos((nS, sC, i, gS) 
                => (Dots.Contains(i) || i == position + 1), (i) => i == position, true))
            {
                int Sum = 0, ItemShift = 0, IsPoint = Points.ContainsKey(Pos) ? 1 : 0;
                int GapSizeCopy = GapSize;
                ItemAtLastGap = Item;
                LastGapIndex = Pos + GapSizeCopy + 1;
                GapSizeCopy += IsPoint;

                if (GapSizeCopy > 0)
                {
                    if (Item < 0 && Block.Size > 0 && Block.Size == GapSizeCopy)
                    {
                        Item? FirstItem = _Items.FirstOrDefault(f => f.Value >= Block.Size && f.Value <= GapSizeCopy);
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
                        PossibleSolids = FindPossibleSolids(Pos - Block.Size);

                    for (int c = Item; c >= 0; c--)
                    {
                        Sum += _Items[c].Value;
                        var Pos2 = Pos + GapSizeCopy - IsPoint - Sum;
                        if (Points.TryGetValue(Pos2, out string? Pt) && Pt == _Items[c].Colour)
                        {
                            Sum++;

                            if (WasSolid && Pt == _Items[c].Colour
                                && Sum <= GapSizeCopy
                                && Pos + GapSizeCopy - IsPoint - Sum <= Block.End
                                && Item - ItemShift <= LineItems - 1
                                && Block.Size > _Items[Item - ItemShift].Value)
                                Item++;
                            else if ((untilDot || (!PossibleSolids.Contains((_Items[Item - ItemShift].Value, "black")) && false))
                                && WasSolid && Sum > GapSizeCopy && Pt == _Items[c].Colour
                                && Item - (ItemShift - 1) <= LineItems - 1
                                && Block.Size > _Items[Item - (ItemShift - 1)].Value)
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
                                        && Block.CanBe(w));
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

                    if (Dots.Contains(Pos) && Block.Size == GapSizeCopy && Item < LineItems)
                    {
                        int UniqueCount = 0, TempItem = 0;

                        for (int Pos2 = LastItemAtEquality - 1; Pos2 >= Item; Pos2--)
                        {
                            if (Block.Is(_Items[Pos2]))
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

                    Size = Block.Size;
                    if (Block.Size < GapSizeCopy)
                        ScValid = false;
                    else
                        ScValid = true;

                    if (Dots.Contains(Pos))
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
            ItemRange Before = CreateRange(Item + 1, ItemAtEquality);
            IEnumerable<Item> After = _Items.Where((w, wi) => wi <= Item);
            HashSet<Block> RightBefore = new Block[] { new Block(ScValid, Size) }.ToHashSet();
            ItemRange Gap = CreateRange(Item + 1, ItemAtLastGap);
            return new LineSegment(Next,
                                   false,
                                   TheItem,
                                   Equality,
                                   Before,
                                   After,
                                   RightBefore,
                                   Gap,
                                   ItemAtLastGap,
                                   ItemAtEquality);
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
            Dictionary<int, Dictionary<(int, string), int>> PossibleSolids = new Dictionary<int, Dictionary<(int, string), int>>(50);
            bool WasSolid = false, AfterFirstSolid = false;
            string PrevColour = "black";

            foreach ((int Pos, _, _, _, _) in ForEachLinePos(start: start))
            {
                if (Points.TryGetValue(Pos, out string? Pt))
                {
                    if (!WasSolid || PrevColour != Pt)
                    {
                        foreach (int PosSolid in PossibleSolids.Keys)
                            PossibleSolids[PosSolid].Remove((Pos - PosSolid, Pt));

                        if (!AfterFirstSolid)
                            PossibleSolids.Add(Pos, new Dictionary<(int, string), int>(50));

                        if (!WasSolid)
                            PrevColour = Pt;
                    }
                }
                else
                {
                    if (!WasSolid && !AfterFirstSolid)
                        PossibleSolids.Add(Pos, new Dictionary<(int, string), int>(50));
                    else if (WasSolid && onlyFirstSolid)
                        AfterFirstSolid = true;
                }

                if (!Points.TryGetValue(Pos, out Pt) || PrevColour != Pt 
                    || Pos == LineLength - 1)
                {
                    foreach (int PosSolid in PossibleSolids.Keys)
                    {
                        int ConsumeCount = 0;
                        string ConsumeColour = "black";
                        bool Valid = true, NextValid = true;
                        for (int Pos2 = PosSolid; Pos2 <= Pos; Pos2++)
                        {
                            //var PosXy = IsRow ? (Pos2, LineIndex) : (LineIndex, Pos2);
                            if (Points.TryGetValue(Pos2, out string? Pt2))
                            {
                                if (ConsumeCount > 0 && Pt2 != ConsumeColour)
                                {
                                    Valid = false;
                                    break;
                                }

                                ConsumeCount++;
                                ConsumeColour = Pt2;

                                if (Pt != (object?)null && Pt2 != Pt)
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

                if (Points.TryGetValue(Pos, out Pt))
                {
                    WasSolid = true;
                    PrevColour = Pt;
                }
                else
                    WasSolid = false;

                if (Dots.Contains(Pos))
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
            int Size = 0;
            bool Consume = true;
            newItem = items.First().Index;

            for (int Pos = start; Pos < LineLength; Pos++)
            {
                if (Points.ContainsKey(Pos))
                    Size++;

                if (Dots.Contains(Pos) || Pos == LineLength - 1)
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

                return PossibleItems == ItemCount && (!consume || ConsumeCount == Size);
            }

            return Consume;
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
                Block.Start = start.Value;
                int StartPos = 0;

                if (item.Index > 0)
                    StartPos = start.Value - _Items[item.Index - 1].Value + 1;

                for (int Pos = start.Value; Pos >= StartPos; Pos--)
                    _BlocksByStart.TryAdd(Pos, Block);
            }

            if (complete && end.HasValue)
            {
                Block.End = end.Value;
                int EndPos = LineLength - 1;

                if (item.Index < LineItems - 1)
                    EndPos = end.Value + _Items[item.Index + 1].Value - 1;

                for (int Pos = end.Value; Pos <= EndPos; Pos++)
                    _BlocksByEnd.TryAdd(Pos, Block);
            }
        }

        //public int GetLinePointsValue(bool includeDots = false)
        //{
        //    int LineValue = 0;

        //    LineValue = Logic.points.Keys.Count(w => IsRow ? w.Item2 == LineIndex : w.Item1 == LineIndex);

        //    if (includeDots)
        //        LineValue += Logic.dots.Keys.Count(w => IsRow ? w.Item2 == LineIndex : w.Item1 == LineIndex);

        //    return LineValue;
        //}

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
                if (Dots.Contains(Pos))
                {
                    GapStart = Pos;
                    break;
                }
            }
            for (int Pos = pos2.GetValueOrDefault(pos); Pos < LineLength; Pos++)
            {
                if (Dots.Contains(Pos))
                {
                    GapEnd = Pos;
                    break;
                }
                else if (Points.ContainsKey(Pos))
                    WasSolid = true;
            }

            return (GapStart, GapEnd, WasSolid);
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
            //2,3,4,5//--00--0---0---- isolated 2,3,4 - invalid as could be 3,4,5
            bool IsIsolated = true, Valid = false, NotSolid = false, RestIsolated = false;
            string Colour = "black";
            bool AllOneColour = ItemsOneColour;
            int SolidBlockCount = 0, Size = 0, CurrentItem = startItem, ReachIndex = 0, StartItem = startItem;
            Dictionary<int, bool> Isolations = new Dictionary<int, bool>();
            Dictionary<int, int> IsolatedItems = new Dictionary<int, int>();
            Dictionary<int, Block> BlockIndexes = new Dictionary<int, Block>();
            Dictionary<int, bool> CanJoin = new Dictionary<int, bool>();
            Dictionary<int, int> Pushes = new Dictionary<int, int>();

            for (int c = position; c < LineLength; c++)
            {
                if (Points.TryGetValue(c, out string? Pt))
                {
                    if (Pt != Colour)
                    {
                        if (Size > 0)
                            SolidBlockCount++;

                        Size = 0;
                        NotSolid = true;
                    }

                    if (BlockIndexes.ContainsKey(SolidBlockCount))
                        BlockIndexes[SolidBlockCount].End = c;
                    else
                        BlockIndexes.TryAdd(SolidBlockCount, new Block(c, c, Pt));

                    if ((!Points.TryGetValue(c + 1, out string? Pt2)
                        || Pt2 != Pt)
                        && SolidBlockCount > 0
                        && CurrentItem < endItem + 1)
                    {
                        int ReachIndexCurrent = BlockIndexes[SolidBlockCount - 1].Start
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
                            && !new Block(-1, Pt) { Size = Size }.CanBe(_Items[CurrentItem]))
                        {
                            Block NoMoreItemBlock = new Block(-1, Pt) { Size = Size };
                            //bool NoMoreItems = UniqueCount();
                            bool Flag = false;
                            int Start = -1;
                            int Stop = 0;

                            if (UniqueCount(NoMoreItemBlock, out Item Itm))
                            {
                                Flag = true;
                                Start = Itm.Index;//FirstOrDefault(new Block(-1, Pt.Colour) { Size = Size }.CanBe)?.Index ?? -1;

                                for (int d = SolidBlockCount - 1; d >= 0; d--)
                                {
                                    int ItemIndex = Start - (SolidBlockCount - d);

                                    if (ItemIndex == -1)
                                    {
                                        Flag = false;
                                        break;
                                    }

                                    if (BlockIndexes.TryGetValue(d, out Block? First)
                                        && BlockIndexes.TryGetValue(d + 1, out Block? Second))
                                    {
                                        if (First.Start + _Items[ItemIndex].Value - 1 >= Second.End
                                            || Second.End - _Items[ItemIndex + 1].Value + 1 <= First.Start)
                                        {
                                            Flag = false;
                                            break;
                                        }
                                        else if (Second.Start - First.End - 1 >= (AllOneColour ? 3 : 0))
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
                                    int ItemIndex = Start - (SolidBlockCount - d);
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
                            if (BackReach <= BlockIndexes[SolidBlockCount - 1].Start)
                            {
                                for (int d = SolidBlockCount - 1; d >= 0; d--)
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

                    if (NotSolid && Valid)
                        Valid = false;

                    if (IsIsolated && SolidBlockCount > 0
                        && Size > 0 && CurrentItem < endItem + 1
                        && _Items[CurrentItem].Value >= Size
                        && UniqueCount(new Block(-1, Pt) { Size = Size }, out Item _))
                    {
                        Valid = true;
                    }

                    if (NotSolid && c < ReachIndex)
                        CanJoin.TryAdd(SolidBlockCount, false);

                    if (NotSolid && SolidBlockCount > 1)
                    {
                        if (Pushes.TryGetValue(SolidBlockCount - 2, out int Item)
                            && Item < LineItems
                            && c < BlockIndexes[SolidBlockCount - 1].Start
                            + _Items[Item].Value)
                            CanJoin.TryAdd(SolidBlockCount, false);
                    }

                    Colour = Pt;
                    NotSolid = false;
                    Size++;

                    if (RestIsolated)
                        Isolations.TryAdd(SolidBlockCount, false);
                }
                else
                {
                    if (Size > 0)
                    {
                        SolidBlockCount++;

                        //start - skip to correct solid count
                        if (SolidBlockCount == 1 && _Items[CurrentItem].Value < Size)
                        {
                            CurrentItem = _Items.Select((w, wi) => (w, wi))
                                               .FirstOrDefault(w => w.w.Value >= Size).wi;
                            StartItem = CurrentItem;
                            IsIsolated = false;
                        }
                    }

                    NotSolid = true;
                    Size = 0;
                }
            }

            if (!NotSolid)
                SolidBlockCount++;

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
            return IsolatedPart(pos, startItem, endItem);
        }

        /// <summary>
        /// Finds the minimum and maximum items at each position along the line
        /// </summary>
        /// <returns>A dictionary of the minimum/maximum items by position</returns>
        public IDictionary<int, (Item, Item)> GetMinMaxItems()
        {
            Dictionary<int, (Item, Item)> MinMaxItems = new Dictionary<int, (Item, Item)>();
            int GapIndex = -1, StartOfGap = -1, EndOfGap = -1;
            bool WasGap = false;

            foreach ((int i, Block Block, bool WasSolid, _, _) in ForEachLinePos())
            {
                if (i >= EndOfGap)
                    (StartOfGap, EndOfGap, _) = FindGapStartEnd(i);

                LineSegment Ls = GetItemAtPosition(i + 1, false);
                //LineSegment Ls = GetItemAtPosition(StartOfGap, Block, EndOfGap);
                LineSegment LsEnd = GetItemAtPositionB(i - 1, Dots.Contains(i));

                if (Ls.Valid || LsEnd.Valid || (Ls.ItemAtStartOfGap == Ls.EqualityIndex))
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

                    if (Dots.Contains(i) && LsEnd.Valid && LsEnd.Eq)
                        End = LsEnd.Index;

                    if (End > Start && End < LineItems && false
                        && !FindPossibleSolids(i - Block.Size)
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
                    if (End > Start && End > 0 && End < LineItems - 1 
                        && !Points.ContainsKey(i))
                    {
                        //for (int Pos = End + 1; Pos >= 1; Pos--)
                        //{
                        for (int LPos = i + GetDotCount(End); LPos <= i + _Items[End + 1].Value - 1 + GetDotCount(End); LPos++)
                        {
                            if (Points.TryGetValue(LPos, out string? Pt)
                                && Pt != _Items[End + 1].Colour)
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
                            if (Dots.Contains(Pos))
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
                        (bool IsolatedEnd, _, _) = GetIsolatedPart(i - Block.Size, End - 1, LineItems - 1);

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
                            if (!Points.ContainsKey(Pos))
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
                    if (Skip(Start).Take(End - Start + 1).All(a => a.Value < Block.Size))
                    {
                        Min = new Item(0, "black");
                        Max = new Item(MaxItem, "black");
                    }
                    else if (Start <= End)
                        (Min, Max) = FindMinMax(Start, End);

                    MinMaxItems.TryAdd(i, (Min, Max));
                    WholeGap = true;

                    if (Ls.ItemAtStartOfGap > -1 && Ls.ItemAtStartOfGap == Ls.EqualityIndex)
                        Start = Ls.ItemAtStartOfGap;
                    else
                        WholeGap = false;

                    if ((Dots.Contains(i) || i == LineLength - 1) && LsEnd.Valid && LsEnd.Eq)
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

                            if (Item.Value < Min.Value && (WholeGap || !WasSolid || Block.CanBe(Item)))
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

                if (!Dots.Contains(i))
                {
                    if (!WasGap)
                        GapIndex = i - 1;

                    WasGap = true;
                }
                else if (Dots.Contains(i))
                    WasGap = false;

                if (!Points.ContainsKey(i) && WasSolid && Block.Size > MinItem)
                {
                    Item[] ItemsGTSolid = _Items.Where(w => w.Value >= Block.Size).ToArray();

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

        public int GetNumberOfBlocks(int start, int End)
        {
            int Count = 0;
            int Pos = start;
            while (Pos <= End)
            {
                Block? Block = GetBlock(Pos);
                Pos = Block?.End + 1 ?? End + 1;
                Count += Block != null ? 1 : 0;
            }
            return Count;
        }

        public Block? GetBlock(int start, bool forward = true)
        {
            Block? Block = null;
            for (int Pos = start; forward ? Pos < LineLength : Pos >= 0; Pos += (forward ? 1 : -1))
            {
                if (Points.TryGetValue(Pos, out string? Pt))
                {
                    int Start = forward && Block != null ? Block.Start : Pos;
                    int End = !forward && Block != null ? Block.End : Pos;
                    Block = new Block(Start, End, Pt);
                }
                else if (Block != null || Dots.Contains(Pos))
                    break;
            }

            return Block;
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

        public void AddDot(int index,
                           GriddlerPath.Action action,
                           bool fromPair = false)
        {
            if (!Dots.Contains(index) && index >= 0 && index <= LineLength - 1)
            {
                Dots.Add(index);

                if (!fromPair && _PairLines.TryGetValue(index, out Line? line))
                    line.AddDot(LineIndex, action, true);
            }
        }

        public void RemoveDot(int index, bool fromPair = false)
        {
            Dots.Remove(index);

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

                if (!fromPair && _PairLines.TryGetValue(index, out Line? line))
                    line.AddPoint(LineIndex, colour, action, item, true);

            }
        }

        public void RemovePoint(int index, bool fromPair = false)
        {
            Points.Remove(index);

            if (!fromPair && _PairLines.TryGetValue(index, out Line? line))
                line.RemovePoint(LineIndex, true);
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
