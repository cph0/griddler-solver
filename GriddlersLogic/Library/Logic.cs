using Griddlers.Database;
using System;
using System.Collections.Generic;
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
            //        dots.Remove((Point.X, Point.Y));
            //    else
            //        points.Remove((Point.X, Point.Y));
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
                    if (First[c] != Last[c])
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
                        if (First[c] != Last[c])
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
            if (!Staging && false && points.Count + dots.Count < Width * Height
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
                    if (Rows.Values.Any(r => r.Points.Keys.Any(p => r.Dots.Contains(p))))
                    {
                        foreach ((int, int) PointKey in TestPointKeys)
                        {
                            if (PointKey.Item1 < Cols.Count)
                                Rows[PointKey.Item1].RemovePoint(PointKey.Item2);

                            points.Remove(PointKey);
                        }

                        foreach ((int, int) DotKey in TestDotKeys)
                        {
                            if (DotKey.Item1 < Cols.Count)
                                Rows[DotKey.Item1].RemoveDot(DotKey.Item2);

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

                    //row full line dots <5ms
                    foreach (Point Point in FullLineDots(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column full line dots <5ms
                    foreach (Point Point in FullLineDots(Cols.Values).SelectMany(s => s))
                        yield return Point;

                    //row line dots <300ms
                    foreach (Point Point in CompleteItem(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column line dots <300ms
                    foreach (Point Point in CompleteItem(Cols.Values).SelectMany(s => s))
                        yield return Point;

                    //row line dots <300ms
                    foreach (Point Point in LineBlocks(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column line dots <300ms
                    foreach (Point Point in LineBlocks(Cols.Values).SelectMany(s => s))
                        yield return Point;

                    //row line gaps <10ms
                    foreach (Point Point in LineGaps(Rows.Values).SelectMany(s => s))
                        yield return Point;

                    //column line gaps < 10ms
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
            //foreach (KeyValuePair<string, int> Method in MethodCounts)
            //{
            //    Console.WriteLine($"{Method.Key}: {Method.Value}");
            //}
        }

        private static bool IsComplete(IEnumerable<Line> lines)
            => lines.All(a => a.IsComplete);

        public IEnumerable<Point> AddPoints(Line line,
                                            int start,
                                            GriddlerPath.Action action,
                                            int? end = null,
                                            string? colour = null)
        {
            bool dot = string.IsNullOrEmpty(colour);
            for (int Pos = start; Pos <= end.GetValueOrDefault(start); Pos++)
            {
                if (Pos < 0 || Pos >= line.LineLength)
                    continue;

                var Xy = line.IsRow ? (Pos, line.LineIndex) : (line.LineIndex, Pos);

                Point NewPoint = new Point(dot, Xy, colour ?? "black", action);
                bool New = (dot && !dots.ContainsKey(Xy)) || (!dot && !points.ContainsKey(Xy));

                if (!Staging && !RemovedActions.Contains((byte)action))
                {
                    if (dot)
                    {
                        dots.TryAdd(Xy, NewPoint);
                        line.AddDot(Pos, action);
                    }
                    else
                    {
                        points.TryAdd(Xy, NewPoint);
                        line.AddPoint(Pos, colour, action);
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
                                            GriddlerPath.Action action)
        {
            Point.Group++;

            foreach (Item Item in line.Skip(startItem).Take(endItem - startItem + 1))
            {
                foreach (Point Point in AddPoints(line,
                                                  linePosition,
                                                  action,
                                                  linePosition + Item.Value - 1,
                                                  Item.Colour))
                    yield return Point;
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
                for (int Pos = position; Pos <= lineEnd; Pos++)
                {
                    if (LineFlagsForward[Pos, Item.Index] && LineFlagsBackward[Pos, Item.Index])
                    {
                        foreach (Point Pt in AddPoints(line, Pos, action, colour: Item.Colour))
                            yield return Pt;
                    }
                }
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
            foreach (Line Line in ForEachLine(lines, (line) => line.Sum(false) == line.Points.Count, 0))
            {
                Point.Group++;

                for (int Pos = 0; Pos < Line.LineLength; Pos++)
                {
                    if (!Line.Points.ContainsKey(Pos))
                    {
                        yield return AddPoints(Line,
                                               Pos,
                                               GriddlerPath.Action.FullLineDots);
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
            static bool NoItemForGap(Gap g, bool e, Item? i)
                => e && (!i.HasValue || g.Size < i.Value.Value);
            static bool ItemFillsGap(Gap g, bool e, Item? i)
                => e && g.HasPoints && i.HasValue && g.Is(i.Value);
            static bool ItemAtEdgeOfGap(bool a, bool e, Line l, Item? i)
                => a && i.HasValue && (e || l.ItemsOneValue);

            foreach (Line Line in ForEachLine(lines))
            {
                for (int i = 1; i < Line.MinItem; i++)
                {
                    foreach (Gap Gap in Line.GetGapsBySize(i))
                    {
                        yield return AddPoints(Line,
                                               Gap.Start,
                                               GriddlerPath.Action.GapDotsMinItem,
                                               Gap.End);
                    }
                }

                foreach (var (Gap, Ls, Skip) in Line.GetGaps(true))
                {
                    var (Item, Equality, Index, _, _) = Ls;
                    if (NoItemForGap(Gap, Equality, Item))
                    {
                        yield return AddPoints(Line,
                                               Gap.Start,
                                               GriddlerPath.Action.GapDotsTooBig,
                                               Gap.End);
                    }
                    else if (ItemFillsGap(Gap, Equality, Item) && Item.HasValue)
                    {
                        yield return AddPoints(Line,
                                               Gap.Start,
                                               GriddlerPath.Action.GapFull,
                                               Gap.End,
                                               Item.Value.Colour);
                    }
                    else
                    {
                        LineSegment LsEnd = Line.GetItemAtPosB(Gap);
                        var (ItemE, EqualityE, IndexE, _, _) = LsEnd;
                        ItemRange Range = Ls.With(LsEnd, true);
                        int Sum = Range.Sum();

                        bool CombinedEqualityNoItem()
                        {
                            if (EqualityE && Skip.LastGap != null 
                                && Skip.LastGap.IsFull()
                                && Index > IndexE)
                            {
                                int ItemShift = Line.SumWhile(IndexE, Gap, null, false);
                                if (!Line.Where(IndexE - ItemShift, IndexE - 1)
                                    .Any(a => Skip.LastGap.Is(a)))
                                    return true;
                            }

                            if (Equality && Index > IndexE)
                            {
                                Gap? NextGap = Line.FindGapAtPos(Gap.End + 1);
                                if (NextGap != null && NextGap.IsFull())
                                {
                                    int ItemShift = Line.SumWhile(Index, Gap);
                                    if (!Line.Where(Index + 1, Index + ItemShift)
                                        .Any(a => NextGap.Is(a)))
                                        return true;
                                }
                            }

                            return false;
                        }

                        if (NoItemForGap(Gap, EqualityE, ItemE)
                            || (Equality && EqualityE && Index > IndexE)
                            || (Equality && EqualityE && Sum > Gap.Size)
                            || CombinedEqualityNoItem())
                        {
                            yield return AddPoints(Line,
                                                   Gap.Start,
                                                   GriddlerPath.Action.GapDotsSum,
                                                   Gap.End);
                        }
                        else if (ItemFillsGap(Gap, EqualityE, ItemE) && ItemE.HasValue)
                        {
                            yield return AddPoints(Line,
                                                   Gap.Start,
                                                   GriddlerPath.Action.GapFull,
                                                   Gap.End,
                                                   ItemE.Value.Colour);
                        }
                        else if (Item.HasValue && ItemE.HasValue && Sum == Gap.Size)
                        {
                            yield return FullPart(Line,
                                                  Gap.Start,
                                                  Item.Value.Index,
                                                  ItemE.Value.Index,
                                                  GriddlerPath.Action.GapFull);
                        }
                        else if (Item.HasValue && ItemE.HasValue && (Index == IndexE
                            || Equality || EqualityE) && Sum < Gap.Size && Sum > Gap.Size / 2)
                        {
                            yield return OverlapPart(Line,
                                                     Gap.Start,
                                                     Gap.End,
                                                     Item.Value.Index,
                                                     ItemE.Value.Index,
                                                     GriddlerPath.Action.GapOverlap);
                        }
                        else if (ItemAtEdgeOfGap(Gap.HasFirstPoint, Equality, Line, Item) && Item.HasValue)
                        {
                            yield return AddPoints(Line,
                                                   Gap.Start + 1,
                                                   GriddlerPath.Action.CompleteItem,
                                                   Gap.Start + Item.Value.Value - 1,
                                                   Item.Value.Colour);

                            if (Line.ShouldAddDots(Item.Value.Index).Item2)
                            {
                                yield return AddPoints(Line,
                                                       Gap.Start + Item.Value.Value,
                                                       GriddlerPath.Action.CompleteItem);
                            }
                        }
                        else if (ItemAtEdgeOfGap(Gap.HasLastPoint, EqualityE, Line, ItemE) && ItemE.HasValue)
                        {
                            if (Gap.End - ItemE.Value.Value == Gap.Start)
                                Skip.Index = Gap.End;
                            yield return AddPoints(Line,
                                                   Gap.End - ItemE.Value.Value + 1,
                                                   GriddlerPath.Action.CompleteItem,
                                                   Gap.End - 1,
                                                   ItemE.Value.Colour);

                            if (Line.ShouldAddDots(ItemE.Value.Index).Item1)
                            {
                                yield return AddPoints(Line,
                                                       Gap.End - ItemE.Value.Value,
                                                       GriddlerPath.Action.CompleteItem);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<IEnumerable<Point>> CompleteItem(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                var (IsLineIsolated, Isolations) = Line.IsLineIsolated();
                foreach (var (Block, Gap, Ls, Skip) in Line.GetBlocks(true))
                {
                    int? IsolatedItem = null;
                    if (Isolations.TryGetValue(Skip.BlockCount, out int IsolateOut))
                        IsolatedItem = IsolateOut;
                    var (_, _, _, EqualityIndex, _) = Ls;
                    bool CompleteItem(out bool S, out bool E, out int? ItmIdx)
                    {
                        (S, E) = (Line.ItemsOneColour, Line.ItemsOneColour);
                        ItmIdx = null;

                        if (Line.MaxItem == Block.Size)
                            return true;
                        else if (IsLineIsolated && Skip.BlockCount < Line.LineItems
                            && Line[Skip.BlockCount].Value == Block.Size)
                        {
                            (S, E) = Line.ShouldAddDots(Skip.BlockCount);
                            return true;
                        }
                        else if (IsolatedItem.HasValue
                            && IsolatedItem.Value < Line.LineItems
                            && Line[IsolatedItem.Value].Value == Block.Size)
                        {
                            (S, E) = Line.ShouldAddDots(IsolatedItem.Value);
                            return true;
                        }
                        else if (Ls.All(Block.IsOrCantBe))
                        {
                            (S, E) = Ls.ShouldAddDots(Block);
                            return true;
                        }

                        Block? LastBlock = Gap.GetLastBlock(Block.Start - 1);
                        if (LastBlock != null)
                        {
                            Item? LastItem = Ls.FirstOrDefault(LastBlock.CanBe);
                            int Start = LastItem.HasValue ? LastItem.Value.Index + 1 : EqualityIndex;
                            ItemRange R = Line.CreateRange(Start, Line.LineIndex - 1);
                            if (Ls.All(a => Line.IsolatedPart(a, LastBlock, Block))
                                && R.All(Block.IsOrCantBe))
                            {
                                (S, E) = R.ShouldAddDots(Block);
                                return true;
                            }
                        }

                        LineSegment LsEnd = Line.GetItemAtPosB(Gap, Block);
                        var (ItemE, EqualityE, IndexE, EqualityIndexE, _) = LsEnd;

                        if (LastBlock != null)
                        {
                            ItemRange ItemsInRange = Ls.With(LsEnd, false, true);
                            if (ItemsInRange.Any(Block.Is)
                                && !ItemsInRange.Any(s => Line.FitsInSpace(LastBlock, Block, s)) //relax this!
                                && !ItemsInRange.Pair().Any(f => LastBlock.CanBe(f.Item1)
                                && f.Item2.Value > Block.Size)
                                && ItemsInRange.All(e => Line.IsolatedPart(e, Block, LastBlock)))
                            {
                                (S, E) = ItemsInRange.ShouldAddDots(Block);
                                return true;
                            }
                        }

                        Block? NextBlock = Gap.GetNextBlock(Block.End + 1);
                        if (NextBlock != null)
                        {
                            Item? LastItem = LsEnd.LastOrDefault(NextBlock.CanBe);
                            int End = LastItem.HasValue ? LastItem.Value.Index - 1 : EqualityIndexE;
                            ItemRange R = Line.CreateRange(LsEnd.Start, End);
                            if (LsEnd.All(e => Line.IsolatedPart(e, Block, NextBlock, false))
                                && R.All(Block.IsOrCantBe))
                            {
                                (S, E) = R.ShouldAddDots(Block);
                                return true;
                            }
                        }

                        if (NextBlock != null)
                        {
                            int EndIndex = EqualityIndexE;
                            if (Line.UniqueCount(NextBlock, out Item NextItem))
                                EndIndex = NextItem.Index;
                            ItemRange R = Line.CreateRange(EqualityIndex, EndIndex - 1);
                            if (R.All(Block.IsOrCantBe)
                                && Line.IsolatedPart(Line[EndIndex], Block, NextBlock, false))
                            {
                                (S, E) = R.ShouldAddDots(Block);
                                return true;
                            }
                        }

                        ItemRange Range = Ls.With(LsEnd);
                        (S, E) = Range.ShouldAddDots(Block);
                        return Range.All(Block.IsOrCantBe);
                    }

                    if (CompleteItem(out bool S, out bool E, out int? ItmIdx))
                    {
                        Point.Group++;

                        if (S)
                        {
                            if (Block.Start - 1 > Gap.Start)
                                Skip.BlockCount--;
                            else
                                Skip.Index = Block.End;

                            yield return AddPoints(Line,
                                                   Block.Start - 1,
                                                   GriddlerPath.Action.CompleteItem);
                        }

                        if (E)
                        {
                            yield return AddPoints(Line,
                                                   Block.End + 1,
                                                   GriddlerPath.Action.CompleteItem);
                        }
                    }
                }
            }
        }

        private IEnumerable<IEnumerable<Point>> LineBlocks(IEnumerable<Line> lines)
        {
            foreach (Line Line in ForEachLine(lines))
            {
                var (LineIsolated, Isolations) = Line.IsLineIsolated();
                foreach (var (Block, Gap, Ls, Skip) in Line.GetBlocks(true))
                {
                    int? IsolatedItem = null;
                    if (Isolations.TryGetValue(Skip.BlockCount, out int IsolateOut))
                        IsolatedItem = IsolateOut;

                    //if (Gap.IsFull)
                    //{
                    //    Continue();
                    //    continue;
                    //}

                    var (Item, Equality, Index, EqualityIndex, IndexAtBlock) = Ls;
                    LineSegment LsEnd = Line.GetItemAtPosB(Gap, Block);
                    var (ItemE, EqualityE, IndexE, EqualityIndexE, IndexAtBlockE) = LsEnd;

                    //no join
                    if (!Line.Dots.Contains(Block.End + 1)
                        && Line.Points.ContainsKey(Block.End + 2))
                    {
                        Block? NextBlock = Gap.GetBlockAtStart(Block.End + 2);
                        if (NextBlock != null && Block.Colour == NextBlock.Colour
                            && Line.Where(EqualityIndex, EqualityIndexE)
                                .All(e => (e.Value > 1 || e.Colour == Block.Colour)
                                && e.Value < NextBlock.End - Block.Start + 1))
                        {
                            yield return AddPoints(Line,
                                                   Block.End + 1,
                                                   GriddlerPath.Action.NoJoin);
                            continue;
                        }
                    }

                    //must join
                    if (!Line.Dots.Contains(Block.End + 1)
                        && !Line.Points.ContainsKey(Block.End + 1)
                        && Line.Points.ContainsKey(Block.End + 2)
                        && !ItemRange.Pair(Line.Where(EqualityIndex, EqualityIndexE))
                        .Any(s => Block.CanBe(s.Item1)
                        && s.Item1.Value <= Block.End - Gap.Start + 1
                        && s.Item2.Value <= Gap.End
                        - (Block.End + 1 + Line.GetDotCount(s.Item1.Index)) + 1))
                    {
                        yield return AddPoints(Line,
                                               Block.End + 1,
                                               GriddlerPath.Action.MustJoin,
                                               colour: Block.Colour);
                        break; //change block count - break isolations
                    }
                    else if (Equality && EqualityE && Index + 1 == IndexE
                        && Gap.NumberOfBlocks == 3)
                    {
                        Block? LastBlock = Gap.GetLastBlock(Block.Start - 1);
                        Block? NextBlock = Gap.GetNextBlock(Block.End + 1);

                        if (LastBlock != null && NextBlock != null && Item.HasValue)
                        {
                            bool Isolated = Line.IsolatedPart(Item.Value, Block, LastBlock);

                            if (Isolated)
                            {
                                yield return AddPoints(Line,
                                                       Block.End + 1,
                                                       GriddlerPath.Action.MustJoin,
                                                       NextBlock.Start - 1,
                                                       Block.Colour);
                                break; //change block count - break isolations
                            }
                        }
                    }

                    //min item backwards
                    bool MinItemBackwards(out int m)
                    {
                        m = 0;

                        if (EqualityE && ItemE.HasValue)
                        {
                            Block? NextBlock = Gap.GetNextBlock(Block.Start + 1);
                            if (NextBlock != null)
                            {
                                int ItemShift = Line.SumWhile(IndexE, Gap, Block, false);
                                bool Isolated = Line.IsolatedPart(ItemE.Value, Block, NextBlock, false);

                                if (ItemShift == 1 && Isolated)
                                {
                                    m = ItemRange.Sum(Line.Where(IndexE - 1, IndexE));
                                    return true;
                                }
                            }
                        }

                        if (Index == 0 && Block.End == Gap.End && Skip.BlockCount == 1)
                        {
                            int ItemShift = Line.SumWhile(0, Gap);
                            if (ItemShift == 2 && Skip.LastBlock != null
                                && Line.IsolatedPart(Line[ItemShift - 2], Block, Skip.LastBlock))
                            {
                                m = Line[ItemShift - 1].Value;
                                return true;
                            }
                        }

                        m = Ls.With(LsEnd).Min(Block);
                        return true;
                    };
                    if (MinItemBackwards(out int mB)
                        && Gap.End - mB + 1 < Block.Start)
                    {
                        yield return AddPoints(Line,
                                               Gap.End - mB + 1,
                                               GriddlerPath.Action.MinItem,
                                               Block.Start - 1,
                                               Block.Colour);
                        continue;
                    }

                    //min item forwards
                    bool MinItemForwards(out int m)
                    {
                        m = 0;

                        if (Line.UniqueCount(Block, out Item Itm))
                        {
                            m = Itm.Value;
                            return true;
                        }

                        if (Equality && Item.HasValue)
                        {
                            Block? LastBlock = Gap.GetLastBlock(Block.Start - 1);
                            if (LastBlock != null)
                            {
                                int ItemShift = Line.SumWhile(Index, Gap, Block);
                                bool Isolated = Line.IsolatedPart(Item.Value, Block, LastBlock);

                                if (ItemShift == 1 && Isolated)
                                {
                                    m = ItemRange.Sum(Line.Where(Index, Index + 1));
                                    return true;
                                }
                            }
                        }
                        m = Ls.With(LsEnd).Min(Block);
                        return true;
                    };
                    if (MinItemForwards(out int m)
                        && Gap.Start + m - 1 > Block.End)
                    {
                        yield return AddPoints(Line,
                                               Block.End + 1,
                                               GriddlerPath.Action.MinItem,
                                               Gap.Start + m - 1,
                                               Block.Colour);
                        continue;
                    }

                    //single item Start
                    bool SingleItemStart(out int singleItem)
                    {
                        singleItem = 0;

                        if (LineIsolated && (Gap.NumberOfBlocks == 1 || Skip.BlockCount == 0
                            || Gap.GetLastBlock(Block.Start - 1) == null)
                            && Block.End - Line[Skip.BlockCount].Value >= Gap.Start)
                        {
                            singleItem = Line[Skip.BlockCount].Value;
                            return true;
                        }
                        else if (IsolatedItem.HasValue
                            && (Gap.NumberOfBlocks == 1 || Skip.BlockCount == 0
                            || Gap.GetLastBlock(Block.Start - 1) == null)
                            && Block.End - Line[IsolatedItem.Value].Value >= Gap.Start)
                        {
                            singleItem = Line[IsolatedItem.Value].Value;
                        }
                        else if (Equality && EqualityE && Index == IndexE
                            && Gap.NumberOfBlocks == 1)
                        {
                            singleItem = Line.Where(Index, IndexE).FirstOrDefault().Value;
                            return true;
                        }
                        else if (Ls.UniqueCount(Block, out Item Item)
                                && Item.Index == EqualityIndex)
                        {
                            singleItem = Item.Value;
                            return true;
                        }

                        return false;
                    }
                    if (SingleItemStart(out int Sis)
                        && Block.End - Sis >= Gap.Start)
                    {
                        yield return AddPoints(Line,
                                               Gap.Start,
                                               GriddlerPath.Action.ItemBackwardReach,
                                               Block.End - Sis);
                        Skip.Index = Block.End;
                    }

                    //single item End
                    bool SingleItemEnd(out int singleItem)
                    {
                        singleItem = 0;

                        if (LineIsolated && (Gap.NumberOfBlocks == 1
                            || Skip.BlockCount == Line.LineItems - 1
                            || Gap.GetNextBlock(Block.End + 1) == null))
                        {
                            singleItem = Line[Skip.BlockCount].Value;
                            return true;
                        }
                        //breaks sevenswansswimming45x35, daffodills30x35, +1 more
                        //else if (isolatedItem && (Gap.NumberOfBlocks == 1
                        //    || isolatedItem == Line.LineItems - 1 || !Gap.GetNextBlock(Block.End + 1))) {
                        //    singleItem = Line[isolatedItem].Value;
                        //}
                        else if (Equality && IndexAtBlock == Index && Gap.NumberOfBlocks == 1
                            && (Index + 1 > Line.LineItems - 1
                            || !Line.FitsInSpace(Block, Gap, Line[Index + 1])))
                        {
                            singleItem = Line[Index].Value;
                            return true;
                        }
                        else if (LsEnd.UniqueCount(Block, out Item item)
                                && item.Index == EqualityIndexE)
                        {
                            singleItem = item.Value;
                            return true;
                        }

                        return false;
                    }
                    if (SingleItemEnd(out int Sie) && Block.Start + Sie <= Gap.End)
                    {
                        yield return AddPoints(Line,
                                               Block.Start + Sie,
                                               GriddlerPath.Action.ItemForwardReach,
                                               Gap.End);
                    }

                    //sum dot forward
                    if (Item.HasValue && IndexAtBlock - 1 >= 0 && IndexAtBlock - 1 <= Line.LineItems - 1
                        && ItemRange.All(Line.Where(EqualityIndex, IndexAtBlock - 1), Block.Is)
                        && Line.Where(Index, IndexAtBlock - 1).Any()
                        && Gap.Start + ItemRange.Sum(Line.Where(Index, IndexAtBlock - 1)) - 1
                        == Block.Start - 1 - Line.GetDotCount(IndexAtBlock - 1))
                    {
                        int GapStart = Gap.Start;
                        yield return AddPoints(Line,
                                               Block.Start - 1,
                                               GriddlerPath.Action.SumDotForward);

                        if (Block.Start - 1 > GapStart)
                        {
                            Skip.BlockCount--;
                            continue;
                        }
                        else
                            Skip.Index = Block.End;
                    }

                    //sum dot backward
                    if (ItemE.HasValue && IndexAtBlockE >= 0
                        && IndexAtBlockE + 1 <= Line.LineItems - 1
                        && ItemRange.All(Line.Where(IndexAtBlockE + 1, EqualityIndexE), Block.Is)
                        && Line.Where(IndexAtBlockE + 1, IndexE).Any()
                        && Gap.End - ItemRange.Sum(Line.Where(IndexAtBlockE + 1, IndexE)) + 1
                        == Block.End + 1 + Line.GetDotCount(IndexAtBlockE))
                    {
                        yield return AddPoints(Line,
                                               Block.End + 1,
                                               GriddlerPath.Action.SumDotBackward);
                    }

                    //half Gap overlap backwards
                    bool HalfGapOverlapBackwards(out ItemRange r)
                    {
                        if (Ls.With(LsEnd).UniqueCount(Block, out Item item)
                            && item.Index > Index)
                        {
                            r = Line.CreateRange(Index, item.Index - 1);
                            return true;
                        }
                        else if (ItemE.HasValue && Equality && IndexAtBlockE - 1 >= 0)
                        {
                            r = Line.CreateRange(EqualityIndex, IndexAtBlockE - 1);
                            return true;
                        }

                        r = null;
                        return false;
                    };
                    if (HalfGapOverlapBackwards(out ItemRange r))
                    {
                        int Sum = r.Sum();
                        var Space = Line.SpaceBetween(Gap, Block, Line[r.End]);
                        if (Sum < Space.Item1 && Sum > Space.Item1 / 2)
                        {
                            int PointsChange = Line.Points.Count;
                            yield return OverlapPart(Line,
                                                     Gap.Start,
                                                     Block.Start - 1 - Space.Item3,
                                                     r.Start,
                                                     r.End,
                                                     GriddlerPath.Action.HalfGapOverlap);

                            if(Line.Points.Count != PointsChange)
                                break; //might be split block - break isolations
                        }
                        else if (Sum == Space.Item1)
                        {
                            yield return FullPart(Line,
                                                  Gap.Start,
                                                  r.Start,
                                                  r.End,
                                                  GriddlerPath.Action.HalfGapFullPart);
                            break;
                        }
                    }

                    //half Gap overlap forwards
                    bool HalfGapOverlapForwards(out ItemRange r)
                    {
                        if (Item.HasValue && EqualityE
                            && IndexAtBlock + 1 <= Line.LineItems - 1)
                        {
                            r = Line.CreateRange(IndexAtBlock + 1, EqualityIndexE);
                            return true;
                        }

                        r = null;
                        return false;
                    };
                    if (HalfGapOverlapForwards(out r))
                    {
                        int Sum = r.Sum();
                        var Space = Line.SpaceBetween(Block, Gap, Line[r.Start]);
                        if (Sum < Space.Item1 && Sum > Space.Item1 / 2)
                        {
                            int PointsChange = Line.Points.Count;
                            yield return OverlapPart(Line,
                                                     Block.End + 1 + Space.Item2,
                                                     Gap.End,
                                                     r.Start,
                                                     r.End,
                                                     GriddlerPath.Action.HalfGapOverlap);

                            if(Line.Points.Count != PointsChange)
                                break; //might be split block - break isolations
                        }
                        else if (Sum == Space.Item1)
                        {
                            yield return FullPart(Line,
                                                  Block.End + 1 + Space.Item2,
                                                  r.Start,
                                                  r.End,
                                                  GriddlerPath.Action.HalfGapFullPart);
                        }
                    }

                    //Isolated items reach
                    if (LineIsolated && Gap.NumberOfBlocks > 1
                        && Skip.BlockCount < Line.LineItems - 1)
                    {
                        Item NextItem = Line[Skip.BlockCount + 1];
                        int Start = Block.Start + Line[Skip.BlockCount].Value;
                        Block? NextBlock = Gap.GetNextBlock(Block.End + 1);

                        if (NextBlock != null)
                        {
                            yield return AddPoints(Line,
                                                   Start,
                                                   GriddlerPath.Action.IsolatedItemsReach,
                                                   NextBlock.End - NextItem.Value);
                        }
                    }
                    else if (IsolatedItem.HasValue
                        && Isolations.TryGetValue(Skip.BlockCount + 1, out int NextIsolated)
                        && IsolatedItem.Value != NextIsolated
                        && Gap.NumberOfBlocks > 1 && IsolatedItem.Value < Line.LineItems - 1)
                    {
                        Item NextItem = Line[IsolatedItem.Value + 1];
                        int Start = Block.Start + Line[IsolatedItem.Value].Value;
                        Block? NextBlock = Gap.GetNextBlock(Block.End + 1);

                        if (NextBlock != null)
                        {
                            yield return AddPoints(Line,
                                                   Start,
                                                   GriddlerPath.Action.IsolatedItemsReach,
                                                   NextBlock.End - NextItem.Value);
                        }
                    }
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
                    List<Block> ColourCounts = new List<Block>(10);
                    string PrevColour = "black";
                    int Start = 0;

                    for (int Pos = 0; Pos < Line.LineLength; Pos++)
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

                        if (Colour != PrevColour)
                        {
                            ColourCounts.Add(new Block(Start, Pos, Colour));
                            Start = Pos;
                            PrevColour = Colour;
                        }
                    }

                    if (Line.Count(c => c.Colour == "green") == ColourCounts.Count(c => c.Colour == "green"))
                    {
                        foreach ((Block, Item) Item in ColourCounts.Where(w => w.Colour == "green").Zip(Line))
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
                foreach (var (Gap, Ls, _) in Line.GetGaps(true))
                {
                    var Xy = Line.IsRow ? (Gap.Start, Line.LineIndex) : (Line.LineIndex, Gap.Start);
                    if (!Line.Points.ContainsKey(Gap.Start)
                        && !Line.Dots.Contains(Gap.Start)
                        && !IncorrectTrials.Contains(Xy))
                    {
                        var (Item, Equality, Index, _, _) = Ls;
                        if (Ls.Valid && Ls.Eq && Ls.Item.HasValue)
                        {
                            //add one point
                            foreach (Point Point in AddPoints(Line,
                                                              Gap.Start,
                                                              GriddlerPath.Action.TrialAndError,
                                                              Gap.Start,
                                                              Item.Value.Colour))
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