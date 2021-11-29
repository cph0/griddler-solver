using System;

namespace Griddlers.Library;

/// <summary>
/// This is returned from <see cref="Line.GetItemAtPosition(int, bool)"/>
/// and <see cref="Line.GetItemAtPositionB(int, bool)"/>.  It contains collections
/// of items around a position in a line.
/// <para>
/// If the position is X then the range after X is
/// between <see cref="EqualityIndex"/> and <see cref="Index"/>
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
    /// <see cref="true"/> if the <see cref="Item"/> exists
    /// </summary>
    public bool Valid => Item.HasValue;
    /// <summary>
    /// The index of the next item, possibly out of range
    /// </summary>
    public int Index { get; private set; }
    /// <summary>
    /// The last item index that can be the next item
    /// </summary>
    public int EqualityIndex { get; private set; }
    /// <summary>
    /// <see cref="true"/> if the next item index cannot be less than <see cref="Index"/>
    /// </summary>
    public bool Eq => EqualityIndex == Index;
    public int IndexAtBlock { get; private set; }

    public LineSegment(Item[] items,
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
        EqualityIndex = equalityIndex;
        IndexAtBlock = index;
    }

    public void SetIndexAtBlock(int shift)
    {
        IndexAtBlock = Index;
        IndexAtBlock += (IsForward ? 1 : -1) * shift;

        if (IsForward)
            SetEnd(IndexAtBlock);
        else
            SetStart(IndexAtBlock);
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
