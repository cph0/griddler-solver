using System.Collections;
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
    public class LineSegment : ItemRange, IEnumerable<Item>
    {
        private IEnumerable<Item> _Items => _ItemsEnum;
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
        public bool Valid => Item != (object?)null;
        /// <summary>
        /// <see cref="true"/> if the next item index cannot be less than <see cref="Index"/>
        /// </summary>
        public bool Eq { get; private set; }
        public bool ScV => RightBefore.First().Complete;
        public int Sc => RightBefore.First().SolidCount;
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

        public LineSegment(IEnumerable<Item> items,
                            bool isForward,
                            Item? item,
                            bool eq,
                            ItemRange before,
                            IEnumerable<Item> after,
                            HashSet<Block> rB,
                            ItemRange gap,
                            int itemAtStartOfGap,
                            int lastItemAtEquality) : base(items)
        {
            IsForward = isForward;
            Item = item;

            if (item != (object?)null)
                Index = item.Index;
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
            LastItemAtEquality = lastItemAtEquality;
        }

        public ItemRange With(LineSegment ls, bool includeItem = true)
        {
            if (IsForward && Index <= ls.Index) //F, F | B
                return new ItemRange(After.Where(w => w.Index < ls.Index || (includeItem && w.Index == ls.Index)));
            else if (Index <= ls.Index) // B B
                return new ItemRange(ls.After.Where(w => w.Index > Index || (includeItem && w.Index == Index)));
            else if (!ls.IsForward) // F | B, B
                return new ItemRange(ls.Where(w => w.Index <= Index));
            else // F F
                return new ItemRange(ls.After.Where(w => w.Index <= Index));
        }

        public ItemRange With(int index)
        {
            return new ItemRange(After.Where(w => w.Index <= index));
        }

        public bool GetItem(out Item? item)
        {
            item = Item;
            return item != (object?)null;
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
