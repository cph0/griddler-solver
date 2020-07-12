using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Griddlers.Library
{
    public class LineSegment : ItemRange, IEnumerable<Item>
    {
        private IEnumerable<Item> _Items => _ItemsEnum;
        private readonly bool IsForward;

        public bool NoMoreItems { get; private set; }
        public Item? Item { get; private set; }
        public int Index { get; private set; }
        public bool Valid => Item != (object?)null;
        public bool Eq { get; private set; }
        public bool ScV => RightBefore.First().Complete;
        public int Sc => RightBefore.First().SolidCount;
        public ItemRange Before { get; private set; }
        public IEnumerable<Item> After { get; private set; }
        public HashSet<Block> RightBefore { get; private set; }
        public ItemRange Gap { get; private set; }
        public int ItemAtStartOfGap { get; private set; }
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
