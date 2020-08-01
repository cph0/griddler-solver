using Griddlers.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Griddlers.Library
{
    public class Logic
    {
        private static IEnumerator<Point>? Stream;
        private static bool Streaming = false;
        private static List<Point> StreamCache = new List<Point>();
        private static int Current = 0;

        private static bool Staging = false;
        private static int TreeLevel = 0;
        private static Dictionary<(int, int), Point> TreeStage = new Dictionary<(int, int), Point>();
        private static List<TreeNode> Excludes = new List<TreeNode>();

        private HashSet<byte> RemovedActions = new HashSet<byte>();

        public Dictionary<(int, int), Point> dots = new Dictionary<(int, int), Point>();
        public Dictionary<(int, int), Point> points = new Dictionary<(int, int), Point>();
        private IReadOnlyDictionary<int, Line> Rows = new Dictionary<int, Line>();
        private IReadOnlyDictionary<int, Line> Cols = new Dictionary<int, Line>();
        private readonly IDictionary<string, int> MethodCounts = new Dictionary<string, int>();

        public void AddMethodCount(string key)
        {
            if (MethodCounts.ContainsKey(key))
                MethodCounts[key]++;
            else
                MethodCounts[key] = 1;
        }

        public static (Dictionary<(int, int), Point>, Dictionary<(int, int), Point>) Run(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            return L.RunNonStatic(rows, columns);
        }

        private (Dictionary<(int, int), Point>, Dictionary<(int, int), Point>) RunNonStatic(Item[][] rows, Item[][] columns)
        {
            Dictionary<(int, int), Point> Points = new Dictionary<(int, int), Point>();
            Dictionary<(int, int), Point> Dots = new Dictionary<(int, int), Point>();
            Staging = false;
            IEnumerable<Point> All = RunI(rows, columns);

            foreach (Point Point in All)
            {
                if (Point.IsDot)
                    Dots.TryAdd((Point.Xpos, Point.Ypos), Point);
                else
                    Points.TryAdd((Point.Xpos, Point.Ypos), Point);
            }

            return (Points, Dots);
        }

        public static void RunAsStream(Item[][] rows, Item[][] columns, Action<Point>? action = null)
        {
            Logic L = new Logic();
            Stream = L.RunI(rows, columns).GetEnumerator();
            Staging = false;
            Current = 0;
            StreamCache = new List<Point>();

            if (action != null)
                Play(action);
        }
        public static void Play(Action<Point> action)
        {
            Streaming = true;
            Task.Run(() =>
            {
                while (Streaming)
                {
                    Point? Pt = Next();
                    if (Pt != (object?)null)
                        action(Pt);
                    else
                        Streaming = false;

                    Thread.Sleep(100);
                }
            });
        }
        public static void Stop()
        {
            Streaming = false;
        }
        public static Point? Next()
        {
            Point? Pt = null;
            if (Stream != null)
            {
                if (Current < StreamCache.Count - 1)
                    Pt = StreamCache[Current + 1];
                else if (Stream.MoveNext())
                {
                    Pt = Stream.Current;
                    StreamCache.Add(Pt);
                }

                //if (StreamCache.Count < points.Count + dots.Count)
                Current++;
            }

            return Pt;
        }
        public static Point? Previous()
        {
            Point? Pt = null;

            if (StreamCache.Count > 0 && Current > 0 && Current <= StreamCache.Count)
            {
                Pt = StreamCache[Current - 1];
                Current--;
            }

            return Pt;
        }

        public static Tree CreateTree(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            Staging = true;
            Point.Group = 0;

            IEnumerable<Point> Stage = L.RunI(rows, columns);

            Tree Tree = new Tree(Stage);

            L.AddChildren(rows, columns, Tree.Root);

            return Tree;
        }
        private void AddChildren(Item[][] rows, Item[][] columns, TreeNode parent)
        {
            foreach (TreeNode Node in parent.Children)
            {
                foreach (Point Point in Node.Points.Values)
                {
                    if (Point.IsDot)
                        dots.TryAdd((Point.Xpos, Point.Ypos), Point);
                    else
                        points.TryAdd((Point.Xpos, Point.Ypos), Point);
                }

                IEnumerable<Point> Stage = RunI(rows, columns);
                //Excludes.AddRange(parent.Children);
                Node.SetNodes(Stage, Excludes);
                AddChildren(rows, columns, Node);
            }

            //if (TreeLevel < 10)
            //{
            //foreach (Point Point in parent.Points.Values)
            //{
            //    if (Point.IsDot)
            //        dots.Remove((Point.Xpos, Point.Ypos));
            //    else
            //        points.Remove((Point.Xpos, Point.Ypos));
            //}
            //}

            //TreeLevel++;
        }
        public static Tree CreateTree2(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            Staging = true;
            Point.Group = 0;

            bool Complete = false;
            Tree Tree = new Tree();
            TreeNode Current = Tree.Root;

            while (!Complete)
            {
                IEnumerable<Point> Stage = L.RunI(rows, columns);
                Current.SetNodes(Stage);

                if (Current.Children.Count > 0 && Current.CurrentNodePos < Current.Children.Count)
                {
                    TreeNode Child = Current.Children[Current.CurrentNodePos];
                    Child.Parent = Current;
                    Current = Child;

                    foreach (Point Point in Current.Points.Values)
                    {
                        if (Point.IsDot)
                            L.dots.TryAdd((Point.Xpos, Point.Ypos), Point);
                        else
                            L.points.TryAdd((Point.Xpos, Point.Ypos), Point);
                    }
                }
                else if (Current.Parent != null)
                {
                    foreach (Point Point in Current.Points.Values)
                    {
                        if (Point.IsDot)
                            L.dots.Remove((Point.Xpos, Point.Ypos));
                        else
                            L.points.Remove((Point.Xpos, Point.Ypos));
                    }

                    Current = Current.Parent;
                    Current.CurrentNodePos++;
                }
                else
                    Complete = true;
            }

            return Tree;
        }

        public static bool IsActionRequired(Item[][] rows,
                                            Item[][] columns,
                                            IReadOnlyDictionary<(int, int), Point> points,
                                            IReadOnlyDictionary<(int, int), Point> dots,
                                            string action)
        {
            bool RetVal = false;
            Logic L = new Logic();
            L.RemovedActions.Add((byte)Enum.Parse(typeof(GriddlerPath.Action), action));

            var Output = L.RunNonStatic(rows, columns);

            if (Output.Item1.Count != points.Count || Output.Item1.Keys.Except(points.Keys).Any()
                && Output.Item2.Count != dots.Count || Output.Item2.Keys.Except(dots.Keys).Any())
                RetVal = true;

            return RetVal;
        }

        public static IReadOnlyList<string> GetRequiredActions(Item[][] rows,
                                                               Item[][] columns,
                                                               IReadOnlyDictionary<(int, int), Point> points,
                                                               IReadOnlyDictionary<(int, int), Point> dots)
        {
            string[] AllActions = Enum.GetNames(typeof(GriddlerPath.Action));
            List<string> Actions = new List<string>(AllActions.Length);

            Parallel.ForEach(AllActions, (Action) =>
            {
                if(IsActionRequired(rows, columns, points, dots, Action))
                    Actions.Add(Action);
            });

            return Actions;
        }
        public static IReadOnlyList<string> GetUsedActions(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            IEnumerable<Point> Points = L.RunI(rows, columns);
            IEnumerable<string> Actions = (from p in Points
                                           select Enum.GetName(typeof(GriddlerPath.Action), p.Action));
            return Actions.Distinct().ToList();
        }


        public static bool IsTrueGriddler(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            int Width = columns.Length;
            int Height = rows.Length;
            IEnumerable<Line> RowLines = rows.Select((s, si) => new Line(si, true, Width, s, L));
            IEnumerable<Line> ColumnLines = columns.Select((s, si) => new Line(si, false, Height, s, L));

            IEnumerable<Point> Rows = L.FullLine(RowLines);
            IEnumerable<Point> Columns = L.FullLine(ColumnLines);

            bool AnyZeroLines = rows.Any(a => a[0].Value == 0) || columns.Any(a => a[0].Value == 0);
            bool AnyFullLines = Rows.Any() || Columns.Any();

            return !AnyZeroLines && !AnyFullLines;
        }


        private IEnumerable<Point> RunI(Item[][] rows, Item[][] columns)
        {
            int Width = columns.Length;
            int Height = rows.Length;

            if (!Staging)
            {
                points = new Dictionary<(int, int), Point>(Width * Height);
                dots = new Dictionary<(int, int), Point>(Width * Height);
                Point.Group = 0;
            }

            int Count = 0;
            int LoopCount = -1;

            Rows = rows.Select((s, si) => new Line(si, true, Width, s, this)).ToDictionary(k => k.LineIndex);
            Cols = columns.Select((s, si) => new Line(si, false, Height, s, this)).ToDictionary(k => k.LineIndex);

            //full rows <5ms
            foreach (Point Point in FullLine(Rows.Values))
                yield return Point;

            //full columns <5ms
            foreach (Point Point in FullLine(Cols.Values))
                yield return Point;

            //overlapping rows <20ms
            foreach (Point Point in OverlapLine(Rows.Values))
                yield return Point;

            //overlapping columns <20ms
            foreach (Point Point in OverlapLine(Cols.Values))
                yield return Point;

            while (!IsComplete(Rows.Values))
            {
                int PointsChange = points.Count;
                int DotsChange = dots.Count;
                bool StrayDog = false;

                if (Count == LoopCount)
                    break;

                //row top edge <20ms
                foreach (Point Point in LineEdgeTL(Rows.Values))
                    yield return Point;

                //column left edge <20ms
                foreach (Point Point in LineEdgeTL(Cols.Values))
                    yield return Point;

                //row bottom edge <20ms
                foreach (Point Point in LineEdgeBR(Rows.Values))
                    yield return Point;

                //column right edge <20ms
                foreach (Point Point in LineEdgeBR(Cols.Values))
                    yield return Point;

                //row full line dots <5ms
                foreach (Point Point in FullLineDots(Rows.Values))
                    yield return Point;

                //column full line dots <5ms
                foreach (Point Point in FullLineDots(Cols.Values))
                    yield return Point;

                (int, int)[] PointsCompare = points.Keys.ToArray();
                (int, int)[] DotsCompare = dots.Keys.ToArray();

                //row line dots <300ms
                foreach (Point Point in LineDots(Rows.Values))
                    yield return Point;

                //column line dots <300ms
                foreach (Point Point in LineDots(Cols.Values))
                    yield return Point;

                IEnumerable<(int, int)> PointKeys = (from k in points.Keys.Except(PointsCompare)
                                                     join p in points on k equals p.Key
                                                     orderby p.Value.Grp
                                                     select k);

                IEnumerable<(int, int)> DotKeys = (from k in dots.Keys.Except(DotsCompare)
                                                   join d in dots on k equals d.Key
                                                   orderby d.Value.Grp
                                                   select k);

                foreach (var PointKey in PointKeys)
                    yield return points[PointKey];

                foreach (var DotKey in DotKeys)
                    yield return dots[DotKey];

                //row line gaps <10ms
                foreach (Point Point in LineGaps(Rows.Values))
                    yield return Point;

                //column line gaps <10ms
                foreach (Point Point in LineGaps(Cols.Values))
                    yield return Point;

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

                //FourCallingBirds25x35 - does not solve
                //dots.TryAdd((9, 26), new Point(9, 26, false));
                //dots.TryAdd((10, 26), new Point(10, 26, false));

                //FiveGoldRing25x35 - does not solve
                //dots.TryAdd((9, 11), new Point(9, 11, false));
                //dots.TryAdd((9, 13), new Point(9, 13, false));

                if (PointsChange == points.Count && DotsChange == dots.Count)
                    break;

                if (StrayDog)
                    break;

                Count++;
            }

            Console.WriteLine($"Loop Count: {Count}");
            //foreach (KeyValuePair<string, int> Method in MethodCounts)
            //{
            //    Console.WriteLine($"{Method.Key}: {Method.Value}");
            //}
        }

        private bool IsComplete(IEnumerable<Line> lines)
        {
            bool Complete = true;

            foreach (Line Line in ForEachLine(lines))
            {
                if (Line.GetLinePointsValue(true) != Line.LineLength)
                {
                    Complete = false;
                    break;
                }
                else
                    Line.IsComplete = true;
            }

            return Complete;
        }

        public IEnumerable<Point> AddPoints(Line line, int start, bool green, GriddlerPath.Action action, int? end = null, bool dot = false)
        {
            for (int Pos = start; Pos <= end.GetValueOrDefault(start); Pos++)
            {
                var Xy = line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos);

                Point NewPoint = new Point(dot, Xy, green, action);
                bool New = (dot && !dots.ContainsKey(Xy)) || (!dot && !points.ContainsKey(Xy));

                if (!Staging && !RemovedActions.Contains((byte)action))
                {
                    if (dot)
                        dots.TryAdd(Xy, NewPoint);
                    else
                        points.TryAdd(Xy, NewPoint);
                }

                if (New && !RemovedActions.Contains((byte)action))
                    yield return NewPoint;

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

        private IEnumerable<Line> ForEachLine(IEnumerable<Line> lines, Func<Line, bool>? run = null, int minLineValue = 1)
        {
            foreach (Line L in lines)
            {
                if (L.LineValue >= minLineValue && !L.IsComplete && (run == null || run(L)))
                    yield return L;
            }
        }

        public IEnumerable<(int, (int, int), Block, bool, int, int)> ForEachLinePos(Line line, Func<bool, bool, int, (int, int), int, bool>? run = null, Predicate<int>? stop = null, bool b = false, int start = 0, Func<int>? advance = null)
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
                    yield return (Pos, Xy, Block, WasSolid, GapSize, SolidCount);
                //Pos += (b ? -1 : 1) * logic(Pos, Xy, Block, WasSolid, GapSize, SolidCount);

                if (advance != null)
                    Pos += (b ? -1 : 1) * advance();

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

        private IEnumerable<Point> FullPart(Line line, int linePosition, int startItem, int endItem, GriddlerPath.Action action, bool complete = true)
        {
            Point.Group++;

            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                bool Green = Item.Green;
                foreach (Point Point in AddPoints(line, linePosition, Green, action, linePosition + Item.Value - 1))
                    yield return Point;
                if (complete)
                    line.AddBlock(Item, true, linePosition, linePosition + Item.Value - 1);
                linePosition += Item.Value;
                if (linePosition < line.LineLength - 1 && line.GetDotCount(Item.Index) == 1)
                {
                    foreach (Point Pt in AddPoints(line, linePosition, Green, action, dot: true))
                        yield return Pt;

                    linePosition++;
                }
            }
        }

        private IEnumerable<Point> OverlapPart(Line line, int position, int lineEnd, int startItem, int endItem, GriddlerPath.Action action)
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
                        foreach (Point Pt in AddPoints(line, Pos, Item.Green, action))
                            yield return Pt;

                        Any = true;
                    }
                }

                if (Any)
                    line.AddBlock(Item);
            }
        }

        /// <summary>
        /// For each line add up the items and if the sum equals the line length fill them in
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> FullLine(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines, (line) => line.LineValue == line.LineLength))
            {
                Line.IsComplete = true;
                foreach (Point Pt in FullPart(Line, 0, 0, Line.LineItems - 1, GriddlerPath.Action.FullLine))
                    yield return Pt;
            }
        }

        /// <summary>
        /// For each line add up the items and if the sum is greater than half the line length do an overlap
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> OverlapLine(IEnumerable<Line> lines)
        {
            static bool Run(Line line) =>
                line.LineValue < line.LineLength && line.LineValue > line.LineLength / 2;
            foreach (Line Line in ForEachLine(lines, Run))
            {
                foreach (Point Pt in OverlapPart(Line, 0, Line.LineLength - 1, 0, Line.LineItems - 1, GriddlerPath.Action.OverlapLine))
                    yield return Pt;
            }
        }

        /// <summary>
        /// For each line add up the points and if the sum equals the sum of the items fill the gaps with dots
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> FullLineDots(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines, (line) => line.LineValue - line.GetDotCount() == line.GetLinePointsValue(), 0))
            {
                Point.Group++;
                Line.IsComplete = true;

                for (int Pos = 0; Pos < Line.LineLength; Pos++)
                    if (!points.ContainsKey(Line.IsRow ? (Pos, Line.LineIndex) : (Line.LineIndex, Pos)))
                        foreach (Point Pt in AddPoints(Line, Pos, false, GriddlerPath.Action.FullLineDots, dot: true))
                            yield return Pt;
            }
        }

        /// <summary>
        /// For each line do the following:        
        /// <list type="number">
        /// <item>For the start of each block compare the position to the line position</item>
        /// <item>If the difference is less than the value of item</item>
        /// <item>Fill in the remainder</item>
        /// </list>
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para> 
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> LineEdgeTL(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                int LinePosition = 0, CurrentItem = 0, EndIndex = 0, FromIndex = 0;
                bool HadGap = false, ItemsOneValue = Line.ItemsOneValue;

                foreach ((int c, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) in ForEachLinePos(Line))
                {
                    if (c >= FromIndex)
                    {
                        //check for gap
                        bool NoMoreItems = false;
                        if (CurrentItem != Line.LineItems - 1 && points.ContainsKey(xy))
                        {
                            (int GapStart, int GapEnd, _) = Line.FindGapStartEnd(c);

                            if (GapStart - LinePosition < Line[CurrentItem].Value
                                && Line[CurrentItem] + Line[CurrentItem + 1] > GapEnd - GapStart - 1)
                                NoMoreItems = true;
                            else
                            {
                                LineSegment LsEnd = Line.GetItemAtPosB(GapEnd - 1);

                                if (LsEnd.Valid && LsEnd.Eq && LsEnd.Index == CurrentItem)
                                    NoMoreItems = true;
                            }
                        }

                        int DotCount = CurrentItem > 0 ? Line.GetDotCount(CurrentItem - 1) : 0;
                        if ((c - LinePosition - DotCount < Line[CurrentItem].Value || NoMoreItems || ItemsOneValue)
                            && points.ContainsKey(xy))
                        {
                            bool EndDot = false;
                            int ShiftCount = 0, CompleteCount = 0;

                            //--O--O
                            if (c > 0 && c + Line[CurrentItem].Value < Line.LineLength)
                            {
                                bool FirstSolid = false;
                                string HasError = "";

                                for (int d = c + Line[CurrentItem].Value; d >= c; d--)
                                {
                                    var PosXy = Line.IsRow ? (d, Line.LineIndex) : (Line.LineIndex, d);
                                    if (points.TryGetValue(PosXy, out Point? Pt)
                                        && Pt == Line[CurrentItem])
                                    {
                                        if (d == c + Line[CurrentItem].Value)
                                            FirstSolid = true;
                                        else if (d < c + Line[CurrentItem].Value)
                                            CompleteCount++;

                                        if (FirstSolid)
                                            ShiftCount = Line[CurrentItem].Value - (d - c) + 1;
                                    }
                                    else
                                    {
                                        if (dots.ContainsKey(PosXy) || Pt != Line[CurrentItem])
                                            ShiftCount = Line[CurrentItem].Value - (d - c);

                                        CompleteCount = 0;
                                        FirstSolid = false;
                                    }
                                }
                                if (CompleteCount == Line[CurrentItem].Value)
                                {
                                    Point.Group++;

                                    if (!NoMoreItems && !ItemsOneValue)
                                        Line.AddBlock(Line[CurrentItem], true, c, c + CompleteCount - 1);

                                    if (Line.ShouldAddDots(CurrentItem).Item1)
                                    {
                                        EndDot = true;
                                        foreach (Point Point in AddPoints(Line, c - 1, false, GriddlerPath.Action.CompleteItem, dot: true))
                                            yield return Point;
                                    }
                                    if (c + CompleteCount < Line.LineLength && Line.ShouldAddDots(CurrentItem).Item2)
                                    {
                                        EndDot = true;
                                        foreach (Point Point in AddPoints(Line, c + CompleteCount, false, GriddlerPath.Action.CompleteItem, dot: true))
                                            yield return Point;
                                    }
                                }
                                else if (CompleteCount > Line[CurrentItem].Value)
                                    HasError = "lineEdgeTL: ahead gap filled";

                                //check for next solid bigger than next item + no room
                                if (!FirstSolid && CurrentItem < Line.LineItems - 1 && Line.ItemsOneColour)
                                {
                                    bool Flag = true;
                                    int NextItemSolidCount = 0;

                                    for (int d = c + 1; d <= c + Line[CurrentItem].Value; d++)
                                    {
                                        var PosXy = Line.IsRow ? (d, Line.LineIndex) : (Line.LineIndex, d);
                                        if (dots.ContainsKey(PosXy))
                                        {
                                            Flag = false;
                                            break;
                                        }
                                    }

                                    for (int d = c + Line[CurrentItem].Value + Line.GetDotCount(CurrentItem); d < Line.LineLength; d++)
                                    {
                                        var PosXy = Line.IsRow ? (d, Line.LineIndex) : (Line.LineIndex, d);
                                        if (points.TryGetValue(PosXy, out Point? Pt)
                                            && Pt?.Green == Line[CurrentItem].Green)
                                            NextItemSolidCount++;
                                        else
                                            break;
                                    }

                                    if (Flag && NextItemSolidCount > Line[CurrentItem + 1].Value
                                            //&& Line[CurrentItem + 1].Value > 1
                                            )
                                        ShiftCount += Line[CurrentItem + 1].Value + Line.GetDotCount(CurrentItem);
                                }

                                if (c - ShiftCount < 0)
                                    HasError = "lineEdgeTL: before shift no room";

                                if (!string.IsNullOrEmpty(HasError))
                                    Console.WriteLine(HasError);

                                if (ShiftCount > 0)
                                    Point.Group++;

                                foreach (Point Point in AddPoints(Line, c - ShiftCount, Line[CurrentItem].Green, GriddlerPath.Action.LineBackwardShift, c - 1))
                                    yield return Point;
                            }

                            if (Line[CurrentItem].Value >= c - LinePosition + 1)
                                Point.Group++;

                            //forward shift and end dot
                            foreach (Point Point in AddPoints(Line, c + 1, Line[CurrentItem].Green, GriddlerPath.Action.LineForwardShift, LinePosition + Line[CurrentItem].Value - 1))
                                yield return Point;
                            if (c - LinePosition == 0 && c + Line[CurrentItem].Value < Line.LineLength && !EndDot
                                && Line.ShouldAddDots(CurrentItem).Item2)
                            {
                                foreach (Point Point in AddPoints(Line, c + Line[CurrentItem].Value, false, GriddlerPath.Action.LineForwardShift, dot: true))
                                    yield return Point;
                            }

                            Line.AddBlock(Line[CurrentItem],
                                          c - LinePosition == 0,
                                          c + 1,
                                          LinePosition + Line[CurrentItem].Value - 1);

                            //back dots
                            int BackReach = c - Line[CurrentItem].Value + (CompleteCount - 1);
                            if (CompleteCount > 0 && (c - LinePosition - 1 < Line[CurrentItem].Value || !ItemsOneValue || Line.LineItems == 1)
                                && BackReach >= EndIndex)
                            {
                                Point.Group++;
                                foreach (Point Point in AddPoints(Line, EndIndex, false, GriddlerPath.Action.LineBackDots, BackReach, true))
                                    yield return Point;
                            }

                            if (CurrentItem == Line.LineItems - 1)
                                break;

                            //3,4|---0-{LP}-{EI}----00
                            EndIndex = c + Line[CurrentItem].Value;
                            LinePosition += (Line[CurrentItem].Value + Line.GetDotCount(CurrentItem));
                            CurrentItem++;
                            FromIndex = c;
                            FromIndex += (Line[CurrentItem - 1].Value + Line.GetDotCount(CurrentItem - 1) - 1 - ShiftCount);
                            FromIndex++;
                        }
                        else if (dots.ContainsKey(xy) && !HadGap)
                            LinePosition = c + 1;
                        else
                            HadGap = true;
                    }
                }
            }
        }

        /// <summary>
        /// For each line working backwards do the following:        
        /// <list type="number">
        /// <item>For the start of each block compare the position to the line position</item>
        /// <item>If the difference is less than the value of item</item>
        /// <item>Fill in the remainder</item>
        /// </list>
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> LineEdgeBR(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                int LinePosition = 0, CurrentItem = Line.LineItems - 1, EndIndex = Line.LineLength - 1;
                int FromIndex = Line.LineLength - 1;
                bool HadGap = false, ItemsOneValue = Line.ItemsOneValue;

                foreach ((int c, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) in ForEachLinePos(Line, b: true))
                {
                    if (c <= FromIndex)
                    {
                        //check for gap
                        bool NoMoreItems = false;
                        if (CurrentItem != 0 && points.ContainsKey(xy) && !ItemsOneValue
                            && Line.LineLength - c - 1 - LinePosition - 1 >= Line[CurrentItem].Value)
                        {
                            (int GapStart, int GapEnd, _) = Line.FindGapStartEnd(c);

                            if (Line.LineLength - GapEnd - 1 - LinePosition < Line[CurrentItem].Value
                                && Line[CurrentItem] + Line[CurrentItem - 1] > GapEnd - GapStart - 1)
                                NoMoreItems = true;
                            else
                            {
                                LineSegment Ls = Line.GetItemAtPos(GapStart + 1);
                                if (Ls.Valid && Ls.Eq && Ls.Index == CurrentItem)
                                    NoMoreItems = true;
                            }
                        }

                        if (points.ContainsKey(xy)
                            && (Line.LineLength - c - 1 - LinePosition - 1 < Line[CurrentItem].Value || NoMoreItems || ItemsOneValue))
                        {
                            bool EndDot = false;
                            int ShiftCount = 0, CompleteCount = 0;

                            if (c < Line.LineLength - 1)
                            {
                                //--O--O
                                bool FirstSolid = false;
                                string HasError = "";
                                for (int d = c - Line[CurrentItem].Value; d <= c; d++)
                                {
                                    var PosXy = Line.IsRow ? (d, Line.LineIndex) : (Line.LineIndex, d);
                                    if (points.TryGetValue(PosXy, out Point? Pt)
                                        && Pt == Line[CurrentItem])
                                    {
                                        if (d == c - Line[CurrentItem].Value)
                                            FirstSolid = true;
                                        else
                                            CompleteCount++;

                                        if (FirstSolid)
                                            ShiftCount = Line[CurrentItem].Value - (c - d) + 1;
                                    }
                                    else
                                    {
                                        if (dots.ContainsKey(PosXy) || Pt != Line[CurrentItem])
                                            ShiftCount = Line[CurrentItem].Value - (c - d);

                                        CompleteCount = 0;
                                        FirstSolid = false;
                                    }
                                }
                                if (CompleteCount == Line[CurrentItem].Value)
                                {
                                    Point.Group++;

                                    if (!NoMoreItems && !ItemsOneValue)
                                        Line.AddBlock(Line[CurrentItem], true, c - CompleteCount + 1, c);

                                    if (Line.ShouldAddDots(CurrentItem).Item2)
                                    {
                                        EndDot = true;
                                        foreach (Point Point in AddPoints(Line, c + 1, false, GriddlerPath.Action.CompleteItem, dot: true))
                                            yield return Point;
                                    }
                                    if (c - CompleteCount >= 0 && Line.ShouldAddDots(CurrentItem).Item1)
                                    {
                                        EndDot = true;
                                        foreach (Point Point in AddPoints(Line, c - CompleteCount, false, GriddlerPath.Action.CompleteItem, dot: true))
                                            yield return Point;
                                    }
                                }
                                else if (CompleteCount > Line[CurrentItem].Value)
                                    HasError = "lineEdgeBR: before gap filled";

                                if (c + ShiftCount > Line.LineLength)
                                    HasError = "lineEdgeBR: ahead shift no room";

                                if (ShiftCount > 0)
                                    Point.Group++;

                                foreach (Point Point in AddPoints(Line, c + 1, Line[CurrentItem].Green, GriddlerPath.Action.LineForwardShift, c + ShiftCount))
                                    yield return Point;

                                if (!string.IsNullOrEmpty(HasError))
                                    Console.WriteLine(HasError);
                            }

                            if (Line[CurrentItem].Value >= (Line.LineLength - c - 1) - LinePosition)
                                Point.Group++;

                            //forward shift and end dot
                            foreach (Point Point in AddPoints(Line, c + 1 - (Line[CurrentItem].Value - ((Line.LineLength - c - 1) - LinePosition)), Line[CurrentItem].Green, GriddlerPath.Action.LineBackwardShift, c - 1))
                                yield return Point;
                            if (!EndDot && (Line.LineLength - c - 1) - LinePosition == 0
                                && Line.LineLength - Line[CurrentItem].Value - LinePosition > 0
                                && Line.ShouldAddDots(CurrentItem).Item1)
                            {
                                foreach (Point Point in AddPoints(Line, c - Line[CurrentItem].Value, false, GriddlerPath.Action.LineBackwardShift, dot: true))
                                    yield return Point;
                            }

                            Line.AddBlock(Line[CurrentItem],
                                          (Line.LineLength - c - 1) - LinePosition == 0,
                                          c + 1 - (Line[CurrentItem].Value - ((Line.LineLength - c - 1) - LinePosition)),
                                          c - 1);

                            //back dots
                            int BackReach = c + Line[CurrentItem].Value - (CompleteCount - 1);
                            if (CompleteCount > 0 && BackReach <= EndIndex &&
                                (Line.LineLength - c - 1 - LinePosition - 1 < Line[CurrentItem].Value
                                || !ItemsOneValue || Line.LineItems == 1))
                            {
                                Point.Group++;
                                foreach (Point Point in AddPoints(Line, BackReach, false, GriddlerPath.Action.LineBackDots, EndIndex, true))
                                    yield return Point;
                            }

                            if (CurrentItem == 0)
                                break;

                            EndIndex = c - Line[CurrentItem].Value;
                            LinePosition += (Line[CurrentItem].Value + Line.GetDotCountB(CurrentItem));
                            CurrentItem--;
                            FromIndex = c;
                            FromIndex -= (Line[CurrentItem + 1].Value + Line.GetDotCountB(CurrentItem + 1) - 1 - ShiftCount);
                            FromIndex--;
                        }
                        else if (points.ContainsKey(xy))
                            break;
                        else if (dots.ContainsKey(xy) && !HadGap)
                            LinePosition = Line.LineLength - c;
                        else
                            HadGap = true;
                    }
                }
            }
        }

        /// <summary>
        /// For each line do the following:        
        /// <list type="number">
        /// <item>After each gap calculate the possible items in the gap</item>
        /// <item>
        ///     The gap is filled with dots if any of the following are true
        ///     <para>
        ///         <list type="bullet">
        ///             <item>All the items are bigger than the gap size</item>
        ///             <item></item>
        ///         </list> 
        ///     </para>
        /// </item>
        /// <item>
        ///     The gap is filled with points if any of the following are true
        ///     <para>
        ///         <list type="bullet">
        ///             <item>The sum of the items equals the gap size</item>
        ///             <item></item>
        ///         </list> 
        ///     </para>
        /// </item>
        /// <item>
        ///     The gap is partially filled with points if any of the following are true
        ///     <para>
        ///         <list type="bullet">
        ///             <item>The sum of the items is greater than half the gap size</item>
        ///             <item></item>
        ///         </list> 
        ///     </para>
        /// </item>
        /// </list>
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> LineGaps(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                bool FirstSolid = false;

                foreach ((int c, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) in ForEachLinePos(Line))
                {
                    int GapSizeCopy = gapSize;
                    if (dots.ContainsKey(xy) || c == Line.LineLength - 1)
                    {
                        int IsEnd = (!dots.ContainsKey(xy) && c == Line.LineLength - 1) ? 1 : 0;

                        if (IsEnd == 1)
                            GapSizeCopy++;

                        if (GapSizeCopy > 0)
                        {
                            LineSegment Ls = Line.GetItemAtPos(c - GapSizeCopy + IsEnd);
                            LineSegment LsEnd = Line.GetItemAtPosB(c - 1 + IsEnd);
                            if (Line.MinItem > GapSizeCopy)
                            {
                                foreach (Point Point in AddPoints(Line, c - GapSizeCopy + IsEnd, false, GriddlerPath.Action.GapDotsMinItem, c + IsEnd - 1, true))
                                    yield return Point;
                            }
                            else
                            {
                                //multiple solids
                                ItemRange Range = Ls.With(LsEnd);
                                int Sum = Range.Sum();

                                bool NextColourSumTooBig = false;
                                Item FirstColourItem = LsEnd.After.LastOrDefault(l => l.Index < LsEnd.Index &&
                                                                l.Green == Ls.RightBefore.First().Green);
                                if (LsEnd.Valid && LsEnd.Eq && FirstColourItem != (object?)null
                                    && Ls.RightBefore.First().EndIndex == c - GapSizeCopy + IsEnd - 2)
                                {
                                    int ColourSum = new ItemRange(LsEnd.After.Where(w => w.Index > FirstColourItem.Index)).Sum();
                                    if (ColourSum > GapSizeCopy)
                                        NextColourSumTooBig = true;
                                }

                                if (Range.All(a => a.Value > GapSizeCopy)
                                    || (Ls.Eq && LsEnd.Eq && Sum > GapSizeCopy)
                                    || (Ls.Eq && Ls.Valid && Ls.Item?.Value > GapSizeCopy)
                                    || (LsEnd.Eq && LsEnd.Valid && LsEnd.Item?.Value > GapSizeCopy)
                                    || (Ls.Eq && Ls.Index > Line.LineItems - 1)
                                    || (LsEnd.Eq && LsEnd.Index < 0)
                                    || (Ls.Eq && LsEnd.Eq && Ls.Index > LsEnd.Index)
                                    || (LsEnd.Valid && Ls.Eq && LsEnd.LastItemAtEquality < Ls.Index)
                                    || (Ls.Valid && LsEnd.Eq && Ls.LastItemAtEquality > LsEnd.Index)
                                    || (NextColourSumTooBig)
                                    || (Ls.Eq && LsEnd.ScV && Ls.Index < Line.LineItems - 1 && Line[Ls.Index + 1].Value > LsEnd.Sc
                                        && Line[Ls.Index + 1] + Line[Ls.Index] > GapSizeCopy)
                                    || (LsEnd.Eq && Ls.ScV && LsEnd.Index > 0 && Line[LsEnd.Index - 1].Value > Ls.Sc
                                        && Line[LsEnd.Index - 1] + Line[LsEnd.Index] > GapSizeCopy)
                                    )
                                {
                                    GriddlerPath.Action Action = GriddlerPath.Action.GapDotsTooBig;
                                    //if (Eq && EqEnd && Sum > GapSizeCopy)
                                    //    Action = GriddlerPath.Action.GapDotsSum;
                                    //else if (Valid && Eq && Line[Item].Value > GapSizeCopy)
                                    //    Action = GriddlerPath.Action.GapDotsSumF;
                                    //else if (ValidEnd && EqEnd && Line[ItemEnd].Value > GapSizeCopy)
                                    //    Action = GriddlerPath.Action.GapDotsSumB;
                                    //else if (Eq && Item > Line.LineItems - 1)
                                    //    Action = GriddlerPath.Action.GapDotsNoMoreItemsF;
                                    //else if (EqEnd && ItemEnd < 0)
                                    //    Action = GriddlerPath.Action.GapDotsNoMoreItemsB;

                                    Point.Group++;
                                    foreach (Point Point in AddPoints(Line, c - GapSizeCopy + IsEnd, false, Action, c + IsEnd - 1, true))
                                        yield return Point;
                                }
                                else if (Ls.Valid && (LsEnd.Valid
                                    || (Ls.Eq && Ls.Index > 0 && LsEnd.ScV && Line[Ls.Index].Value != LsEnd.Sc)
                                    ) && Sum == GapSizeCopy)
                                {
                                    foreach (Point Point in FullPart(Line, c - GapSizeCopy + IsEnd, Ls.Index, LsEnd.Index, GriddlerPath.Action.GapFull))
                                        yield return Point;
                                }
                                else if (Ls.Valid && FirstSolid && Ls.Index <= LsEnd.Index && Ls.ItemsOneValue)
                                {
                                    foreach (Point Point in FullPart(Line, c - GapSizeCopy + IsEnd, Ls.Index, Ls.Index, GriddlerPath.Action.GapFull, false))
                                        yield return Point;
                                }
                                else if (Ls.Valid && LsEnd.Valid && (Ls.Index == LsEnd.Index || Ls.Eq || LsEnd.Eq)
                                    && Sum < GapSizeCopy && Sum > GapSizeCopy / 2)
                                {
                                    GriddlerPath.Action Action = GriddlerPath.Action.GapOverlapSameItem;
                                    if (Ls.Eq || LsEnd.Eq)
                                        Action = GriddlerPath.Action.GapOverlap;

                                    foreach (Point Point in OverlapPart(Line, c - GapSizeCopy + IsEnd, c - 1 + IsEnd, Ls.Index, LsEnd.Index, Action))
                                        yield return Point;
                                }
                            }
                            FirstSolid = false;
                        }
                    }
                    else
                    {
                        if (GapSizeCopy == 0 && points.ContainsKey(xy))
                            FirstSolid = true;
                    }
                }
            }
        }

        /// <summary>
        /// For each line do the following:        
        /// <list type="number">
        ///     <item>After each block try the following</item>
        ///     <item>No Join</item>
        ///     <item>Minimum Item Fowards</item>
        ///     <item>Minimum Item Backwards</item>
        ///     <item>Single Item Forwards</item>
        ///     <item>Singlel Item Backwards</item>
        ///     <item>Sum dots forward</item>
        ///     <item>Sum dots backward</item>
        ///     <item>Half Gap Overlap Forwards</item>
        ///     <item>Half Gap Overlap Backwards</item>
        ///     <item>Half Gap Full Part</item>
        ///     <item>Isolated Gap Dots</item>
        ///     <item>Complete Item</item>
        /// </list>
        /// <para>
        ///     Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> LineDots(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                Item MinItem = new Item(Line.MinItem, false), MaxItem = new Item(Line.MaxItem, false);
                int MaxItemIndex = -1, IsolatedItem = 0;
                (bool LineIsolated, bool Valid, IDictionary<int, int> Isolations) = Line.IsLineIsolated();
                IDictionary<int, (Item, Item)> MinMaxItems = Line.GetMinMaxItems();

                foreach (Item Item in Line)
                {
                    if (Item.Value == MaxItem.Value)
                        MaxItemIndex = Item.Index;
                }

                foreach ((int c, (int, int) xy, Block block, bool wasSolid, int gapSize, int solidCount) in ForEachLinePos(Line, (nS, wS, c, xy, gS) => nS && wS))
                {
                    bool Break = false, NoMoreItems = Line.UniqueCount(block);
                    LineSegment Ls = Line.GetItemAtPos(c + 1, false);
                    LineSegment LsEnd = Line.GetItemAtPosB(c - 1, false);
                    (int StartOfGap, int EndOfGap, bool WasSolid) = Line.FindGapStartEnd(c - 1, c);
                    IsolatedItem = block.BlockIndex;

                    if (MinMaxItems.TryGetValue(c, out (Item, Item) M))
                    {
                        MinItem = M.Item1;
                        MaxItem = M.Item2;
                    }
                    else
                    {
                        MinItem = new Item(Line.MinItem, false);
                        MaxItem = new Item(Line.MaxItem, false);
                    }

                    //No Join//3,2,5,...//|---.-00-0- to |---.-00.0-
                    if (Ls.Valid && !dots.ContainsKey(xy)
                        && points.TryGetValue(Line.IsRow ? (c + 1, Line.LineIndex) : (Line.LineIndex, c + 1), out Point? Pt)
                        && Pt.Green == block.Green)
                    {
                        int Start = block.StartIndex - 1, End = Line.LineLength;
                        bool Flag = false, PrevPtG = block.Green;
                        for (int Pos = c + 1; Pos < Line.LineLength; Pos++)
                        {
                            var PosXy = Line.IsRow ? (Pos, Line.LineIndex) : (Line.LineIndex, Pos);
                            if (!points.TryGetValue(PosXy, out Pt) || Pt.Green != PrevPtG)
                            {
                                End = Pos;
                                break;
                            }
                            else
                                PrevPtG = Pt.Green;
                        }

                        Flag = Ls.All(a => a.Value < End - Start - 1 && (a.Value > 1 || a.Green == block.Green));

                        if (!Flag && Ls.All(a => a.Value > 1 || a.Green == block.Green)
                            && Ls.Gap.Any() && Line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                            && Ls.Before.All(a => a.Value < End - Start - 1 && (a.Value > 1 || a.Green == block.Green)))
                            Flag = true;

                        if (Flag)
                        {
                            Point.Group++;
                            foreach (Point Point in AddPoints(Line, c, false, GriddlerPath.Action.NoJoin, dot: true))
                                yield return Point;
                            break;
                        }
                    }

                    bool MinItemForwards(out int start, out int min)
                    {
                        start = 0;
                        min = MinItem.Value;

                        if (c - StartOfGap - 1 < MinItem.Value)
                            return true;

                        var PosXy = Line.IsRow ? (c - block.SolidCount - 1, Line.LineIndex) : (Line.LineIndex, c - block.SolidCount - 1);
                        if (points.TryGetValue(PosXy, out Pt) && Pt.Green != block.Green
                            && Ls.Valid && Ls.Gap.Any() && Ls.Before.Any()
                            && Line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount)
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

                        if (Ls.Valid && Ls.Gap.Any() && Ls.Before.All(a => a.Value < block.SolidCount))
                        {
                            start = Ls.Gap.Sum() + Line.GetDotCount(Ls.Index - 1);
                            return true;
                        }

                        return false;
                    }

                    bool MinItemBackwards(out int end)
                    {
                        end = 0;

                        if (EndOfGap - (c - block.SolidCount - 1) - 1 < MinItem.Value)
                            return true;

                        //xy = Line.IsRow ? (c, Line.LineIndex) : (Line.LineIndex, c);
                        if (points.TryGetValue(xy, out Pt) && Pt.Green != block.Green)
                        {
                            end = EndOfGap - c;
                            return true;
                        }

                        if (LsEnd.Valid && LsEnd.Gap.Any()
                                && LsEnd.Before.All(a => a < block))
                        {
                            end = LsEnd.Gap.Sum() + Line.GetDotCountB(LsEnd.Index + 1);
                            return true;
                        }

                        return false;
                    }

                    //min item backwards
                    if (MinItemBackwards(out int En))
                    {
                        Point.Group++;
                        foreach (Point Point in AddPoints(Line, EndOfGap - En - MinItem.Value, block.Green, GriddlerPath.Action.MinItem, c - block.SolidCount - 1))
                            yield return Point;
                    }

                    //min item forwards
                    if (MinItemForwards(out int St, out int Min))
                    {
                        Point.Group++;
                        int PointsChange = points.Count;
                        foreach (Point Point in AddPoints(Line, c, block.Green, GriddlerPath.Action.MinItem, StartOfGap + St + Min))
                            yield return Point;
                        Break = points.Count - PointsChange > 0;
                    }

                    bool SingleItemStart(out int m, out int gap)
                    {
                        int Start = 0, End = Line.LineItems - 1;
                        (m, gap) = (0, StartOfGap);

                        if ((LineIsolated || (NoMoreItems && MaxItemIndex == 0)
                            || (Valid && Isolations.TryGetValue(block.BlockIndex, out IsolatedItem)))
                            && IsolatedItem == 0)
                        {
                            m = Line[IsolatedItem].Value;
                            gap = -1;
                            return true;
                        }

                        if (Ls.Gap.Any())
                            End = Ls.Gap.FirstItemIndex;

                        if (Ls.Before.Any())
                            Start = Ls.Before.FirstItemIndex;

                        if (Start == Line.LineItems - 1)
                        {
                            m = Line[Start].Value;
                            return true;
                        }

                        if (Start == End && Start < Line.LineItems - 1
                            && Line[Start + 1] < block
                            && Line.IsEq(new ItemRange(new Item[] { Line[Start], Line[Start + 1] }), Start + 1, StartOfGap, c - block.SolidCount))
                        {
                            m = Line[End].Value;
                            return true;
                        }

                        for (int d = Start; d <= End; d++)
                        {
                            if (Line[d].Value > m)
                                m = Line[d].Value;

                            if (StartOfGap + Line[d].Value < c - block.SolidCount - Line.GetDotCount(d))
                                return false;
                        }

                        return true;
                    }

                    bool SingleItemEnd(out int m, out int gap)
                    {
                        int Start = 0, End = Line.LineItems - 1;
                        (m, gap) = (0, EndOfGap);

                        if (!WasSolid && (LineIsolated
                            || (Valid && Isolations.TryGetValue(block.BlockIndex, out IsolatedItem)
                            && IsolatedItem == End)))
                        {
                            m = Line[IsolatedItem].Value;

                            if (IsolatedItem == End)
                                gap = Line.LineLength;

                            return true;
                        }

                        if (LsEnd.ItemAtStartOfGap >= 0)
                            Start = LsEnd.ItemAtStartOfGap;

                        if (LsEnd.LastItemAtEquality == LsEnd.ItemAtStartOfGap)
                            End = LsEnd.ItemAtStartOfGap;
                        else if (Ls.Index < End && Ls.Index >= Start
                                && Ls.Gap.Any() && Line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount))
                            End = Ls.Index;

                        if (Start == End && End > 0
                            && Line[End - 1] < block
                            && End != Ls.Index
                            && Line.IsEqB(new ItemRange(new Item[] { Line[End] }), End - 1, c, EndOfGap)
                            //&& EndOfGap - c - Line[End].Value <= Line.GetDotCountB(End)
                            )
                        {
                            m = Line[End].Value;
                            return true;
                        }

                        if (LsEnd.Valid && LsEnd.Before.Any()
                            && LsEnd.UniqueCount(block)
                            && Line.IsEqB(LsEnd.Gap, LsEnd.Index + 1, c, EndOfGap))
                        {
                            Item Match = LsEnd.First(f => f >= block);
                            if (Match.Index == Line.LineItems - 1)
                            {
                                m = Match.Value;
                                return true;
                            }
                        }

                        foreach (Item Item in Line.Skip(Start).Take(End - Start + 1))
                        {
                            if (Item.Value > m)
                                m = Item.Value;

                            if (c + Item.Value + Line.GetDotCount(Item.Index - 1 >= 0 ? Item.Index - 1 : 0) <= EndOfGap)
                                return false;
                        }

                        return true;
                    }

                    //item backward reach//
                    if (SingleItemStart(out int MaxItemS, out int GapStart))
                    {
                        Point.Group++;
                        foreach (Point Point in AddPoints(Line, GapStart + 1, false, GriddlerPath.Action.ItemBackwardReach, c - MaxItemS - 1, true))
                            yield return Point;
                    }

                    //item forward reach//
                    if (SingleItemEnd(out int MaxItemE, out int GapEnd) && c - block.SolidCount + MaxItemE < GapEnd)
                    {
                        Point.Group++;
                        int DotsChange = dots.Count;
                        foreach (Point Point in AddPoints(Line, c - block.SolidCount + MaxItemE, false, GriddlerPath.Action.ItemForwardReach, GapEnd - 1, true))
                            yield return Point;
                        Break = dots.Count - DotsChange > 0;
                    }

                    //Sum Dots Forward//1,1,1,4,...//|------0--// to |-----.0--
                    if (Ls.Valid && c - block.SolidCount - 1 >= 0
                        && Ls.Before.All(a => block == a)
                        && Ls.Gap.Any() && Line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount - 1))
                    {
                        Point.Group++;
                        foreach (Point Point in AddPoints(Line, c - block.SolidCount - 1, false, GriddlerPath.Action.SumDotForward, dot: true))
                            yield return Point;
                    }

                    ///Sum Dot Backward//...,3,1,1,3//-0----.000.|// to -0.---.000.|
                    if (LsEnd.Valid && LsEnd.Before.All(a => block == a)
                        && LsEnd.Gap.Any() && Line.IsEqB(LsEnd.Gap, LsEnd.Index + 1, c, EndOfGap))
                    {
                        Point.Group++;
                        foreach (Point Point in AddPoints(Line, c, false, GriddlerPath.Action.SumDotBackward, dot: true))
                            yield return Point;
                    }

                    //Half gap overlap forwards//...,2,2//.--0----| to .--0--0-|
                    if (!Break && Ls.Valid && Ls.Index > 0 && EndOfGap - (c + 1) >= Line[Ls.Index].Value)
                    {
                        ItemRange ER = Ls.With(LsEnd.ItemAtStartOfGap);//Ls.With(LsE);
                                                                       //need to check if c is on Item
                        if (Ls.Index > Ls.ItemAtStartOfGap && Ls.Index <= LsEnd.ItemAtStartOfGap && ER.Any()
                        && ER.Sum() <= EndOfGap - (c + 1) && ER.Sum() > (EndOfGap - (c + 1)) / 2
                        && Ls.Gap.Any() && Line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                        )
                        {
                            Point.Group++;
                            int PointsChange = points.Count;
                            foreach (Point Point in OverlapPart(Line, c + Line.GetDotCount(Ls.Index - 1), EndOfGap - 1, Ls.Index, LsEnd.ItemAtStartOfGap, GriddlerPath.Action.HalfGapOverlap))
                                yield return Point;
                            Break = points.Count - PointsChange > 0;
                        }
                    }

                    //Half gap overlap backwards//4,6,...//|.-----000000. to |.-000-000000.
                    if (Ls.Valid && Ls.Index > 0)
                    {
                        int BlockIndex = Line.FirstOrDefault(w => block == w)?.Index ?? -1;

                        ItemRange ER = new ItemRange(Line.Where(w => w.Index >= Ls.ItemAtStartOfGap && w.Index < BlockIndex));


                        if (NoMoreItems && ER.Any()
                        && ER.Sum() <= c - block.SolidCount - 1 - StartOfGap
                        && ER.Sum() > (c - block.SolidCount - 1 - StartOfGap) / 2
                        //&& Line.IsEq(ER, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                        )
                        {
                            Point.Group++;
                            foreach (Point Point in OverlapPart(Line, StartOfGap + 1, c - block.SolidCount - 1, Ls.ItemAtStartOfGap, BlockIndex - 1, GriddlerPath.Action.HalfGapOverlap))
                                yield return Point;
                        }
                    }

                    //Isolated Items Reach//1,9,3// |---0-0------------0 to |---0-0--------..--0
                    if ((LineIsolated ||
                        (StartOfGap <= 0 && Valid))
                        && Isolations.TryGetValue(block.BlockIndex - 1, out int FirstI)
                        && Isolations.TryGetValue(block.BlockIndex, out int SecondI))
                    {
                        int End = c - Line[SecondI].Value - 1;
                        int Start = -1;
                        bool Flag = false;

                        for (int d = End; d >= 0; d--)
                        {
                            var PosXy = Line.IsRow ? (d, Line.LineIndex) : (Line.LineIndex, d);

                            if (points.TryGetValue(PosXy, out Point? Pt2) && Pt2 == Line[FirstI])
                                Flag = true;
                            else if (Flag)
                            {
                                Start = d + Line[FirstI].Value + 1;
                                break;
                            }
                        }

                        if (Start > -1 && Start <= End)
                        {
                            Point.Group++;
                            foreach (Point Point in AddPoints(Line, Start, false, GriddlerPath.Action.IsolatedItemsReach, End, true))
                                yield return Point;
                        }
                    }

                    //Half Gap Full Part//2,2,1,1,1,2,1,1// --0--.0| to --0.0.0|
                    if (Ls.Valid && Ls.Index < Line.LineItems - 1
                    && !dots.ContainsKey(xy) && StartOfGap <= 0)
                    {
                        int Sum = 0;

                        for (int d = 0; d < Ls.Index; d++)
                        {
                            Sum += Line[d].Value;

                            var Pos = Line.IsRow ? (StartOfGap + Sum + 1, Line.LineIndex) : (Line.LineIndex, StartOfGap + Sum + 1);
                            if (points.ContainsKey(Pos))
                                Sum++;

                            Sum += Line.GetDotCount(d);
                        }

                        if (c - StartOfGap - 1 == Sum
                        && LsEnd.Gap.Any() && LsEnd.ItemAtStartOfGap == Ls.Index
                        && EndOfGap - c - Line.GetDotCount(Ls.Index - 1) == Line[Ls.Index].Value)
                        {
                            if (Line.ShouldAddDots(Ls.Index - 1).Item2)
                            {
                                foreach (Point Point in AddPoints(Line, c, false, GriddlerPath.Action.HalfGapFullPart, dot: true))
                                    yield return Point;
                            }

                            foreach (Point Point in FullPart(Line, c + Line.GetDotCount(Ls.Index - 1), Ls.Index, LsEnd.ItemAtStartOfGap, GriddlerPath.Action.HalfGapFullPart))
                                yield return Point;

                            break;
                        }
                    }

                    bool CompleteItem(out bool s, out bool e, out int? itmIdx, out Item? item)
                    {
                        (s, e, itmIdx, item) = (false, false, null, null);

                        if ((LineIsolated || (Valid && Isolations.TryGetValue(block.BlockIndex, out IsolatedItem)))
                        && block.SolidCount == Line[IsolatedItem].Value)
                        {
                            (s, e) = Line.ShouldAddDots(IsolatedItem);
                            (itmIdx, item) = (IsolatedItem, Line[IsolatedItem]);
                            return true;
                        }

                        if (block == MaxItem)
                        {
                            bool Flag = true;

                            if (!Line.ItemsOneColour && MaxItem.Index > 0
                                    && Line[MaxItem.Index - 1].Value == MaxItem.Value)
                                Flag = false;
                            else if (!Line.ItemsOneColour && MaxItem.Index < Line.LineItems - 1
                                        && Line[MaxItem.Index + 1].Value == MaxItem.Value)
                                Flag = false;

                            var PosXy = Line.IsRow ? (c - block.SolidCount - 1, Line.LineIndex) : (Line.LineIndex, c - block.SolidCount - 1);
                            if (points.TryGetValue(PosXy, out Pt) && Pt.Green != block.Green && MaxItem.Index > 0)
                                Flag = true;

                            if (Flag)
                                (s, e) = Line.ShouldAddDots(MaxItem.Index);

                            return true;
                        }

                        if (Ls.Valid && Ls.Before.ItemsOneValue && Ls.Gap.Any()
                            && Line.IsEq(Ls.Gap, Ls.Index - 1, StartOfGap, c - block.SolidCount)
                            && block.SolidCount == Line[Ls.Index - 1].Value)
                        {
                            (s, e) = Line.ShouldAddDots(Ls.Index - 1);
                            return true;
                        }

                        return false;
                    }

                    //solid count equals item//
                    if (CompleteItem(out bool S, out bool E, out int? ItmIdx, out Item? Itm))
                    {
                        Point.Group++;

                        if (ItmIdx.HasValue && Itm != (object?)null)
                            Line.AddBlock(Itm, true, c - block.SolidCount, c - 1);

                        if (c - block.SolidCount - 1 > 0 && S)
                        {
                            foreach (Point Point in AddPoints(Line, c - block.SolidCount - 1, false, GriddlerPath.Action.CompleteItem, dot: true))
                                yield return Point;
                        }

                        if (E)
                        {
                            foreach (Point Point in AddPoints(Line, c, false, GriddlerPath.Action.CompleteItem, dot: true))
                                yield return Point;
                        }
                    }

                    if (Break)
                        break;
                }
            }
        }
    }
}