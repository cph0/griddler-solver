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
        private static readonly List<TreeNode> Excludes = new List<TreeNode>();

        private readonly HashSet<byte> RemovedActions = new HashSet<byte>();

        public Dictionary<(int, int), Point> dots = new Dictionary<(int, int), Point>();
        public Dictionary<(int, int), Point> points = new Dictionary<(int, int), Point>();
        private IReadOnlyDictionary<int, Line> Rows = new Dictionary<int, Line>();
        private IReadOnlyDictionary<int, Line> Cols = new Dictionary<int, Line>();
        private readonly HashSet<(int, int)> Trials = new HashSet<(int, int)>();
        private readonly HashSet<(int, int)> IncorrectTrials = new HashSet<(int, int)>();

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
                    Dots.TryAdd((Point.X, Point.Y), Point);
                else
                    Points.TryAdd((Point.X, Point.Y), Point);
            }

            return (Points, Dots);
        }

        /// <summary>
        /// Starts solving a griddler as a stream.  
        /// If no action is passed then the stream will be paused.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <param name="action">The method to call for each point/dot solved.</param>
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
        /// <summary>
        /// Starts/Restarts a stream.  
        /// Must call <see cref="RunAsStream(Item[][], Item[][], Action{Point}?)"/> first.
        /// </summary>
        /// <param name="action">The method to call for each point/dot solved.</param>
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
        /// <summary>
        /// Stops a stream.
        /// </summary>
        public static void Stop()
        {
            Streaming = false;
        }
        /// <summary>
        /// Gets the next point to be solved.
        /// </summary>
        /// <returns>The next point in the stream.</returns>
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
        /// <summary>
        /// Gets the previous point that was solved.
        /// </summary>
        /// <returns>The previous point in the stream.</returns>
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
                        dots.TryAdd((Point.X, Point.Y), Point);
                    else
                        points.TryAdd((Point.X, Point.Y), Point);
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
                            L.dots.TryAdd((Point.X, Point.Y), Point);
                        else
                            L.points.TryAdd((Point.X, Point.Y), Point);
                    }
                }
                else if (Current.Parent != null)
                {
                    foreach (Point Point in Current.Points.Values)
                    {
                        if (Point.IsDot)
                            L.dots.Remove((Point.X, Point.Y));
                        else
                            L.points.Remove((Point.X, Point.Y));
                    }

                    Current = Current.Parent;
                    Current.CurrentNodePos++;
                }
                else
                    Complete = true;
            }

            return Tree;
        }

        /// <summary>
        /// Determines if a griddler cannot solve without a particular action.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <param name="points">The points</param>
        /// <param name="dots">The dots</param>
        /// <param name="action">The action to test</param>
        /// <returns><see cref="true"/> if the action is required.</returns>
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
        /// <summary>
        /// Determines what actions a griddler needs to solve.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <param name="points">The points</param>
        /// <param name="dots">The dots</param>
        /// <returns>A list of required actions.</returns>
        public static IReadOnlyList<string> GetRequiredActions(Item[][] rows,
                                                               Item[][] columns,
                                                               IReadOnlyDictionary<(int, int), Point> points,
                                                               IReadOnlyDictionary<(int, int), Point> dots)
        {
            string[] AllActions = Enum.GetNames(typeof(GriddlerPath.Action));
            List<string> Actions = new List<string>(AllActions.Length);

            Parallel.ForEach(AllActions, (Action) =>
            {
                if (IsActionRequired(rows, columns, points, dots, Action))
                    Actions.Add(Action);
            });

            return Actions;
        }
        /// <summary>
        /// Determines what actions are used by a griddler.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>A list of used actions.</returns>
        public static IReadOnlyList<string> GetUsedActions(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            IEnumerable<Point> Points = L.RunI(rows, columns);
            IEnumerable<string> Actions = (from p in Points
                                           select Enum.GetName(typeof(GriddlerPath.Action), p.Action));
            return Actions.Distinct().ToList();
        }

        /// <summary>
        /// Determines if a griddler satisfies the following
        /// <list type="number">
        /// <item>No zero lines</item>
        /// <item>No trivial lines (full lines)</item>
        /// <item>Size is a mulitple of 5</item>
        /// <item>No symmetry (TEST)</item>
        /// <item>No trial and error (TODO)</item>
        /// </list>
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns><see cref="true"/> if the griddler is a true griddler</returns>
        public static bool IsTrueGriddler(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            int Width = columns.Length;
            int Height = rows.Length;
            IEnumerable<Line> RowLines = rows.Select((s, si) => new Line(si, true, Width, s));
            IEnumerable<Line> ColumnLines = columns.Select((s, si) => new Line(si, false, Height, s));

            IEnumerable<Point> Rows = L.FullLine(RowLines).SelectMany(s => s);
            IEnumerable<Point> Columns = L.FullLine(ColumnLines).SelectMany(s => s);

            bool AnyZeroLines = rows.Any(a => a[0].Value == 0) || columns.Any(a => a[0].Value == 0);
            bool AnyFullLines = Rows.Any() || Columns.Any();
            bool MultipleOfFive = rows.Length % 5 == 0 && columns.Length % 5 == 0;
            bool Symmetry = true;
            bool TrialAndError = true;

            //symmetry
            for (int index = 0; index < Height; index++)
            {
                Item[] First = rows[index];
                Item[] Last = rows[Height - index - 1];

                for (int c = 0; c < Width; c++)
                {
                    if (!First[c].Is(Last[c]))
                    {
                        Symmetry = false;
                        break;
                    }
                }

                if (!Symmetry)
                    break;
            }

            if (Symmetry)
            {
                for (int index = 0; index < Width; index++)
                {
                    Item[] First = rows[index];
                    Item[] Last = rows[Width - index - 1];

                    for (int c = 0; c < Height; c++)
                    {
                        if (!First[c].Is(Last[c]))
                        {
                            Symmetry = false;
                            break;
                        }
                    }

                    if (!Symmetry)
                        break;
                }
            }

            return !AnyZeroLines && !AnyFullLines && MultipleOfFive && !Symmetry && !TrialAndError;
        }

        /// <summary>
        /// Determines the number of trivial lines (full lines).
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>The number of full lines.</returns>
        public static int FullLineCount(Item[][] rows, Item[][] columns)
        {
            (int, int) Counts = FullLineCounts(rows, columns);
            return Counts.Item1 + Counts.Item2;
        }
        /// <summary>
        /// Determines the number of trivial lines (full lines).
        /// Split into rows/columns.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>The number of full rows and columns.</returns>
        public static (int, int) FullLineCounts(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            int Width = columns.Length;
            int Height = rows.Length;
            L.Rows = rows.Select((s, si) => new Line(si, true, Width, s)).ToDictionary(k => k.LineIndex);
            L.Cols = columns.Select((s, si) => new Line(si, false, Height, s)).ToDictionary(k => k.LineIndex);

            IEnumerable<Point> Rows = L.FullLine(L.Rows.Values).SelectMany(s => s);
            IEnumerable<Point> Columns = L.FullLine(L.Cols.Values).SelectMany(s => s);

            return (Rows.GroupBy(g => g.Y).Count(), Columns.GroupBy(g => g.X).Count());
        }

        /// <summary>
        /// Determines the number of overlap lines.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>The number of overlap liens.</returns>
        public static int OverlapLineCount(Item[][] rows, Item[][] columns)
        {
            (int, int) Counts = OverlapLineCounts(rows, columns);
            return Counts.Item1 + Counts.Item2;
        }
        /// <summary>
        /// Determines the number of overlap lines.
        /// Split into rows/columns.
        /// </summary>
        /// <param name="rows">The rows</param>
        /// <param name="columns">The columns</param>
        /// <returns>The number of overlap rows and columns.</returns>
        public static (int, int) OverlapLineCounts(Item[][] rows, Item[][] columns)
        {
            Logic L = new Logic();
            int Width = columns.Length;
            int Height = rows.Length;
            L.Rows = rows.Select((s, si) => new Line(si, true, Width, s)).ToDictionary(k => k.LineIndex);
            L.Cols = columns.Select((s, si) => new Line(si, false, Height, s)).ToDictionary(k => k.LineIndex);

            IEnumerable<Point> Rows = L.OverlapLine(L.Rows.Values).SelectMany(s => s);
            IEnumerable<Point> Columns = L.OverlapLine(L.Cols.Values).SelectMany(s => s);

            return (Rows.GroupBy(g => g.Y).Count(), Columns.GroupBy(g => g.X).Count());
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

            Rows = rows.Select((s, si) => new Line(si, true, Width, s)).ToDictionary(k => k.LineIndex);
            Cols = columns.Select((s, si) => new Line(si, false, Height, s)).ToDictionary(k => k.LineIndex);

            foreach (Line Row in Rows.Values)
                Row.SetPairLines(Cols.Values);

            foreach (Line Col in Cols.Values)
                Col.SetPairLines(Rows.Values);

            //full rows <5ms
            foreach (Point Point in FullLine(Rows.Values).SelectMany(s => s))
                yield return Point;

            //full columns <5ms
            foreach (Point Point in FullLine(Cols.Values).SelectMany(s => s))
                yield return Point;

            //overlapping rows <20ms
            foreach (Point Point in OverlapLine(Rows.Values).SelectMany(s => s))
                yield return Point;

            //overlapping columns <20ms
            foreach (Point Point in OverlapLine(Cols.Values).SelectMany(s => s))
                yield return Point;

            //multi colour cross reference rows
            foreach (Point Point in MultiColourCrossReference(Rows.Values).SelectMany(s => s))
                yield return Point;

            //multi colour cross reference columns
            foreach (Point Point in MultiColourCrossReference(Cols.Values).SelectMany(s => s))
                yield return Point;

            foreach (Point Point in Run())
                yield return Point;

            //trial and error - EXPERIMENTAL
            if (!Staging && points.Count + dots.Count < Width * Height
                && points.Count + dots.Count > (Width * Height * 3) / 4)
            {
                int TrialCount = 1;
                List<(int, int)> TestPointKeys = new List<(int, int)>(50);
                List<(int, int)> TestDotKeys = new List<(int, int)>(50);

                while (!IsComplete(Rows.Values))
                {
                    foreach (Point Point in TrialAndError(Rows.Values))
                    {
                        if (Point.IsDot)
                            TestDotKeys.Add((Point.X, Point.Y));
                        else
                            TestPointKeys.Add((Point.X, Point.Y));
                    }

                    foreach (Point Point in Run())
                    {
                        if (Point.IsDot)
                            TestDotKeys.Add((Point.X, Point.Y));
                        else
                            TestPointKeys.Add((Point.X, Point.Y));
                    }

                    //undo
                    if (points.Keys.Any(a => dots.ContainsKey(a)))
                    {
                        foreach ((int, int) PointKey in TestPointKeys)
                        {
                            if (PointKey.Item2 < Rows.Count)
                                Rows[PointKey.Item2].RemovePoint(PointKey.Item1);

                            if (PointKey.Item1 < Cols.Count)
                                Cols[PointKey.Item1].RemovePoint(PointKey.Item2);

                            points.Remove(PointKey);
                        }

                        foreach ((int, int) DotKey in TestDotKeys)
                        {
                            if (DotKey.Item2 < Rows.Count)
                                Rows[DotKey.Item2].RemoveDot(DotKey.Item1);

                            if (DotKey.Item1 < Cols.Count)
                                Cols[DotKey.Item1].RemoveDot(DotKey.Item2);

                            dots.Remove(DotKey);
                        }

                        TestPointKeys.Clear();
                        TestDotKeys.Clear();

                        IncorrectTrials.UnionWith(Trials);
                        Trials.Clear();
                    }

                    if (TrialCount == 10)
                        break;

                    TrialCount++;
                }

                foreach ((int, int) PointKey in TestPointKeys)
                    yield return points[PointKey];

                foreach ((int, int) DotKey in TestDotKeys)
                    yield return dots[DotKey];
            }

            IEnumerable<Point> Run()
            {
                while (!IsComplete(Rows.Values))
                {
                    int PointsChange = points.Count;
                    int DotsChange = dots.Count;
                    bool StrayDog = false;

                    if (Count == LoopCount)
                        break;

                    //row top edge <20ms
                    foreach (Point Point in LineEdgeTL(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column left edge <20ms
                    foreach (Point Point in LineEdgeTL(Cols.Values).SelectMany(s => s))
                        yield return Point;

                    //row bottom edge <20ms
                    foreach (Point Point in LineEdgeBR(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column right edge <20ms
                    foreach (Point Point in LineEdgeBR(Cols.Values).SelectMany(s => s))
                        yield return Point;

                    //row full line dots <5ms
                    foreach (Point Point in FullLineDots(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column full line dots <5ms
                    foreach (Point Point in FullLineDots(Cols.Values).SelectMany(s => s))
                        yield return Point;

                    (int, int)[] PointsCompare = points.Keys.ToArray();
                    (int, int)[] DotsCompare = dots.Keys.ToArray();

                    //row line dots <300ms
                    foreach (Point Point in LineDots(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column line dots <300ms
                    foreach (Point Point in LineDots(Cols.Values).SelectMany(s => s))
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
                    foreach (Point Point in LineGaps(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column line gaps <10ms
                    foreach (Point Point in LineGaps(Cols.Values).SelectMany(s => s))
                        yield return Point;

                    foreach ((int, int) Item in points.Keys.Where(w => dots.ContainsKey(w)))
                    {
                        Console.WriteLine($"stray dog x:{Item.Item1}y:{Item.Item2}");
                        StrayDog = true;
                    }

                    //busker30x30 - does not solve
                    //points.TryAdd((3, 13), new Point(false, 3, 13, false));
                    //points.TryAdd((0, 15), new Point(false, 0, 15, false));
                    //points.TryAdd((7, 23), new Point(false, 7, 23, true));
                    //points.TryAdd((7, 25), new Point(false, 7, 25, false)); //bug

                    //FourCallingBirds25x35 - does not solve
                    //points.TryAdd((6, 13), new Point(false, 6, 13, false));
                    //points.TryAdd((6, 14), new Point(false, 6, 14, false));
                    //points.TryAdd((6, 15), new Point(false, 6, 15, false));

                    //FiveGoldRing25x35 - does not solve
                    //dots.TryAdd((9, 11), new Point(true, 9, 11, false));
                    //dots.TryAdd((9, 13), new Point(true, 9, 13, false));
                                       

                    if (PointsChange == points.Count && DotsChange == dots.Count)
                        break;

                    if (StrayDog)
                        break;

                    Count++;
                }

            }

            Console.WriteLine($"Loop Count: {Count}");
        }

        private static bool IsComplete(IEnumerable<Line> lines)
            => lines.All(a => a.IsComplete);

        public IEnumerable<Point> AddPoints(Line line,
                                            int start,
                                            GriddlerPath.Action action,
                                            int? end = null,
                                            string? colour = null)
        {
            bool Dot = string.IsNullOrEmpty(colour);
            for (int Pos = start; Pos <= end.GetValueOrDefault(start); Pos++)
            {
                if (Pos < 0 || Pos >= line.LineLength)
                    continue;

                var Xy = line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos);

                Point NewPoint = new Point(Dot, Xy, colour ?? "black", action);
                bool New = (Dot && !line.Dots.Contains(Pos)) || (!Dot && !line.Points.ContainsKey(Pos));

                if (!Staging && !RemovedActions.Contains((byte)action))
                {
                    if (Dot)
                    {
                        dots.TryAdd(Xy, NewPoint);
                        line.AddDot(Pos, action);
                    }
                    else
                    {
                        points.TryAdd(Xy, NewPoint);
                        line.AddPoint(Pos, colour ?? "black", action);
                    }
                }

                if (New && !RemovedActions.Contains((byte)action))
                    yield return NewPoint;
            }
        }

        private static IEnumerable<Line> ForEachLine(IEnumerable<Line> lines,
                                                     Func<Line, bool>? run = null,
                                                     int minLineValue = 1)
        {
            foreach (Line L in lines)
            {
                if (L.LineValue >= minLineValue && !L.IsComplete && (run == null || run(L)))
                    yield return L;
            }
        }

        private IEnumerable<Point> FullPart(Line line,
                                            int linePosition,
                                            int startItem,
                                            int endItem,
                                            GriddlerPath.Action action,
                                            bool complete = true)
        {
            Point.Group++;

            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                string Colour = Item.Colour;
                foreach (Point Point in AddPoints(line, linePosition, action, linePosition + Item.Value - 1, Colour))
                    yield return Point;
                if (complete)
                    line.AddBlock(Item, true, linePosition, linePosition + Item.Value - 1);
                linePosition += Item.Value;
                if (linePosition < line.LineLength - 1 && line.GetDotCount(Item.Index) == 1)
                {
                    foreach (Point Pt in AddPoints(line, linePosition, action))
                        yield return Pt;

                    linePosition++;
                }
            }
        }

        private IEnumerable<Point> OverlapPart(Line line,
                                               int position,
                                               int lineEnd,
                                               int startItem,
                                               int endItem,
                                               GriddlerPath.Action action)
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
                        foreach (Point Pt in AddPoints(line, Pos, action, colour: Item.Colour))
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
        private IEnumerable<IEnumerable<Point>> FullLine(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines, (line) => line.LineValue == line.LineLength))
            {
                yield return FullPart(Line,
                                      0,
                                      0,
                                      Line.LineItems - 1,
                                      GriddlerPath.Action.FullLine);
            }
        }

        /// <summary>
        /// For each line add up the items and if the sum is greater than half the line length do an overlap
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<IEnumerable<Point>> OverlapLine(IEnumerable<Line> lines)
        {
            static bool Run(Line line) =>
                line.LineValue < line.LineLength && line.LineValue > line.LineLength / 2;
            foreach (Line Line in ForEachLine(lines, Run))
            {
                yield return OverlapPart(Line,
                                         0,
                                         Line.LineLength - 1,
                                         0,
                                         Line.LineItems - 1,
                                         GriddlerPath.Action.OverlapLine);
            }
        }

        /// <summary>
        /// For each line add up the points and if the sum equals the sum of the items fill the gaps with dots
        /// <para>
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<IEnumerable<Point>> FullLineDots(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines, (line) => line.LineValue - line.GetDotCount() == line.Points.Count, 0))
            {
                Point.Group++;
                for (int Pos = 0; Pos < Line.LineLength; Pos++)
                    if (!Line.Points.ContainsKey(Pos))
                        yield return AddPoints(Line, Pos, GriddlerPath.Action.FullLineDots);
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
        private IEnumerable<IEnumerable<Point>> LineEdgeTL(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                int LinePosition = 0, CurrentItem = 0, End = 0, FromIndex = 0;
                bool HadGap = false, ItemsOneValue = Line.ItemsOneValue;

                foreach ((int Pos, _, _, _, _) in Line.ForEachLinePos())
                {
                    if (Pos >= FromIndex)
                    {
                        //check for gap
                        bool NoMoreItems = false;
                        if (CurrentItem != Line.LineItems - 1 && Line.Points.ContainsKey(Pos))
                        {
                            (int GapStart, int GapEnd, _) = Line.FindGapStartEnd(Pos);

                            if (GapStart - LinePosition < Line[CurrentItem].Value
                                && Line[CurrentItem] + Line[CurrentItem + 1] > GapEnd - GapStart - 1)
                                NoMoreItems = true;
                            else
                            {
                                LineSegment LsEnd = Line.GetItemAtPositionB(GapEnd, null);
                                if (LsEnd.Valid && LsEnd.Eq && LsEnd.Index == CurrentItem)
                                    NoMoreItems = true;
                            }
                        }

                        int DotCount = CurrentItem > 0 ? Line.GetDotCount(CurrentItem - 1) : 0;
                        if ((Pos - LinePosition - DotCount < Line[CurrentItem].Value || NoMoreItems || ItemsOneValue)
                            && Line.Points.ContainsKey(Pos))
                        {
                            bool EndDot = false;
                            int ShiftCount = 0, CompleteCount = 0;

                            //--O--O
                            if (Pos > 0 && Pos + Line[CurrentItem].Value < Line.LineLength)
                            {
                                bool FirstSolid = false;
                                string HasError = "";

                                for (int d = Pos + Line[CurrentItem].Value; d >= Pos; d--)
                                {
                                    //var PosXy = Line.IsRow ? (d, Line.LineIndex) : (Line.LineIndex, d);
                                    if (Line.Points.TryGetValue(d, out string? Pt)
                                        && Pt == Line[CurrentItem].Colour)
                                    {
                                        if (d == Pos + Line[CurrentItem].Value)
                                            FirstSolid = true;
                                        else if (d < Pos + Line[CurrentItem].Value)
                                            CompleteCount++;

                                        if (FirstSolid)
                                            ShiftCount = Line[CurrentItem].Value - (d - Pos) + 1;
                                    }
                                    else
                                    {
                                        if (Line.Dots.Contains(d) 
                                            || (Pt != null && Pt != Line[CurrentItem].Colour))
                                            ShiftCount = Line[CurrentItem].Value - (d - Pos);

                                        CompleteCount = 0;
                                        FirstSolid = false;
                                    }
                                }
                                if (CompleteCount == Line[CurrentItem].Value)
                                {
                                    Point.Group++;

                                    if (!NoMoreItems && !ItemsOneValue)
                                        Line.AddBlock(Line[CurrentItem], true, Pos, Pos + CompleteCount - 1);

                                    if (Line.ShouldAddDots(CurrentItem).Item1)
                                    {
                                        EndDot = true;
                                        yield return AddPoints(Line,
                                                               Pos - 1,
                                                               GriddlerPath.Action.CompleteItem);
                                    }
                                    if (Pos + CompleteCount < Line.LineLength
                                        && Line.ShouldAddDots(CurrentItem).Item2)
                                    {
                                        EndDot = true;
                                        yield return AddPoints(Line,
                                                               Pos + CompleteCount,
                                                               GriddlerPath.Action.CompleteItem);
                                    }
                                }
                                else if (CompleteCount > Line[CurrentItem].Value)
                                    HasError = "lineEdgeTL: ahead gap filled";

                                //check for next solid bigger than next item + no room
                                if (!FirstSolid && CurrentItem < Line.LineItems - 1 && Line.ItemsOneColour)
                                {
                                    bool Flag = true;
                                    int NextItemSolidCount = 0;

                                    for (int d = Pos + 1; d <= Pos + Line[CurrentItem].Value; d++)
                                    {
                                        if (Line.Dots.Contains(d))
                                        {
                                            Flag = false;
                                            break;
                                        }
                                    }

                                    for (int d = Pos + Line[CurrentItem].Value + Line.GetDotCount(CurrentItem); d < Line.LineLength; d++)
                                    {
                                        if (Line.Points.TryGetValue(d, out string? Pt)
                                            && Pt == Line[CurrentItem].Colour)
                                            NextItemSolidCount++;
                                        else
                                            break;
                                    }

                                    if (Flag && NextItemSolidCount > Line[CurrentItem + 1].Value)
                                        ShiftCount += Line[CurrentItem + 1].Value + Line.GetDotCount(CurrentItem);
                                }

                                if (Pos - ShiftCount < 0)
                                    HasError = "lineEdgeTL: before shift no room";

                                if (!string.IsNullOrEmpty(HasError))
                                    Console.WriteLine(HasError);

                                if (ShiftCount > 0)
                                    Point.Group++;

                                yield return AddPoints(Line,
                                                       Pos - ShiftCount,
                                                       GriddlerPath.Action.LineBackwardShift,
                                                       Pos - 1,
                                                       Line[CurrentItem].Colour);
                            }

                            if (Line[CurrentItem].Value >= Pos - LinePosition + 1)
                                Point.Group++;

                            //forward shift and end dot
                            yield return AddPoints(Line,
                                                   Pos + 1,
                                                   GriddlerPath.Action.LineForwardShift,
                                                   LinePosition + Line[CurrentItem].Value - 1,
                                                   Line[CurrentItem].Colour);
                            if (Pos - LinePosition == 0 && Pos + Line[CurrentItem].Value < Line.LineLength && !EndDot
                                && Line.ShouldAddDots(CurrentItem).Item2)
                            {
                                yield return AddPoints(Line,
                                                       Pos + Line[CurrentItem].Value,
                                                       GriddlerPath.Action.LineForwardShift);
                            }

                            Line.AddBlock(Line[CurrentItem],
                                          Pos - LinePosition == 0,
                                          Pos + 1,
                                          LinePosition + Line[CurrentItem].Value - 1);

                            //back dots
                            int BackReach = Pos - Line[CurrentItem].Value + (CompleteCount - 1);
                            if (CompleteCount > 0 && (Pos - LinePosition - 1 < Line[CurrentItem].Value || !ItemsOneValue || Line.LineItems == 1)
                                && BackReach >= End)
                            {
                                Point.Group++;
                                yield return AddPoints(Line,
                                                       End,
                                                       GriddlerPath.Action.LineBackDots,
                                                       BackReach);
                            }

                            if (CurrentItem == Line.LineItems - 1)
                                break;

                            //3,4|---0-{LP}-{EI}----00
                            End = Pos + Line[CurrentItem].Value;
                            LinePosition += (Line[CurrentItem].Value + Line.GetDotCount(CurrentItem));
                            CurrentItem++;
                            FromIndex = Pos;
                            FromIndex += (Line[CurrentItem - 1].Value + Line.GetDotCount(CurrentItem - 1) - 1 - ShiftCount);
                            FromIndex++;
                        }
                        else if (Line.Dots.Contains(Pos) && !HadGap)
                            LinePosition = Pos + 1;
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
        private IEnumerable<IEnumerable<Point>> LineEdgeBR(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                int LinePosition = 0, CurrentItem = Line.LineItems - 1, End = Line.LineLength - 1;
                int FromIndex = Line.LineLength - 1;
                bool HadGap = false, ItemsOneValue = Line.ItemsOneValue;

                foreach ((int Pos, _, _, _, _) in Line.ForEachLinePos(b: true))
                {
                    if (Pos <= FromIndex)
                    {
                        //check for gap
                        bool NoMoreItems = false;
                        if (CurrentItem != 0 && Line.Points.ContainsKey(Pos) && !ItemsOneValue
                            && Line.LineLength - Pos - 1 - LinePosition - 1 >= Line[CurrentItem].Value)
                        {
                            (int GapStart, int GapEnd, _) = Line.FindGapStartEnd(Pos);

                            if (Line.LineLength - GapEnd - 1 - LinePosition < Line[CurrentItem].Value
                                && Line[CurrentItem] + Line[CurrentItem - 1] > GapEnd - GapStart - 1)
                                NoMoreItems = true;
                            else
                            {
                                LineSegment Ls = Line.GetItemAtPosition(GapStart);
                                if (Ls.Valid && Ls.Eq && Ls.Index == CurrentItem)
                                    NoMoreItems = true;
                            }
                        }

                        if (Line.Points.ContainsKey(Pos)
                            && (Line.LineLength - Pos - 1 - LinePosition - 1 < Line[CurrentItem].Value || NoMoreItems || ItemsOneValue))
                        {
                            bool EndDot = false;
                            int ShiftCount = 0, CompleteCount = 0;

                            if (Pos < Line.LineLength - 1)
                            {
                                //--O--O
                                bool FirstSolid = false;
                                string HasError = "";
                                for (int d = Pos - Line[CurrentItem].Value; d <= Pos; d++)
                                {
                                    if (Line.Points.TryGetValue(d, out string? Pt)
                                        && Pt == Line[CurrentItem].Colour)
                                    {
                                        if (d == Pos - Line[CurrentItem].Value)
                                            FirstSolid = true;
                                        else
                                            CompleteCount++;

                                        if (FirstSolid)
                                            ShiftCount = Line[CurrentItem].Value - (Pos - d) + 1;
                                    }
                                    else
                                    {
                                        if (Line.Dots.Contains(d) 
                                            || (Pt != null && Pt != Line[CurrentItem].Colour))
                                            ShiftCount = Line[CurrentItem].Value - (Pos - d);

                                        CompleteCount = 0;
                                        FirstSolid = false;
                                    }
                                }
                                if (CompleteCount == Line[CurrentItem].Value)
                                {
                                    Point.Group++;

                                    if (!NoMoreItems && !ItemsOneValue)
                                        Line.AddBlock(Line[CurrentItem], true, Pos - CompleteCount + 1, Pos);

                                    if (Line.ShouldAddDots(CurrentItem).Item2)
                                    {
                                        EndDot = true;
                                        yield return AddPoints(Line,
                                                               Pos + 1,
                                                               GriddlerPath.Action.CompleteItem);
                                    }
                                    if (Pos - CompleteCount >= 0 && Line.ShouldAddDots(CurrentItem).Item1)
                                    {
                                        EndDot = true;
                                        yield return AddPoints(Line,
                                                               Pos - CompleteCount,
                                                               GriddlerPath.Action.CompleteItem);
                                    }
                                }
                                else if (CompleteCount > Line[CurrentItem].Value)
                                    HasError = "lineEdgeBR: before gap filled";

                                if (Pos + ShiftCount > Line.LineLength)
                                    HasError = "lineEdgeBR: ahead shift no room";

                                if (ShiftCount > 0)
                                    Point.Group++;

                                yield return AddPoints(Line,
                                                       Pos + 1,
                                                       GriddlerPath.Action.LineForwardShift,
                                                       Pos + ShiftCount,
                                                       Line[CurrentItem].Colour);

                                if (!string.IsNullOrEmpty(HasError))
                                    Console.WriteLine(HasError);
                            }

                            if (Line[CurrentItem].Value >= (Line.LineLength - Pos - 1) - LinePosition)
                                Point.Group++;

                            //forward shift and end dot
                            yield return AddPoints(Line,
                                                   Pos + 1 - (Line[CurrentItem].Value - ((Line.LineLength - Pos - 1) - LinePosition)),
                                                   GriddlerPath.Action.LineBackwardShift,
                                                   Pos - 1,
                                                   Line[CurrentItem].Colour);
                            if (!EndDot && (Line.LineLength - Pos - 1) - LinePosition == 0
                                && Line.LineLength - Line[CurrentItem].Value - LinePosition > 0
                                && Line.ShouldAddDots(CurrentItem).Item1)
                            {
                                yield return AddPoints(Line,
                                                       Pos - Line[CurrentItem].Value,
                                                       GriddlerPath.Action.LineBackwardShift);
                            }

                            Line.AddBlock(Line[CurrentItem],
                                          (Line.LineLength - Pos - 1) - LinePosition == 0,
                                          Pos + 1 - (Line[CurrentItem].Value - ((Line.LineLength - Pos - 1) - LinePosition)),
                                          Pos - 1);

                            //back dots
                            int BackReach = Pos + Line[CurrentItem].Value - (CompleteCount - 1);
                            if (CompleteCount > 0 && BackReach <= End &&
                                (Line.LineLength - Pos - 1 - LinePosition - 1 < Line[CurrentItem].Value
                                || !ItemsOneValue || Line.LineItems == 1))
                            {
                                Point.Group++;
                                yield return AddPoints(Line,
                                                       BackReach,
                                                       GriddlerPath.Action.LineBackDots,
                                                       End);
                            }

                            if (CurrentItem == 0)
                                break;

                            End = Pos - Line[CurrentItem].Value;
                            LinePosition += (Line[CurrentItem].Value + Line.GetDotCountB(CurrentItem));
                            CurrentItem--;
                            FromIndex = Pos;
                            FromIndex -= (Line[CurrentItem + 1].Value + Line.GetDotCountB(CurrentItem + 1) - 1 - ShiftCount);
                            FromIndex--;
                        }
                        else if (Line.Points.ContainsKey(Pos))
                            break;
                        else if (Line.Dots.Contains(Pos) && !HadGap)
                            LinePosition = Line.LineLength - Pos;
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
        private IEnumerable<IEnumerable<Point>> LineGaps(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                bool FirstSolid = false;

                foreach ((int Pos, _, _, int GapSize, _) in Line.ForEachLinePos())
                {
                    int GapSizeCopy = GapSize;
                    Debug.Assert(Line.Dots.Contains(Pos) == Line.Dots.Contains(Pos));
                    if (Line.Dots.Contains(Pos) || Pos == Line.LineLength - 1)
                    {
                        int IsEnd = (!Line.Dots.Contains(Pos) && Pos == Line.LineLength - 1) ? 1 : 0;

                        if (IsEnd == 1)
                            GapSizeCopy++;

                        if (GapSizeCopy > 0)
                        {
                            if (Line.MinItem > GapSizeCopy)
                            {
                                yield return AddPoints(Line,
                                                       Pos - GapSizeCopy + IsEnd,
                                                       GriddlerPath.Action.GapDotsMinItem,
                                                       Pos + IsEnd - 1);
                            }
                            else
                            {
                                static bool NoItemForGap(int g, bool e, Item? i)
                                    => e && (!i.HasValue || g < i.Value.Value);

                                //multiple solids
                                LineSegment Ls = Line.GetItemAtPosition(Pos - GapSizeCopy + IsEnd);
                                var (Item, Equality, Index, EqualityIndex, _) = Ls;

                                if (NoItemForGap(GapSizeCopy, Equality, Item))
                                {
                                    yield return AddPoints(Line,
                                                           Pos - GapSizeCopy + IsEnd,
                                                           GriddlerPath.Action.GapDotsTooBig,
                                                           Pos + IsEnd - 1);
                                }
                                else
                                {
                                    LineSegment LsEnd = Line.GetItemAtPositionB(Pos + IsEnd, null);
                                    var (ItemE, EqualityE, IndexE, EqualityIndexE, _) = LsEnd;
                                    ItemRange Range = Line.CreateRange(Math.Min(Index, IndexE), Math.Max(Index, IndexE));//Ls.With(LsEnd, true);
                                    int Sum = Range.Sum();

                                    bool NextColourSumTooBig = false;
                                    Item? FirstColourItem = LsEnd.After.LastOrDefault(l => l.Index < IndexE &&
                                                                    l.Colour == Ls.RightBefore.First().Colour);
                                    if (LsEnd.Valid && EqualityE && FirstColourItem.HasValue
                                        && Ls.RightBefore.First().End == Pos - GapSizeCopy + IsEnd - 2)
                                    {
                                        //int ColourSum = new ItemRange(LsEnd.After.Where(w => w.Index > FirstColourItem.Value.Index)).Sum();
                                        int ColourSum = Line.CreateRange(FirstColourItem.Value.Index + 1, IndexE).Sum();
                                        if (ColourSum > GapSizeCopy)
                                            NextColourSumTooBig = true;
                                    }

                                    if (NoItemForGap(GapSizeCopy, EqualityE, ItemE)
                                        || (Equality && EqualityE && Index > IndexE)
                                        || (Equality && EqualityE && Sum > GapSizeCopy)
                                        || Range.All(a => a.Value > GapSizeCopy)
                                        || (LsEnd.Valid && Equality && EqualityIndexE < Index)
                                        || (Ls.Valid && EqualityE && EqualityIndex > IndexE)
                                        || (NextColourSumTooBig)
                                        || (Equality && LsEnd.ScV && Index < Line.LineItems - 1 && Line[Index + 1].Value > LsEnd.Sc
                                            && Line[Index + 1] + Line[Index] > GapSizeCopy)
                                        || (EqualityE && Ls.ScV && IndexE > 0 && Line[IndexE - 1].Value > Ls.Sc
                                            && Line[IndexE - 1] + Line[IndexE] > GapSizeCopy)
                                        )
                                    {
                                        GriddlerPath.Action Action = GriddlerPath.Action.GapDotsTooBig;
                                        Point.Group++;
                                        yield return AddPoints(Line,
                                                               Pos - GapSizeCopy + IsEnd,
                                                               Action,
                                                               Pos + IsEnd - 1);
                                    }
                                    else if (Ls.Valid && (LsEnd.Valid
                                        || (Equality && Index > 0 && LsEnd.ScV && Line[Index].Value != LsEnd.Sc)
                                        ) && Sum == GapSizeCopy)
                                    {
                                        yield return FullPart(Line,
                                                              Pos - GapSizeCopy + IsEnd,
                                                              Index,
                                                              IndexE,
                                                              GriddlerPath.Action.GapFull);
                                    }
                                    else if (Ls.Valid && FirstSolid
                                        && Index <= IndexE && Ls.ItemsOneValue)
                                    {
                                        yield return FullPart(Line,
                                                              Pos - GapSizeCopy + IsEnd,
                                                              Index,
                                                              Index,
                                                              GriddlerPath.Action.GapFull,
                                                              false);
                                    }
                                    else if (Item.HasValue && ItemE.HasValue && (Index == IndexE
                                            || Equality || EqualityE) && Sum < GapSizeCopy && Sum > GapSizeCopy / 2)
                                    {
                                        GriddlerPath.Action Action = GriddlerPath.Action.GapOverlapSameItem;
                                        if (Equality || EqualityE)
                                            Action = GriddlerPath.Action.GapOverlap;

                                        yield return OverlapPart(Line,
                                                                 Pos - GapSizeCopy + IsEnd,
                                                                 Pos - 1 + IsEnd,
                                                                 Index,
                                                                 IndexE,
                                                                 Action);
                                    }
                                }
                            }
                            FirstSolid = false;
                        }
                    }
                    else
                    {
                        if (GapSizeCopy == 0 && Line.Points.ContainsKey(Pos))
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
        private IEnumerable<IEnumerable<Point>> LineDots(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                Item MinItem = new Item(Line.MinItem, "black"), MaxItem = new Item(Line.MaxItem, "black");
                int MaxItemIndex = -1, IsolatedItem = 0;
                (bool LineIsolated, bool Valid, IDictionary<int, int> Isolations) = Line.IsLineIsolated();
                IDictionary<int, (Item, Item)> MinMaxItems = Line.GetMinMaxItems();

                foreach (Item Item in Line)
                {
                    if (Item.Value == MaxItem.Value)
                        MaxItemIndex = Item.Index;
                }

                foreach ((int Pos, Block Block, _, _, _) in Line.ForEachLinePos((nS, wS, c, gS) => nS && wS))
                {
                    bool NoMoreItems = false;
                    if (Line.UniqueCount(Block, out Item _))
                        NoMoreItems = true;

                    bool Break = false;

                    (int StartOfGap, int EndOfGap, bool WasSolid) = Line.FindGapStartEnd(Pos - 1, Pos);
                    LineSegment Ls = Line.GetItemAtPosition(StartOfGap, Block, EndOfGap);
                    var (Item, Equality, Index, EqualityIndex, _) = Ls;
                    LineSegment LsEnd = Line.GetItemAtPositionB(Pos - 1, false);
                    //LineSegment LsEnd = Line.GetItemAtPositionB(EndOfGap, Block, StartOfGap);
                    var (ItemE, EqualityE, IndexE, EqualityIndexE, _) = LsEnd;

                    IsolatedItem = Block.BlockIndex;

                    if (MinMaxItems.TryGetValue(Pos, out (Item, Item) M))
                    {
                        MinItem = M.Item1;
                        MaxItem = M.Item2;
                    }
                    else
                    {
                        MinItem = new Item(Line.MinItem, "black");
                        MaxItem = new Item(Line.MaxItem, "black");
                    }

                    //must join
                    if (!Line.Dots.Contains(Block.End + 1)
                        && !Line.Points.ContainsKey(Block.End + 1)
                        && Line.Points.ContainsKey(Block.End + 2)
                        && !ItemRange.Pair(Line.Where(EqualityIndex, EqualityIndexE))
                        .Any(s => Block.CanBe(s.Item1)
                        && s.Item1.Value <= Block.End - (StartOfGap + 1) + 1
                        && s.Item2.Value <= (EndOfGap - 1)
                        - (Block.End + 1 + Line.GetDotCount(s.Item1.Index)) + 1))
                    {
                        int PointsChange = Line.Points.Count;
                        yield return AddPoints(Line,
                                               Block.End + 1,
                                               GriddlerPath.Action.MustJoin,
                                               colour: Block.Colour);
                        if (Line.Points.Count != PointsChange)
                            break; //change block count - break isolations
                    }
                    else if ((EqualityIndex == Ls.ItemAtStartOfGap 
                        || (EqualityIndex == -1 && Ls.ItemAtStartOfGap == 0))
                        && (EqualityIndexE == LsEnd.ItemAtStartOfGap 
                        || (EqualityIndexE == Line.LineItems && LsEnd.ItemAtStartOfGap == Line.LineItems - 1))
                        && Ls.ItemAtStartOfGap + 1 == LsEnd.ItemAtStartOfGap
                        && Line.GetNumberOfBlocks(StartOfGap + 1, EndOfGap - 1) == 3)
                    {
                        Block? LastBlock = Line.GetBlock(Block.Start - 1, false);
                        Block? NextBlock = Line.GetBlock(Block.End + 1);

                        if (LastBlock != null && NextBlock != null
                            && Ls.ItemAtStartOfGap < Line.LineItems)
                        {
                            bool Isolated = Line.IsolatedPart(Line[Ls.ItemAtStartOfGap],
                                                              Block,
                                                              LastBlock);

                            if (Isolated)
                            {
                                int PointsChange = Line.Points.Count;
                                yield return AddPoints(Line,
                                                       Block.End + 1,
                                                       GriddlerPath.Action.MustJoin,
                                                       NextBlock.Start - 1,
                                                       Block.Colour);
                                if (Line.Points.Count != PointsChange)
                                    break; //change block count - break isolations
                            }
                        }
                    }

                    //No Join//3,2,5,...//|---.-00-0- to |---.-00.0-
                    if (!Line.Dots.Contains(Pos)
                        && Line.Points.TryGetValue(Pos + 1, out string? Pt)
                        && Pt == Block.Colour)
                    {
                        int Start = Block.Start - 1, End = Line.LineLength;
                        bool Flag = false;
                        string PrevPtG = Block.Colour;
                        for (int Pos2 = Pos + 1; Pos2 < Line.LineLength; Pos2++)
                        {
                            if (!Line.Points.TryGetValue(Pos2, out Pt) || Pt != PrevPtG)
                            {
                                End = Pos2;
                                break;
                            }
                            else
                                PrevPtG = Pt;
                        }

                        Flag = Line.Where(EqualityIndex, EqualityIndexE)
                            .All(a => a.Value < End - Start - 1 && (a.Value > 1 || a.Colour == Block.Colour));

                        if (!Flag && Ls.Valid && Ls.All(a => a.Value > 1 || a.Colour == Block.Colour)
                            && Line.IsEq(Ls.Gap, Index - 1, StartOfGap, Pos - Block.Size)
                            && Ls.Before.All(a => a.Value < End - Start - 1 && (a.Value > 1 || a.Colour == Block.Colour)))
                            Flag = true;

                        Block BlackOne = new Block(-1, Block.Colour == "black" ? "lightgreen" : "black") { Size = 1 };
                        if (!Flag && !Line.ItemsOneColour
                            && Line.Where(Ls.EqualityIndex, Index + 1)
                            .All(a => a.Value < End - Start - 1))
                        {
                            IEnumerable<Item> Items = Line.Triple().Where((w, i) => w.Item3.Index >= Ls.EqualityIndex
                                                                        && w.Item3.Index <= Index + 1
                                                                        && w.Item1.Colour == Block.Colour
                                                                        && BlackOne.Is(w.Item2)
                                                                        && w.Item3.Colour == Block.Colour)
                                                                .Select(s => s.Item3);
                            Item? Second = Items.FirstOrDefault();

                            if (!Second.HasValue
                                || (Items.Count() == 1
                                    && !Line.FindPossibleSolids(Pos + 1).Contains((Second.Value.Value, Second.Value.Colour))))
                                Flag = true;
                        }

                        if (Flag)
                        {
                            Point.Group++;
                            yield return AddPoints(Line,
                                                   Pos,
                                                   GriddlerPath.Action.NoJoin);
                            break;
                        }
                    }

                    bool MinItemForwards(out int start, out int min)
                    {
                        start = 0;
                        min = MinItem.Value;

                        if (Pos - StartOfGap - 1 < MinItem.Value)
                            return true;

                        if (Line.Points.TryGetValue(Pos - Block.Size - 1, out Pt) 
                            && Pt != Block.Colour
                            && Ls.Valid && Ls.Before.Any()
                            && Line.IsEq(Ls.Gap, Index - 1, StartOfGap, Pos - Block.Size)
                            )
                        {
                            Item[] Matches = Ls.Before.Pair().Where(f => f.Item1.Colour == Pt
                                                                && f.Item2.Colour == Block.Colour)
                                                            .Select(s => s.Item2).ToArray();

                            if (Matches.Length == 1)
                            {
                                min = Matches[0].Value;
                                start = Pos - StartOfGap - Block.Size - 1;
                                return true;
                            }
                        }

                        if (Ls.Valid && Ls.Gap.Any() && Ls.Before.All(a => a.Value < Block.Size))
                        {
                            start = Ls.Gap.Sum() + Line.GetDotCount(Index - 1);
                            return true;
                        }

                        return false;
                    }

                    bool MinItemBackwards(out int end)
                    {
                        end = 0;

                        if (EndOfGap - (Pos - Block.Size - 1) - 1 < MinItem.Value)
                            return true;

                        if (Line.Points.TryGetValue(Pos, out Pt) && Pt != Block.Colour)
                        {
                            end = EndOfGap - Pos;
                            return true;
                        }

                        if (LsEnd.Valid && LsEnd.Gap.Any()
                                && LsEnd.Before.All(a => !Block.CanBe(a)))
                        {
                            end = LsEnd.Gap.Sum() + Line.GetDotCountB(IndexE + 1);
                            return true;
                        }

                        return false;
                    }

                    //min item backwards
                    if (MinItemBackwards(out int En))
                    {
                        Point.Group++;
                        yield return AddPoints(Line,
                                               EndOfGap - En - MinItem.Value,
                                               GriddlerPath.Action.MinItem,
                                               Pos - Block.Size - 1,
                                               Block.Colour);
                    }

                    //min item forwards
                    if (MinItemForwards(out int St, out int Min))
                    {
                        Point.Group++;
                        int PointsChange = points.Count;
                        yield return AddPoints(Line,
                                               Pos,
                                               GriddlerPath.Action.MinItem,
                                               StartOfGap + St + Min,
                                               Block.Colour);
                        Break = points.Count - PointsChange > 0;
                    }

                    bool SingleItemStart(out int m, out int gap)
                    {
                        int Start = 0, End = Line.LineItems - 1;
                        (m, gap) = (0, StartOfGap);

                        if ((LineIsolated || (NoMoreItems && MaxItemIndex == 0)
                            || (Valid && Isolations.TryGetValue(Block.BlockIndex, out IsolatedItem)))
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
                            && !Block.CanBe(Line[Start + 1])
                            && Line.IsEq(Line.CreateRange(Start, Start + 1), Start + 1, StartOfGap, Pos - Block.Size))
                        {
                            m = Line[End].Value;
                            return true;
                        }

                        for (int d = Start; d <= End; d++)
                        {
                            if (Line[d].Value > m)
                                m = Line[d].Value;

                            if (StartOfGap + Line[d].Value < Pos - Block.Size - Line.GetDotCount(d))
                                return false;
                        }

                        return true;
                    }

                    bool SingleItemEnd(out int m, out int gap)
                    {
                        int Start = 0, End = Line.LineItems - 1;
                        (m, gap) = (0, EndOfGap);

                        if (!WasSolid && (LineIsolated
                            || (Valid && Isolations.TryGetValue(Block.BlockIndex, out IsolatedItem)
                            && IsolatedItem == End)))
                        {
                            m = Line[IsolatedItem].Value;

                            if (IsolatedItem == End)
                                gap = Line.LineLength;

                            return true;
                        }

                        if (LsEnd.ItemAtStartOfGap >= 0)
                            Start = LsEnd.ItemAtStartOfGap;

                        if (LsEnd.EqualityIndex == LsEnd.ItemAtStartOfGap)
                            End = LsEnd.ItemAtStartOfGap;
                        else if (Index < End && Index >= Start
                                && Line.IsEq(Ls.Gap, Index - 1, StartOfGap, Pos - Block.Size))
                            End = Index;

                        if (Start == End && End > 0
                            && !Block.CanBe(Line[End - 1])
                            && End != Index
                            && Line.IsEqB(End - 1, Pos, EndOfGap, Line[End].Value)
                            //&& EndOfGap - Pos - Line[End].Value <= Line.GetDotCountB(End)
                            )
                        {
                            m = Line[End].Value;
                            return true;
                        }

                        if (LsEnd.Valid && LsEnd.Before.Any()
                            && LsEnd.UniqueCount(Block, out Item Match)
                            && Line.IsEqB(LsEnd.Gap, IndexE + 1, Pos, EndOfGap)
                            && Match.Index == Line.LineItems - 1)
                        {
                            m = Match.Value;
                            return true;
                        }

                        foreach (Item Item in Line.Skip(Start).Take(End - Start + 1))
                        {
                            if (Item.Value > m)
                                m = Item.Value;

                            if (Pos + Item.Value + Line.GetDotCount(Item.Index - 1 >= 0 ? Item.Index - 1 : 0) <= EndOfGap)
                                return false;
                        }

                        return true;
                    }

                    //item backward reach//
                    if (SingleItemStart(out int MaxItemS, out int GapStart))
                    {
                        Point.Group++;
                        yield return AddPoints(Line,
                                               GapStart + 1,
                                               GriddlerPath.Action.ItemBackwardReach,
                                               Pos - MaxItemS - 1);
                    }

                    //item forward reach//
                    if (SingleItemEnd(out int MaxItemE, out int GapEnd)
                        && Pos - Block.Size + MaxItemE < GapEnd)
                    {
                        Point.Group++;
                        int DotsChange = dots.Count;
                        yield return AddPoints(Line,
                                               Pos - Block.Size + MaxItemE,
                                               GriddlerPath.Action.ItemForwardReach,
                                               GapEnd - 1);
                        Break = dots.Count - DotsChange > 0;
                    }

                    //Sum Dots Forward//1,1,1,4,...//|------0--// to |-----.0--
                    if (Ls.Valid && Pos - Block.Size - 1 >= 0
                        && Ls.Before.All(Block.Is)
                        && Line.IsEq(Ls.Gap, Index - 1, StartOfGap, Pos - Block.Size - 1))
                    {
                        Point.Group++;
                        yield return AddPoints(Line,
                                               Pos - Block.Size - 1,
                                               GriddlerPath.Action.SumDotForward);
                    }

                    ///Sum Dot Backward//...,3,1,1,3//-0----.000.|// to -0.---.000.|
                    if (LsEnd.Valid && LsEnd.Before.All(Block.Is)
                        && Line.IsEqB(LsEnd.Gap, IndexE + 1, Pos, EndOfGap))
                    {
                        Point.Group++;
                        yield return AddPoints(Line,
                                               Pos,
                                               GriddlerPath.Action.SumDotBackward);
                    }

                    //Half gap overlap forwards//...,2,2//.--0----| to .--0--0-|
                    if (!Break && Ls.Valid && Index > 0
                        && EndOfGap - (Pos + 1) >= Line[Index].Value)
                    {
                        //need to check if Pos is on Item
                        ItemRange ER = Line.CreateRange(Index, LsEnd.ItemAtStartOfGap);

                        if (Index > Ls.ItemAtStartOfGap
                            && Index <= LsEnd.ItemAtStartOfGap && ER.Any()
                        && ER.Sum() <= EndOfGap - (Pos + 1)
                        && ER.Sum() > (EndOfGap - (Pos + 1)) / 2
                        && Line.IsEq(Ls.Gap, Index - 1, StartOfGap, Pos - Block.Size)
                        )
                        {
                            Point.Group++;
                            int PointsChange = points.Count;
                            yield return OverlapPart(Line,
                                                     Pos + Line.GetDotCount(Index - 1),
                                                     EndOfGap - 1,
                                                     Index,
                                                     LsEnd.ItemAtStartOfGap,
                                                     GriddlerPath.Action.HalfGapOverlap);
                            Break = points.Count - PointsChange > 0;
                        }
                    }

                    //Half gap overlap backwards//4,6,...//|.-----000000. to |.-000-000000.
                    if (Ls.Valid && Index > 0)
                    {
                        int BlockIndex = Line.FirstOrDefault(Block.Is)?.Index ?? -1;
                        //ItemRange ER = new ItemRange(Line.Where(w => w.Index >= Ls.ItemAtStartOfGap && w.Index < BlockIndex));
                        ItemRange ER = Line.CreateRange(Ls.ItemAtStartOfGap, BlockIndex - 1);
                        if (NoMoreItems && ER.Any()
                        && ER.Sum() <= Pos - Block.Size - 1 - StartOfGap
                        && ER.Sum() > (Pos - Block.Size - 1 - StartOfGap) / 2
                        //&& Line.IsEq(ER, Index - 1, StartOfGap, Pos - Block.Size)
                        )
                        {
                            Point.Group++;
                            yield return OverlapPart(Line,
                                                     StartOfGap + 1,
                                                     Pos - Block.Size - 1,
                                                     Ls.ItemAtStartOfGap,
                                                     BlockIndex - 1,
                                                     GriddlerPath.Action.HalfGapOverlap);
                        }
                    }

                    //Isolated Items Reach//1,9,3// |---0-0------------0 to |---0-0--------..--0
                    if ((LineIsolated ||
                        (StartOfGap <= 0 && Valid))
                        && Isolations.TryGetValue(Block.BlockIndex - 1, out int FirstI)
                        && Isolations.TryGetValue(Block.BlockIndex, out int SecondI))
                    {
                        int End = Pos - Line[SecondI].Value - 1;
                        int Start = -1;
                        bool Flag = false;

                        for (int d = End; d >= 0; d--)
                        {
                            if (Line.Points.TryGetValue(d, out string? Pt2) 
                                && Pt2 == Line[FirstI].Colour)
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
                            yield return AddPoints(Line,
                                                   Start,
                                                   GriddlerPath.Action.IsolatedItemsReach,
                                                   End);
                        }
                    }

                    //Half Gap Full Part//2,2,1,1,1,2,1,1// --0--.0| to --0.0.0|
                    if (Ls.Valid && Index < Line.LineItems - 1
                    && !Line.Dots.Contains(Pos) && StartOfGap <= 0)
                    {
                        int Sum = 0;

                        for (int d = 0; d < Index; d++)
                        {
                            Sum += Line[d].Value;

                            if (Line.Points.ContainsKey(StartOfGap + Sum + 1))
                                Sum++;

                            Sum += Line.GetDotCount(d);
                        }

                        if (Pos - StartOfGap - 1 == Sum
                        && LsEnd.Gap.Any() && LsEnd.ItemAtStartOfGap == Index
                        && EndOfGap - Pos - Line.GetDotCount(Index - 1) == Line[Index].Value)
                        {
                            if (Line.ShouldAddDots(Index - 1).Item2)
                            {
                                yield return AddPoints(Line,
                                                       Pos,
                                                       GriddlerPath.Action.HalfGapFullPart);
                            }

                            yield return FullPart(Line,
                                                  Pos + Line.GetDotCount(Index - 1),
                                                  Index,
                                                  LsEnd.ItemAtStartOfGap,
                                                  GriddlerPath.Action.HalfGapFullPart);

                            break;
                        }
                    }

                    bool CompleteItem(out bool s, out bool e, out int? itmIdx, out Item? item)
                    {
                        (s, e, itmIdx, item) = (false, false, null, null);

                        if ((LineIsolated || (Valid && Isolations.TryGetValue(Block.BlockIndex, out IsolatedItem)))
                        && Block.Size == Line[IsolatedItem].Value)
                        {
                            (s, e) = Line.ShouldAddDots(IsolatedItem);
                            (itmIdx, item) = (IsolatedItem, Line[IsolatedItem]);
                            return true;
                        }

                        if (Block.Is(MaxItem))
                        {
                            bool Flag = true;

                            if (!Line.ItemsOneColour && MaxItem.Index > 0
                                    && Line[MaxItem.Index - 1].Value == MaxItem.Value)
                                Flag = false;
                            else if (!Line.ItemsOneColour && MaxItem.Index < Line.LineItems - 1
                                        && Line[MaxItem.Index + 1].Value == MaxItem.Value)
                                Flag = false;

                            //var PosXy = Line.IsRow ? (Pos - Block.Size - 1, Line.LineIndex) : (Line.LineIndex, Pos - Block.Size - 1);
                            if (Line.Points.TryGetValue(Pos - Block.Size - 1, out Pt) 
                                && Pt != Block.Colour && MaxItem.Index > 0)
                                Flag = true;

                            if (Flag)
                                (s, e) = Line.ShouldAddDots(MaxItem.Index);

                            return true;
                        }

                        if (Ls.Valid && Ls.Before.ItemsOneValue
                            && Line.IsEq(Ls.Gap, Index - 1, StartOfGap, Pos - Block.Size)
                            && Block.Size == Line[Index - 1].Value)
                        {
                            (s, e) = Line.ShouldAddDots(Index - 1);
                            return true;
                        }

                        return false;
                    }

                    //solid count equals item//
                    if (CompleteItem(out bool S, out bool E, out int? ItmIdx, out Item? Itm))
                    {
                        Point.Group++;

                        if (ItmIdx.HasValue && Itm.HasValue)
                            Line.AddBlock(Itm.Value, true, Pos - Block.Size, Pos - 1);

                        if (Pos - Block.Size - 1 > 0 && S)
                        {
                            yield return AddPoints(Line,
                                                   Pos - Block.Size - 1,
                                                   GriddlerPath.Action.CompleteItem);
                        }

                        if (E)
                        {
                            yield return AddPoints(Line,
                                                   Pos,
                                                   GriddlerPath.Action.CompleteItem);
                        }
                    }

                    if (Break)
                        break;
                }
            }
        }

        /// <summary>
        /// Restrict where items of a certain colour can go by looking at 
        /// the perpendicular lines and their appropriate items. 
        /// Probably only for the edges.
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<IEnumerable<Point>> MultiColourCrossReference(IEnumerable<Line> lines)
        {
            int MaxLines = lines.First().IsRow ? Rows.Count : Cols.Count;

            foreach (Line Line in ForEachLine(lines))
            {
                if (Line.LineIndex == 0 || Line.LineIndex == MaxLines - 1)
                {
                    int ColourBlockIndex = 0;
                    List<Block> ColourCounts = new List<Block>(10) { new Block(false, 0) };
                    string PrevColour = "black";

                    foreach ((int Pos, _, _, _, _) in Line.ForEachLinePos())
                    {
                        string Colour = "black";

                        if (Line.IsRow)
                        {
                            int ItemIndex = Line.LineIndex == 0 ? 0 : Cols[Pos].LineItems - 1;
                            Colour = Cols[Pos][ItemIndex].Colour;
                        }
                        else
                        {
                            int ItemIndex = Line.LineIndex == 0 ? 0 : Rows[Pos].LineItems - 1;
                            Colour = Rows[Pos][ItemIndex].Colour;
                        }

                        if (Colour == PrevColour)
                        {
                            ColourCounts[ColourBlockIndex].End = Pos;
                            ColourCounts[ColourBlockIndex].Size++;
                        }
                        else
                        {
                            PrevColour = Colour;
                            ColourBlockIndex++;
                            ColourCounts.Add(new Block(Pos, Pos, Colour));
                        }
                    }

                    if (Line.Count(c => c.Colour == "lightgreen") == ColourCounts.Count(c => c.Colour == "lightgreen"))
                    {
                        foreach ((Block, Item) Item in ColourCounts.Where(w => w.Colour == "lightgreen").Zip(Line))
                        {
                            if (Item.Item2.Value == Item.Item1.Size)
                            {
                                yield return FullPart(Line,
                                                      Item.Item1.Start,
                                                      Item.Item2.Index,
                                                      Item.Item2.Index,
                                                      GriddlerPath.Action.GapFull);
                            }
                            else if (Item.Item2.Value < Item.Item1.Size
                                    && Item.Item2.Value >= Item.Item1.Size / 2)
                            {
                                yield return OverlapPart(Line,
                                                         Item.Item1.Start,
                                                         Item.Item1.End,
                                                         Item.Item2.Index,
                                                         Item.Item2.Index,
                                                         GriddlerPath.Action.GapFull);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add one point at a time
        /// Note: Two adjacent items of the same colour must have a dot between them
        /// </para>
        /// </summary>
        /// <param name="lines">The rows or colums</param>
        private IEnumerable<Point> TrialAndError(IEnumerable<Line> lines)
        {
            bool Flag = false;
            foreach (Line Line in ForEachLine(lines))
            {
                foreach ((int Pos, _, _, _, _) in Line.ForEachLinePos())
                {
                    var Xy = Line.IsRow ? (Pos, Line.LineIndex) : (Line.LineIndex, Pos);
                    if (!Line.Points.ContainsKey(Pos) && !Line.Dots.Contains(Pos)
                        && !IncorrectTrials.Contains(Xy))
                    {
                        LineSegment Ls = Line.GetItemAtPosition(Pos);

                        if (Ls.Valid && Ls.Eq && Ls.Item.HasValue)
                        {
                            //add one point
                            foreach (Point Point in AddPoints(Line, Pos, GriddlerPath.Action.TrialAndError, Pos, Ls.Item.Value.Colour))
                            {
                                Trials.Add((Point.X, Point.Y));
                                yield return Point;
                            }

                            Flag = true;
                            break;
                        }
                    }
                }

                if (Flag)
                    break;
            }
        }
    }
}