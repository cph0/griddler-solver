using System;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    /// <summary>
    /// This is returned from <see cref="Line.GetItemAtPosition(int, bool)"/>
    /// and <see cref="Line.GetItemAtPositionB(int, bool)"/>.  It contains collections
    /// of items around a position in a line.
    /// <para>
    /// If the position is X then the range after X is
    /// between <see cref="LastItemAtEquality"/> and <see cref="Index"/>
    /// </para>
    /// </summary>
    public class LineSegment : ItemRange
    {
        private readonly bool IsForward;

        /// <summary>
        /// The next item, if any
        /// </summary>
        public Item? Item { get; private set; }
        /// <summary>
        /// The index of the next item, possibly out of range
        /// </summary>
        public int Index { get; private set; }
        /// <summary>
        /// <see cref="true"/> if the <see cref="Item"/> exists
        /// </summary>
        public bool Valid => Item.HasValue;
        /// <summary>
        /// <see cref="true"/> if the next item index cannot be less than <see cref="Index"/>
        /// </summary>
        public bool Eq { get; private set; } //=> EqualityIndex == Index;
        public bool ScV => RightBefore.First().Complete;
        public int Sc => RightBefore.First().Size;
        /// <summary>
        /// The items that come before <see cref="Item"/>
        /// </summary>
        public ItemRange Before { get; private set; }
        /// <summary>
        /// The items including and after <see cref="Item"/>, may be empty
        /// </summary>
        public IEnumerable<Item> After { get; private set; }
        public HashSet<Block> RightBefore { get; private set; }
        /// <summary>
        /// The items that come after the last dot but are before <see cref="Item"/>
        /// </summary>
        public ItemRange Gap { get; private set; }
        /// <summary>
        /// The item index that is after the last dot
        /// </summary>
        public int ItemAtStartOfGap { get; private set; }
        /// <summary>
        /// The last item index that can be the next item
        /// </summary>
        public int EqualityIndex { get; private set; }
        public int IndexAtBlock { get; private set; }

        public LineSegment(IEnumerable<Item> items,
                            bool isForward,
                            Item? item,
                            bool eq,
                            ItemRange before,
                            IEnumerable<Item> after,
                            HashSet<Block> rB,
                            ItemRange gap,
                            int itemAtStartOfGap,
                            int equalityIndex) : base(items.ToArray(),
                                                      0,
                                                      0,
                                                      true)
        {
            IsForward = isForward;
            Item = item;

            if (item.HasValue)
                Index = item.Value.Index;
            else if (isForward)
                Index = 999;
            else
                Index = -1;

            Eq = eq;
            Before = before;
            After = after;
            RightBefore = rB;
            Gap = gap;
            ItemAtStartOfGap = itemAtStartOfGap;
            EqualityIndex = equalityIndex;
            IndexAtBlock = Index;
        }

        public LineSegment SetIndexAtBlock(int indexAtBlock,
                                           Item? item,
                                           IEnumerable<Item> next,
                                           ItemRange before,
                                           IEnumerable<Item> after,
                                           ItemRange gap) 
        {
            IndexAtBlock = indexAtBlock;
            Eq = false;
            ItemAtStartOfGap = Index;
            Index = indexAtBlock;
            Item = item;
            _ItemsArray = next.ToArray();
            Before = before;
            After = after;
            Gap = gap;
            return this;
        }

        public ItemRange With(LineSegment ls, bool gapOnly = false, bool equalityOnly = false)
        {
            (int Start, int End) = (EqualityIndex, ls.EqualityIndex);
            if (!equalityOnly)
            {
                Start = gapOnly ? Math.Min(Index, ls.Index) : Math.Max(EqualityIndex, ls.IndexAtBlock);
                End = gapOnly ? Math.Max(ls.Index, Index) : Math.Min(IndexAtBlock, ls.EqualityIndex);
            }
            return CreateRange(Start, End);
        }

        public bool GetItem(out Item? item)
        {
            item = Item;
            return item.HasValue;
        }

        public void Deconstruct(out Item? item,
                                out bool equality,
                                out int index,
                                out int equalityIndex,
                                out int indexAtBlock)
        {
            item = Item;
            equality = Eq;
            index = Index;
            equalityIndex = EqualityIndex;
            indexAtBlock = IndexAtBlock;
        }
    }
}
