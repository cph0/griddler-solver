using System;
using System.Collections;
using System.Collections.Generic;

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
    public class LineSegment : ItemRange, IEnumerable<Item>
    {
        private new IEnumerable<Item> _Items => base._Items;
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
        public bool Eq => LastItemAtEquality == Index;
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
        public int LastItemAtEquality { get; private set; }

        public int IndexAtBlock { get; private set; }

        public void SetIndexAtBlock(int shift)
        {
            IndexAtBlock = Index;
            IndexAtBlock += (IsForward ? 1 : -1) * shift;

            if (IsForward)
                SetEnd(IndexAtBlock);
            else
                SetStart(IndexAtBlock);
        }

        public LineSegment(IEnumerable<Item> items,
                           bool isForward,
                           Item? item,
                           int index,
                           int equalityIndex) : base(items,
                                                     isForward ? equalityIndex : index,
                                                     isForward ? index : equalityIndex)
        {
            IsForward = isForward;
            Item = item;
            Index = index;
            LastItemAtEquality = equalityIndex;
            IndexAtBlock = index;

            Before = new();
            After = Array.Empty<Item>();
            RightBefore = new();
            Gap = new();
        }

        public ItemRange With(LineSegment ls, bool gapOnly = false)
        {
            int Start = gapOnly ? Index : Math.Max(LastItemAtEquality, ls.IndexAtBlock);
            int End = gapOnly ? ls.Index : Math.Min(IndexAtBlock, ls.LastItemAtEquality);
            return CreateRange(Start, End);
        }

        public bool GetItem(out Item? item)
        {
            item = Item;
            return item.HasValue;
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return ((IEnumerable<Item>)_Items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
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
            equalityIndex = LastItemAtEquality;
            indexAtBlock = IndexAtBlock;
        }
    }
}
