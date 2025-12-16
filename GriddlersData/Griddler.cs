using Griddlers.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Griddlers.Database;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA1707 // Identifiers should not contain underscores

public class Griddler
{
    [Key]
    public short griddler_id { get; set; }
    public string griddler_name { get; set; } = string.Empty;
    public byte width { get; set; }
    public byte height { get; set; }
    public DateTime start_date { get; set; }

    [NotMapped]
    public GriddlerItem[] Items { get; set; } = null!;

    [NotMapped]
    public GriddlerSolid[] Solids { get; set; } = null!;

    [NotMapped]
    public GriddlerPath[] Paths { get; set; } = null!;

    [NotMapped]
    public Item[][] Rows { get; set; } = null!;

    [NotMapped]
    public Item[][] Cols { get; set; } = null!;

    [NotMapped]
    public Dictionary<(int, int), Point> Pts { get; set; } = null!;
}

public class GriddlerItem
{
    [Key]
    public short griddler_item_id { get; set; }
    public short griddler_id { get; set; }
    public byte line_number { get; set; }
    public bool is_row { get; set; }
    public byte position { get; set; }
    public byte value { get; set; }
    public bool green { get; set; }
    public DateTime start_date { get; set; }
}

public class GriddlerSolid
{
    [Key]
    public int griddler_solid_id { get; set; }
    public short griddler_id { get; set; }
    public byte x_position { get; set; }
    public byte y_position { get; set; }
    public bool green { get; set; }
    public DateTime start_date { get; set; }
}

public class GriddlerPath
{
    [Key]
    public short griddler_path_id { get; set; }
    public short griddler_id { get; set; }
    public byte action_id { get; set; }
    public string action_name { get; set; } = string.Empty;
    public byte x_position { get; set; }
    public byte y_position { get; set; }
    public short group_num { get; set; }
    public byte sort_order { get; set; }
    public DateTime start_date { get; set; }


    public enum Action : byte
    {
        /// <summary>
        /// <para>1,2,3|-------- -> 0.00.000</para>
        /// Sum of items equals line length
        /// </summary>
        FullLine = 1,
        /// <summary>
        /// <para>1,4|-------- -> ----00--</para>
        /// Sum of items at least half the line
        /// </summary>
        OverlapLine = 2,
        /// <summary>
        /// <para>2,2,3|--.0---> |--.00--</para>
        /// Solid count is less than the min item
        /// </summary>
        MinItem = 3,
        /// <summary>
        /// <para>1,3,3|--.00-- -> |--.000.---</para>
        /// Solid count is greater than all items except biggest
        /// </summary>
        MaxItem = 4,
        /// <summary>
        /// <para>1,2,3|--00---- -> |-.00.----</para>
        /// Solid count equals item
        /// </summary>
        CompleteItem = 5,
        /// <summary>
        /// <para>1,2,3|0.-.00.000 -> |0...00.000</para>
        /// Sum of solids equals items.  Dots in gaps.
        /// </summary>
        FullLineDots = 6,
        /// <summary>
        /// <para>1,2,3|-0-0-----| -> |-0.00.---|</para>
        /// Item pushes the next item forwards
        /// </summary>
        LineForwardShift = 7,
        /// <summary>
        /// <para>1,2,3|.0.-0----0-| -> |.0.-0-.--0-|</para>
        /// Last item and item cannot reach here
        /// </summary>
        LineBackDots = 8,
        /// <summary>
        /// <para>1,2,3|-0--0-0---| -> |-0-00-0---|</para>
        /// Next item pushes the item backwards
        /// </summary>
        LineBackwardShift = 9,
        /// <summary>
        /// <para>1,2,3|0.--.000 -> |0.00.000</para>
        /// Sum of items equals gap length
        /// </summary>
        GapFull = 10,
        /// <summary>
        /// <para>1,2,3|0.------- -> 0.-0--00-</para>
        /// Eq or EqEnd
        /// </summary>
        GapOverlap = 11,
        /// <summary>
        /// <para>1,2,2,3,2|0---.---.-----0| -> |0---.-0-.-----0|</para>
        /// Item equals ItemEnd
        /// </summary>
        GapOverlapSameItem = 12,
        /// <summary>
        /// <para>1,2,2,3|0---.-.---| -> |0---...---|</para>
        /// All valid items greater than gap size
        /// </summary>
        GapDotsTooBig = 13,
        /// <summary>
        /// <para>2,2,3|0.-.---- -> |0...----</para>
        /// Min item is greater than gap size
        /// </summary>
        GapDotsMinItem = 14,
        /// <summary>
        /// <para>1,2,3|0.00.-.--0| -> |0.00.-.--0|</para>
        /// Eq and EqEnd and sum greater than gap size
        /// </summary>
        GapDotsSum = 15,
        /// <summary>
        /// <para>1,2,3|0.-.---- -> |0...----</para>
        /// Eq and next item bigger than gap
        /// </summary>
        GapDotsSumF = 16,
        /// <summary>
        /// <para>1,2,3//---.-.000| ->---...000|</para>
        /// EqEnd and next item bigger than gap
        /// </summary>
        GapDotsSumB = 17,
        /// <summary>
        /// <para>1,2,3|0.0-.0--.--- -> |0.0-.0--....para>
        /// Eq and no more items left at end
        /// </summary>
        GapDotsNoMoreItemsF = 18,
        /// <summary>
        /// <para>1,2,3//---.-0.-0.--0| -> ....-0.-0.--0|</para>
        /// EqEnd and no more items left at start
        /// </summary>
        GapDotsNoMoreItemsB = 19,
        /// <summary>
        /// <para>2,2,1,1,1,2,1,1// --0--.0| to --0.0.0|</para>
        /// Between solid and end, next item equals length
        /// </summary>
        HalfGapFullPart = 20,
        /// <summary>
        /// <para>...,2,2//.--0----| to .--0--0-|</para>
        /// Between solid and end, next item is over half the length
        /// </summary>
        HalfGapOverlap = 21,
        /// <summary>
        /// <para>1,1,4,...//|.--.0.0-000- to |.--.0.0.000-</para>
        /// Two solids cannot join         
        /// </summary>
        NoJoin = 22,
        /// <summary>
        /// <para>1,9,...//|.---0-0--0--- to |.---0-000000-</para>
        /// Two solids must join
        /// </summary>
        MustJoin = 23,
        /// <summary>
        /// <para>1,1,1,4,...//|------0--// to |-----.0--</para>
        /// All items before equal solid count and sum fits exactly
        /// </summary>
        SumDotForward = 24,
        /// <summary>
        /// <para>...,3,1,1,3//-0----.000.|// to -0.---.000.|</para>
        /// All items after equal solid count and sum fits exactly
        /// </summary>
        SumDotBackward = 25,
        /// <summary>
        /// <para>1,9,3// |---0-0------------0 to |---0-0--------..--0</para>
        /// Two isolated items cannot reach here
        /// </summary>
        IsolatedItemsReach = 26,
        /// <summary>
        /// <para>1,2,3|0.-0--.000| to |0.-0-..000|</para>
        /// Item cannot reach gap end and no more items
        /// </summary>
        ItemForwardReach = 27,
        /// <summary>
        /// <para>1,2,3|0.--0-.000| to |0..-0-.000|</para>
        /// Item cannot reach gap start and no more items
        /// </summary>
        ItemBackwardReach = 28,
        /// <summary>
        /// <para>1,2,3|0.----.000| to |0.0---.000|</para>
        /// Try a point at the first possible position
        /// </summary>
        TrialAndError = 29
    }
}
