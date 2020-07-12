using Griddlers.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Griddlers.Library
{
    public class Logic
    {
        public static Dictionary<(int, int), Point> dots = new Dictionary<(int, int), Point>();
        public static Dictionary<(int, int), Point> points = new Dictionary<(int, int), Point>();
        private static IReadOnlyDictionary<int, Line> Rows = new Dictionary<int, Line>();
        private static IReadOnlyDictionary<int, Line> Cols = new Dictionary<int, Line>();
        private static readonly IDictionary<string, int> MethodCounts = new Dictionary<string, int>();

        public static void AddMethodCount(string key)
        {
            if (MethodCounts.ContainsKey(key))
                MethodCounts[key]++;
            else
                MethodCounts[key] = 1;
        }

        public static (Dictionary<(int, int), Point>, Dictionary<(int, int), Point>) Run(int width, int height, (Item[][], Item[][]) data)
            => Run(width, height, data.Item1, data.Item2);

        public static (Dictionary<(int, int), Point>, Dictionary<(int, int), Point>) Run(int width, int height, Item[][] rows, Item[][] columns)
        {
            points = new Dictionary<(int, int), Point>(width * height);
            dots = new Dictionary<(int, int), Point>(width * height);
            Point.Group = 0;
            int Count = 0;
            int LoopCount = -1;

            Rows = rows.Select((s, si) => new Line(si, true, width, s)).ToDictionary(k => k.LineIndex);
            Cols = columns.Select((s, si) => new Line(si, false, height, s)).ToDictionary(k => k.LineIndex);

            //full rows <5ms
            FullLine(Rows.Values);

            //full columns <5ms
            FullLine(Cols.Values);

            //overlapping rows <20ms
            OverlapLine(Rows.Values);

            //overlapping columns <20ms
            OverlapLine(Cols.Values);

            while (!IsComplete(Rows.Values))
            {
                int PointsChange = points.Count;
                int DotsChange = dots.Count;
                bool StrayDog = false;

                if (Count == LoopCount)
                    break;

                //row top edge <20ms
                LineEdgeTL(Rows.Values);

                //column left edge <20ms
                LineEdgeTL(Cols.Values);

                //row bottom edge <20ms
                LineEdgeBR(Rows.Values);

                //column right edge <20ms
                LineEdgeBR(Cols.Values);
                
                //row full line dots <5ms
                FullLineDots(Rows.Values);

                //column full line dots <5ms
                FullLineDots(Cols.Values);

                //row line dots <300ms
                LineDots(Rows.Values);

                //column line dots <300ms
                LineDots(Cols.Values);

                //row line gaps <10ms
                LineGaps(Rows.Values);

                //column line gaps <10ms
                LineGaps(Cols.Values);

                foreach ((int, int) Item in points.Keys.Where(w => dots.ContainsKey(w)))
                {
                    Console.WriteLine($"stray dog x:{Item.Item1}y:{Item.Item2}");
                    StrayDog = true;
                }

                //busker30x30 - does not solve
                //dots.TryAdd((6, 9), new Point(6, 9, false));
                //dots.TryAdd((6, 11), new Point(6, 11, false));
                //points.TryAdd((0, 13), new Point(0, 13, true));
                //points.TryAdd((0, 14), new Point(0, 14, true));
                //points.TryAdd((1, 11), new Point(1, 11, true));
                //points.TryAdd((2, 11), new Point(2, 11, false));
                //points.TryAdd((2, 8), new Point(2, 8, true));
                //points.TryAdd((3, 13), new Point(3, 13, false));
                //points.TryAdd((0, 15), new Point(0, 15, false));
                //points.TryAdd((7, 23), new Point(7, 23, true));
                //points.TryAdd((7, 25), new Point(7, 25, false)); //bug

                if (PointsChange == points.Count && DotsChange == dots.Count)
                    break;

                if (StrayDog)
                    break;

                Count++;
            }

            Console.WriteLine($"Loop Count: {Count}");
            foreach (KeyValuePair<string, int> Method in MethodCounts)
            {
                Console.WriteLine($"{Method.Key}: {Method.Value}");
            }

            return (points, dots);
        }

        private static bool IsComplete(IEnumerable<Line> lines)
        {
            bool Complete = true;

            ForEachLine(lines, (Line line) =>
            {
                if (line.GetLinePointsValue(true) != line.LineLength)
                {
                    Complete = false;
                    return true;
                }
                else
                    line.IsComplete = true;

                return false;
            });

            return Complete;
        }

        public static void AddPoints(Line line, int start, bool green, GriddlerPath.Action action, int? end = null, bool dot = false)
        {
            for (int Pos = start; Pos <= end.GetValueOrDefault(start); Pos++)
            {
                var Xy = line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos);

                if (dot)
                    dots.TryAdd(Xy, new Point(Xy, green, action));
                else
                    points.TryAdd(Xy, new Point(Xy, green, action));

                if (line.IsRow)
                {
                    Rows[line.LineIndex].ClearCaches(start);
                    Cols[Pos].ClearCaches(line.LineIndex);
                }
                else
                {
                    Cols[line.LineIndex].ClearCaches(start);
                    Rows[Pos].ClearCaches(line.LineIndex);
                }
            }
        }

        private static void ForEachLine(IEnumerable<Line> lines, Func<Line, bool> logic, Func<Line, bool>? run = null, int minLineValue = 1)
        {
            foreach (Line L in lines)
            {
                if (L.LineValue >= minLineValue && !L.IsComplete && (run == null || run(L)))
                    logic(L);
            }
        }

        public static void ForEachLinePos(Line line, Func<int, (int, int), Block, bool, int, int, int> logic, Func<bool, bool, int, (int, int), int, bool>? run = null, Predicate<int>? stop = null, bool b = false, int start = 0)
        {
            bool WasSolid = false;
            int SolidBlockCount = 0, GapSize = 0, SolidCount = 0;
            Block Block = new Block(0, false);
            for (int Pos = b ? line.LineLength - 1 : start; b ? Pos >= start : Pos < line.LineLength; Pos += (b ? -1 : 1))
            {
                var Xy = line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos);
                bool ChangedColour = false;

                if (stop != null && stop(Pos))
                    break;

                if (points.TryGetValue(Xy, out Point? Pt) && Block.Green != Pt.Green)
                    ChangedColour = true;

                if (run == null || run(Pt == (object?)null || ChangedColour, WasSolid, Pos, Xy, GapSize))
                    Pos += (b ? -1 : 1) * logic(Pos, Xy, Block, WasSolid, GapSize, SolidCount);

                if (!dots.ContainsKey(Xy))
                    GapSize++;
                else
                    GapSize = 0;

                if (points.TryGetValue(Xy, out Pt))
                {
                    if (Block.Green != Pt.Green)
                    {
                        if (Block.SolidCount > 0)
                            SolidBlockCount++;

                        Block = new Block(SolidBlockCount, Pt.Green);
                    }

                    if (!WasSolid || ChangedColour)
                        Block.StartIndex = Pos;

                    Block.EndIndex = Pos;
                    Block.SolidCount++;
                    SolidCount++;

                    WasSolid = true;
                }
                else
                {
                    if (WasSolid)
                    {
                        SolidBlockCount++;
                        Block = new Block(SolidBlockCount, false);
                        SolidCount = 0;
                    }

                    WasSolid = false;
                }
            }
        }


        private static void FullPart(Line line, int linePosition, int startItem, int endItem, GriddlerPath.Action action, bool complete = true)
        {
            Point.Group++;

            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                bool Green = Item.Green;
                AddPoints(line, linePosition, Green, action, linePosition + Item.Value - 1);
                if (complete)
                    line.AddBlock(Item, true, linePosition, linePosition + Item.Value - 1);
                linePosition += Item.Value;
                if (linePosition < line.LineLength - 1 && line.GetDotCount(Item.Index) == 1)
                {
                    AddPoints(line, linePosition, Green, action, dot: true);
                    linePosition++;
                }
            }
        }

        private static void OverlapPart(Line line, int position, int lineEnd, int startItem, int endItem, GriddlerPath.Action action)
        {
            int LinePosition = position;
            bool[,] LineFlagsForward = new bool[lineEnd + 1, endItem + 1];
            bool[,] LineFlagsBackward = new bool[lineEnd + 1, endItem + 1];
            Point.Group++;

            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                for (int Pos = 0; Pos < Item.Value; Pos++)
                {
                    LineFlagsForward[LinePosition, Item.Index] = true;
                    LinePosition++;
                }
                LinePosition += line.GetDotCount(Item.Index);
            }
            LinePosition = 0;
            foreach (Item Item in line.Reverse(endItem).Take(endItem - startItem + 1))
            {
                for (int Pos = 0; Pos < Item.Value; Pos++)
                {
                    LineFlagsBackward[lineEnd - LinePosition, Item.Index] = true;
                    LinePosition += 1;
                }
                LinePosition += line.GetDotCountB(Item.Index);
            }
            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                bool Any = false;
                for (int Pos = position; Pos <= lineEnd; Pos++)
                {
                    if (LineFlagsForward[Pos, Item.Index] && LineFlagsBackward[Pos, Item.Index])
                    {
                        AddPoints(line, Pos, Item.Green, action);
                        Any = true;
                    }
                }

                if (Any)
                    line.AddBlock(Item);
            }
        }

        private static void FullLine(IEnumerable<Line> lines)
        {
            ForEachLine(lines, (Line line) =>
            {
                line.IsComplete = true;
                FullPart(line, 0, 0, line.LineItems - 1, GriddlerPath.Action.FullLine);
                return false;
            }, (line) => line.LineValue == line.LineLength);
        }

        private static void OverlapLine(IEnumerable<Line> lines)
        {
            ForEachLine(lines, (Line line) =>
            {
                OverlapPart(line, 0, line.LineLength - 1, 0, line.LineItems - 1, GriddlerPath.Action.OverlapLine);
                return false;
            }, (line) => line.LineValue < line.LineLength && line.LineValue > line.LineLength / 2);
        }

        private static void FullLineDots(IEnumerable<Line> lines)
        {
            ForEachLine(lines, (Line line) =>
            {
                Point.Group++;
                line.IsComplete = true;

                for (int Pos = 0; Pos < line.LineLength; Pos++)
                    if (!points.ContainsKey(line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos)))
                        AddPoints(line, Pos, false, GriddlerPath.Action.FullLineDots, dot: true);
                return false;
            }, (line) => line.LineValue - line.GetDotCount() == line.GetLinePointsValue(), minLineValue: 0);
        }

        private static void LineEdgeTL(IEnumerable<Line> lines)
        {
            ForEachLine(lines, (Line line) =>
            {
                int LinePosition = 0, CurrentItem = 0, EndIndex = 0;
                bool HadGap = false, ItemsOneValue = line.ItemsOneValue;
                ForEachLinePos(line, (int c, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) =>
                {
                    //check for gap
                    bool NoMoreItems = false;
                    if (CurrentItem != line.LineItems - 1 && points.ContainsKey(xy))
                    {
                        (int GapStart, int GapEnd, _) = line.FindGapStartEnd(c);

                        if (GapStart - LinePosition < line[CurrentItem].Value
                            && line[CurrentItem] + line[CurrentItem + 1] > GapEnd - GapStart - 1)
                            NoMoreItems = true;
                        else
                        {
                            LineSegment LsEnd = line.GetItemAtPosB(GapEnd - 1);

                            if (LsEnd.Valid && LsEnd.Eq && LsEnd.Index == CurrentItem)
                                NoMoreItems = true;
                        }
                    }

                    int DotCount = CurrentItem > 0 ? line.GetDotCount(CurrentItem - 1) : 0;
                    if ((c - LinePosition - DotCount < line[CurrentItem].Value || NoMoreItems || ItemsOneValue)
                        && points.ContainsKey(xy))
                    {
                        bool EndDot = false;
                        int ShiftCount = 0, CompleteCount = 0;

                        //--O--O
                        if (c > 0 && c + line[CurrentItem].Value < line.LineLength)
                        {
                            bool FirstSolid = false;
                            string HasError = "";

                            for (int d = c + line[CurrentItem].Value; d >= c; d--)
                            {
                                xy = line.IsRow ? (d, line.LineIndex) : (line.LineIndex, d);
                                if (points.TryGetValue(xy, out Point? Pt)
                                    && Pt == line[CurrentItem])
                                {
                                    if (d == c + line[CurrentItem].Value)
                                        FirstSolid = true;
                                    else if(d < c + line[CurrentItem].Value)
                                        CompleteCount++;

                                    if (FirstSolid)
                                        ShiftCount = line[CurrentItem].Value - (d - c) + 1;
                                }
                                else
                                {
                                    if (dots.ContainsKey(xy) || Pt != line[CurrentItem])
                                        ShiftCount = line[CurrentItem].Value - (d - c);

                                    CompleteCount = 0;
                                    FirstSolid = false;
                                }
                            }
                            if (CompleteCount == line[CurrentItem].Value)
                            {
                                Point.Group++;

                                if (!NoMoreItems && !ItemsOneValue)
                                    line.AddBlock(line[CurrentItem], true, c, c + CompleteCount - 1);

                                if (line.ShouldAddDots(CurrentItem).Item1)
                                {
                                    EndDot = true;
                                    AddPoints(line, c - 1, false, GriddlerPath.Action.CompleteItem, dot: true);
                                }
                                if (c + CompleteCount < line.LineLength && line.ShouldAddDots(CurrentItem).Item2)
                                {
                                    EndDot = true;
                                    AddPoints(line, c + CompleteCount, false, GriddlerPath.Action.CompleteItem, dot: true);
                                }
                            }
                            else if (CompleteCount > line[CurrentItem].Value)
                                HasError = "lineEdgeTL: ahead gap filled";

                            //check for next solid bigger than next item + no room
                            if (!FirstSolid && CurrentItem < line.LineItems - 1 && line.ItemsOneColour)
                            {
                                bool Flag = true;
                                int NextItemSolidCount = 0;

                                for (int d = c + 1; d <= c + line[CurrentItem].Value; d++)
                                {
                                    xy = line.IsRow ? (d, line.LineIndex) : (line.LineIndex, d);
                                    if (dots.ContainsKey(xy))
                                    {
                                        Flag = false;
                                        break;
                                    }
                                }

                                for (int d = c + line[CurrentItem].Value + line.GetDotCount(CurrentItem); d < line.LineLength; d++)
                                {
                                    xy = line.IsRow ? (d, line.LineIndex) : (line.LineIndex, d);
                                    if (points.TryGetValue(xy, out Point? Pt)
                                        && Pt?.Green == line[CurrentItem].Green)
                                        NextItemSolidCount++;
                                    else
                                        break;
                                }

                                if (Flag && NextItemSolidCount > line[CurrentItem + 1].Value
                                        //&& line[CurrentItem + 1].Value > 1
                                        )
                                    ShiftCount += line[CurrentItem + 1].Value + line.GetDotCount(CurrentItem);
                            }

                            if (c - ShiftCount < 0)
                                HasError = "lineEdgeTL: before shift no room";

                            if (!string.IsNullOrEmpty(HasError))
                                Console.WriteLine(HasError);

                            if (ShiftCount > 0)
                                Point.Group++;

                            AddPoints(line, c - ShiftCount, line[CurrentItem].Green, GriddlerPath.Action.LineBackwardShift, c - 1);
                        }

                        if (line[CurrentItem].Value >= c - LinePosition + 1)
                            Point.Group++;

                        //forward shift and end dot
                        AddPoints(line, c + 1, line[CurrentItem].Green, GriddlerPath.Action.LineForwardShift, LinePosition + line[CurrentItem].Value - 1);
                        if (c - LinePosition == 0 && c + line[CurrentItem].Value < line.LineLength && !EndDot
                            && line.ShouldAddDots(CurrentItem).Item2)
                        {
                            AddPoints(line, c + line[CurrentItem].Value, false, GriddlerPath.Action.LineForwardShift, dot: true);
                        }

                        line.AddBlock(line[CurrentItem],
                                      c - LinePosition == 0,
                                      c + 1,
                                      LinePosition + line[CurrentItem].Value - 1);

                        //back dots
                        int BackReach = c - line[CurrentItem].Value + (CompleteCount - 1);
                        if (CompleteCount > 0 && (c - LinePosition - 1 < line[CurrentItem].Value || !ItemsOneValue || line.LineItems == 1)
                            && BackReach >= EndIndex)
                        {
                            Point.Group++;
                            AddPoints(line, EndIndex, false, GriddlerPath.Action.LineBackDots, BackReach, true);
                        }

                        if (CurrentItem == line.LineItems - 1)
                            return line.LineLength;

                        //3,4|---0-{LP}-{EI}----00
                        EndIndex = c + line[CurrentItem].Value;
                        LinePosition += (line[CurrentItem].Value + line.GetDotCount(CurrentItem));
                        CurrentItem++;
                        return (line[CurrentItem - 1].Value + line.GetDotCount(CurrentItem - 1) - 1 - ShiftCount);
                    }
                    else if (dots.ContainsKey(xy) && !HadGap)
                        LinePosition = c + 1;
                    else
                        HadGap = true;

                    return 0;
                });
                return false;
            });
        }

        private static void LineEdgeBR(IEnumerable<Line> lines)
        {
            ForEachLine(lines, (Line line) =>
            {
                int LinePosition = 0, CurrentItem = line.LineItems - 1, EndIndex = line.LineLength - 1;
                bool HadGap = false, ItemsOneValue = line.ItemsOneValue;
                ForEachLinePos(line, (int c, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) =>
                {
                    //check for gap
                    bool NoMoreItems = false;
                    if (CurrentItem != 0 && points.ContainsKey(xy) && !ItemsOneValue
                        && line.LineLength - c - 1 - LinePosition - 1 >= line[CurrentItem].Value)
                    {
                        (int GapStart, int GapEnd, _) = line.FindGapStartEnd(c);

                        if (line.LineLength - GapEnd - 1 - LinePosition < line[CurrentItem].Value
                            && line[CurrentItem] + line[CurrentItem - 1] > GapEnd - GapStart - 1)
                            NoMoreItems = true;
                        else
                        {
                            LineSegment Ls = line.GetItemAtPos(GapStart + 1);
                            if (Ls.Valid && Ls.Eq && Ls.Index == CurrentItem)
                                NoMoreItems = true;
                        }
                    }

                    if (points.ContainsKey(xy)
                        && (line.LineLength - c - 1 - LinePosition - 1 < line[CurrentItem].Value || NoMoreItems || ItemsOneValue))
                    {
                        bool EndDot = false;
                        int ShiftCount = 0, CompleteCount = 0;

                        if (c < line.LineLength - 1)
                        {
                            //--O--O
                            bool FirstSolid = false;
                            string HasError = "";
                            for (int d = c - line[CurrentItem].Value; d <= c; d++)
                            {
                                xy = line.IsRow ? (d, line.LineIndex) : (line.LineIndex, d);
                                if (points.TryGetValue(xy, out Point? Pt)
                                    && Pt == line[CurrentItem])
                                {
                                    if (d == c - line[CurrentItem].Value)
                                        FirstSolid = true;
                                    else
                                        CompleteCount++;

                                    if (FirstSolid)
                                        ShiftCount = line[CurrentItem].Value - (c - d) + 1;
                                }
                                else
                                {
                                    if (dots.ContainsKey(xy) || Pt != line[CurrentItem])
                                        ShiftCount = line[CurrentItem].Value - (c - d);

                                    CompleteCount = 0;
                                    FirstSolid = false;
                                }
                            }
                            if (CompleteCount == line[CurrentItem].Value)
                            {
                                Point.Group++;

                                if (!NoMoreItems && !ItemsOneValue)
                                    line.AddBlock(line[CurrentItem], true, c - CompleteCount + 1, c);

                                if (line.ShouldAddDots(CurrentItem).Item2)
                                {
                                    EndDot = true;
                                    AddPoints(line, c + 1, false, GriddlerPath.Action.CompleteItem, dot: true);
                                }
                                if (c - CompleteCount >= 0 && line.ShouldAddDots(CurrentItem).Item1)
                                {
                                    EndDot = true;
                                    AddPoints(line, c - CompleteCount, false, GriddlerPath.Action.CompleteItem, dot: true);
                                }
                            }
                            else if (CompleteCount > line[CurrentItem].Value)
                                HasError = "lineEdgeBR: before gap filled";

                            if (c + ShiftCount > line.LineLength)
                                HasError = "lineEdgeBR: ahead shift no room";

                            if (ShiftCount > 0)
                                Point.Group++;

                            AddPoints(line, c + 1, line[CurrentItem].Green, GriddlerPath.Action.LineForwardShift, c + ShiftCount);

                            if (!string.IsNullOrEmpty(HasError))
                                Console.WriteLine(HasError);
                        }

                        if (line[CurrentItem].Value >= (line.LineLength - c - 1) - LinePosition)
                            Point.Group++;

                        //forward shift and end dot
                        AddPoints(line, c + 1 - (line[CurrentItem].Value - ((line.LineLength - c - 1) - LinePosition)), line[CurrentItem].Green, GriddlerPath.Action.LineBackwardShift, c - 1);
                        if (!EndDot && (line.LineLength - c - 1) - LinePosition == 0
                            && line.LineLength - line[CurrentItem].Value - LinePosition > 0
                            && line.ShouldAddDots(CurrentItem).Item1)
                        {
                            AddPoints(line, c - line[CurrentItem].Value, false, GriddlerPath.Action.LineBackwardShift, dot: true);
                        }

                        line.AddBlock(line[CurrentItem],
                                      (line.LineLength - c - 1) - LinePosition == 0,
                                      c + 1 - (line[CurrentItem].Value - ((line.LineLength - c - 1) - LinePosition)),
                                      c - 1);

                        //back dots
                        int BackReach = c + line[CurrentItem].Value - (CompleteCount - 1);
                        if (CompleteCount > 0 && BackReach <= EndIndex &&
                            (line.LineLength - c - 1 - LinePosition - 1 < line[CurrentItem].Value
                            || !ItemsOneValue || line.LineItems == 1))
                        {
                            Point.Group++;
                            AddPoints(line, BackReach, false, GriddlerPath.Action.LineBackDots, EndIndex, true);
                        }

                        if (CurrentItem == 0)
                            return line.LineLength;

                        EndIndex = c - line[CurrentItem].Value;
                        LinePosition += (line[CurrentItem].Value + line.GetDotCountB(CurrentItem));
                        CurrentItem--;
                        return (line[CurrentItem + 1].Value + line.GetDotCountB(CurrentItem + 1) - 1 - ShiftCount);
                    }
                    else if (points.ContainsKey(xy))
                        return line.LineLength;
                    else if (dots.ContainsKey(xy) && !HadGap)
                        LinePosition = line.LineLength - c;
                    else
                        HadGap = true;

                    return 0;
                }, b: true);
                return false;
            });
        }

        private static void LineGaps(IEnumerable<Line> lines)
        {
            ForEachLine(lines, (Line line) =>
            {
                bool FirstSolid = false;
                ForEachLinePos(line, (int c, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) =>
                {
                    if (dots.ContainsKey(xy) || c == line.LineLength - 1)
                    {
                        int IsEnd = (!dots.ContainsKey(xy) && c == line.LineLength - 1) ? 1 : 0;

                        if (IsEnd == 1)
                            gapSize++;

                        if (gapSize > 0)
                        {
                            LineSegment Ls = line.GetItemAtPos(c - gapSize + IsEnd);
                            LineSegment LsEnd = line.GetItemAtPosB(c - 1 + IsEnd);
                            if (line.MinItem > gapSize)
                                AddPoints(line, c - gapSize + IsEnd, false, GriddlerPath.Action.GapDotsMinItem, c + IsEnd - 1, true);
                            else
                            {
                                //multiple solids
                                ItemRange Range = Ls.With(LsEnd);
                                int Sum = Range.Sum();

                                bool NextColourSumTooBig = false;
                                Item FirstColourItem = LsEnd.After.LastOrDefault(l => l.Index < LsEnd.Index &&
                                                                l.Green == Ls.RightBefore.First().Green);
                                if (LsEnd.Valid && LsEnd.Eq && FirstColourItem != (object?)null
                                    && Ls.RightBefore.First().EndIndex == c - gapSize + IsEnd - 2)
                                {
                                    int ColourSum = new ItemRange(LsEnd.After.Where(w => w.Index > FirstColourItem.Index)).Sum();
                                    if(ColourSum > gapSize)
                                        NextColourSumTooBig = true;
                                }

                                if (Range.All(a => a.Value > gapSize)
                                    || (Ls.Eq && LsEnd.Eq && Sum > gapSize)
                                    || (Ls.Eq && Ls.Valid && Ls.Item?.Value > gapSize)
                                    || (LsEnd.Eq && LsEnd.Valid && LsEnd.Item?.Value > gapSize)
                                    || (Ls.Eq && Ls.Index > line.LineItems - 1)
                                    || (LsEnd.Eq && LsEnd.Index < 0)
                                    || (Ls.Eq && LsEnd.Eq && Ls.Index > LsEnd.Index)
                                    || (LsEnd.Valid && Ls.Eq && LsEnd.LastItemAtEquality < Ls.Index)
                                    || (Ls.Valid && LsEnd.Eq && Ls.LastItemAtEquality > LsEnd.Index)
                                    || (NextColourSumTooBig)
                                    || (Ls.Eq && LsEnd.ScV && Ls.Index < line.LineItems - 1 && line[Ls.Index + 1].Value > LsEnd.Sc
                                        && line[Ls.Index + 1] + line[Ls.Index] > gapSize)
                                    || (LsEnd.Eq && Ls.ScV && LsEnd.Index > 0 && line[LsEnd.Index - 1].Value > Ls.Sc
                                        && line[LsEnd.Index - 1] + line[LsEnd.Index] > gapSize)
                                    )
                                {
                                    GriddlerPath.Action Action = GriddlerPath.Action.GapDotsTooBig;
                                    //if (Eq && EqEnd && Sum > gapSize)
                                    //    Action = GriddlerPath.Action.GapDotsSum;
                                    //else if (Valid && Eq && line[Item].Value > gapSize)
                                    //    Action = GriddlerPath.Action.GapDotsSumF;
                                    //else if (ValidEnd && EqEnd && line[ItemEnd].Value > gapSize)
                                    //    Action = GriddlerPath.Action.GapDotsSumB;
                                    //else if (Eq && Item > line.LineItems - 1)
                                    //    Action = GriddlerPath.Action.GapDotsNoMoreItemsF;
                                    //else if (EqEnd && ItemEnd < 0)
                                    //    Action = GriddlerPath.Action.GapDotsNoMoreItemsB;

                                    Point.Group++;
                                    AddPoints(line, c - gapSize + IsEnd, false, Action, c + IsEnd - 1, true);
                                }
                                else if (Ls.Valid && (LsEnd.Valid
                                    || (Ls.Eq && Ls.Index > 0 && LsEnd.ScV && line[Ls.Index].Value != LsEnd.Sc)
                                    ) && Sum == gapSize)
                                    FullPart(line, c - gapSize + IsEnd, Ls.Index, LsEnd.Index, GriddlerPath.Action.GapFull);
                                else if (Ls.Valid && FirstSolid && Ls.Index <= LsEnd.Index && Ls.GroupBy(g => g.Value).Count() == 1)
                                    FullPart(line, c - gapSize + IsEnd, Ls.Index, Ls.Index, GriddlerPath.Action.GapFull, false);
                                else if (Ls.Valid && LsEnd.Valid && (Ls.Index == LsEnd.Index || Ls.Eq || LsEnd.Eq)
                                    && Sum < gapSize && Sum > gapSize / 2)
                                {
                                    GriddlerPath.Action Action = GriddlerPath.Action.GapOverlapSameItem;
                                    if (Ls.Eq || LsEnd.Eq)
                                        Action = GriddlerPath.Action.GapOverlap;

                                    OverlapPart(line, c - gapSize + IsEnd, c - 1 + IsEnd, Ls.Index, LsEnd.Index, Action);
                                }
                            }
                            FirstSolid = false;
                        }
                    }
                    else
                    {
                        if (gapSize == 0 && points.ContainsKey(xy))
                            FirstSolid = true;
                    }
                    return 0;
                });
                return false;
            });
        }

        private static void LineDots(IEnumerable<Line> lines)
        {
            ForEachLine(lines, (Line line) =>
            {
                Item MinItem = new Item(line.MinItem, false), MaxItem = new Item(line.MaxItem, false);
                int MaxItemIndex = -1, IsolatedItem = 0;
                (bool LineIsolated, bool Valid, IDictionary<int, int> Isolations) = line.IsLineIsolated();
                IDictionary<int, (Item, Item)> MinMaxItems = line.GetMinMaxItems();

                foreach (Item Item in line)
                {
                    if (Item.Value == MaxItem.Value)
                        MaxItemIndex = Item.Index;
                }
                ForEachLinePos(line, (int c, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) =>
                {
                    bool Break = false, NoMoreItems = line.UniqueCount(block);
                    LineSegment Ls = line.GetItemAtPos(c + 1, false);
                    LineSegment LsEnd = line.GetItemAtPosB(c - 1, false);
                    (int StartOfGap, int EndOfGap, bool WasSolid) = line.FindGapStartEnd(c - 1, c);
                    IsolatedItem = block.BlockIndex;

                    if (MinMaxItems.TryGetValue(c, out (Item, Item) M))
                    {
                        MinItem = M.Item1;
                        MaxItem = M.Item2;
                    }
                    else
                    {
                        MinItem = new Item(line.MinItem, false);
                        MaxItem = new Item(line.MaxItem, false);
                    }

                    //No Join//3,2,5,...//|---.-00-0- to |---.-00.0-
                    xy = line.IsRow ? (c, line.LineIndex) : (line.LineIndex, c);
                    if (Ls.Valid && !dots.ContainsKey(xy)
                        && points.TryGetValue(line.IsRow ? (c + 1, line.LineIndex) : (line.LineIndex, c + 1), out Point? Pt)
                        && Pt.Green == block.Green)
                    {
                        int Start = block.StartIndex - 1, End = line.LineLength;
                        bool Flag = false, PrevPtG = block.Green;
                        for (int Pos = c + 1; Pos < line.LineLength; Pos++)
                        {
                            xy = line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos);
                            if (!points.TryGetValue(xy, out Pt) || Pt.Green != PrevPtG)
                            {
                                End = Pos;
                                break;
                            }
                            else
                                PrevPtG = Pt.Green;
                        }

                        Flag = Ls.All(a => a.Value < End - Start - 1 && (a.Value > 1 || a.Green == block.Green));

                        if (!Flag && Ls.All(a => a.Value > 1 || a.Green == block.Green)
                            && Ls.Gap.Any() && line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                            && Ls.Before.All(a => a.Value < End - Start - 1 && (a.Value > 1 || a.Green == block.Green)))
                            Flag = true;

                        if (Flag)
                        {
                            Point.Group++;
                            AddPoints(line, c, false, GriddlerPath.Action.NoJoin, dot: true);
                            return line.LineLength;
                        }
                    }

                    bool MinItemForwards(out int start, out int min)
                    {
                        start = 0;
                        min = MinItem.Value;

                        if (c - StartOfGap - 1 < MinItem.Value)
                            return true;

                        var PosXy = line.IsRow ? (c - block.SolidCount - 1, line.LineIndex) : (line.LineIndex, c - block.SolidCount - 1);
                        if (points.TryGetValue(PosXy, out Pt) && Pt.Green != block.Green
                            && Ls.Valid && Ls.Gap.Any() && Ls.Before.Any()
                            && line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                            )
                        {
                            Item[] Matches = Ls.Before.Pair().Where(f => f.Item1.Green == Pt.Green
                                                                && f.Item2.Green == block.Green)
                                                            .Select(s => s.Item2).ToArray();

                            if (Matches.Length == 1)
                            {
                                min = Matches[0].Value;
                                start = c - StartOfGap - block.SolidCount - 1;
                                return true;                            
                            }
                        }

                        if (Ls.Valid && Ls.Gap.Any() && Ls.Before.Any() && Ls.Before.All(a => a.Value < block.SolidCount))
                        {
                            start = Ls.Gap.Sum() + line.GetDotCount(Ls.Index - 1);
                            return true;
                        }

                        return false;
                    }

                    bool MinItemBackwards(out int end) 
                    {
                        end = 0;

                        if (EndOfGap - (c - block.SolidCount - 1) - 1 < MinItem.Value)
                            return true;

                        xy = line.IsRow ? (c, line.LineIndex) : (line.LineIndex, c);
                        if (points.TryGetValue(xy, out Pt) && Pt.Green != block.Green)
                        {
                            end = EndOfGap - c;
                            return true;
                        }

                        if (LsEnd.Valid && LsEnd.Gap.Any() && LsEnd.Before.Any() 
                                && LsEnd.Before.All(a => a < block))
                        {
                            end = LsEnd.Gap.Sum() + line.GetDotCountB(LsEnd.Index + 1);
                            return true;
                        }

                        return false;
                    }

                    //min item backwards
                    if (MinItemBackwards(out int En))
                    {
                        Point.Group++;
                        AddPoints(line, EndOfGap - En - MinItem.Value, block.Green, GriddlerPath.Action.MinItem, c - block.SolidCount - 1);
                    }

                    //min item forwards
                    if (MinItemForwards(out int St, out int Min))
                    {
                        Point.Group++;
                        int PointsChange = points.Count;
                        AddPoints(line, c, block.Green, GriddlerPath.Action.MinItem, StartOfGap + St + Min);
                        Break = points.Count - PointsChange > 0;
                    }

                    bool SingleItemStart(out int m, out int gap)
                    {
                        int Start = 0, End = line.LineItems - 1;
                        (m, gap) = (0, StartOfGap);

                        if ((LineIsolated || (NoMoreItems && MaxItemIndex == 0)
                            || (Valid && Isolations.TryGetValue(block.BlockIndex, out IsolatedItem)))
                            && IsolatedItem == 0)
                        {
                            m = line[IsolatedItem].Value;
                            gap = -1;
                            return true;
                        }

                        if (Ls.Gap.Any())
                            End = Ls.Gap.FirstItemIndex;

                        if (Ls.Before.Any())
                            Start = Ls.Before.FirstItemIndex;

                        if (Start == line.LineItems - 1)
                        {
                            m = line[Start].Value;
                            return true;
                        }

                        if (Start == End && Start < line.LineItems - 1
                            && line[Start + 1] < block
                            && line.IsEq(new ItemRange(new Item[] { line[Start], line[Start + 1] }), Start + 1, StartOfGap, c - block.SolidCount))
                        {
                            m = line[End].Value;
                            return true;
                        }

                        for (int d = Start; d <= End; d++)
                        {
                            if (line[d].Value > m)
                                m = line[d].Value;

                            if (StartOfGap + line[d].Value < c - block.SolidCount - line.GetDotCount(d))
                                return false;
                        }

                        return true;
                    }

                    bool SingleItemEnd(out int m, out int gap)
                    {
                        int Start = 0, End = line.LineItems - 1;
                        (m, gap) = (0, EndOfGap);

                        if (!WasSolid && (LineIsolated
                            || (Valid && Isolations.TryGetValue(block.BlockIndex, out IsolatedItem)
                            && IsolatedItem == End)))
                        {
                            m = line[IsolatedItem].Value;

                            if (IsolatedItem == End)
                                gap = line.LineLength;

                            return true;
                        }

                        if (LsEnd.ItemAtStartOfGap >= 0)
                            Start = LsEnd.ItemAtStartOfGap;

                        if (LsEnd.LastItemAtEquality == LsEnd.ItemAtStartOfGap)
                            End = LsEnd.ItemAtStartOfGap;
                        else if (Ls.Index < End && Ls.Index >= Start
                                && Ls.Gap.Any() && line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount))
                            End = Ls.Index;

                        if (Start == End && End > 0
                            && line[End - 1] < block
                            && End != Ls.Index
                            && line.IsEqB(new ItemRange(new Item[] { line[End] }), End - 1, c, EndOfGap)
                            //&& EndOfGap - c - line[End].Value <= line.GetDotCountB(End)
                            )
                        {
                            m = line[End].Value;
                            return true;
                        }

                        IEnumerable<Item> R = line.Where(w => w.Index >= LsEnd.Index && w.Index <= LsEnd.LastItemAtEquality);
                        if (LsEnd.Valid && LsEnd.Before.Any()
                            && R.Count(c => c.Green == block.Green) == 1
                            && line.IsEqB(LsEnd.Gap, LsEnd.Index + 1, c, EndOfGap))
                        {
                            Item Match = R.First(f => f.Green == block.Green);
                            if (Match.Index == line.LineItems - 1)
                            { 
                                m = Match.Value;
                                return true;                            
                            }
                        }

                        foreach (Item Item in line.Skip(Start).Take(End - Start + 1))
                        {
                            if (Item.Value > m)
                                m = Item.Value;

                            if (c + Item.Value + line.GetDotCount(Item.Index - 1 >= 0 ? Item.Index - 1 : 0) <= EndOfGap)
                                return false;
                        }

                        return true;
                    }

                    //item backward reach//
                    if (SingleItemStart(out int MaxItemS, out int GapStart))
                    {
                        Point.Group++;
                        AddPoints(line, GapStart + 1, false, GriddlerPath.Action.ItemBackwardReach, c - MaxItemS - 1, true);
                    }

                    //item forward reach//
                    if (SingleItemEnd(out int MaxItemE, out int GapEnd) && c - block.SolidCount + MaxItemE < GapEnd)
                    {
                        Point.Group++;
                        int DotsChange = dots.Count;
                        AddPoints(line, c - block.SolidCount + MaxItemE, false, GriddlerPath.Action.ItemForwardReach, GapEnd - 1, true);
                        Break = dots.Count - DotsChange > 0;
                    }

                    //Sum Dots Forward//1,1,1,4,...//|------0--// to |-----.0--
                    if (Ls.Valid && c - block.SolidCount - 1 >= 0 && Ls.Before.Any()
                            && Ls.Before.All(a => block == a)
                            && Ls.Gap.Any() && line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount - 1))
                    {
                        Point.Group++;
                        AddPoints(line, c - block.SolidCount - 1, false, GriddlerPath.Action.SumDotForward, dot: true);
                    }

                    ///Sum Dot Backward//...,3,1,1,3//-0----.000.|// to -0.---.000.|
                    if (LsEnd.Valid && LsEnd.Before.Any() && LsEnd.Before.All(a => block == a)
                            && LsEnd.Gap.Any() && line.IsEqB(LsEnd.Gap, LsEnd.Index + 1, c, EndOfGap))
                    {
                        Point.Group++;
                        AddPoints(line, c, false, GriddlerPath.Action.SumDotBackward, dot: true);
                    }

                    //Half gap overlap forwards//...,2,2//.--0----| to .--0--0-|
                    if (!Break && Ls.Valid && Ls.Index > 0 && EndOfGap - (c + 1) >= line[Ls.Index].Value)
                    {
                        ItemRange ER = Ls.With(LsEnd.ItemAtStartOfGap);//Ls.With(LsE);
                        //need to check if c is on Item
                        if (Ls.Index > Ls.ItemAtStartOfGap && Ls.Index <= LsEnd.ItemAtStartOfGap && ER.Any()
                            && ER.Sum() <= EndOfGap - (c + 1) && ER.Sum() > (EndOfGap - (c + 1)) / 2
                            && Ls.Gap.Any() && line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                            )
                        {
                            Point.Group++;
                            int PointsChange = points.Count;
                            OverlapPart(line, c + line.GetDotCount(Ls.Index - 1), EndOfGap - 1, Ls.Index, LsEnd.ItemAtStartOfGap, GriddlerPath.Action.HalfGapOverlap);
                            Break = points.Count - PointsChange > 0;
                        }
                    }

                    //Half gap overlap backwards//4,6,...//|.-----000000. to |.-000-000000.
                    if (Ls.Valid && Ls.Index > 0)
                    {
                        int BlockIndex = line.FirstOrDefault(w => block == w)?.Index ?? -1;

                        ItemRange ER = new ItemRange(line.Where(w => w.Index >= Ls.ItemAtStartOfGap && w.Index < BlockIndex));


                        if (NoMoreItems && ER.Any() 
                        && ER.Sum() <= c - block.SolidCount - 1 - StartOfGap
                        && ER.Sum() > (c - block.SolidCount - 1 - StartOfGap) / 2
                        //&& line.IsEq(ER, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                        )
                        {
                            Point.Group++;
                            OverlapPart(line, StartOfGap + 1, c - block.SolidCount - 1, Ls.ItemAtStartOfGap, BlockIndex - 1, GriddlerPath.Action.HalfGapOverlap);
                        }
                    }

                    //Isolated Items Reach//1,9,3// |---0-0------------0 to |---0-0--------..--0
                    if ((LineIsolated || 
                    (StartOfGap <= 0 && Valid))
                        && Isolations.TryGetValue(block.BlockIndex - 1, out int FirstI)
                        && Isolations.TryGetValue(block.BlockIndex, out int SecondI))
                    {
                        int End = c - line[SecondI].Value - 1;
                        int Start = -1;
                        bool Flag = false;

                        for (int d = End; d >= 0; d--)
                        {
                            xy = line.IsRow ? (d, line.LineIndex) : (line.LineIndex, d);

                            if (points.TryGetValue(xy, out Point? Pt2) && Pt2 == line[FirstI])
                                Flag = true;
                            else if (Flag)
                            {
                                Start = d + line[FirstI].Value + 1;
                                break;
                            }
                        }

                        if (Start > -1 && Start <= End)
                        {
                            Point.Group++;
                            AddPoints(line, Start, false, GriddlerPath.Action.IsolatedItemsReach, End, true);
                        }
                    }

                    xy = line.IsRow ? (c, line.LineIndex) : (line.LineIndex, c);

                    //Half Gap Full Part//2,2,1,1,1,2,1,1// --0--.0| to --0.0.0|
                    if (Ls.Valid && Ls.Index < line.LineItems - 1
                        && !dots.ContainsKey(xy) && StartOfGap <= 0)
                    {
                        int Sum = 0;

                        for (int d = 0; d < Ls.Index; d++)
                        {
                            Sum += line[d].Value;

                            var Pos = line.IsRow ? (StartOfGap + Sum + 1, line.LineIndex) : (line.LineIndex, StartOfGap + Sum + 1);
                            if (points.ContainsKey(Pos))
                                Sum++;

                            Sum += line.GetDotCount(d);
                        }

                        if (c - StartOfGap - 1 == Sum
                        && LsEnd.Gap.Any() && LsEnd.ItemAtStartOfGap == Ls.Index
                        && EndOfGap - c - line.GetDotCount(Ls.Index - 1) == line[Ls.Index].Value)
                        {
                            if(line.ShouldAddDots(Ls.Index - 1).Item2)
                                AddPoints(line, c, false, GriddlerPath.Action.HalfGapFullPart, dot: true);
                            
                            FullPart(line, c + line.GetDotCount(Ls.Index - 1), Ls.Index, LsEnd.ItemAtStartOfGap, GriddlerPath.Action.HalfGapFullPart);
                            return line.LineLength;
                        }
                    }

                    bool CompleteItem(out bool s, out bool e, out int? itmIdx, out Item? item)
                    {
                        (s, e, itmIdx, item) = (false, false, null, null);

                        if ((LineIsolated || (Valid && Isolations.TryGetValue(block.BlockIndex, out IsolatedItem)))
                        && block.SolidCount == line[IsolatedItem].Value)
                        {
                            (s, e) = line.ShouldAddDots(IsolatedItem);
                            (itmIdx, item) = (IsolatedItem, line[IsolatedItem]);
                            return true;
                        }

                        if (block == MaxItem)
                        {
                            bool Flag = true;

                            if (!line.ItemsOneColour && MaxItem.Index > 0 
                                    && line[MaxItem.Index - 1].Value == MaxItem.Value)
                                Flag = false;
                            else if (!line.ItemsOneColour && MaxItem.Index < line.LineItems - 1
                                        && line[MaxItem.Index + 1].Value == MaxItem.Value)
                                Flag = false;

                            var PosXy = line.IsRow ? (c - block.SolidCount - 1, line.LineIndex) : (line.LineIndex, c - block.SolidCount - 1);
                            if (points.TryGetValue(PosXy, out Pt) && Pt.Green != block.Green && MaxItem.Index > 0)
                                Flag = true;

                            if (Flag)
                                (s, e) = line.ShouldAddDots(MaxItem.Index);

                            return true;
                        }

                        if (Ls.Valid && Ls.Before.ItemsOneValue && Ls.Gap.Any()
                            && line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                            && block.SolidCount == line[Ls.Index - 1].Value)
                        {
                            (s, e) = line.ShouldAddDots(Ls.Index - 1);
                            return true;
                        }

                        return false;
                    }

                    //solid count equals item//
                    if (CompleteItem(out bool S, out bool E, out int? ItmIdx, out Item? Itm))
                    {
                        Point.Group++;

                        if (ItmIdx.HasValue && Itm != (object?)null)
                            line.AddBlock(Itm, true, c - block.SolidCount, c - 1);

                        if (c - block.SolidCount - 1 > 0 && S)
                            AddPoints(line, c - block.SolidCount - 1, false, GriddlerPath.Action.CompleteItem, dot: true);

                        if (E)
                            AddPoints(line, c, false, GriddlerPath.Action.CompleteItem, dot: true);
                    }

                    if (Break)
                        return line.LineLength;

                    return 0;
                }, (nS, wS, c, xy, gS) => nS && wS);
                return false;
            });
        }
    }
}